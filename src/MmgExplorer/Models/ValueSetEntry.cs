using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record ValueSetEntry(
    [property: JsonPropertyName("valueSet")] ValueSet ValueSet,
    [property: JsonPropertyName("valueSetVersion")] ValueSetVersion ValueSetVersion,
    [property: JsonPropertyName("conceptsCount")] int ConceptsCount,
    [property: JsonPropertyName("concepts")] List<ValueSetConcept> Concepts
);
