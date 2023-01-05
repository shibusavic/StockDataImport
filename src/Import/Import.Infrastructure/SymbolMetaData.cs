namespace Import.Infrastructure
{
    internal class SymbolMetaData : IEquatable<SymbolMetaData?>
    {
        public SymbolMetaData(string symbol, string? exchange = null)
            : this(symbol, exchange, DateTime.UtcNow)
        {
        }

        internal SymbolMetaData(string symbol, string? exchange, DateTime lastUpdated, bool hasOptions = false)
        {
            Symbol = symbol;
            Exchange = exchange;
            LastUpdated = lastUpdated;
            HasOptions = hasOptions;
        }

        public string Symbol { get; }

        public string? Exchange { get; }

        public string Code => ToString();

        public string? Type { get; internal set; }

        public bool HasOptions { get; internal set; }

        public string? Sector { get; internal set; }

        public string? Industry { get; internal set; }

        public (DateTime? Start, decimal? Close) LastTrade { get; internal set; }

        public DateTime LastUpdated { get; internal set; }

        public DateTime? LastUpdatedOptions { get; internal set; }

        public DateTime? LastUpdatedCompany { get; internal set; }

        public DateTime? LastUpdatedIncomeStatement { get; internal set; }

        //public bool ReadyForFundamentalsUpdate => 


        public override string ToString() => Exchange == null ? Symbol : $"{Symbol}.{Exchange}";

        public override bool Equals(object? obj)
        {
            return Equals(obj as SymbolMetaData);
        }

        public bool Equals(SymbolMetaData? other)
        {
            return other is not null &&
                   Code == other.Code;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Code);
        }

        public static bool operator ==(SymbolMetaData? left, SymbolMetaData? right)
        {
            return EqualityComparer<SymbolMetaData>.Default.Equals(left, right);
        }

        public static bool operator !=(SymbolMetaData? left, SymbolMetaData? right)
        {
            return !(left == right);
        }
    }
}
