using System.Text.Json;
using System.Text.RegularExpressions;
using Azure.Communication.Email;
using CFCTicketWatcher.Func.TableStorage;
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

        logger.LogInformation("Handling result for request {RequestId} with {Count} fixtures ({NewCount} new)", 
            resultMessage.RequestId, resultMessage.Fixtures.Count, resultMessage.NewFixtures.Count);

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

        var hasNewFixtures = resultMessage.NewFixtures.Count > 0;
        var isUrgent = resultMessage.Fixtures.Any(f => StMirrenRegex().IsMatch(f.Opponent));

        var subjectPrefix = isUrgent ? "[URGENT] " : hasNewFixtures ? "[NEW FIXTURE] " : "";
        var subject = $"{subjectPrefix}Celtic FC Upcoming Fixtures";

        var emailMessage = new EmailMessage(
            senderAddress: senderEmail,
            content: new EmailContent(subject)
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
        var hasNewFixtures = result.NewFixtures.Count > 0;
        var newFixtureRowKeys = result.NewFixtures
            .Select(FixtureTableStorageService.BuildRowKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var fixtureRows = string.Join("", result.Fixtures.Select(f =>
        {
            var isNew = newFixtureRowKeys.Contains(FixtureTableStorageService.BuildRowKey(f));
            var newBadge = isNew ? " <strong style=\"color: #005500;\">[NEW]</strong>" : "";
            var rowStyle = isNew ? " style=\"background-color: #d4f5d4;\"" : "";
            return $"<tr{rowStyle}><td>{f.Date:ddd, MMM d yyyy HH:mm}</td><td>{f.Opponent}{newBadge}</td><td>{f.Venue}</td></tr>";
        }));

        var newFixturesBanner = hasNewFixtures
            ? $"""
                <div style="background-color: #d4f5d4; border: 2px solid #005500; padding: 12px; margin-bottom: 16px; border-radius: 4px;">
                    <strong style="color: #005500;">&#127381; {result.NewFixtures.Count} new fixture{(result.NewFixtures.Count == 1 ? "" : "s")} available!</strong>
                    Rows highlighted in green are newly added since the last check.
                </div>
              """
            : "";

        return $"""
            <html>
            <body>
                <h1>Celtic FC Upcoming Fixtures</h1>
                {newFixturesBanner}
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
