using Dapper;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    public async Task SaveEtfAsync(EodHistoricalData.Sdk.Models.Fundamentals.Etf.EtfFundamentalsCollection etf,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var etfId = await GetEtfIdAsync(etf.General.Code, etf.General.Exchange, etf.General.Type, etf.General.Name);

        var etfGeneral = new DataAccessObjects.Etf(etf, etfId);
        var technicals = new DataAccessObjects.EtfTechnicals(etfId, etf.Technicals);
        var marketCapitaliation = new DataAccessObjects.EtfMarketCapitalization(etfId, etf.Data.MarketCapitalization);

        List<DataAccessObjects.EtfAssetAllocation> assetAllocations = new();
        List<DataAccessObjects.EtfWorldRegion> worldRegions = new();
        List<DataAccessObjects.EtfSectorWeight> sectorWeights = new();
        List<DataAccessObjects.EtfFixedIncome> fixedIncomes = new();
        List<DataAccessObjects.EtfValuationGrowth> growths = new();

        if (etf.Data.AssetAllocations != null)
        {
            foreach (var kvp in etf.Data.AssetAllocations)
            {
                assetAllocations.Add(new DataAccessObjects.EtfAssetAllocation(etfId,
                    kvp.Key, Convert.ToDouble(kvp.Value.LongPercentage),
                    Convert.ToDouble(kvp.Value.ShortPercentage),
                    Convert.ToDouble(kvp.Value.NetAssetsPercentage), DateTime.UtcNow));
            }
        }

        if (etf.Data.WorldRegions != null)
        {
            foreach (var kvp in etf.Data.WorldRegions)
            {
                worldRegions.Add(new DataAccessObjects.EtfWorldRegion(etfId,
                    kvp.Key, Convert.ToDouble(kvp.Value.EquityPercentage),
                    Convert.ToDouble(kvp.Value.RelativeToCategory), DateTime.UtcNow));
            }
        }

        if (etf.Data.SectorWeights != null)
        {

            foreach (var kvp in etf.Data.SectorWeights)
            {
                sectorWeights.Add(new DataAccessObjects.EtfSectorWeight(etfId,
                    kvp.Key, Convert.ToDouble(kvp.Value.EquityPercentage),
                    Convert.ToDouble(kvp.Value.RelativeToCategory), DateTime.UtcNow));
            }

        }

        if (etf.Data.FixedIncome != null)
        {
            foreach (var kvp in etf.Data.FixedIncome)
            {
                fixedIncomes.Add(new DataAccessObjects.EtfFixedIncome(etfId,
                    kvp.Key,
                    Convert.ToDouble(kvp.Value.FundPercentage),
                    Convert.ToDouble(kvp.Value.RelativeToCategory), DateTime.UtcNow));
            }
        }

        DataAccessObjects.EtfTopTenHoldings[] top10Holdings = Array.Empty<DataAccessObjects.EtfTopTenHoldings>();
        DataAccessObjects.EtfHolding[] holdings = Array.Empty<DataAccessObjects.EtfHolding>();

        if (etf.Data.TopTenHoldings != null)
        {
            top10Holdings = etf.Data.TopTenHoldings.Values
                .Select(v => new DataAccessObjects.EtfTopTenHoldings(etfId, v)).ToArray();
        }

        if (etf.Data.Holdings != null)
        {
            holdings = etf.Data.Holdings.Values
                   .Select(h => new DataAccessObjects.EtfHolding(etfId, h)).ToArray();
        }

        if (etf.Data.ValuationsGrowth != null)
        {
            foreach (var kvp in etf.Data.ValuationsGrowth)
            {
                try
                {
                    growths.Add(new DataAccessObjects.EtfValuationGrowth(etfId,
                        kvp.Key, Convert.ToDouble(kvp.Value.PriceProspectiveEarnings),
                        Convert.ToDouble(kvp.Value.PriceBook),
                        Convert.ToDouble(kvp.Value.PriceSales),
                        Convert.ToDouble(kvp.Value.PriceCashFlow),
                        Convert.ToDouble(kvp.Value.DividendYieldFactor), DateTime.UtcNow));
                }
                catch (Exception ex)
                {
                    string m = ex.Message;
                }
            }
        }

        List<Task> taskList = new()
        {
            Task.Run(() => SaveFundamentalDataAsync(etfGeneral,cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(technicals,cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(marketCapitaliation,cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(assetAllocations.ToArray(),cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(worldRegions.ToArray(),cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(sectorWeights.ToArray(),cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(fixedIncomes.ToArray(),cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(top10Holdings,cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(holdings,cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(growths.ToArray(),cancellationToken), cancellationToken),
        };

        if (!etf.Data.MorningStar.Equals(default))
        {
            var morningStar = new DataAccessObjects.EtfMorningStar(etfId, etf.Data.MorningStar);
            taskList.Add(Task.Run(() => SaveFundamentalDataAsync(morningStar, cancellationToken), cancellationToken));
        }

        if (!etf.Data.Performance.Equals(default))
        {
            var performance = new DataAccessObjects.EtfPerformance(etfId, etf.Data.Performance);
            taskList.Add(Task.Run(() => SaveFundamentalDataAsync(performance, cancellationToken), cancellationToken));
        }

        Task.WaitAll(taskList.ToArray(), cancellationToken);
    }

    internal async Task<Guid> GetEtfIdAsync(string symbol, string exchange,
        string type, string name)
    {
        using var connection = await GetOpenConnectionAsync();

        try
        {
            string sql = @"SELECT global_id FROM public.etfs
WHERE symbol = @Symbol AND exchange = @Exchange AND type = @Type AND name = @Name";

            var id = await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new
            {
                Symbol = symbol,
                Exchange = exchange,
                Type = type,
                Name = name
            });

            return id ?? Guid.NewGuid();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
