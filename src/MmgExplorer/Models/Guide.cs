using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record Guide(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("guideStatus")] string GuideStatus,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("shortName")] string? ShortName,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("cmgDescription")] string? CmgDescription,
    [property: JsonPropertyName("excludeGenV2")] string? ExcludeGenV2,
    [property: JsonPropertyName("isActive")] bool IsActive,
    [property: JsonPropertyName("createdBy")] Guid CreatedBy,
    [property: JsonPropertyName("ownedBy")] Guid OwnedBy,
    [property: JsonPropertyName("internalVersion")] int InternalVersion,
    [property: JsonPropertyName("createdDate")] DateTimeOffset CreatedDate,
    [property: JsonPropertyName("lastUpdatedDate")] DateTimeOffset LastUpdatedDate,
    [property: JsonPropertyName("publishVersion")] string? PublishVersion,
    [property: JsonPropertyName("publishDate")] DateTime? PublishDate,
    [property: JsonPropertyName("published")] List<PublishedVersion>? Published,
    [property: JsonPropertyName("profileIdentifier")] string? ProfileIdentifier,
    [property: JsonPropertyName("blocks")] List<Block> Blocks,
    [property: JsonPropertyName("testScenarios")] List<object>? TestScenarios,
    //[property: JsonPropertyName("testCaseScenarioWorksheetColumns")] List<ColumnDefinition>? TestCaseScenarioWorksheetColumns,
    //[property: JsonPropertyName("columns")] List<ColumnDefinition>? Columns,
    [property: JsonPropertyName("templates")] List<TemplateSummary>? Templates,
    [property: JsonPropertyName("valueSets")] List<ValueSetEntry>? ValueSets);
