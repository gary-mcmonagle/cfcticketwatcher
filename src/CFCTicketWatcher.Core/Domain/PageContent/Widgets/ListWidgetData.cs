using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent.Widgets;

/// <summary>
/// Widget data for ListWidgetV2.
/// </summary>
public record ListWidgetData
{
    [JsonPropertyName("listTitle")]
    public string? ListTitle { get; init; }

    [JsonPropertyName("listDescription")]
    public string? ListDescription { get; init; }

    [JsonPropertyName("listData")]
    public List<ListItem>? ListData { get; init; }
}

/// <summary>
/// An item in a list widget.
/// </summary>
public record ListItem
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("buttonText")]
    public string? ButtonText { get; init; }

    [JsonPropertyName("buttonLink")]
    public string? ButtonLink { get; init; }

    [JsonPropertyName("buttonIcon")]
    public string? ButtonIcon { get; init; }

    [JsonPropertyName("buttonType")]
    public string? ButtonType { get; init; }

    [JsonPropertyName("imageID")]
    public string? ImageID { get; init; }
}
