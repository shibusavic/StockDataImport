using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models.Fundamentals.CommonStock;
using EodHistoricalData.Sdk.Models.Fundamentals.Etf;

namespace EodHistoricalData.Sdk.Tests;

[Collection("API Tests")]
public class CommonStockFundamentalsTests : BaseTest
{
    [Fact]
    public async Task GetFundamentals_Valid_NotEmpty()
    {
        var dataClient = new DataClient(apiKey);
        var fundamentals = await dataClient.GetFundamentalsForSymbolAsync<FundamentalsCollection>("MSFT");
        Assert.NotEqual(new FundamentalsCollection(), fundamentals);
    }

    [Fact]
    public async Task GetFundamentalsForSymbolAsync_BadApiKey_ThrowsUnauthorizedAccessException()
    {
        var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

        List<ApiResponseException> excs = new();

        ApiEventPublisher.RaiseApiResponseEventHandler += (sender, e) =>
        {
            if (e.ApiResponseException != null)
            {
                excs.Add(e.ApiResponseException);
            }
        };

        Assert.Equal(default, await dataClient.GetFundamentalsForSymbolAsync<FundamentalsCollection>("AAPL"));
        Assert.Single(excs);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetFundamentalsForSymbolAsync_BadSymbol_Empty()
    {
        var dataClient = new DataClient(apiKey);

        Assert.Null(await dataClient.GetFundamentalsForSymbolAsync(Guid.NewGuid().ToString()[..4]));
    }

    [Theory] // [Theory(Skip = "Expensive")]
    [InlineData("TSLA")]
    //[InlineData("MSFT")]
    [InlineData("AAPL")]
    [InlineData("OXY")]
    public async Task GetFundamentalsForSymbolAsync_Fields_NotEmpty(string symbol)
    {
        var dataClient = new DataClient(apiKey);

        var actual = await dataClient.GetFundamentalsForSymbolAsync<FundamentalsCollection>(symbol);

        Assert.NotEqual(new FundamentalsCollection(), actual);
        Assert.Equal(symbol, actual.General.Code);
        Assert.True(actual.Highlights.MarketCapitalization.HasValue);
        Assert.True(actual.Valuation.EnterpriseValue.HasValue);
        Assert.True(actual.SharesStats.SharesOutstanding.HasValue);
        Assert.True(actual.Technicals.TwoHundredDayMovingAverage.GetValueOrDefault() > 0M);
        Assert.True(actual.SplitsDividends.ForwardAnnualDividendYield.HasValue);
        Assert.True(actual.AnalystRatings.Rating.HasValue);
        Assert.NotNull(actual.Holders.GetValueOrDefault().Institutions);
        Assert.NotEmpty(actual.Holders.GetValueOrDefault().Institutions!);
        Assert.NotNull(actual.Holders.GetValueOrDefault().Funds); 
        Assert.NotEmpty(actual.Holders.GetValueOrDefault().Funds!);
        Assert.NotNull(actual.EsgScores.Disclaimer);
        Assert.NotNull(actual.OutstandingShares.Annual);
        Assert.NotEmpty(actual.OutstandingShares.Annual);
        Assert.NotNull(actual.OutstandingShares.Quarterly);
        Assert.NotEmpty(actual.OutstandingShares.Quarterly);
        Assert.NotNull(actual.Earnings.History);
        Assert.NotEmpty(actual.Earnings.History);
        Assert.NotNull(actual.Earnings.Trend);
        Assert.NotEmpty(actual.Earnings.Trend);
        Assert.NotNull(actual.Earnings.Annual);
        Assert.NotEmpty(actual.Earnings.Annual);
        Assert.NotNull(actual.Financials.BalanceSheet.Quarterly);
        Assert.NotEmpty(actual.Financials.BalanceSheet.Quarterly);
        Assert.NotNull(actual.Financials.BalanceSheet.Yearly);
        Assert.NotEmpty(actual.Financials.BalanceSheet.Yearly);
        Assert.NotNull(actual.Financials.CashFlow.Quarterly);
        Assert.NotEmpty(actual.Financials.CashFlow.Quarterly);
        Assert.NotNull(actual.Financials.CashFlow.Yearly);
        Assert.NotEmpty(actual.Financials.CashFlow.Yearly);
        Assert.NotNull(actual.Financials.IncomeStatement.Quarterly);
        Assert.NotEmpty(actual.Financials.IncomeStatement.Quarterly);
        Assert.NotNull(actual.Financials.IncomeStatement.Yearly);
        Assert.NotEmpty(actual.Financials.IncomeStatement.Yearly);
    }

    [Fact]
    public async Task GetFundamentalsForSymbolAsync_FindsEtf()
    {
        var dataClient = new DataClient(apiKey);

        var actual = await dataClient.GetFundamentalsForSymbolAsync("VTI");

        Assert.True(actual is EtfFundamentalsCollection);
    }

    [Fact]
    public async Task GetFundamentalsForSymbolAsync_FindsCommonStock()
    {
        var dataClient = new DataClient(apiKey);

        var actual = await dataClient.GetFundamentalsForSymbolAsync("AMX");

        Assert.True(actual is FundamentalsCollection);
    }

}

