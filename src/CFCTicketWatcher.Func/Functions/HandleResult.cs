using System.Text.Json;
using System.Text.RegularExpressions;
using Azure.Communication.Email;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CFCTicketWatcher.Func.Functions;

public partial class HandleResult(
    ILogger<HandleResult> logger, 
    EmailClient emailClient,
    IConfiguration configuration)
{
    [GeneratedRegex(@"^St\.?\s*Mirren$|^Saint\s*Mirren$", RegexOptions.IgnoreCase)]
    private static partial Regex StMirrenRegex();
    [Function(nameof(HandleResult))]
    public async Task Run([QueueTrigger("fixture-results")] string message)
    {
        var resultMessage = JsonSerializer.Deserialize<FixtureResultMessage>(message);
        
        if (resultMessage == null)
        {
            logger.LogWarning("Failed to deserialize fixture result message");
            return;
        }

        logger.LogInformation("Handling result for request {RequestId} with {Count} fixtures", 
            resultMessage.RequestId, resultMessage.Fixtures.Count);

        if (resultMessage.Fixtures.Count == 0)
        {
            logger.LogInformation("No fixtures to report for request {RequestId}", resultMessage.RequestId);
            return;
        }

        var recipientEmail = configuration["NotificationEmail"];
        var senderEmail = configuration["SenderEmail"];

        if (string.IsNullOrEmpty(recipientEmail) || string.IsNullOrEmpty(senderEmail))
        {
            logger.LogWarning("NotificationEmail or SenderEmail not configured. Skipping email notification.");
            return;
        }

        var emailContent = BuildEmailContent(resultMessage);

        var isUrgent = resultMessage.Fixtures.Any(f => StMirrenRegex().IsMatch(f.Opponent));
        
        var emailMessage = new EmailMessage(
            senderAddress: senderEmail,
            content: new EmailContent($"{(isUrgent ? "[URGENT] " : "")}Celtic FC Upcoming Fixtures")
            {
                Html = emailContent
            },
            recipients: new EmailRecipients([new EmailAddress(recipientEmail)]));

        try
        {
            var operation = await emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage);
            logger.LogInformation("Email sent successfully for request {RequestId}. Operation ID: {OperationId}", 
                resultMessage.RequestId, operation.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email for request {RequestId}", resultMessage.RequestId);
            throw;
        }
    }

    private static string BuildEmailContent(FixtureResultMessage result)
    {
        var fixtureRows = string.Join("", result.Fixtures.Select(f => 
            $"<tr><td>{f.Date:ddd, MMM d yyyy HH:mm}</td><td>{f.Opponent}</td><td>{f.Venue}</td></tr>"));

        return $"""
            <html>
            <body>
                <h1>Celtic FC Upcoming Fixtures</h1>
                <p>Here are the upcoming fixtures as of {result.ProcessedAt:f}:</p>
                <table border="1" cellpadding="8" cellspacing="0">
                    <thead>
                        <tr>
                            <th>Date</th>
                            <th>Opponent</th>
                            <th>Venue</th>
                        </tr>
                    </thead>
                    <tbody>
                        {fixtureRows}
                    </tbody>
                </table>
                <p>Request ID: {result.RequestId}</p>
            </body>
            </html>
            """;
    }
}
