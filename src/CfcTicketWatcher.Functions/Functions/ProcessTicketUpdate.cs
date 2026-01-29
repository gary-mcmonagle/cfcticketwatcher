using System.Text.Json;
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
    private readonly TableClient _upcomingMatchesTable;
    private readonly TableClient _sentNotificationsTable;

    public ProcessTicketUpdate(
        ITicketParserService ticketParserService,
        IConfiguration configuration,
        ILogger<ProcessTicketUpdate> logger)
    {
        _ticketParserService = ticketParserService;
        _logger = logger;

        var storageConnectionString = configuration["AzureWebJobsStorage"] ?? "UseDevelopmentStorage=true";
        var tableServiceClient = new TableServiceClient(storageConnectionString);
        
        _upcomingMatchesTable = tableServiceClient.GetTableClient("UpcomingMatches");
        _upcomingMatchesTable.CreateIfNotExists();
        
        _sentNotificationsTable = tableServiceClient.GetTableClient("SentNotifications");
        _sentNotificationsTable.CreateIfNotExists();
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
            var matches = _ticketParserService.ParseUpcomingMatches(blobContent);

            _logger.LogInformation("Parsed {Count} matches from blob", matches.Count);

            foreach (var match in matches)
            {
                // Check if this match already exists in table storage
                var existingMatch = await GetExistingMatchAsync(match.PartitionKey, match.RowKey);

                if (existingMatch == null)
                {
                    // New match - add to table and check if notification needed
                    _logger.LogInformation("New match detected: {MatchLabel}", match.MatchLabel);
                    
                    await _upcomingMatchesTable.UpsertEntityAsync(match);

                    // Check if we've already sent a notification for this match
                    var notificationSent = await IsNotificationSentAsync(match.RowKey);
                    
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
                    // Update existing match
                    match.ETag = existingMatch.ETag;
                    await _upcomingMatchesTable.UpsertEntityAsync(match);
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

    private async Task<UpcomingMatch?> GetExistingMatchAsync(string partitionKey, string rowKey)
    {
        try
        {
            var response = await _upcomingMatchesTable.GetEntityAsync<UpcomingMatch>(partitionKey, rowKey);
            return response.Value;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    private async Task<bool> IsNotificationSentAsync(string matchId)
    {
        try
        {
            await _sentNotificationsTable.GetEntityAsync<SentNotification>(matchId, "TicketAvailable");
            return true;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }
}
