using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record PublishedVersion(
    [property: JsonPropertyName("internalVersion")] int InternalVersion,
    [property: JsonPropertyName("publishVersion")] string PublishVersion,
    [property: JsonPropertyName("publishDate")] DateTime PublishDate);
