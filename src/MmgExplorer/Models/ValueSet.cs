using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record ValueSet(
    [property: JsonPropertyName("valueSetId")] Guid ValueSetId,
    [property: JsonPropertyName("valueSetOid")] string ValueSetOid,
    [property: JsonPropertyName("valueSetName")] string ValueSetName,
    [property: JsonPropertyName("valueSetCode")] string? ValueSetCode,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("statusDate")] DateTime StatusDate,
    [property: JsonPropertyName("definitionText")] string? DefinitionText,
    [property: JsonPropertyName("scopeNoteText")] string? ScopeNoteText,
    [property: JsonPropertyName("assigningAuthorityId")] Guid? AssigningAuthorityId,
    [property: JsonPropertyName("legacyFlag")] string? LegacyFlag
);
