# End-of-Day Import Requirements

## Overview

The primary goal is to deliver a command-line interface (CLI) for managing calls to the [EOD Historical Data](https://eodhistoricaldata.com/) API and preserving the data returned by the API in a permanent data store.

A secondary goal is to achieve the first goal in the most efficient manner. There is a credit cost to every API call, so I want to be able to run the CLI at regular intervals with a varied mix of arguments to achieve data collection goals over a regular time period (e.g., one calendar week) wihtout exceeding our daily API call limit.

A final goal is to construct the code in a way so as to maximize the flexibility for other developers to fork and modify the code for a scenario I didn't consider or one rejected as unnecessary for my purposes.

### Stretch Goals

1. Error handling and logging: Construct an HTML document containing the request url, the request body, a timestamp, and the response. This would be useful for reporting errors to `support@eodhistoricaldata.com`.

---

## Requirements

### Architecture

1. The CLI will be built using C# and .NET 6/7.
1. CLI configuration will be a combination of `appsettings.json` files for infrastructure needs and other configuration YAML files for data rules.
1. The data store will use [PostgreSQL](https://www.postgresql.org/).
1. The runtime executable will be named `import`.
1. The scheduling of CLI executions with varied configurations will attempt to maximize efficiency of API credits.

### Security

This is not a legitimate concern for this project - this code base is designed to be run on a local machine.
The reason for this is _cost_; running these processes every day and storing all of this data could be expensive on a cloud provider.

For this reason, _security_ will essentially be ignored; the only real security issues are database passwords.
No connection strings will ever be stored in this repo; _Visual Studio user secrets_ are used to manage individualized, private information.

### Functional

1. The CLI will accept a YAML file that defines which actions will be execute.

### Logging

1. The CLI will capture and log any errors from calls to the [EOD Historical Data](https://eodhistoricaldata.com/) API.
1. The CLI will capture informational logs about when it was invoked and how long it took to complete its tasks.

### Configuration

1. Need to be able to exclude and/or filter what is queried.
    1. Exchange
    1. Either approved or not disapproved ticker lists ...?
    1. Security Type (e.g., "Common Stock", "ETF", etc.)
    1. Duration ticker has been available (how much data is available)
1. Need rules around when certain things are queried. For example, it's unnecessary to query fundamental data for a security more than once every three months when the numbers (that can't otherwise be calculated) will not change from day-to-day. **May need to double-check this assumption with a careful review of the fundamentals API.**
1. Auto Mode
    1. Days of the Week - different priorities for different days.
    1. Need to preserve and reference the cost of each call in order to plan properly.
    1. Max number of calls in a 24-hour period.

## Thoughts On Efficient Scheduling

1. Full imports - Saturday thru Monday.
1. Bulk imports - Tuesday thru Friday.
1. Fundamentals - more thought required here
    1. see [api limits](https://eodhistoricaldata.com/financial-apis/api-limits/); need to plug this header stuff into the NuGet package.
    1. might try to space this out evenly?
    1. maybe use Yahoo Finance API here as a guide for what to query and when to query it?
    1. maybe the `calendar` API can inform us on what to query.

## API Endpoints

[List of Tickers](https://eodhistoricaldata.com/financial-apis/exchanges-api-list-of-tickers-and-trading-hours/)
[EOD Historical Data](https://eodhistoricaldata.com/financial-apis/api-for-historical-data-and-volumes/)
[Bulk API](https://eodhistoricaldata.com/financial-apis/bulk-api-eod-splits-dividends/)
[Historical Splits and Dividends](https://eodhistoricaldata.com/financial-apis/api-splits-dividends/)
[Economic Events Data API](https://eodhistoricaldata.com/financial-apis/economic-events-data-api/)
[Fundamental Data](https://eodhistoricaldata.com/financial-apis/stock-etfs-fundamental-data-feeds/)
[Insider Transactions](https://eodhistoricaldata.com/financial-apis/insider-transactions-api/)
[Calendar](https://eodhistoricaldata.com/financial-apis/calendar-upcoming-earnings-ipos-and-splits/)
[Exchange Trading Hours and Holidays](https://eodhistoricaldata.com/financial-apis/exchanges-api-trading-hours-and-holidays/)
[Technical Indicators](https://eodhistoricaldata.com/financial-apis/technical-indicators-api/)
[Screener](https://eodhistoricaldata.com/financial-apis/stock-market-screener-api/)
[List of Exchanges](https://eodhistoricaldata.com/financial-apis/list-supported-exchanges/)
[Financial News](https://eodhistoricaldata.com/financial-apis/financial-news-api/)
[Macroeconomics](https://eodhistoricaldata.com/financial-apis/macroeconomics-data-and-macro-indicators-api/)