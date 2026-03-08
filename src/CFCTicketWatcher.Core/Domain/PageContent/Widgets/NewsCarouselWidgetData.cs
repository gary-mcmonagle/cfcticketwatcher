using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent.Widgets;

/// <summary>
/// Widget data for NewsCarouselWidget.
/// </summary>
public record NewsCarouselWidgetData
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("category")]
    public string? Category { get; init; }

    [JsonPropertyName("style")]
    public string? Style { get; init; }
}
