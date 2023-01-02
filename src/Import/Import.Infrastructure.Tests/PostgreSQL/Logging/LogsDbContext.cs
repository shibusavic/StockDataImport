using Dapper;
using Microsoft.Extensions.Logging;

namespace Import.Infrastructure.IntegrationTests.PostgreSQL.Logging
{
    internal class LogsDbContext : Infrastructure.PostgreSQL.LogsDbContext
    {
        public LogsDbContext(string connectionString, ILogger? logger = null) : base(connectionString, logger)
        {
        }

        internal async Task<IEnumerable<Guid>> GetAllLogIdsAsync()
        {
            var connection = await GetOpenConnectionAsync();

            string sql = @"SELECT global_id FROM logs";

            try
            {
                return await connection.QueryAsync<Guid>(sql);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        internal async Task<IEnumerable<Guid>> GetAllLogsIdsBeforeDateAsync(DateTime date)
        {
            const string sql = @"SELECT global_id FROM logs WHERE utc_timestamp < @Date";

            var connection = await GetOpenConnectionAsync();

            try
            {
                return await connection.QueryAsync<Guid>(sql, new
                {
                    Date = date
                });
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        internal async Task<int> CountActionLogsAsync()
        {
            const string sql = @"SELECT COUNT(1) FROM action_items";

            var connection = await GetOpenConnectionAsync();

            try
            {
                return await connection.QuerySingleAsync<int>(sql);
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}
