namespace CfcTicketWatcher.Models;

/// <summary>
/// Message placed on the queue for new match ticket availability
/// </summary>
public class MatchNotificationMessage
{
    /// <summary>
    /// The unique match identifier
    /// </summary>
    public string MatchId { get; set; } = string.Empty;

    /// <summary>
    /// Full match description
    /// </summary>
    public string MatchLabel { get; set; } = string.Empty;

    /// <summary>
    /// Competition category
    /// </summary>
    public string Competition { get; set; } = string.Empty;

    /// <summary>
    /// Season identifier
    /// </summary>
    public string Season { get; set; } = string.Empty;

    /// <summary>
    /// When the match was detected with tickets available
    /// </summary>
    public DateTimeOffset DetectedAt { get; set; }
}
