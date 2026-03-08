using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.Fixture;

/// <summary>
/// Detailed fixture/match data.
/// </summary>
public record FixtureData
{
    [JsonPropertyName("matchID")]
    public string? MatchID { get; init; }

    [JsonPropertyName("matchSlug")]
    public string? MatchSlug { get; init; }

    [JsonPropertyName("homeTeamID")]
    public string? HomeTeamID { get; init; }

    [JsonPropertyName("awayTeamID")]
    public string? AwayTeamID { get; init; }

    [JsonPropertyName("homeOrAway")]
    public string? HomeOrAway { get; init; }

    [JsonPropertyName("kickOff")]
    public string? KickOff { get; init; }

    [JsonPropertyName("kickOffUTC")]
    public string? KickOffUTC { get; init; }

    [JsonPropertyName("kickOffUTCTimestamp")]
    public long? KickOffUTCTimestamp { get; init; }

    [JsonPropertyName("kickOffTZ")]
    public string? KickOffTZ { get; init; }

    [JsonPropertyName("venue")]
    public string? Venue { get; init; }

    [JsonPropertyName("venueID")]
    public int? VenueID { get; init; }

    [JsonPropertyName("city")]
    public string? City { get; init; }

    [JsonPropertyName("period")]
    public string? Period { get; init; }

    [JsonPropertyName("matchType")]
    public string? MatchType { get; init; }

    [JsonPropertyName("matchDay")]
    public string? MatchDay { get; init; }

    [JsonPropertyName("seasonID")]
    public int? SeasonID { get; init; }

    [JsonPropertyName("seasonName")]
    public string? SeasonName { get; init; }

    [JsonPropertyName("seasonSlug")]
    public string? SeasonSlug { get; init; }

    [JsonPropertyName("competitionID")]
    public int? CompetitionID { get; init; }

    [JsonPropertyName("competitionName")]
    public string? CompetitionName { get; init; }

    [JsonPropertyName("competitionCode")]
    public string? CompetitionCode { get; init; }

    [JsonPropertyName("competitionFormat")]
    public string? CompetitionFormat { get; init; }

    [JsonPropertyName("teamData")]
    public List<TeamData>? TeamData { get; init; }

    [JsonPropertyName("mediaAssets")]
    public MediaAssets? MediaAssets { get; init; }

    [JsonPropertyName("competitionIcons")]
    public CompetitionIcons? CompetitionIcons { get; init; }

    [JsonPropertyName("matchStats")]
    public bool? MatchStats { get; init; }

    [JsonPropertyName("liveUpdates")]
    public bool? LiveUpdates { get; init; }

    [JsonPropertyName("broadcasters")]
    public List<object>? Broadcasters { get; init; }

    [JsonPropertyName("matchMinute")]
    public int? MatchMinute { get; init; }

    [JsonPropertyName("teamSlug")]
    public string? TeamSlug { get; init; }

    [JsonPropertyName("teamID")]
    public string? TeamID { get; init; }

    [JsonPropertyName("clientMatch")]
    public int? ClientMatch { get; init; }

    [JsonPropertyName("published")]
    public int? Published { get; init; }

    [JsonPropertyName("tbc")]
    public int? Tbc { get; init; }

    [JsonPropertyName("gameWinner")]
    public string? GameWinner { get; init; }

    [JsonPropertyName("matchWinnerTeamID")]
    public string? MatchWinnerTeamID { get; init; }

    [JsonPropertyName("abandonedReason")]
    public string? AbandonedReason { get; init; }

    [JsonPropertyName("postponedReason")]
    public string? PostponedReason { get; init; }
}
