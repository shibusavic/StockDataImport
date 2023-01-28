namespace EodHistoricalData.Sdk.Events;

public class ApiResponseEventArgs : ApiEventArgs
{
    public ApiResponseEventArgs(int statusCode, string request, string response, string source,
        ApiResponseException? apiResponseException = null) : base(source)
    {
        StatusCode = statusCode;
        Request = request;
        Response = response;
        ApiResponseException = apiResponseException;
    }

    public int StatusCode { get; }

    public bool IsSuccessCode => StatusCode >= 200 && StatusCode < 300;

    public string Request { get; }

    public string Response { get; }

    public ApiResponseException? ApiResponseException { get; }
}