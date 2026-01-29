using CfcTicketWatcher.Models;

namespace CfcTicketWatcher.Functions.Services;

/// <summary>
/// Service for sending emails via Azure Communication Services
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email notification for a new ticket availability
    /// </summary>
    /// <param name="message">The email message to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the email was sent successfully</returns>
    Task<bool> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
