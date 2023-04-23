namespace EodHistoricalData.Sdk;

public static class ApiService
{
    public const string BaseUri = "https://eodhistoricaldata.com/api/";
    public const string EodUri = $"{BaseUri}eod/";
    public const string ExchangesUri = $"{BaseUri}exchanges-list/";
    public const string ExchangeDetailsUri = $"{BaseUri}exchange-details/";
    public const string ExchangeSymbolListUri = $"{BaseUri}exchange-symbol-list/";
    public const string BulkEodUri = $"{BaseUri}eod-bulk-last-day/";
    public const string DividendUri = $"{BaseUri}div/";
    public const string SplitsUri = $"{BaseUri}splits/";
    public const string CalendarUri = $"{BaseUri}calendar/";
    public const string FundamentalsUri = $"{BaseUri}fundamentals/";
    public const string OptionsUri = $"{BaseUri}options/";
    public const string UserUri = $"{BaseUri}user/";

    static ApiService()
    {
        EndPoints = new EndPoint[] {
            new(EodUri,1),
            new(ExchangesUri,1),
            new(ExchangeDetailsUri,5),
            new(ExchangeSymbolListUri,1),
            new(BulkEodUri,100),
            new(DividendUri,1),
            new(SplitsUri,1),
            new(CalendarUri,1),
            new(FundamentalsUri,10),
            new(OptionsUri,10),
            new(UserUri, 0)
        };
    }

    internal static EndPoint[] EndPoints { get; }

    public static int Usage { get; internal set; }

    public static int DailyLimit { get; internal set; } = 100_000;

    public static int Available => Math.Max(DailyLimit - Usage, 0);

    public static string GetAvailableCreditFormula() => $"{DailyLimit} - {Usage} = {Available}";

    public static bool LimitReached => Usage >= DailyLimit;

    internal static void AddCallToUsage(string uri)
    {
        Usage += FindEndPointForUri(uri).Cost.GetValueOrDefault();
    }

    public static int GetCost(string? uri, int factor = 1) => FindEndPointForUri(uri).Cost.GetValueOrDefault() * factor;

    private static EndPoint FindEndPointForUri(string? uri)
    {
        if (uri != null)
        {
            foreach (var endpoint in EndPoints)
            {
                if (!string.IsNullOrWhiteSpace(endpoint.Uri) &&
                    uri.StartsWith(endpoint.Uri, StringComparison.OrdinalIgnoreCase))
                {
                    return endpoint;
                }
            }
        }

        return EndPoint.Empty;
    }

    internal struct EndPoint
    {
        public EndPoint(string? uri, int? cost)
        {
            Uri = uri;
            Cost = cost;
        }
        
        public string? Uri;
        public int? Cost;

        public int MaxNumberCanCall(int available) =>
            available < 1 
            ? 0 
            : Cost.GetValueOrDefault() == 0 
                ? int.MaxValue 
                : available / Cost.GetValueOrDefault();
        
        public static EndPoint Empty => new(null, null);
    }
}