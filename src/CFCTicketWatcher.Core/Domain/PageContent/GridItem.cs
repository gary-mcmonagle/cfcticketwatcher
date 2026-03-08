using System.Text.Json;
using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent;

/// <summary>
/// Represents a grid item containing a widget.
/// </summary>
public record GridItem
{
    [JsonPropertyName("gridItemID")]
    public string? GridItemID { get; init; }

    [JsonPropertyName("gridItemStyle")]
    public string? GridItemStyle { get; init; }

    [JsonPropertyName("widgetName")]
    public string? WidgetName { get; init; }

    [JsonPropertyName("widgetType")]
    public string? WidgetType { get; init; }

    [JsonPropertyName("widgetData")]
    public JsonElement? WidgetData { get; init; }

    // For GridRow items
    [JsonPropertyName("gridItemType")]
    public string? GridItemType { get; init; }

    [JsonPropertyName("gridItemData")]
    public JsonElement? GridItemData { get; init; }
}
