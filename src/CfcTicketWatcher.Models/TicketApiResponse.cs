using System.Text.Json.Serialization;

namespace CfcTicketWatcher.Models;

/// <summary>
/// Root response from the Celtic FC ticket API
/// </summary>
public class TicketApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("body")]
    public TicketApiBody? Body { get; set; }
}

public class TicketApiBody
{
    [JsonPropertyName("content")]
    public List<ContentRow>? Content { get; set; }
}

public class ContentRow
{
    [JsonPropertyName("rowType")]
    public string? RowType { get; set; }

    [JsonPropertyName("rowTitle")]
    public string? RowTitle { get; set; }

    [JsonPropertyName("rowData")]
    public List<RowDataItem>? RowData { get; set; }

    [JsonPropertyName("rowID")]
    public string? RowID { get; set; }

    [JsonPropertyName("displayRowTitle")]
    public bool DisplayRowTitle { get; set; }
}

public class RowDataItem
{
    [JsonPropertyName("gridItemID")]
    public string? GridItemID { get; set; }

    [JsonPropertyName("widgetName")]
    public string? WidgetName { get; set; }

    [JsonPropertyName("widgetType")]
    public string? WidgetType { get; set; }

    [JsonPropertyName("widgetData")]
    public WidgetData? WidgetData { get; set; }
}

public class WidgetData
{
    [JsonPropertyName("fixtures")]
    public List<Fixture>? Fixtures { get; set; }

    [JsonPropertyName("listData")]
    public List<ListItem>? ListData { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

public class Fixture
{
    [JsonPropertyName("matchDetails")]
    public List<MatchDetail>? MatchDetails { get; set; }

    [JsonPropertyName("season")]
    public int Season { get; set; }

    [JsonPropertyName("guid")]
    public long Guid { get; set; }

    [JsonPropertyName("competition")]
    public Competition? Competition { get; set; }

    [JsonPropertyName("squad")]
    public string? Squad { get; set; }

    [JsonPropertyName("matchID")]
    public string? MatchID { get; set; }
}

public class MatchDetail
{
    [JsonPropertyName("matchHomeCrest")]
    public string? MatchHomeCrest { get; set; }

    [JsonPropertyName("matchID")]
    public string? MatchID { get; set; }

    [JsonPropertyName("matchLabel")]
    public string? MatchLabel { get; set; }

    [JsonPropertyName("matchAwayCrest")]
    public string? MatchAwayCrest { get; set; }

    [JsonPropertyName("seasonName")]
    public string? SeasonName { get; set; }
}

public class Competition
{
    [JsonPropertyName("seasonID")]
    public int SeasonID { get; set; }

    [JsonPropertyName("matchID")]
    public string? MatchID { get; set; }

    [JsonPropertyName("teamID")]
    public string? TeamID { get; set; }
}

public class ListItem
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("buttonText")]
    public string? ButtonText { get; set; }

    [JsonPropertyName("buttonLink")]
    public string? ButtonLink { get; set; }
}
