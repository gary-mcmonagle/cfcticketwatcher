using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent;

/// <summary>
/// Represents a row of content on the page (WidgetRow or GridRow).
/// </summary>
public record ContentRow
{
    [JsonPropertyName("rowType")]
    public string? RowType { get; init; }

    [JsonPropertyName("rowTitle")]
    public string? RowTitle { get; init; }

    [JsonPropertyName("rowStyle")]
    public string? RowStyle { get; init; }

    [JsonPropertyName("gridType")]
    public string? GridType { get; init; }

    [JsonPropertyName("rowID")]
    public string? RowID { get; init; }

    [JsonPropertyName("displayRowTitle")]
    public bool? DisplayRowTitle { get; init; }

    [JsonPropertyName("addToQuickMenu")]
    public bool? AddToQuickMenu { get; init; }

    [JsonPropertyName("customTag")]
    public string? CustomTag { get; init; }

    [JsonPropertyName("rowData")]
    public List<GridItem>? RowData { get; init; }
}
