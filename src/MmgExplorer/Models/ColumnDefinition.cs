using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

public class ColumnDefinition(
    [property: JsonPropertyName("label")] string Label,
    [property: JsonPropertyName("path")] string Path
    );
