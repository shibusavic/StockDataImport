using Dapper;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    public async Task SaveCompanyAsync(EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.FundamentalsCollection company,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var companyId = await GetCompanyIdAsync(company.General.Code,
            company.General.Exchange, company.General.Type, company.General.Name);

        var companyGeneral = new DataAccessObjects.Company(company, companyId);

        var address = company.General.AddressData == null ? null :
            new DataAccessObjects.CompanyAddress(companyId, company.General.AddressData.GetValueOrDefault());

        var listings = company.General.Listings == null ? null
            : company.General.Listings.Values.Select(v => new DataAccessObjects.CompanyListing(companyId, v)).ToArray();

        var officers = company.General.Officers == null ? null : company.General.Officers.Values
            .Select(v => new DataAccessObjects.CompanyOfficer(companyId, v)).ToArray();

        var highlights = new DataAccessObjects.CompanyHighlights(companyId, company.Highlights);
        var valuation = new DataAccessObjects.CompanyValuation(companyId, company.Valuation);
        var shareStats = new DataAccessObjects.CompanySharesStat(companyId, company.SharesStats);
        var technicals = new DataAccessObjects.CompanyTechnicals(companyId, company.Technicals);
        var dividend = new DataAccessObjects.CompanyDividend(companyId, company.SplitsDividends);
        var dividendsByYear = company.SplitsDividends.NumberDividendsByYear == null ? null
            : company.SplitsDividends.NumberDividendsByYear.Values
                .Select(v => new DataAccessObjects.CompanyDividendsByYear(companyId, v)).ToArray();
        var analystRatings = new DataAccessObjects.CompanyAnalystRating(companyId, company.AnalystRatings);

        var institutionHolders = company.Holders.Institutions == null ? null
            : company.Holders.Institutions.Values
            .Select(h => new DataAccessObjects.CompanyHolder(companyId, "Institution", h));

        var fundHolders = company.Holders.Funds == null ? null 
            : company.Holders.Funds.Values
                .Select(h => new DataAccessObjects.CompanyHolder(companyId, "Fund", h));

        var holders = (institutionHolders ?? Array.Empty<DataAccessObjects.CompanyHolder>()).Union(
            (fundHolders ?? Array.Empty<DataAccessObjects.CompanyHolder>())).ToArray();

        var insiderTransactions = company.InsiderTransactions == null ? null
            : company.InsiderTransactions.Values
                .Select(t => new DataAccessObjects.CompanyInsiderTransaction(companyId, t)).ToArray();

        var esgScore = new DataAccessObjects.CompanyEsgScore(companyId, company.EsgScores);

        var activitiesInvolvement = company.EsgScores.ActivitiesInvolvement == null ? null
            : company.EsgScores.ActivitiesInvolvement.Values
                .Select(a => new DataAccessObjects.CompanyEsgActivity(companyId, a)).ToArray();

        var outstandingSharesAnnual = company.OutstandingShares.Annual == null ? null
            : company.OutstandingShares.Annual.Values
                .Select(s => new DataAccessObjects.CompanyOutstandingShares(companyId, s));

        var outstandingSharesQuarterly = company.OutstandingShares.Quarterly == null ? null
            : company.OutstandingShares.Quarterly.Values
                .Select(s => new DataAccessObjects.CompanyOutstandingShares(companyId, s));

        var outstandingShares = (outstandingSharesAnnual ?? Array.Empty<DataAccessObjects.CompanyOutstandingShares>())
            .Union(outstandingSharesQuarterly ?? Array.Empty<DataAccessObjects.CompanyOutstandingShares>()).ToArray();

        var earningsHistory = company.Earnings.History == null ? null 
            : company.Earnings.History.Values
                .Select(h => new DataAccessObjects.CompanyEarningsHistory(companyId, h)).ToArray();

        var earningsTrends = company.Earnings.Trend == null ? null
            : company.Earnings.Trend.Values
                .Select(t => new DataAccessObjects.CompanyEarningsTrend(companyId, t)).ToArray();

        var earningsAnnual = company.Earnings.Annual == null ? null
            : company.Earnings.Annual.Values
                .Select(a => new DataAccessObjects.CompanyEarningsAnnual(companyId, a)).ToArray();

        var balanceSheetQuarterly = company.Financials.BalanceSheet.Quarterly == null ? null
            : company.Financials.BalanceSheet.Quarterly.Values
                .Select(b => new DataAccessObjects.CompanyBalanceSheet(companyId, "Quarterly", b));

        var balanceSheetAnnual = company.Financials.BalanceSheet.Yearly == null ? null
            : company.Financials.BalanceSheet.Yearly.Values
                .Select(b => new DataAccessObjects.CompanyBalanceSheet(companyId, "Annual", b));

        var balanceSheets = (balanceSheetQuarterly ?? Array.Empty<DataAccessObjects.CompanyBalanceSheet>())
            .Union(balanceSheetAnnual ?? Array.Empty<DataAccessObjects.CompanyBalanceSheet>()).ToArray();

        var cashFlowQuarterly = company.Financials.CashFlow.Quarterly == null ? null
            : company.Financials.CashFlow.Quarterly.Values
                .Select(c => new DataAccessObjects.CompanyCashFlow(companyId, "Quarterly", c));

        var cashFlowAnnual = company.Financials.CashFlow.Yearly == null ? null
            : company.Financials.CashFlow.Yearly.Values
                .Select(c => new DataAccessObjects.CompanyCashFlow(companyId, "Annual", c));

        var cashFlows = (cashFlowQuarterly ?? Array.Empty<DataAccessObjects.CompanyCashFlow>())
            .Union(cashFlowAnnual ?? Array.Empty<DataAccessObjects.CompanyCashFlow>()).ToArray();

        var incomeStatementQuarterly = company.Financials.IncomeStatement.Quarterly == null? null
            : company.Financials.IncomeStatement.Quarterly.Values
                .Select(i => new DataAccessObjects.CompanyIncomeStatement(companyId, "Quarterly", i));

        var incomeStatementAnnual = company.Financials.IncomeStatement.Yearly == null ? null
            : company.Financials.IncomeStatement.Yearly.Values
                .Select(i => new DataAccessObjects.CompanyIncomeStatement(companyId, "Annual", i));

        var incomeStatements = (incomeStatementQuarterly ?? Array.Empty<DataAccessObjects.CompanyIncomeStatement>())
            .Union(incomeStatementAnnual ?? Array.Empty<DataAccessObjects.CompanyIncomeStatement>()).ToArray();

        List<Task> taskList = new()
        {
            Task.Run(() => SaveFundamentalDataAsync(companyGeneral,cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(address,cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(listings, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(officers, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(highlights, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(valuation, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(shareStats, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(technicals, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(dividend, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(dividendsByYear, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(analystRatings, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(holders, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(insiderTransactions, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(esgScore, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(activitiesInvolvement, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(outstandingShares, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(earningsHistory, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(earningsTrends, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(earningsAnnual, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(balanceSheets, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(cashFlows, cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(incomeStatements, cancellationToken), cancellationToken)
        };

        Task.WaitAll(taskList.ToArray(), cancellationToken);
    }

    internal async Task<Guid> GetCompanyIdAsync(string symbol, string exchange,
        string type, string name)
    {
        using var connection = await GetOpenConnectionAsync();

        try
        {
            string sql = @"SELECT global_id FROM public.companies
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

    private async Task SaveFundamentalDataAsync<T>(T obj, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (obj == null) { return; }

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(T));

        if (sql == null) { throw new Exception($"Could not create upsert for {obj!.GetType().Name}"); }

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, obj, transaction);
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

    private async Task SaveFundamentalDataAsync<T>(T[]? obj, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if ((obj?.Length ?? 0) == 0) return;

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(T));

        if (sql == null) { throw new Exception($"Could not create upsert for {obj!.GetType()?.GetElementType()?.Name}"); }

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, obj, transaction);
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
}
