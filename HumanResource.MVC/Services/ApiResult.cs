namespace HumanResource.MVC.Services;

public class ApiResult<T>
{
    public bool Succeeded { get; set; }

    public T? Data { get; set; }

    public string? ErrorMessage { get; set; }

    public int StatusCode { get; set; }

    public static ApiResult<T> Success(T? data, int statusCode)
    {
        return new ApiResult<T>
        {
            Succeeded = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResult<T> Failure(string message, int statusCode)
    {
        return new ApiResult<T>
        {
            Succeeded = false,
            ErrorMessage = message,
            StatusCode = statusCode
        };
    }
}
