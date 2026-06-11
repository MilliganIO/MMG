using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record Block(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("guideId")] Guid GuideId,
    [property: JsonPropertyName("ordinal")] int Ordinal,
    [property: JsonPropertyName("template")] TemplateSummary? Template,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("startingDescription")] string? StartingDescription,
    [property: JsonPropertyName("endingDescription")] string? EndingDescription,
    [property: JsonPropertyName("shortName")] string? ShortName,
    [property: JsonPropertyName("shouldDisplayName")] bool ShouldDisplayName,
    [property: JsonPropertyName("elements")] List<DataElement> Elements,
    [property: JsonPropertyName("expanded")] bool Expanded
    );