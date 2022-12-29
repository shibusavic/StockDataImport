using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;
namespace Import.Infrastructure.PostgreSQL.DataAccessObjects
{
    [Table(name: "etf_fixed_incomes", Schema = "public")]
    internal class EtfFixedIncome
    {
        public EtfFixedIncome(Guid etfId,
            string category,
            EodHistoricalData.Sdk.Models.Fundamentals.Etf.FundWeightItem fixedIncome)
        {
            EtfId = etfId;
            DateCaptured = DateTime.UtcNow;
            Category = category;
            FundPercentage = Convert.ToDouble(fixedIncome.FundPercentage);
            RelativeToCategory = Convert.ToDouble(fixedIncome.RelativeToCategory);
            UtcTimestamp = DateTime.UtcNow;
        }

        public EtfFixedIncome(
            Guid etfId,
            DateTime dateCaptured,
            string category,
            double fundPercentage,
            double relativeToCategory,
            DateTime utcTimestamp)
        {
            EtfId = etfId;
            DateCaptured = dateCaptured;
            Category = category;
            FundPercentage = fundPercentage;
            RelativeToCategory = relativeToCategory;
            UtcTimestamp = utcTimestamp;
        }


        [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
        public Guid EtfId { get; }

        [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
        public DateTime DateCaptured { get; }

        [ColumnWithKey("category", Order = 3, TypeName = "text", IsPartOfKey = false)]
        public string Category { get; }

        [ColumnWithKey("fund_percentage", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
        public double FundPercentage { get; }

        [ColumnWithKey("relative_to_category", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
        public double RelativeToCategory { get; }

        [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
        public DateTime UtcTimestamp { get; }
    }
}
