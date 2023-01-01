namespace Import.Infrastructure
{
    internal class SymbolMetaData : IEquatable<SymbolMetaData?>
    {
        public SymbolMetaData(string symbol, string? exchange = null)
            : this(symbol, exchange, DateTime.UtcNow)
        {
        }

        internal SymbolMetaData(string symbol, string? exchange, DateTime lastUpdated)
        {
            Symbol = symbol;
            Exchange = exchange;
            LastUpdated = lastUpdated;
        }

        public string Symbol { get; }

        public string? Exchange { get; }

        public string Code => ToString();

        public DateTime LastUpdated { get; internal set; }

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
