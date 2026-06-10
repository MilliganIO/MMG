namespace MmgExplorer.Models;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public T Result { get; set; } = default!;
    public string Message { get; set; } = string.Empty;
}
