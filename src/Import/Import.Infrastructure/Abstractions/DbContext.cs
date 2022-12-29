using Microsoft.Extensions.Logging;
using Shibusa.Data.Abstractions;

namespace Import.Infrastructure.Abstractions
{
    /// <summary>
    /// Base database context, housing the connection string and database engine.
    /// </summary>
    internal abstract class DbContext : IDbContext
    {
        protected ILogger? logger;

        protected DbContext(string connectionString, ILogger? logger = null)
        {
            ConnectionString = connectionString;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the connection string for this context.
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Gets the engine for this context.
        /// </summary>
        public abstract DatabaseEngine DatabaseEngine { get; }
    }
}
