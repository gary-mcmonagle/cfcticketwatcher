using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.Fixture;

/// <summary>
/// Root response from the Celtic FC fixture API.
/// </summary>
public record Fixture
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("imageHandlerURL")]
    public string? ImageHandlerURL { get; init; }

    [JsonPropertyName("body")]
    public FixtureData? Body { get; init; }
}
