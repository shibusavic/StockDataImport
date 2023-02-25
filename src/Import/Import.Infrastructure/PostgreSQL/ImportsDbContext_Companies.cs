using Dapper;
using EodHistoricalData.Sdk.Models.Fundamentals.CommonStock;
using Import.Infrastructure.Events;
using System.Collections.ObjectModel;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    public async Task SaveCompanyAsync(FundamentalsCollection company,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (company.General.Code == null) throw new ArgumentException($"{nameof(company)} is missing Code.");
        if (company.General.Exchange == null) throw new ArgumentException($"{nameof(company)} is missing Exchange.");
        if (company.General.Type == null) throw new ArgumentException($"{nameof(company)} is missing Type.");
        if (company.General.Name == null) throw new ArgumentException($"{nameof(company)} is missing Name.");

        var companyId = await GetCompanyIdAsync(company.General.Code,
            company.General.Exchange, company.General.Type, company.General.Name);

        var companyGeneral = new DataAccessObjects.Company(company, companyId);

        var address = company.General.AddressData?.IsValid ?? false
            ? new DataAccessObjects.CompanyAddress(companyId, company.General.AddressData.GetValueOrDefault())
            : null;

        Collection<DataAccessObjects.CompanyListing> listings = new();

        if (company.General.Listings != null)
        {
            foreach (var lst in company.General.Listings.Values)
            {
                if (!default(Listing).Equals(lst))
                {
                    listings.Add(new DataAccessObjects.CompanyListing(companyId, lst));
                }
            }
        }

        var officers = company.General.Officers?.Values
            .Select(v => new DataAccessObjects.CompanyOfficer(companyId, v)).ToArray();

        var highlights = new DataAccessObjects.CompanyHighlights(companyId, company.Highlights);
        var valuation = new DataAccessObjects.CompanyValuation(companyId, company.Valuation);
        var shareStats = new DataAccessObjects.CompanySharesStat(companyId, company.SharesStats);
        var technicals = new DataAccessObjects.CompanyTechnicals(companyId, company.Technicals);
        var dividend = new DataAccessObjects.CompanyDividend(companyId, company.SplitsDividends);
        var dividendsByYear = company.SplitsDividends.NumberDividendsByYear?.Values
                .Select(v => new DataAccessObjects.CompanyDividendsByYear(companyId, v)).ToArray();
        var analystRatings = company.AnalystRatings.Rating == null ? null
            : new DataAccessObjects.CompanyAnalystRating(companyId, company.AnalystRatings);

        var institutionHolders = company.Holders?.Institutions?.Values
            .Select(h => new DataAccessObjects.CompanyHolder(companyId, "Institution", h));

        var fundHolders = company.Holders?.Funds?.Values
                .Select(h => new DataAccessObjects.CompanyHolder(companyId, "Fund", h));

        var holders = (institutionHolders ?? Array.Empty<DataAccessObjects.CompanyHolder>()).Union(
            (fundHolders ?? Array.Empty<DataAccessObjects.CompanyHolder>())).ToArray();

        var insiderTransactions = company.InsiderTransactions?.Values
                .Select(t => new DataAccessObjects.CompanyInsiderTransaction(companyId, t)).ToArray();

        var esgScore = new DataAccessObjects.CompanyEsgScore(companyId, company.EsgScores);

        var activitiesInvolvement = company.EsgScores.ActivitiesInvolvement?.Values
                .Select(a => new DataAccessObjects.CompanyEsgActivity(companyId, a)).ToArray();

        var outstandingSharesAnnual = company.OutstandingShares.Annual?.Values
                .Select(s => new DataAccessObjects.CompanyOutstandingShares(companyId, s));

        var outstandingSharesQuarterly = company.OutstandingShares.Quarterly?.Values
                .Select(s => new DataAccessObjects.CompanyOutstandingShares(companyId, s));

        var outstandingShares = (outstandingSharesAnnual ?? Array.Empty<DataAccessObjects.CompanyOutstandingShares>())
            .Union(outstandingSharesQuarterly ?? Array.Empty<DataAccessObjects.CompanyOutstandingShares>()).ToArray();

        var earningsHistory = company.Earnings.History?.Values
                .Select(h => new DataAccessObjects.CompanyEarningsHistory(companyId, h)).ToArray();

        var earningsTrends = company.Earnings.Trend?.Values
                .Select(t => new DataAccessObjects.CompanyEarningsTrend(companyId, t)).ToArray();

        var earningsAnnual = company.Earnings.Annual?.Values
                .Select(a => new DataAccessObjects.CompanyEarningsAnnual(companyId, a)).ToArray();

        var balanceSheetQuarterly = company.Financials.BalanceSheet.Quarterly?.Values
                .Select(b => new DataAccessObjects.CompanyBalanceSheet(companyId, "Quarterly", b));

        var balanceSheetAnnual = company.Financials.BalanceSheet.Yearly?.Values
                .Select(b => new DataAccessObjects.CompanyBalanceSheet(companyId, "Annual", b));

        var balanceSheets = (balanceSheetQuarterly ?? Array.Empty<DataAccessObjects.CompanyBalanceSheet>())
            .Union(balanceSheetAnnual ?? Array.Empty<DataAccessObjects.CompanyBalanceSheet>()).ToArray();

        var cashFlowQuarterly = company.Financials.CashFlow.Quarterly?.Values
                .Select(c => new DataAccessObjects.CompanyCashFlow(companyId, "Quarterly", c));

        var cashFlowAnnual = company.Financials.CashFlow.Yearly?.Values
                .Select(c => new DataAccessObjects.CompanyCashFlow(companyId, "Annual", c));

        var cashFlows = (cashFlowQuarterly ?? Array.Empty<DataAccessObjects.CompanyCashFlow>())
            .Union(cashFlowAnnual ?? Array.Empty<DataAccessObjects.CompanyCashFlow>()).ToArray();

        var incomeStatementQuarterly = company.Financials.IncomeStatement.Quarterly?.Values
                .Select(i => new DataAccessObjects.CompanyIncomeStatement(companyId, "Quarterly", i));

        var incomeStatementAnnual = company.Financials.IncomeStatement.Yearly?.Values
                .Select(i => new DataAccessObjects.CompanyIncomeStatement(companyId, "Annual", i));

        var incomeStatements = (incomeStatementQuarterly ?? Array.Empty<DataAccessObjects.CompanyIncomeStatement>())
            .Union(incomeStatementAnnual ?? Array.Empty<DataAccessObjects.CompanyIncomeStatement>()).ToArray();

        Collection<Task> taskList = new()
        {
            Task.Run(() => SaveFundamentalDataAsync(companyGeneral,cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(address,cancellationToken), cancellationToken),
            Task.Run(() => SaveFundamentalDataAsync(listings.ToArray(), cancellationToken), cancellationToken),
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

        try
        {
            Task.WaitAll(taskList.ToArray(), cancellationToken);
        }
        catch (AggregateException exc)
        {
            for (int i = 0; i < exc.InnerExceptions.Count; i++)
            {
                DomainEventPublisher.RaiseMessageEvent(this, exc.InnerExceptions[i].ToString(),
                    nameof(SaveCompanyAsync),
                    Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }
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

    private Task SaveFundamentalDataAsync<T>(T obj, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (obj == null) { return Task.CompletedTask; }

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(T));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {obj!.GetType().Name}"); }

        return ExecuteAsync(sql, obj, null, cancellationToken);
    }

    private Task SaveFundamentalDataAsync<T>(T[]? obj, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if ((obj?.Length ?? 0) == 0) return Task.CompletedTask;

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(T));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {obj!.GetType()?.GetElementType()?.Name}"); }

        return ExecuteAsync(sql, obj, null, cancellationToken);
    }
}
