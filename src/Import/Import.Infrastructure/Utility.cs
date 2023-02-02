namespace Import.Infrastructure
{
    internal static class Utility
    {
        public static (string? Symbol, string? ExchangeCode) ParseSymbolCode(string? code)
        {
            if (code == null) return (null, null);
            var split = code.Split('.');
            string? s = split[0];
            string? e = split.Length > 1 ? split[1] : null;
            return (s, e);
        }
    }
}
