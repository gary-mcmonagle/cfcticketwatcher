using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent.Widgets;

/// <summary>
/// Widget data for TextBlockWidget containing HTML content.
/// </summary>
public record TextBlockWidgetData
{
    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("contentDouble")]
    public string? ContentDouble { get; init; }

    [JsonPropertyName("toggleDouble")]
    public bool? ToggleDouble { get; init; }

    [JsonPropertyName("mediaLibraryID")]
    public string? MediaLibraryID { get; init; }

    [JsonPropertyName("imageKey")]
    public string? ImageKey { get; init; }
}
