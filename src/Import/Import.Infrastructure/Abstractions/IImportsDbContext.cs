using EodHistoricalData.Sdk.Models;

namespace Import.Infrastructure.Abstractions;

internal partial interface IImportsDbContext : IDbContext
{
    /// <summary>
    /// Check to determine if the "On Empty Database" actions should be invoked.
    /// </summary>
    /// <returns>A task representing the asyncronous operation; the task contains a boolean indicating
    /// whether the "On Empty Database" actions should be executed.
    /// </returns>
    Task<bool> IsDatabaseEmptyAsync();

    Task PurgeAsync(CancellationToken cancellationToken = default);

    Task SaveSymbolsAsync(IEnumerable<Symbol> symbols, CancellationToken cancellationToken = default);

    Task SetOptionableOnSymbolsAsync(IEnumerable<string> symbols, CancellationToken cancellationToken = default);

    Task<IEnumerable<Symbol>> GetAllSymbolsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Symbol>> GetSymbolsForExchangeAsync(string exchange, CancellationToken cancellationToken);

    Task<Symbol> GetSymbolAsync(string symbol, string? exchange = null, CancellationToken cancellationToken = default);

    Task SaveSplitsAsync(IEnumerable<Domain.Split> splits, CancellationToken cancellationToken = default);

    Task SavePriceActionsAsync(string symbol, string exchange, IEnumerable<PriceAction> priceActions, CancellationToken cancellationToken = default);

    Task SaveDividendsAsync(string symbol, string exchange, IEnumerable<Dividend> dividends, CancellationToken cancellationToken = default);

    Task SaveExchangesAsync(IEnumerable<Exchange> exchanges, CancellationToken cancellationToken = default);

    Task<IEnumerable<Symbol>> GetSymbolsWithOptionsAsync(CancellationToken cancellationToken = default);

    Task SaveOptionsAsync(EodHistoricalData.Sdk.Models.Options.OptionsCollection options, CancellationToken cancellationToken = default);
}
