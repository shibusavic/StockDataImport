using Dapper;
using Microsoft.Extensions.Logging;

namespace Import.Infrastructure.Tests.PostgreSQL;

internal class ImportsDbContext : Infrastructure.PostgreSQL.ImportsDbContext
{
    public ImportsDbContext(string connectionString, ILogger? logger = null) : base(connectionString, logger)
    {
    }

    public async Task DeleteCompanyAsync(string symbol, string exchange,
        string type, string name,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = await GetCompanyIdAsync(symbol, exchange, type, name);

        string[] tables = new string[] {
            "public.company_addresses",
            "public.company_listings",
            "public.company_officers",
            "public.company_highlights",
            "public.company_valuations",
            "public.company_shares_stats",
            "public.company_technicals",
            "public.company_dividends",
            "public.company_dividends_by_year",
            "public.company_analyst_ratings",
            "public.company_holders",
            "public.company_insider_transactions",
            "public.company_esg_scores",
            "public.company_esg_activities",
            "public.company_outstanding_shares",
            "public.company_earnings_history",
            "public.company_earnings_trends",
            "public.company_earnings_annual",
            "public.company_balance_sheets",
            "public.company_cash_flows",
            "public.company_income_statements"
        };

        List<string> sqlStatements = new()
        {
            @"DELETE FROM public.companies WHERE global_id = @CompanyId"
        };

        foreach (var table in tables)
        {
            sqlStatements.Add($@"DELETE FROM {table} WHERE company_id = @CompanyId");
        }

        var sql = string.Join(';', sqlStatements);

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                CompanyId = id
            }, commandTimeout: 300);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }

    }

    public async Task DeleteEtfAsync(string symbol, string exchange,
        string type, string name,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = await GetEtfIdAsync(symbol, exchange, type, name);

        string[] tables = new string[] {
            "public.etf_technicals",
            "public.etf_market_capitalization",
            "public.etf_asset_allocations",
            "public.etf_world_regions",
            "public.etf_sector_weights",
            "public.etf_fixed_incomes",
            "public.etf_top_ten_holdings",
            "public.etf_holdings",
            "public.etf_valuation_growths",
            "public.etf_morning_star",
            "public.etf_performance"
        };

        List<string> sqlStatements = new()
        {
            @"DELETE FROM public.etfs WHERE global_id = @EtfId"
        };

        foreach (var table in tables)
        {
            sqlStatements.Add($@"DELETE FROM {table} WHERE etf_id = @EtfId");
        }

        var sql = string.Join(';', sqlStatements);

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                EtfId = id
            }, commandTimeout: 300);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }

    }

    public async Task DeleteAsync(string tableName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql = $@"DELETE FROM {tableName}";

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, commandTimeout: 300);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public async Task ClearOptionsAsync()
    {
        string[] sqlStatements = new string[]
        {
            Shibusa.Data.PostgeSQLSqlBuilder.CreateDelete(typeof(Infrastructure.PostgreSQL.DataAccessObjects.Options)) ?? "",
            Shibusa.Data.PostgeSQLSqlBuilder.CreateDelete(typeof(Infrastructure.PostgreSQL.DataAccessObjects.OptionData)) ?? "",
            Shibusa.Data.PostgeSQLSqlBuilder.CreateDelete(typeof(Infrastructure.PostgreSQL.DataAccessObjects.OptionContract)) ?? ""
        };

        var sql = string.Join(';', sqlStatements);

        Assert.NotNull(sql);
        Assert.NotEmpty(sql);

        using var connection = await GetOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public async Task<int> CountSymbolsAsync() => await CountForTable("public.symbols");

    public async Task<int> CountSymbolsToIgnoreAsync() => await CountForTable("public.symbols_to_ignore");

    public async Task<int> CountSplitsAsync() => await CountForTable("public.splits");

    public async Task<int> CountDividendsAsync() => await CountForTable("public.dividends");

    public async Task<int> CountPriceActionsAsync() => await CountForTable("public.price_actions");

    public async Task<int> CountExchangesAsync() => await CountForTable("public.exchanges");

    public async Task<int> CountOptionsAsync() => await CountForTable("public.options");

    public async Task<int> CountOptionDataAsync() => await CountForTable("public.option_data");

    public async Task<int> CountOptionContractAsync() => await CountForTable("public.option_contracts");

    public async Task<int> CountCompaniesAsync(string columnName, Guid value) => await CountForTable("public.companies", columnName, value);

    public async Task<int> CountCompanyAddressesAsync(string columnName, Guid value) => await CountForTable("public.company_addresses", columnName, value);

    public async Task<int> CountCompanyListingsAsync(string columnName, Guid value) => await CountForTable("public.company_listings", columnName, value);

    public async Task<int> CountCompanyOfficersAsync(string columnName, Guid value) => await CountForTable("public.company_officers", columnName, value);

    public async Task<int> CountCompanyHighlightsAsync(string columnName, Guid value) => await CountForTable("public.company_highlights", columnName, value);

    public async Task<int> CountCompanyValuationsAsync(string columnName, Guid value) => await CountForTable("public.company_valuations", columnName, value);

    public async Task<int> CountCompanyShareStatsAsync(string columnName, Guid value) => await CountForTable("public.company_shares_stats", columnName, value);

    public async Task<int> CountCompanyTechnicalsAsync(string columnName, Guid value) => await CountForTable("public.company_technicals", columnName, value);

    public async Task<int> CountCompanyDividendsAsync(string columnName, Guid value) => await CountForTable("public.company_dividends", columnName, value);

    public async Task<int> CountCompanyDividendsByYearAsync(string columnName, Guid value) => await CountForTable("public.company_dividends_by_year", columnName, value);

    public async Task<int> CountCompanyAnalystRatingsAsync(string columnName, Guid value) => await CountForTable("public.company_analyst_ratings", columnName, value);

    public async Task<int> CountCompanyHoldersAsync(string columnName, Guid value) => await CountForTable("public.company_holders", columnName, value);

    public async Task<int> CountCompanyInsiderTransactionsAsync(string columnName, Guid value) => await CountForTable("public.company_insider_transactions", columnName, value);

    public async Task<int> CountCompanyEsgScoresAsync(string columnName, Guid value) => await CountForTable("public.company_esg_scores", columnName, value);

    public async Task<int> CountCompanyEsgActivitiesAsync(string columnName, Guid value) => await CountForTable("public.company_esg_activities", columnName, value);

    public async Task<int> CountCompanyOutstandingSharesAsync(string columnName, Guid value) => await CountForTable("public.company_outstanding_shares", columnName, value);

    public async Task<int> CountCompanyEarningsHistoryAsync(string columnName, Guid value) => await CountForTable("public.company_earnings_history", columnName, value);

    public async Task<int> CountCompanyEarningsTrendsAsync(string columnName, Guid value) => await CountForTable("public.company_earnings_trends", columnName, value);

    public async Task<int> CountCompanyEarningsAnnualAsync(string columnName, Guid value) => await CountForTable("public.company_earnings_annual", columnName, value);

    public async Task<int> CountCompanyBalanceSheetsAsync(string columnName, Guid value) => await CountForTable("public.company_balance_sheets", columnName, value);

    public async Task<int> CountCompanyCashFlowsAsync(string columnName, Guid value) => await CountForTable("public.company_cash_flows", columnName, value);

    public async Task<int> CountCompanyIncomeStatementsAsync(string columnName, Guid value) => await CountForTable("public.company_income_statements", columnName, value);

    public async Task<int> CountEtfsAsync(string columnName, Guid value) => await CountForTable("public.etfs", columnName, value);

    public async Task<int> CountEtfTechnicalsAsync(string columnName, Guid value) => await CountForTable("public.etf_technicals", columnName, value);

    public async Task<int> CountEtfMarketCapitalizationAsync(string columnName, Guid value) => await CountForTable("public.etf_market_capitalization", columnName, value);

    public async Task<int> CountEtfAssetAllocationAsync(string columnName, Guid value) => await CountForTable("public.etf_asset_allocations", columnName, value);

    public async Task<int> CountEtfWorldRegionsAsync(string columnName, Guid value) => await CountForTable("public.etf_world_regions", columnName, value);

    public async Task<int> CountEtfSectorWeightsAsync(string columnName, Guid value) => await CountForTable("public.etf_sector_weights", columnName, value);

    public async Task<int> CountEtfFixedIncomesAsync(string columnName, Guid value) => await CountForTable("public.etf_fixed_incomes", columnName, value);

    public async Task<int> CountEtfTopTenHoldingsAsync(string columnName, Guid value) => await CountForTable("public.etf_top_ten_holdings", columnName, value);

    public async Task<int> CountEtfHoldingsAsync(string columnName, Guid value) => await CountForTable("public.etf_holdings", columnName, value);

    public async Task<int> CountEtfValuationGrowthAsync(string columnName, Guid value) => await CountForTable("public.etf_valuation_growths", columnName, value);

    public async Task<int> CountEtfMorningStarAsync(string columnName, Guid value) => await CountForTable("public.etf_morning_star", columnName, value);

    public async Task<int> CountEtfPerformanceAsync(string columnName, Guid value) => await CountForTable("public.etf_performance", columnName, value);

    public async Task<int> CountIposAsync() => await CountForTable("public.calendar_ipos");

    public async Task<int> CountTrendsAsync() => await CountForTable("public.calendar_trends");

    public async Task<int> CountEarningsAsync() => await CountForTable("public.calendar_earnings");

    private async Task<int> CountForTable(string tableName)
    {
        string sql = $"SELECT COUNT(*) FROM {tableName}";

        using var connection = await GetOpenConnectionAsync();

        try
        {
            return await connection.QuerySingleAsync<int>(sql);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    private async Task<int> CountForTable(string tableName, string uuidColumnName, Guid value)
    {
        string sql = $"SELECT COUNT(*) FROM {tableName} WHERE {uuidColumnName} = @Id";

        using var connection = await GetOpenConnectionAsync();

        try
        {
            return await connection.QuerySingleAsync<int>(sql, new { Id = value });
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}