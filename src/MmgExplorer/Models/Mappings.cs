using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record Mappings(
[property: JsonPropertyName("hl7v251")] Hl7V251Mapping? Hl7V251
);
