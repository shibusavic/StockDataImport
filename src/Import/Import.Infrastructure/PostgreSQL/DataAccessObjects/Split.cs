using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;
namespace Import.Infrastructure.PostgreSQL.DataAccessObjects
{
    [Table(name: "splits", Schema = "public")]
    internal class Split
    {
        public Split(
            string symbol,
            string exchange,
            DateTime dateOfSplit,
            double beforeSplit,
            double afterSplit,
            DateTime utcTimestamp)
        {
            Symbol = symbol;
            Exchange = exchange;
            DateOfSplit = dateOfSplit;
            BeforeSplit = beforeSplit;
            AfterSplit = afterSplit;
            UtcTimestamp = utcTimestamp;
        }

        public Split(Domain.Split split)
        {
            Symbol = split.Symbol;
            Exchange = split.Exchange;
            DateOfSplit = split.DateOfSplit.ToDateTime(TimeOnly.MinValue);
            BeforeSplit = Math.Round(split.SharesBeforeSplit, 2);
            AfterSplit = Math.Round(split.SharesAfterSplit, 2);
        }


        [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
        public string Symbol { get; }

        [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
        public string Exchange { get; }

        [ColumnWithKey("date_of_split", Order = 3, TypeName = "date", IsPartOfKey = true)]
        public DateTime DateOfSplit { get; }

        [ColumnWithKey("before_split", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
        public double BeforeSplit { get; }

        [ColumnWithKey("after_split", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
        public double AfterSplit { get; }

        [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
        public DateTime UtcTimestamp { get; }
    }
}
