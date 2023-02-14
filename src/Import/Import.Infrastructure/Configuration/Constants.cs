namespace Import.Infrastructure.Configuration;

public static class Constants
{
    public static class ConfigurationKeys
    {
        public const string AnyDay = "Any Day";
        public const string ApiKey = "API Key";
        public const string CancelOnException = "Cancel On Exception";
        public const string DatabasePurge = "Purge Data";
        public const string DataRetention = "Data Retention";
        public const string Exchanges = "Exchange Codes";
        public const string Mode = "Mode";
        public const string OnEmptyDb = "On Empty Database";
        public const string Options = "Options";
        public const string Priority = "Priority";
        public const string ReasonsToIgnore = "Reasons to Ignore";
        public const string Scope = "Scope";
        public const string Skip = "Skip";
        public const string Sunday = "Sunday";
        public const string Monday = "Monday";
        public const string Tuesday = "Tuesday";
        public const string Wednesday = "Wednesday";
        public const string Thursday = "Thursday";
        public const string Friday = "Friday";
        public const string Saturday = "Saturday";
    }

    public static class Modes
    {
        public const string Discovery = "Discover";
        public const string Economy = "Economy";
    }

    public static class ActionNames
    {
        public const string Import = "Import";
        public const string Purge = "Purge";
        public const string Truncate = "Truncate";
        public const string Skip = "Skip";
    }

    public static class DataTypes // target_name
    {
        public const string Dividends = "Dividends";
        public const string Earnings = "Earnings";
        public const string Exchanges = "Exchanges";
        public const string Fundamentals = "Fundamentals";
        public const string Ipos = "IPOs";
        public const string Options = "Options";
        public const string Prices = "Prices";
        public const string Splits = "Splits";
        public const string Symbols = "Symbols";
        public const string Trends = "Trends";
    }

    public static class DataTypeScopes // target_scope
    {
        public const string Bulk = "Bulk";
        public const string Full = "Full";
        public const string TryBulkThenFull = "Try Bulk";
    }

    public static class PurgeName
    {
        public const string Cycles = "Cycles";
        public const string Imports = "Imports";
        public const string Logs = "Logs";
    }
}
