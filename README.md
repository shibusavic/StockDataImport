# StockDataImport

This project is an endeavor to build the basis for a stock back-testing application.

# EodHistoricalData.com C# SDK and Import CLI Tool

## NuGet SDK

![Nuget](https://img.shields.io/nuget/v/EodHistoricalData.Sdk)

---

### Overview

This is an unofficial SDK for the [EOD Historical Data](https://eodhistoricaldata.com/) API, and to use this code, a valid, active [subscription](https://eodhistoricaldata.com/pricing) is required.

#### Note

Under the `/docs` folder there exists other markdown files including requirement specs and journal entries explaining motivations and decisions.

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
This is the only class you will need to instantiate to access the [EOD Historical Data](https://eodhistoricaldata.com/) API; hopefully the function names speak for themselves.
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

---

## Import CLI Tool

### Overview

The `eod-import` project is a C# command-line tool that can be used in scripts and schedulers (e.g., `cron`) to automate the daily capture and permanent storage of data from the [EOD Historical Data](https://eodhistoricaldata.com/) API.

The `/docs` folder is the place to go for more detailed guidance and explanation.

### Getting Started

#### Database

The first place to visit is the `/database` folder. This folder contains scripts for building the necessary database(s).
I built my local system using postgreSQL, but I attempted to construct it so that I (or others) could easily migrate to another database, or use a mix of databases simultaneously.

The `Import.Cli/appsettings.json` file looks like this:

```
{
  "DatabaseEngines": {
    "Logs": "PostgreSQL",
    "Imports":  "PostgreSQL"
  }
}
```

The keys (e.g., `Logs`) must tie to a corresponding connection string key, and in this case, must be a valid PostgreSQL connection string. I used [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0) via [Visual Studio](https://visualstudio.microsoft.com/) to manage this locally in `Import.Cli` and `Import.Infrastructure.IntegrationTests`. You may choose to manage them differently, but here's an example of mine:

```
{
  "ConnectionStrings": {
    "Logs": "User ID=postgres;Password=REPLACEME;Host=127.0.0.1;Port=5432;Database=eod_logs_test;",
    "Imports": "User ID=postgres;Password=REPLACEME2;Host=127.0.0.1;Port=5432;Database=eod_imports_test;"
  }
}
```

The keys under `DatabaseEngines` line up with they keys under `ConnectionStrings`. 

---

To set up the four databases from scratch on my local machine, it looks something like this:

```
cd ~/myrepos/EodHistoricalDataCSharpSdk/database/postgreSQL

psql -Upostgres
# provide password when prompted.

create database eod_logs;
create database eod_logs_test;
create database eod_imports;
create database eod_imports_test;

\c eod_logs_test
\i Logs/create_tables.sql

\c eod_logs
\i Logs/create_tables.sql

\c eod_imports_test
\i Imports/create_tables.sql

\c eod_imports
\i Imports/create_tables.sql

\q
```

You can test your setup by running the integration tests.

#### Configuration

The import tool's configuration for what actions to take is controlled by [YAML files](https://yaml.org/).
I made use of [YamlDotNetCore](https://github.com/aaubry/YamlDotNet) to parse the YAML files.

A default file is provided and may work for you. It looks something like this:

```
Max Token Usage: 990
API Key: TEST
Cancel On Exception: true

Purge Data:
- Logs
- Actions
- Imports

Log Retention:
  Critical: 1 year
  Debug: 2 weeks
  Error: 1 year
  Information: 3 months
  Trace: 1 week
  Warning: 6 months

On Empty Database:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Exchanges
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Fundamentals

Any Day:
- Priority: 1
  Scope: Bulk
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Fundamentals

Sunday:
- Skip: true

Monday:
- Skip: true

Tuesday:
- Priority: 1
  Scope: Bulk
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Fundamentals

Wednesday:
- Priority: 1
  Scope: Bulk
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Fundamentals

Thursday:
- Priority: 1
  Scope: Bulk
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Fundamentals

Friday:
- Priority: 1
  Skip: false
  Scope: Bulk
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Fundamentals

Saturday:
- Priority: 1
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Symbols
  - Prices
  - Options
  - Dividends
  - Splits
- Priority: 2
  Scope: Full
  Exchanges:
  - NYSE
  - NASDAQ
  - AMEX
  Data Types:
  - Fundamentals
...
```

The sections of the configuration file inform the import engine what to do in specific circumstances or on particular days of the week.
For a more detailed explanation of each section, see the `/docs` folder.

You can pass in any configuration file you want using the `-c` argument.
Configuration files are selected in the following order:

1. Command-line. Passing a config file on the command line will get it processed.
1. The JSON configuration (e.g., appsettings.json or secrets.json). 
1. The YAML configuration (see example above).

If no API key is found, the program will end in error.

```
eod-import -v -c myconfig.yml
```