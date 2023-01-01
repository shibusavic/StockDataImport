using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;
namespace Import.Infrastructure.PostgreSQL.DataAccessObjects
{
    [Table(name: "etfs", Schema = "public")]
    internal class Etf
    {
        public Etf(EodHistoricalData.Sdk.Models.Fundamentals.Etf.EtfFundamentalsCollection etf, Guid? etfId = null)
        {
            GlobalId = etfId ?? Guid.NewGuid();
            DateCaptured = DateTime.UtcNow;
            Symbol = etf.General.Code;
            Exchange = etf.General.Exchange;
            Name = etf.General.Name;
            Type = etf.General.Type;
            CurrencyCode = etf.General.CurrencyCode;
            CurrencyName = etf.General.CurrencyName;
            CurrencySymbol = etf.General.CurrencySymbol;
            CountryName = etf.General.CountryName;
            CountryIso = etf.General.CountryIso;
            Description = etf.General.Description;
            Category = etf.General.Category;
            UpdateAt = etf.General.UpdatedAt.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
            Isin = etf.Data.Isin;
            CompanyName = etf.Data.CompanyName;
            CompanyUrl = etf.Data.Url; // TODO: not sure about this one.
            EtfUrl = etf.Data.Url;
            Domicile = etf.Data.Domicile;
            IndexName = etf.Data.IndexName ?? "Unknown";
            Yield = etf.Data.Yield.GetValueOrDefault();
            DividendPayingFrequency = etf.Data.DividendPayingFrequency;
            InceptionDate = etf.Data.InceptionDate.ToDateTime(TimeOnly.MinValue);
            MaxAnnualMgmtCharge = Convert.ToDecimal(etf.Data.MaxAnnualManagementCharge);
            OngoingCharge = Convert.ToDecimal(etf.Data.OngoingCharge);
            DateOngoingCharge = etf.Data.DateOngoingCharge.ToDateTime(TimeOnly.MinValue);
            NetExpenseRatio = Convert.ToDouble(etf.Data.NetExpenseRatio);
            AnnualHoldingsTurnover = Convert.ToDouble(etf.Data.AnnualHoldingsTurnover);
            TotalAssets = Convert.ToDecimal(etf.Data.TotalAssets);
            AverageMktCapMln = Convert.ToDecimal(etf.Data.AverageMarketCapitalizationMillions);
            HoldingsCount = etf.Data.HoldingsCount;
            UtcTimestamp = DateTime.UtcNow;
        }

        public Etf(
            Guid globalId,
            DateTime dateCaptured,
            string symbol,
            string exchange,
            string name,
            string type,
            string currencyCode,
            string currencyName,
            string currencySymbol,
            string countryName,
            string countryIso,
            string description,
            string category,
            DateTime updateAt,
            string isin,
            string companyName,
            string companyUrl,
            string etfUrl,
            string domicile,
            string indexName,
            double yield,
            string dividendPayingFrequency,
            DateTime inceptionDate,
            decimal maxAnnualMgmtCharge,
            decimal ongoingCharge,
            DateTime? dateOngoingCharge,
            double netExpenseRatio,
            double annualHoldingsTurnover,
            decimal totalAssets,
            decimal averageMktCapMln,
            int holdingsCount,
            DateTime utcTimestamp)
        {
            GlobalId = globalId;
            DateCaptured = dateCaptured;
            Symbol = symbol;
            Exchange = exchange;
            Name = name;
            Type = type;
            CurrencyCode = currencyCode;
            CurrencyName = currencyName;
            CurrencySymbol = currencySymbol;
            CountryName = countryName;
            CountryIso = countryIso;
            Description = description;
            Category = category;
            UpdateAt = updateAt;
            Isin = isin;
            CompanyName = companyName;
            CompanyUrl = companyUrl;
            EtfUrl = etfUrl;
            Domicile = domicile;
            IndexName = indexName;
            Yield = yield;
            DividendPayingFrequency = dividendPayingFrequency;
            InceptionDate = inceptionDate;
            MaxAnnualMgmtCharge = maxAnnualMgmtCharge;
            OngoingCharge = ongoingCharge;
            DateOngoingCharge = dateOngoingCharge;
            NetExpenseRatio = netExpenseRatio;
            AnnualHoldingsTurnover = annualHoldingsTurnover;
            TotalAssets = totalAssets;
            AverageMktCapMln = averageMktCapMln;
            HoldingsCount = holdingsCount;
            UtcTimestamp = utcTimestamp;
        }


        [ColumnWithKey("global_id", Order = 1, TypeName = "uuid", IsPartOfKey = false)]
        public Guid GlobalId { get; }

        [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
        public DateTime DateCaptured { get; }

        [ColumnWithKey("symbol", Order = 3, TypeName = "text", IsPartOfKey = true)]
        public string Symbol { get; }

        [ColumnWithKey("exchange", Order = 4, TypeName = "text", IsPartOfKey = true)]
        public string Exchange { get; }

        [ColumnWithKey("name", Order = 5, TypeName = "text", IsPartOfKey = false)]
        public string Name { get; }

        [ColumnWithKey("type", Order = 6, TypeName = "text", IsPartOfKey = true)]
        public string Type { get; }

        [ColumnWithKey("currency_code", Order = 7, TypeName = "text", IsPartOfKey = false)]
        public string CurrencyCode { get; }

        [ColumnWithKey("currency_name", Order = 8, TypeName = "text", IsPartOfKey = false)]
        public string CurrencyName { get; }

        [ColumnWithKey("currency_symbol", Order = 9, TypeName = "text", IsPartOfKey = false)]
        public string CurrencySymbol { get; }

        [ColumnWithKey("country_name", Order = 10, TypeName = "text", IsPartOfKey = false)]
        public string CountryName { get; }

        [ColumnWithKey("country_iso", Order = 11, TypeName = "text", IsPartOfKey = false)]
        public string CountryIso { get; }

        [ColumnWithKey("description", Order = 12, TypeName = "text", IsPartOfKey = false)]
        public string Description { get; }

        [ColumnWithKey("category", Order = 13, TypeName = "text", IsPartOfKey = false)]
        public string Category { get; }

        [ColumnWithKey("update_at", Order = 14, TypeName = "date", IsPartOfKey = false)]
        public DateTime UpdateAt { get; }

        [ColumnWithKey("isin", Order = 15, TypeName = "text", IsPartOfKey = false)]
        public string Isin { get; }

        [ColumnWithKey("company_name", Order = 16, TypeName = "text", IsPartOfKey = false)]
        public string CompanyName { get; }

        [ColumnWithKey("company_url", Order = 17, TypeName = "text", IsPartOfKey = false)]
        public string CompanyUrl { get; }

        [ColumnWithKey("etf_url", Order = 18, TypeName = "text", IsPartOfKey = false)]
        public string EtfUrl { get; }

        [ColumnWithKey("domicile", Order = 19, TypeName = "text", IsPartOfKey = false)]
        public string Domicile { get; }

        [ColumnWithKey("index_name", Order = 20, TypeName = "text", IsPartOfKey = false)]
        public string IndexName { get; }

        [ColumnWithKey("yield", Order = 21, TypeName = "double precision", IsPartOfKey = false)]
        public double Yield { get; }

        [ColumnWithKey("dividend_paying_frequency", Order = 22, TypeName = "text", IsPartOfKey = false)]
        public string DividendPayingFrequency { get; }

        [ColumnWithKey("inception_date", Order = 23, TypeName = "date", IsPartOfKey = false)]
        public DateTime InceptionDate { get; }

        [ColumnWithKey("max_annual_mgmt_charge", Order = 24, TypeName = "numeric", IsPartOfKey = false)]
        public decimal MaxAnnualMgmtCharge { get; }

        [ColumnWithKey("ongoing_charge", Order = 25, TypeName = "numeric", IsPartOfKey = false)]
        public decimal OngoingCharge { get; }

        [ColumnWithKey("date_ongoing_charge", Order = 26, TypeName = "date", IsPartOfKey = false)]
        public DateTime? DateOngoingCharge { get; }

        [ColumnWithKey("net_expense_ratio", Order = 27, TypeName = "double precision", IsPartOfKey = false)]
        public double NetExpenseRatio { get; }

        [ColumnWithKey("annual_holdings_turnover", Order = 28, TypeName = "double precision", IsPartOfKey = false)]
        public double AnnualHoldingsTurnover { get; }

        [ColumnWithKey("total_assets", Order = 29, TypeName = "numeric", IsPartOfKey = false)]
        public decimal TotalAssets { get; }

        [ColumnWithKey("average_mkt_cap_mln", Order = 30, TypeName = "numeric", IsPartOfKey = false)]
        public decimal AverageMktCapMln { get; }

        [ColumnWithKey("holdings_count", Order = 31, TypeName = "integer", IsPartOfKey = false)]
        public int HoldingsCount { get; }

        [ColumnWithKey("utc_timestamp", Order = 32, TypeName = "timestamp with time zone", IsPartOfKey = false)]
        public DateTime UtcTimestamp { get; }
    }
}
