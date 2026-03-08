using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent.Widgets;

/// <summary>
/// Represents a fixture/match in the fixtures list.
/// </summary>
public record Fixture
{
    [JsonPropertyName("matchID")]
    public string? MatchID { get; init; }

    [JsonPropertyName("season")]
    public int? Season { get; init; }

    [JsonPropertyName("guid")]
    public long? Guid { get; init; }

    [JsonPropertyName("squad")]
    public string? Squad { get; init; }

    [JsonPropertyName("matchDetails")]
    public List<MatchDetails>? MatchDetails { get; init; }

    [JsonPropertyName("competition")]
    public Competition? Competition { get; init; }
}
