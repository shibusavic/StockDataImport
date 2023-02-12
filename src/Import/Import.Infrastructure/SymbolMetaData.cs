using EodHistoricalData.Sdk;
using Shibusa.Extensions;

namespace Import.Infrastructure
{
    public class SymbolMetaData : IEquatable<SymbolMetaData?>
    {
        public SymbolMetaData(string code, string symbol, string? exchange, string? type)
        {
            Code = code;
            Symbol = symbol;
            Exchange = exchange;
            Type = type;
        }

        public string Code { get; }

        public string Symbol { get; }

        public string? Exchange { get; }

        public string? Type { get; set; }

        public bool UseCompanyFundamentals => Type != null &&
            (Type.Equals(SymbolType.CommonStock.GetDescription()) ||
            Type.Equals(SymbolType.PreferredStock.GetDescription()));

        public bool UseEtfFundamentals => Type != null &&
            (Type.Equals(SymbolType.Etf.GetDescription()) ||
            Type.Equals(SymbolType.Fund.GetDescription()) ||
            Type.Equals(SymbolType.MutualFund.GetDescription()));

        public string? Sector { get; internal set; }

        public string? Industry { get; internal set; }

        public (DateTime? Start, decimal? Close) LastTrade { get; internal set; }

        public DateTime LastUpdated { get; internal set; }

        public DateTime? LastUpdatedOptions { get; internal set; }

        public DateTime? LastUpdatedEntity { get; internal set; }

        public DateTime? LastUpdatedFinancials { get; internal set; }

        public bool HasSplits { get; internal set; }

        public bool HasDividends { get; internal set; }

        public bool HasOptions { get; internal set; }

        public bool RequiresFundamentalUpdate => Type != null
            && (UseCompanyFundamentals ? LastUpdatedFinancials.GetValueOrDefault() < DateTime.Now.AddDays(-90)
                    : UseEtfFundamentals && LastUpdatedEntity.GetValueOrDefault() < DateTime.Now.AddDays(-7));

        internal void Update() => LastUpdated = DateTime.UtcNow;

        internal void Update(SymbolMetaData metaDataItem, bool allowReplacementWithNull = false)
        {
            Update();

            Sector = Sector == null || allowReplacementWithNull ? metaDataItem.Sector
                : metaDataItem.Sector ?? Sector;

            Industry = Industry == null || allowReplacementWithNull ? metaDataItem.Sector
                : metaDataItem.Industry ?? Industry;

            LastTrade = LastTrade.Start == null || allowReplacementWithNull ? metaDataItem.LastTrade
                : metaDataItem.LastTrade.Start == null ? LastTrade : metaDataItem.LastTrade;

            LastUpdatedOptions = LastUpdatedOptions == null || allowReplacementWithNull ? metaDataItem.LastUpdatedOptions
                : metaDataItem.LastUpdatedOptions == null ? LastUpdatedOptions : metaDataItem.LastUpdatedOptions;

            LastUpdatedEntity = LastUpdatedEntity == null || allowReplacementWithNull ? metaDataItem.LastUpdatedEntity
                : metaDataItem.LastUpdatedEntity == null ? LastUpdatedEntity : metaDataItem.LastUpdatedEntity;

            LastUpdatedFinancials = LastUpdatedFinancials == null || allowReplacementWithNull ? metaDataItem.LastUpdatedFinancials
                : metaDataItem.LastUpdatedFinancials == null ? LastUpdatedFinancials : metaDataItem.LastUpdatedFinancials;

            HasSplits = metaDataItem.HasSplits;
            HasDividends = metaDataItem.HasDividends;
            HasOptions = metaDataItem.HasOptions;
        }

        public override string ToString() => Code;

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
