using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent.Widgets;

/// <summary>
/// Details about a specific match including teams and schedule.
/// </summary>
public record MatchDetails
{
    [JsonPropertyName("matchID")]
    public string? MatchID { get; init; }

    [JsonPropertyName("matchLabel")]
    public string? MatchLabel { get; init; }

    [JsonPropertyName("matchHomeCrest")]
    public string? MatchHomeCrest { get; init; }

    [JsonPropertyName("matchAwayCrest")]
    public string? MatchAwayCrest { get; init; }

    [JsonPropertyName("seasonName")]
    public string? SeasonName { get; init; }
}
