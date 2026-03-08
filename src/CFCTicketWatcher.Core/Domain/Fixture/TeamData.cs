using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.Fixture;

/// <summary>
/// Team information for a fixture.
/// </summary>
public record TeamData
{
    [JsonPropertyName("teamID")]
    public string? TeamID { get; init; }

    [JsonPropertyName("teamName")]
    public string? TeamName { get; init; }

    [JsonPropertyName("shortTeamName")]
    public string? ShortTeamName { get; init; }

    [JsonPropertyName("teamNameInitials")]
    public string? TeamNameInitials { get; init; }

    [JsonPropertyName("side")]
    public string? Side { get; init; }

    [JsonPropertyName("country")]
    public string? Country { get; init; }

    [JsonPropertyName("teamCrest")]
    public string? TeamCrest { get; init; }

    [JsonPropertyName("altTeamCrest")]
    public string? AltTeamCrest { get; init; }

    [JsonPropertyName("teamCrests")]
    public TeamCrests? TeamCrests { get; init; }

    [JsonPropertyName("ninetyScore")]
    public int? NinetyScore { get; init; }

    [JsonPropertyName("extraScore")]
    public int? ExtraScore { get; init; }

    [JsonPropertyName("penaltyScore")]
    public int? PenaltyScore { get; init; }
}
