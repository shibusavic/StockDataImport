namespace EodHistoricalData.Sdk.Events;

public class ApiLimitReachedEventArgs : DomainEventArgs
{
    public ApiLimitReachedEventArgs(ApiLimitReachedException exception, string source) : base(source)
    {
        ApiLimitReachedException = exception;
    }

    public ApiLimitReachedException ApiLimitReachedException { get; set; }
}
