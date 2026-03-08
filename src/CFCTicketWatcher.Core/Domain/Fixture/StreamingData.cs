using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.Fixture;

/// <summary>
/// Streaming asset information.
/// </summary>
public record StreamingData
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("mediaType")]
    public string? MediaType { get; init; }

    [JsonPropertyName("assetID")]
    public string? AssetID { get; init; }
}
