using Microsoft.FluentUI.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public record DataElement(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("guideId")] Guid GuideId,
    [property: JsonPropertyName("guideInternalVersion")] int? GuideInternalVersion,
    [property: JsonPropertyName("blockId")] Guid BlockId,
    [property: JsonPropertyName("ordinal")] int Ordinal,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("shortName")] string? ShortName,
    [property: JsonPropertyName("comments")] string? Comments,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("dataType")] string DataType,
    [property: JsonPropertyName("businessRules")] string? BusinessRules,
    [property: JsonPropertyName("isUnitOfMeasure")] bool IsUnitOfMeasure,
    [property: JsonPropertyName("relatedElementId")] Guid? RelatedElementId,
    [property: JsonPropertyName("legacyCodeSystem")] string? LegacyCodeSystem,
    [property: JsonPropertyName("codeSystem")] string? CodeSystem,
    [property: JsonPropertyName("legacyPriority")] string? LegacyPriority,
    [property: JsonPropertyName("priority")] string? Priority,
    [property: JsonPropertyName("isRepeat")] bool IsRepeat,
    [property: JsonPropertyName("repetitions")] int? Repetitions,
    [property: JsonPropertyName("mayRepeat")] string? MayRepeat,
    [property: JsonPropertyName("valueSetCode")] string? ValueSetCode,
    [property: JsonPropertyName("valueSetVersionNumber")] int? ValueSetVersionNumber,
    [property: JsonPropertyName("valueSetLink")] string? ValueSetLink,
    [property: JsonPropertyName("csvImplementationNote")] string? CsvImplementationNote,
    [property: JsonPropertyName("csvOthName")] string? CsvOthName,
    [property: JsonPropertyName("csvOthDesc")] string? CsvOthDesc,
    [property: JsonPropertyName("csvSampleSegment")] string? CsvSampleSegment,
    [property: JsonPropertyName("allowLogicBypass")] bool AllowLogicBypass,
    [property: JsonPropertyName("mappings")] Mappings? Mappings,
    [property: JsonPropertyName("defaultValue")] DefaultValue? DefaultValue,
    [property: JsonPropertyName("selectedValues")] List<string>? SelectedValues,
    [property: JsonPropertyName("includeCsvOth")] bool IncludeCsvOth
);