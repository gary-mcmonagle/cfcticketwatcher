using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent;

/// <summary>
/// Root response from the Celtic FC ticket API.
/// </summary>
public record PageData
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("body")]
    public PageBody? Body { get; init; }

    [JsonPropertyName("datetime")]
    public DateTime? DateTime { get; init; }
}
