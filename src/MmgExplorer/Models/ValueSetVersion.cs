using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record ValueSetVersion(
    [property: JsonPropertyName("valueSetVersionId")] Guid ValueSetVersionId,
    [property: JsonPropertyName("valueSetVersionNumber")] int ValueSetVersionNumber,
    [property: JsonPropertyName("valueSetVersionDescriptionText")] string? ValueSetVersionDescriptionText,
    [property: JsonPropertyName("statusCode")] string StatusCode,
    [property: JsonPropertyName("statusDate")] DateTime StatusDate,
    [property: JsonPropertyName("assigningAuthorityVersionText")] string? AssigningAuthorityVersionText,
    [property: JsonPropertyName("assigningAuthorityReleaseDate")] DateTime? AssigningAuthorityReleaseDate,
    [property: JsonPropertyName("noteText")] string? NoteText,
    [property: JsonPropertyName("effectiveDate")] DateTime? EffectiveDate,
    [property: JsonPropertyName("valueSetOid")] string ValueSetOid
);