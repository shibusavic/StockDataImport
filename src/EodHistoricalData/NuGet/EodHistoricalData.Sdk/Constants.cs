using System.ComponentModel;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("EodHistoricalData.Sdk.Tests"),
    InternalsVisibleTo("Import.Infrastructure.Tests")]
namespace EodHistoricalData.Sdk;

public static class Constants
{
    public const string UnknownValue = "UNKNOWN";

    public static class Period
    {
        public const string Daily = "d";
        public const string Weekly = "w";
        public const string Monthly = "m";
    }

    public static class Order
    {
        public const string Ascending = "a";
        public const string Descending = "d";
    }
}

public enum SymbolType
{
    None = 0,
    [Description("Common Stock")]
    CommonStock,
    [Description("ETF")]
    Etf,
    [Description("FUND")]
    Fund,
    [Description("Preferred Stock")]
    PreferredStock,
    [Description("Mutual Fund")]
    MutualFund
}
