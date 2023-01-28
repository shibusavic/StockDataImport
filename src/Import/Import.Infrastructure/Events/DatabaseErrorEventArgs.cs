namespace Import.Infrastructure.Events
{
    public class DatabaseErrorEventArgs : EventArgs
    {
        public DatabaseErrorEventArgs(string source, Exception exception,
            string? sql = null,
            object? parameters = null)
        {
            Source = source;
            Sql = sql;
            Parameters = parameters;
            Exception = exception;
        }

        public string Source { get; }

        public Exception Exception { get; }

        public string? Sql { get; }

        public object? Parameters { get; }
    }
}
