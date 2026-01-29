using CfcTicketWatcher.Functions.Services;
using CfcTicketWatcher.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CfcTicketWatcher.Functions.Functions;

/// <summary>
/// Azure Function that sends email notifications via Azure Communication Services.
/// Triggered by messages on the email queue.
/// </summary>
public class SendEmail
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendEmail> _logger;

    public SendEmail(
        IEmailService emailService,
        ILogger<SendEmail> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Queue-triggered function that sends email notifications
    /// </summary>
    [Function(nameof(SendEmail))]
    public async Task Run(
        [QueueTrigger("email-queue", Connection = "AzureWebJobsStorage")] EmailMessage message,
        FunctionContext context)
    {
        _logger.LogInformation(
            "SendEmail triggered for match: {MatchId}, Subject: {Subject}",
            message.MatchId,
            message.Subject);

        try
        {
            var success = await _emailService.SendEmailAsync(message);

            if (success)
            {
                _logger.LogInformation(
                    "Successfully sent email notification for match {MatchId}",
                    message.MatchId);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to send email for match {MatchId}, message will be retried",
                    message.MatchId);
                
                // Throw to trigger retry
                throw new InvalidOperationException($"Failed to send email for match {message.MatchId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email for match {MatchId}", message.MatchId);
            throw;
        }
    }
}
