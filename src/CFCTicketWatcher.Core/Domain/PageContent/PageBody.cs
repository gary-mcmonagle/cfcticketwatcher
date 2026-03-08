using System.Text.Json.Serialization;

namespace CFCTicketWatcher.Core.Domain.PageContent;

/// <summary>
/// The body content of the page response containing rows and metadata.
/// </summary>
public record PageBody
{
    [JsonPropertyName("content")]
    public List<ContentRow>? Content { get; init; }

    [JsonPropertyName("parentID")]
    public string? ParentID { get; init; }

    [JsonPropertyName("pageOrder")]
    public int? PageOrder { get; init; }

    [JsonPropertyName("fullPath")]
    public string? FullPath { get; init; }

    [JsonPropertyName("postSlug")]
    public string? PostSlug { get; init; }

    [JsonPropertyName("postTitle")]
    public string? PostTitle { get; init; }

    [JsonPropertyName("postSummary")]
    public string? PostSummary { get; init; }

    [JsonPropertyName("postStatus")]
    public string? PostStatus { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("keywords")]
    public string? Keywords { get; init; }

    [JsonPropertyName("metaTitle")]
    public string? MetaTitle { get; init; }

    [JsonPropertyName("postID")]
    public string? PostID { get; init; }

    [JsonPropertyName("pageTemplate")]
    public string? PageTemplate { get; init; }

    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; init; }

    [JsonPropertyName("savedTimestamp")]
    public string? SavedTimestamp { get; init; }
}
