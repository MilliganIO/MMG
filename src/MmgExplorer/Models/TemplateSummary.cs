using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record TemplateSummary(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("internalVersion")] int InternalVersion,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("blockId")] Guid? BlockId
);