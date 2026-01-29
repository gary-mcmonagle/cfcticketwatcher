namespace CfcTicketWatcher.Models;

/// <summary>
/// Message placed on the email queue for sending ticket availability notifications
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Email subject line
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// HTML body content
    /// </summary>
    public string HtmlBody { get; set; } = string.Empty;

    /// <summary>
    /// Plain text body content
    /// </summary>
    public string PlainTextBody { get; set; } = string.Empty;

    /// <summary>
    /// Match ID for tracking
    /// </summary>
    public string MatchId { get; set; } = string.Empty;

    /// <summary>
    /// When the email was queued
    /// </summary>
    public DateTimeOffset QueuedAt { get; set; }
}
