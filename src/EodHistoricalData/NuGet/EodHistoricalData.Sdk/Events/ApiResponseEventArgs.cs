namespace EodHistoricalData.Sdk.Events;

public class ApiResponseEventArgs : DomainEventArgs
{
    public ApiResponseEventArgs(int statusCode, string request, string response, string source) : base(source)
    {
        StatusCode = statusCode;
        Request = request;
        Response = response;
        ApiResponseException = null;
    }

    public ApiResponseEventArgs(int statusCode, string request,
        ApiResponseException? apiResponseException, string source) : base(source)
    {
        StatusCode = statusCode;
        Request = request;
        Response = null;
        ApiResponseException = apiResponseException;
    }

    public int StatusCode { get; }

    public bool IsSuccessCode => StatusCode >= 200 && StatusCode < 300;

    public string Request { get; }

    public string? Response { get; }

    public ApiResponseException? ApiResponseException { get; }
}
