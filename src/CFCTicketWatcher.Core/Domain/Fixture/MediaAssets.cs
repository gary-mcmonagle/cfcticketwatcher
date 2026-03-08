using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.Fixture;

/// <summary>
/// Media assets and URLs associated with a fixture.
/// </summary>
public record MediaAssets
{
    [JsonPropertyName("ticketUrl")]
    public string? TicketUrl { get; init; }

    [JsonPropertyName("hospitalityUrl")]
    public string? HospitalityUrl { get; init; }

    [JsonPropertyName("matchReportUrl")]
    public string? MatchReportUrl { get; init; }

    [JsonPropertyName("matchDayInfoUrl")]
    public string? MatchDayInfoUrl { get; init; }

    [JsonPropertyName("highlightsVideoID")]
    public string? HighlightsVideoID { get; init; }

    [JsonPropertyName("featuredImageID")]
    public string? FeaturedImageID { get; init; }

    [JsonPropertyName("enableMatchCentre")]
    public bool? EnableMatchCentre { get; init; }

    [JsonPropertyName("enableStream")]
    public bool? EnableStream { get; init; }

    [JsonPropertyName("enableMatchdayTakeover")]
    public int? EnableMatchdayTakeover { get; init; }

    [JsonPropertyName("enableScorePredictor_App")]
    public bool? EnableScorePredictorApp { get; init; }

    [JsonPropertyName("votingStatus")]
    public string? VotingStatus { get; init; }

    [JsonPropertyName("hashtag")]
    public string? Hashtag { get; init; }

    [JsonPropertyName("customInfo")]
    public string? CustomInfo { get; init; }

    [JsonPropertyName("venueCustom")]
    public string? VenueCustom { get; init; }

    [JsonPropertyName("homeOrAway_Custom")]
    public string? HomeOrAwayCustom { get; init; }

    [JsonPropertyName("streamingData")]
    public List<StreamingData>? StreamingData { get; init; }

    [JsonPropertyName("calendarWidgetData")]
    public CalendarWidgetData? CalendarWidgetData { get; init; }

    [JsonPropertyName("teamID")]
    public string? TeamID { get; init; }

    [JsonPropertyName("seasonID")]
    public int? SeasonID { get; init; }
}
