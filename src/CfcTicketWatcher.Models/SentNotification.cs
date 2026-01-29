using Azure;
using Azure.Data.Tables;

namespace CfcTicketWatcher.Models;

/// <summary>
/// Tracks sent notifications to prevent duplicate emails
/// </summary>
public class SentNotification : ITableEntity
{
    /// <summary>
    /// Partition key - the match ID
    /// </summary>
    public string PartitionKey { get; set; } = string.Empty;

    /// <summary>
    /// Row key - notification type (e.g., "TicketAvailable")
    /// </summary>
    public string RowKey { get; set; } = string.Empty;

    /// <summary>
    /// Azure Table Storage timestamp
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Azure Table Storage ETag for concurrency
    /// </summary>
    public ETag ETag { get; set; }

    /// <summary>
    /// When the notification was sent
    /// </summary>
    public DateTimeOffset SentAt { get; set; }

    /// <summary>
    /// Email recipient
    /// </summary>
    public string? EmailTo { get; set; }

    /// <summary>
    /// Match label for reference
    /// </summary>
    public string? MatchLabel { get; set; }
}
