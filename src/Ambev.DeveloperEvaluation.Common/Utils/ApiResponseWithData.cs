namespace Ambev.DeveloperEvaluation.Common.Utils;

public class ApiResponseWithData<T> : ApiResponse
{
    public T? Data { get; set; }
}