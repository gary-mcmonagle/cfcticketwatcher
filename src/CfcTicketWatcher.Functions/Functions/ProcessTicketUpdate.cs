using Azure.Data.Tables;
using CfcTicketWatcher.Functions.Services;
using CfcTicketWatcher.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CfcTicketWatcher.Functions.Functions;

/// <summary>
/// Azure Function that processes ticket data from blob storage,
/// extracts upcoming matches, and stores them in table storage.
/// Also queues new matches for notification.
/// </summary>
public class ProcessTicketUpdate
{
    private readonly ITicketParserService _ticketParserService;
    private readonly ILogger<ProcessTicketUpdate> _logger;
    private readonly string _storageConnectionString;
    private TableClient? _upcomingMatchesTable;
    private TableClient? _sentNotificationsTable;

    public ProcessTicketUpdate(
        ITicketParserService ticketParserService,
        IConfiguration configuration,
        ILogger<ProcessTicketUpdate> logger)
    {
        _ticketParserService = ticketParserService;
        _logger = logger;
        _storageConnectionString = configuration["AzureWebJobsStorage"] ?? "UseDevelopmentStorage=true";
    }

    private async Task<(TableClient upcomingMatches, TableClient sentNotifications)> GetTableClientsAsync()
    {
        if (_upcomingMatchesTable == null || _sentNotificationsTable == null)
        {
            var tableServiceClient = new TableServiceClient(_storageConnectionString);
            
            _upcomingMatchesTable = tableServiceClient.GetTableClient("UpcomingMatches");
            await _upcomingMatchesTable.CreateIfNotExistsAsync();
            
            _sentNotificationsTable = tableServiceClient.GetTableClient("SentNotifications");
            await _sentNotificationsTable.CreateIfNotExistsAsync();
        }
        return (_upcomingMatchesTable, _sentNotificationsTable);
    }

    /// <summary>
    /// Blob-triggered function that processes ticket data and outputs to table storage and queue
    /// </summary>
    [Function(nameof(ProcessTicketUpdate))]
    [QueueOutput("new-matches-queue", Connection = "AzureWebJobsStorage")]
    public async Task<MatchNotificationMessage[]> Run(
        [BlobTrigger("ticket-data/responses/{name}", Connection = "AzureWebJobsStorage")] string blobContent,
        string name,
        FunctionContext context)
    {
        _logger.LogInformation("ProcessTicketUpdate triggered by blob: {Name}", name);

        var messagesToQueue = new List<MatchNotificationMessage>();

        try
        {
            var (upcomingMatchesTable, sentNotificationsTable) = await GetTableClientsAsync();
            var matches = _ticketParserService.ParseUpcomingMatches(blobContent);

            _logger.LogInformation("Parsed {Count} matches from blob", matches.Count);

            foreach (var match in matches)
            {
                // Try to add the match as a new entity
                var isNewMatch = await TryAddNewMatchAsync(upcomingMatchesTable, match);

                if (isNewMatch)
                {
                    // New match - check if notification was already queued (by another trigger)
                    _logger.LogInformation("New match detected: {MatchLabel}", match.MatchLabel);
                    
                    // Check if we've already sent a notification for this match
                    var notificationSent = await IsNotificationSentAsync(sentNotificationsTable, match.RowKey);
                    
                    if (!notificationSent)
                    {
                        messagesToQueue.Add(new MatchNotificationMessage
                        {
                            MatchId = match.RowKey,
                            MatchLabel = match.MatchLabel,
                            Competition = match.Competition,
                            Season = match.PartitionKey,
                            DetectedAt = DateTimeOffset.UtcNow
                        });
                    }
                }
                else
                {
                    // Existing match - update with latest info
                    var existingMatch = await GetExistingMatchAsync(upcomingMatchesTable, match.PartitionKey, match.RowKey);
                    if (existingMatch != null)
                    {
                        match.ETag = existingMatch.ETag;
                        await upcomingMatchesTable.UpsertEntityAsync(match);
                    }
                }
            }

            _logger.LogInformation(
                "Processed {Total} matches, {New} new matches queued for notification",
                matches.Count,
                messagesToQueue.Count);

            return messagesToQueue.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process ticket update from blob: {Name}", name);
            throw;
        }
    }

    /// <summary>
    /// Attempts to add a match as a new entity. Returns true if successful, false if it already exists.
    /// </summary>
    private static async Task<bool> TryAddNewMatchAsync(TableClient tableClient, UpcomingMatch match)
    {
        try
        {
            await tableClient.AddEntityAsync(match);
            return true;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 409) // Conflict - entity exists
        {
            return false;
        }
    }

    private static async Task<UpcomingMatch?> GetExistingMatchAsync(TableClient tableClient, string partitionKey, string rowKey)
    {
        try
        {
            var response = await tableClient.GetEntityAsync<UpcomingMatch>(partitionKey, rowKey);
            return response.Value;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    private static async Task<bool> IsNotificationSentAsync(TableClient tableClient, string matchId)
    {
        try
        {
            await tableClient.GetEntityAsync<SentNotification>(matchId, "TicketAvailable");
            return true;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }
}
