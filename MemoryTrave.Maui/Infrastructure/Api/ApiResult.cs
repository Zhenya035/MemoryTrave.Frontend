namespace MemoryTrave.Maui.Infrastructure.Api;

public class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public int? StatusCode { get; set; }

    public static ApiResult<T> Success(T data, int statusCode) =>
        new() { IsSuccess = true, Data = data, StatusCode = statusCode };
    
    public static ApiResult<T> Failure(string errorMessage, int? statusCode = null) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage, StatusCode = statusCode };
}