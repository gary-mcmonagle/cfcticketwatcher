using Azure;
using Azure.Data.Tables;

namespace CfcTicketWatcher.Models;

/// <summary>
/// Represents an upcoming Celtic match stored in Azure Table Storage
/// </summary>
public class UpcomingMatch : ITableEntity
{
    /// <summary>
    /// Partition key - typically the season (e.g., "2025")
    /// </summary>
    public string PartitionKey { get; set; } = string.Empty;

    /// <summary>
    /// Row key - the match ID (e.g., "g2563072")
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
    /// Full match label (e.g., "Celtic Vs. Falkirk - Sun, Feb 1st 2026, 15:00")
    /// </summary>
    public string MatchLabel { get; set; } = string.Empty;

    /// <summary>
    /// Competition name/category (e.g., "SPFL Matches", "Europa League Matches")
    /// </summary>
    public string Competition { get; set; } = string.Empty;

    /// <summary>
    /// Season name (e.g., "Season 2025/2026")
    /// </summary>
    public string SeasonName { get; set; } = string.Empty;

    /// <summary>
    /// Whether tickets are currently available for purchase
    /// </summary>
    public bool TicketsAvailable { get; set; }

    /// <summary>
    /// Home team crest image ID
    /// </summary>
    public string? HomeCrest { get; set; }

    /// <summary>
    /// Away team crest image ID
    /// </summary>
    public string? AwayCrest { get; set; }

    /// <summary>
    /// When this record was last updated
    /// </summary>
    public DateTimeOffset LastUpdated { get; set; }

    /// <summary>
    /// The date/time of the match (parsed from MatchLabel)
    /// </summary>
    public DateTimeOffset? MatchDate { get; set; }
}
