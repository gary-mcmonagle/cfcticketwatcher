using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent.Widgets;

/// <summary>
/// Widget data for FixturesListWidget containing match fixtures.
/// </summary>
public record FixturesListWidgetData
{
    [JsonPropertyName("fixtures")]
    public List<Fixture>? Fixtures { get; init; }
}
