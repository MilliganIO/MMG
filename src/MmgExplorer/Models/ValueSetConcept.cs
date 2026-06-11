using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record ValueSetConcept(
    [property: JsonPropertyName("valueSetConceptId")] Guid ValueSetConceptId,
    [property: JsonPropertyName("codeSystemConceptName")] string? CodeSystemConceptName,
    [property: JsonPropertyName("valueSetConceptStatusCode")] string? ValueSetConceptStatusCode,
    [property: JsonPropertyName("valueSetConceptStatusDate")] DateTime? ValueSetConceptStatusDate,
    [property: JsonPropertyName("valueSetConceptDefinitionText")] string? ValueSetConceptDefinitionText,
    [property: JsonPropertyName("cdcPreferredDesignation")] string? CdcPreferredDesignation,
    [property: JsonPropertyName("scopeNoteText")] string? ScopeNoteText,
    [property: JsonPropertyName("valueSetVersionId")] Guid ValueSetVersionId,
    [property: JsonPropertyName("codeSystemOid")] string? CodeSystemOid,
    [property: JsonPropertyName("conceptCode")] string ConceptCode,
    [property: JsonPropertyName("hL70396Identifier")] string? HL70396Identifier
);
