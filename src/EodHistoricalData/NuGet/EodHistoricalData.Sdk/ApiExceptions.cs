namespace EodHistoricalData.Sdk;

/// <summary>
/// Represents an exception thrown from making a call to the eodhistoricaldata.com API.
/// </summary>
public class ApiResponseException : Exception
{
    /// <summary>
    /// Create a new instance of <see cref="ApiResponseException"/>.
    /// </summary>
    /// <param name="uri">The URI called.</param>
    /// <param name="httpResponseMessage">The response message received.</param>
    /// <param name="source">A string representation of the source of the error,
    /// e.g., the name of a method.</param>
    public ApiResponseException(string uri, HttpResponseMessage httpResponseMessage, string? source = null)
        : base($"'{uri}' returned status code {httpResponseMessage.StatusCode}")
    {
        Uri = uri;
        HttpResponseMessage = httpResponseMessage;
        Source = source;
    }

    /// <summary>
    /// Gets the URI called.
    /// </summary>
    public string Uri { get; }

    /// <summary>
    /// Gets the response message received.
    /// </summary>
    public HttpResponseMessage HttpResponseMessage { get; }
}

public class ApiLimitReachedException : Exception
{
    public ApiLimitReachedException(string source, int usage, int projectedFinal)
        : base($"API call limit will reach {projectedFinal} if {source} task executes. Current usage is {usage}")
    {
    }
    public ApiLimitReachedException(int usage)
        : base($"API call limit reached; usage is {usage}.")
    {
    }
}
