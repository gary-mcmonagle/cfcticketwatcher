using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.Fixture;

/// <summary>
/// Calendar widget display data.
/// </summary>
public record CalendarWidgetData
{
    [JsonPropertyName("buttonText")]
    public string? ButtonText { get; init; }

    [JsonPropertyName("buttonLink")]
    public string? ButtonLink { get; init; }

    [JsonPropertyName("buttonIcon")]
    public string? ButtonIcon { get; init; }

    [JsonPropertyName("image")]
    public string? Image { get; init; }
}
