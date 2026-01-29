using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AcsEmailMessage = Azure.Communication.Email.EmailMessage;

namespace CfcTicketWatcher.Functions.Services;

/// <summary>
/// Implementation of IEmailService using Azure Communication Services
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly string? _connectionString;
    private readonly string? _fromEmail;
    private readonly string? _toEmail;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _logger = logger;
        _connectionString = configuration["AzureCommunicationServicesConnectionString"];
        _fromEmail = configuration["NotificationEmailFrom"];
        _toEmail = configuration["NotificationEmailTo"];
    }

    public async Task<bool> SendEmailAsync(Models.EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            _logger.LogError("Azure Communication Services connection string is not configured");
            return false;
        }

        if (string.IsNullOrEmpty(_fromEmail) || string.IsNullOrEmpty(_toEmail))
        {
            _logger.LogError("Email from/to addresses are not configured");
            return false;
        }

        try
        {
            var emailClient = new EmailClient(_connectionString);

            var emailContent = new EmailContent(message.Subject)
            {
                PlainText = message.PlainTextBody,
                Html = message.HtmlBody
            };

            var acsMessage = new AcsEmailMessage(
                senderAddress: _fromEmail,
                content: emailContent,
                recipients: new EmailRecipients(new List<EmailAddress>
                {
                    new EmailAddress(_toEmail)
                }));

            EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                WaitUntil.Completed,
                acsMessage,
                cancellationToken);

            _logger.LogInformation(
                "Email sent successfully for match {MatchId}. Operation ID: {OperationId}",
                message.MatchId,
                emailSendOperation.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email for match {MatchId}", message.MatchId);
            return false;
        }
    }
}
