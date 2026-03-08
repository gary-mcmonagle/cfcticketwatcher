using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.Fixture;

/// <summary>
/// Team crest image references in different styles.
/// </summary>
public record TeamCrests
{
    [JsonPropertyName("crestDefaultKey")]
    public string? CrestDefaultKey { get; init; }

    [JsonPropertyName("crestDefaultMediaLibraryID")]
    public string? CrestDefaultMediaLibraryID { get; init; }

    [JsonPropertyName("crestLightKey")]
    public string? CrestLightKey { get; init; }

    [JsonPropertyName("crestLightMediaLibraryID")]
    public string? CrestLightMediaLibraryID { get; init; }

    [JsonPropertyName("crestDarkKey")]
    public string? CrestDarkKey { get; init; }

    [JsonPropertyName("crestDarkMediaLibraryID")]
    public string? CrestDarkMediaLibraryID { get; init; }
}
