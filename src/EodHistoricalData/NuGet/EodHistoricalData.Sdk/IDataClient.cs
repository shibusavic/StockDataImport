using EodHistoricalData.Sdk.Models;
using EodHistoricalData.Sdk.Models.Bulk;
using EodHistoricalData.Sdk.Models.Calendar;
using EodHistoricalData.Sdk.Models.Options;

namespace EodHistoricalData.Sdk
{
    /// <summary>
    /// The interface for IDataClient.
    /// </summary>
    /// <remarks>This interface only exists to faciliate easy mocking of the  <see cref="DataClient"/> class for injection purposes,
    /// to cut down on API call costs during automated testing.</remarks>
    public interface IDataClient
    {
        event DataClient.ApiResponseExceptionHandler? ApiResponseExceptionEventHandler;

        event DataClient.CommunicationHandler? CommunicationEventHandler;

        event DataClient.ApiLimitReachedHandler? ApiLimitReachedEventHandler;
        Task<IEnumerable<BulkDividend>> GetBulkDividendsForExchangeAsync(string exchangeCode, DateOnly? date = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<BulkPriceAction>> GetBulkHistoricalDataForExchangeAsync(string exchangeCode, IEnumerable<string>? symbols = null, DateOnly? date = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<BulkSplit>> GetBulkSplitsForExchangeAsync(string exchangeCode, DateOnly? date = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<Dividend>> GetDividendsForSymbolAsync(string symbol, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);

        Task<EarningsCollection> GetEarningsForSymbolsAsync(IEnumerable<string> symbols, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);

        Task<EarningsCollection> GetEarningsForSymbolsAsync(string symbols, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<Exchange>> GetExchangeListAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<BulkExtendedPriceAction>> GetExtendedBulkHistoricalDataForExchangeAsync(string exchangeCode, IEnumerable<string>? symbols = null, DateOnly? date = null, CancellationToken cancellationToken = default);

        Task<object> GetFundamentalsForSymbolAsync(string symbol, CancellationToken cancellationToken = default);

        Task<T> GetFundamentalsForSymbolAsync<T>(string symbol, CancellationToken cancellationToken = default) where T : struct;

        Task<IEnumerable<PriceAction>> GetPricesForSymbolAsync(string symbol, string period = "d", string order = "a", DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);

        Task<IpoCollection> GetIposAsync(DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);

        Task<OptionsCollection> GetOptionsForSymbolAsync(string symbol, string? contractName = null, DateOnly? from = null, DateOnly? to = null, DateOnly? tradeDateFrom = null, DateOnly? tradeDateTo = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<Split>> GetSplitsForSymbolAsync(string symbol, DateOnly? from = null, DateOnly? to = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<Symbol>> GetSymbolListAsync(string exchangeCode, CancellationToken cancellationToken = default);

        Task<TrendCollection> GetTrendsForSymbolsAsync(string symbols, CancellationToken cancellationToken = default);

        Task<(int Requests, int Limit)> GetUsageAsync();

        Task<(int Requests, int Limit)> ResetUsageAsync(int limit = 100000);
    }
}