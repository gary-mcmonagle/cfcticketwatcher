using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.Fixture;

/// <summary>
/// Competition icon references in various styles.
/// </summary>
public record CompetitionIcons
{
    [JsonPropertyName("pill-full-colour")]
    public string? PillFullColour { get; init; }

    [JsonPropertyName("crest-full-colour")]
    public string? CrestFullColour { get; init; }

    [JsonPropertyName("crest-white-outline")]
    public string? CrestWhiteOutline { get; init; }

    [JsonPropertyName("crest-black-outline")]
    public string? CrestBlackOutline { get; init; }

    [JsonPropertyName("pill-black-colour")]
    public string? PillBlackColour { get; init; }

    [JsonPropertyName("pill-white-colour")]
    public string? PillWhiteColour { get; init; }

    [JsonPropertyName("wide-full-colour")]
    public string? WideFullColour { get; init; }
}
