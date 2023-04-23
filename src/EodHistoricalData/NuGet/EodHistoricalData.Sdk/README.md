# EodHistoricalData.com SDK

![Nuget](https://img.shields.io/nuget/v/EodHistoricalData.Sdk)

---

### Overview

This is an unofficial SDK for the [EOD Historical Data](https://eodhistoricaldata.com/) API, and to use this code, a valid, active [subscription](https://eodhistoricaldata.com/pricing) is required.

---

### Data Feeds Supported

1. [EOD Historical Data](https://eodhistoricaldata.com/financial-apis/api-for-historical-data-and-volumes/)
1. [Split Data Feed](https://eodhistoricaldata.com/financial-apis/api-splits-dividends/)
1. [Dividends Data Feed](https://eodhistoricaldata.com/financial-apis/api-splits-dividends/)
1. [Calendar Data](https://eodhistoricaldata.com/financial-apis/calendar-upcoming-earnings-ipos-and-splits/)
1. [Fundamental Data](https://eodhistoricaldata.com/financial-apis/category/fundamental-and-economic-financial-data-api/)
1. [Stock Options Data](https://eodhistoricaldata.com/financial-apis/stock-options-data/)

The specific API uris that are supported are all identified in the `ApiService` class:

```
    public const string BaseUri = "https://eodhistoricaldata.com/api/";
    public const string EodUri = $"{BaseUri}eod/";
    public const string ExchangesUri = $"{BaseUri}exchanges-list/";
    public const string ExchangeDetailsUri = $"{BaseUri}exchange-details/";
    public const string ExchangeSymbolListUri = $"{BaseUri}exchange-symbol-list/";
    public const string BulkEodUri = $"{BaseUri}eod-bulk-last-day/";
    public const string DividendUri = $"{BaseUri}div/";
    public const string SplitsUri = $"{BaseUri}splits/";
    public const string CalendarUri = $"{BaseUri}calendar/";
    public const string FundamentalsUri = $"{BaseUri}fundamentals/";
    public const string OptionsUri = $"{BaseUri}options/";
    public const string UserUri = $"{BaseUri}user/";
```

#### Notes

1. The `Fundamentals` API has a query string parameter called `filter`; this feature is not currently supported by this SDK and there are no plans to add it.
1. The `Bulk Fundamentals` endpoint is not supported by this SDK and there are no plans to add it.
1. The `Technicals` API is not supported by this SDK; it remains to be seen whether it will be supported in the future.

### Usage

The `DataClient` class is a `partial` class spread across a number of files whose names begin with `DataClient_`.
This is the only class to instantiate to gain access to the [EOD Historical Data](https://eodhistoricaldata.com/) API; hopefully the function names speak for themselves.
The import CLI and automated tests are good sources for usage examples.

The models to which the API results are mapped are all `struct`s. the results coming from the API are immutable values that can be transformed into the domain objects (i.e., `class`es you require.

```
var dataClient = new DataClient("your api key");

List<ApiResponseException> excs = new();

// The `ApiResponseExceptionEventHandler` captures errors originating from calls to the eodhistoricaldata.com API.

dataClient.ApiResponseExceptionEventHandler += (sender, apiResponseException, symbols) =>
{
    // do something useful here.
    excs.Add(apiResponseException);
};

var bulkLastDayNyse = await dataClient.GetBulkHistoricalDataForExchangeAsync("NYSE");
```