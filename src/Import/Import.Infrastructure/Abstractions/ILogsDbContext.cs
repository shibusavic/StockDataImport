using Import.Infrastructure.Logging;

namespace Import.Infrastructure.Abstractions;

internal interface ILogsDbContext : IDbContext
{
    Task SaveLogAsync(LogItem logItem, CancellationToken cancellationToken = default);

    Task SaveActionItemAsync(Domain.ActionItem actionItem, CancellationToken cancellationToken = default);

    Task SaveActionItemsAsync(IEnumerable<Domain.ActionItem> actions, CancellationToken cancellationToken = default);
    
    Task<Domain.ActionItem?> GetActionItemAsync(Guid globalId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Domain.ActionItem>> GetActionItemsByStatusAsync(ImportActionStatus status, CancellationToken cancellationToken = default);

    Task PurgeLogsAsync(CancellationToken cancellationToken = default);

    Task PurgeActionItemsAsync(CancellationToken cancellationToken = default);

    Task TruncateLogsAsync(string logLevel, DateTime date, CancellationToken cancellationToken = default);

    Task TruncateActionItemsAsync(DateTime date, CancellationToken cancellationToken = default);
}
