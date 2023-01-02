using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;
namespace Import.Infrastructure.PostgreSQL.DataAccessObjects
{
    [Table(name: "logs_extended", Schema = "public")]
    internal class LogExtended
    {
        public LogExtended(
            Guid logId,
            string logKey,
            string logValue,
            DateTime utcTimestamp)
        {
            LogId = logId;
            LogKey = logKey;
            LogValue = logValue;
            UtcTimestamp = utcTimestamp;
        }


        [ColumnWithKey("log_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
        public Guid LogId { get; }

        [ColumnWithKey("log_key", Order = 2, TypeName = "text", IsPartOfKey = true)]
        public string LogKey { get; }

        [ColumnWithKey("log_value", Order = 3, TypeName = "text", IsPartOfKey = false)]
        public string LogValue { get; }

        [ColumnWithKey("utc_timestamp", Order = 4, TypeName = "timestamp with time zone", IsPartOfKey = false)]
        public DateTime UtcTimestamp { get; }
    }
}
