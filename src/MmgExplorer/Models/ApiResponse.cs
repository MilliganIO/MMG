namespace MmgExplorer.Models;

/// <summary>
/// Represents a standard response returned by an API operation, including success status, data, error information, and
/// a timestamp.
/// </summary>
/// <remarks>Use this class to encapsulate the result of an API call, providing a consistent structure for both
/// successful and failed responses. The generic parameter allows the response to carry data of any type relevant to the
/// operation. The static methods can be used to easily create success or error responses.</remarks>
/// <typeparam name="T">The type of the data returned by the API operation.</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResult(T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResult(string errorMessage)
    {
        return new ApiResponse<T>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
