using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MmgExplorer.Models;


public sealed record ApiMultipleResponse(
    [property: JsonPropertyName("statusCode")] int StatusCode,
    [property: JsonPropertyName("result")] List<Guide> Result,
    [property: JsonPropertyName("message")] string? Message
);

public sealed record ApiSingleResponse(
    [property: JsonPropertyName("statusCode")] int StatusCode,
    [property: JsonPropertyName("result")] Guide Result,
    [property: JsonPropertyName("message")] string? Message
);

public sealed record Guide(
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
    [property: JsonPropertyName("testCaseScenarioWorksheetColumns")] List<ColumnDefinition>? TestCaseScenarioWorksheetColumns,
    [property: JsonPropertyName("columns")] List<ColumnDefinition>? Columns,
    [property: JsonPropertyName("templates")] List<TemplateSummary>? Templates,
    [property: JsonPropertyName("valueSets")] List<ValueSetEntry>? ValueSets
);

public sealed record PublishedVersion(
    [property: JsonPropertyName("internalVersion")] int InternalVersion,
    [property: JsonPropertyName("publishVersion")] string PublishVersion,
    [property: JsonPropertyName("publishDate")] DateTime PublishDate
);

public sealed record ColumnDefinition(
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("path")] string Path
);

public sealed record TemplateSummary(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("internalVersion")] int InternalVersion,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("blockId")] Guid? BlockId
);

public sealed record Block(
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

public sealed record DataElement(
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

public sealed record Mappings(
    [property: JsonPropertyName("hl7v251")] Hl7V251Mapping? Hl7V251
);

public sealed record Hl7V251Mapping(
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

public sealed record DefaultValue(
    [property: JsonPropertyName("value")] string? Value,
    [property: JsonPropertyName("label")] string? Label
);

public sealed record ValueSetEntry(
    [property: JsonPropertyName("valueSet")] ValueSet ValueSet,
    [property: JsonPropertyName("valueSetVersion")] ValueSetVersion ValueSetVersion,
    [property: JsonPropertyName("conceptsCount")] int ConceptsCount,
    [property: JsonPropertyName("concepts")] List<ValueSetConcept> Concepts
);

public sealed record ValueSet(
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

public sealed record ValueSetVersion(
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

public sealed record ValueSetConcept(
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