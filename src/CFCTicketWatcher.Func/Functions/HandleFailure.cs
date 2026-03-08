using System.Text.Json;
using Azure.Communication.Email;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CFCTicketWatcher.Func.Functions;

public class HandleFailure(
    ILogger<HandleFailure> logger,
    EmailClient emailClient,
    IConfiguration configuration)
{
    [Function(nameof(HandleFailure))]
    public async Task Run([QueueTrigger("fixture-failures")] string message)
    {
        var failureMessage = JsonSerializer.Deserialize<FailureMessage>(message);

        if (failureMessage == null)
        {
            logger.LogWarning("Failed to deserialize failure message");
            return;
        }

        logger.LogWarning("Handling failure for request {RequestId}: {ErrorMessage}",
            failureMessage.RequestId, failureMessage.ErrorMessage);

        var recipientEmail = configuration["NotificationEmail"];
        var senderEmail = configuration["SenderEmail"];

        if (string.IsNullOrEmpty(recipientEmail) || string.IsNullOrEmpty(senderEmail))
        {
            logger.LogWarning("NotificationEmail or SenderEmail not configured. Skipping failure notification.");
            return;
        }

        var emailContent = BuildFailureEmailContent(failureMessage);

        var emailMessage = new EmailMessage(
            senderAddress: senderEmail,
            content: new EmailContent("Celtic FC Ticket Watcher - Error Alert")
            {
                Html = emailContent
            },
            recipients: new EmailRecipients([new EmailAddress(recipientEmail)]));

        try
        {
            var operation = await emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage);
            logger.LogInformation("Failure notification email sent for request {RequestId}. Operation ID: {OperationId}",
                failureMessage.RequestId, operation.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send failure notification email for request {RequestId}",
                failureMessage.RequestId);
            throw;
        }
    }

    private static string BuildFailureEmailContent(FailureMessage failure)
    {
        return $"""
            <html>
            <body>
                <h1 style="color: #cc0000;">Celtic FC Ticket Watcher - Error Alert</h1>
                <p>An error occurred while processing fixture requests.</p>
                
                <table border="1" cellpadding="8" cellspacing="0">
                    <tr>
                        <th>Request ID</th>
                        <td>{failure.RequestId}</td>
                    </tr>
                    <tr>
                        <th>Error Time</th>
                        <td>{failure.FailedAt:f}</td>
                    </tr>
                    <tr>
                        <th>Function</th>
                        <td>{failure.FunctionName}</td>
                    </tr>
                    <tr>
                        <th>Error Message</th>
                        <td style="color: #cc0000;">{failure.ErrorMessage}</td>
                    </tr>
                    <tr>
                        <th>Stack Trace</th>
                        <td><pre style="font-size: 10px; overflow: auto;">{failure.StackTrace}</pre></td>
                    </tr>
                </table>
                
                <p style="margin-top: 20px; color: #666;">
                    This is an automated alert from the Celtic FC Ticket Watcher system.
                </p>
            </body>
            </html>
            """;
    }
}

public class FailureMessage
{
    public string RequestId { get; init; } = string.Empty;
    public string FunctionName { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
    public string? StackTrace { get; init; }
    public DateTime FailedAt { get; init; }
}
