using System.Text.Json.Serialization;

namespace MmgExplorer.Models;

/// <summary>
/// Response envelope returned by every MMGAT API endpoint:
/// <c>{ "statusCode": 200, "result": ... }</c>.
/// </summary>
/// <typeparam name="T">The payload type carried in <c>result</c>.</typeparam>
public record MmgatResponse<T>(
    [property: JsonPropertyName("statusCode")] int StatusCode,
    [property: JsonPropertyName("result")] T? Result,
    [property: JsonPropertyName("message")] string Message);
