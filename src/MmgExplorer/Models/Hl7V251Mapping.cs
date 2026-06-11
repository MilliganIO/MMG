using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record Hl7V251Mapping(
    [property: JsonPropertyName("legacyIdentifier")] string? LegacyIdentifier,
    [property: JsonPropertyName("identifier")] string? Identifier,
    [property: JsonPropertyName("messageContext")] string? MessageContext,
    [property: JsonPropertyName("dataType")] string? DataType,
    [property: JsonPropertyName("segmentType")] string? SegmentType,
    [property: JsonPropertyName("obrPosition")] int? ObrPosition,
    [property: JsonPropertyName("fieldPosition")] int? FieldPosition,
    [property: JsonPropertyName("componentPosition")] int? ComponentPosition,
    [property: JsonPropertyName("usage")] string? Usage,
    [property: JsonPropertyName("cardinality")] string? Cardinality,
    [property: JsonPropertyName("literalFieldValues")] Dictionary<string, string>? LiteralFieldValues,
    [property: JsonPropertyName("repeatingGroupElementType")] string? RepeatingGroupElementType,
    [property: JsonPropertyName("implementationNotes")] string? ImplementationNotes,
    [property: JsonPropertyName("sampleSegment")] string? SampleSegment
);