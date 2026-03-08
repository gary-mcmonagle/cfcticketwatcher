using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent.Widgets;

/// <summary>
/// Competition information for a match.
/// </summary>
public record Competition
{
    [JsonPropertyName("seasonID")]
    public int? SeasonID { get; init; }

    [JsonPropertyName("matchID")]
    public string? MatchID { get; init; }

    [JsonPropertyName("teamID")]
    public string? TeamID { get; init; }
}
