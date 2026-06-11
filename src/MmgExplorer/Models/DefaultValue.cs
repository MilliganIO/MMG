using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record DefaultValue(
    [property: JsonPropertyName("value")] string? Value,
    [property: JsonPropertyName("label")] string? Label
);