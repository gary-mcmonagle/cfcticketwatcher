using System.Text.Json;
using Azure.Data.Tables;
using CfcTicketWatcher.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CfcTicketWatcher.Functions.Functions;

/// <summary>
/// Azure Function that processes new match notifications from the queue.
/// Checks if a notification has already been sent and if not, queues an email.
/// </summary>
public class ProcessUpcomingMatches
{
    private readonly ILogger<ProcessUpcomingMatches> _logger;
    private readonly TableClient _sentNotificationsTable;

    public ProcessUpcomingMatches(
        IConfiguration configuration,
        ILogger<ProcessUpcomingMatches> logger)
    {
        _logger = logger;

        var storageConnectionString = configuration["AzureWebJobsStorage"] ?? "UseDevelopmentStorage=true";
        var tableServiceClient = new TableServiceClient(storageConnectionString);
        
        _sentNotificationsTable = tableServiceClient.GetTableClient("SentNotifications");
        _sentNotificationsTable.CreateIfNotExists();
    }

    /// <summary>
    /// Queue-triggered function that processes match notifications and queues emails
    /// </summary>
    [Function(nameof(ProcessUpcomingMatches))]
    [QueueOutput("email-queue", Connection = "AzureWebJobsStorage")]
    public async Task<EmailMessage?> Run(
        [QueueTrigger("new-matches-queue", Connection = "AzureWebJobsStorage")] MatchNotificationMessage message,
        FunctionContext context)
    {
        _logger.LogInformation(
            "ProcessUpcomingMatches triggered for match: {MatchId} - {MatchLabel}",
            message.MatchId,
            message.MatchLabel);

        try
        {
            // Double-check if notification was already sent (for idempotency)
            var alreadySent = await IsNotificationSentAsync(message.MatchId);
            
            if (alreadySent)
            {
                _logger.LogInformation(
                    "Notification already sent for match {MatchId}, skipping",
                    message.MatchId);
                return null;
            }

            // Create email message
            var emailMessage = CreateEmailMessage(message);

            // Record that we're sending this notification
            await RecordNotificationSentAsync(message);

            _logger.LogInformation(
                "Queuing email notification for match {MatchId}",
                message.MatchId);

            return emailMessage;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process match notification for {MatchId}", message.MatchId);
            throw;
        }
    }

    private EmailMessage CreateEmailMessage(MatchNotificationMessage message)
    {
        var subject = $"🎫 Celtic FC Tickets Available: {message.MatchLabel}";
        
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; }}
        .header {{ background: #16773D; color: white; padding: 20px; text-align: center; border-radius: 10px 10px 0 0; margin: -30px -30px 20px -30px; }}
        .match-info {{ background: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0; }}
        .cta {{ text-align: center; margin: 30px 0; }}
        .cta a {{ background: #16773D; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; }}
        .footer {{ text-align: center; color: #666; font-size: 12px; margin-top: 30px; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🍀 Celtic FC Ticket Alert</h1>
        </div>
        <p>Great news! Tickets are now available for an upcoming Celtic match:</p>
        <div class=""match-info"">
            <h2>{message.MatchLabel}</h2>
            <p><strong>Competition:</strong> {message.Competition}</p>
            <p><strong>Season:</strong> {message.Season}</p>
            <p><strong>Detected:</strong> {message.DetectedAt:dddd, dd MMMM yyyy HH:mm} UTC</p>
        </div>
        <div class=""cta"">
            <a href=""https://www.celticfc.com/tickets"">Buy Tickets Now</a>
        </div>
        <div class=""footer"">
            <p>This is an automated notification from CFC Ticket Watcher.</p>
            <p>Hail Hail! 🍀</p>
        </div>
    </div>
</body>
</html>";

        var plainTextBody = $@"
Celtic FC Ticket Alert
======================

Tickets are now available for an upcoming Celtic match!

Match: {message.MatchLabel}
Competition: {message.Competition}
Season: {message.Season}
Detected: {message.DetectedAt:dddd, dd MMMM yyyy HH:mm} UTC

Buy Tickets: https://www.celticfc.com/tickets

Hail Hail! 🍀
";

        return new EmailMessage
        {
            Subject = subject,
            HtmlBody = htmlBody,
            PlainTextBody = plainTextBody,
            MatchId = message.MatchId,
            QueuedAt = DateTimeOffset.UtcNow
        };
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

    private async Task RecordNotificationSentAsync(MatchNotificationMessage message)
    {
        var notification = new SentNotification
        {
            PartitionKey = message.MatchId,
            RowKey = "TicketAvailable",
            SentAt = DateTimeOffset.UtcNow,
            MatchLabel = message.MatchLabel
        };

        await _sentNotificationsTable.UpsertEntityAsync(notification);
    }
}
