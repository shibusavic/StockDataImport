# End-of-Day Import Requirements

## Overview

The primary goal is to deliver a command-line interface (CLI) for managing calls to the [EOD Historical Data](https://eodhistoricaldata.com/) API and preserving the data returned by the API in a permanent data store.

A secondary goal is to achieve the first goal in the most efficient manner. There is a credit cost to every API call, and so I want to be able to run the CLI at regular intervales with a varied mix of arguments to achieve our data collection goals over a regular time period (e.g., one calendar week) wihtout exceeding our daily API call limit.

A final goal is to construct the code in a way so as to maximize the flexibility for another developer who wants to fork and then modify the code for a purpose I failed to consider or rejected as unnecessary.

### Stretch Goals / Requirements

1. Error handling and logging: Construct an HTML document containing the request url, the request body, a timestamp, and the response. This would be useful for reporting errors to `support@eodhistoricaldata.com`.

## Requirements

### Architecture

1. The CLI will be built using C# and .NET 6.0.
1. CLI configuration will be a combination of `appsettings.json` files for infrastructure needs and other configuration json files for data rules.
1. The data store will use [PostgreSQL](https://www.postgresql.org/).
1. The runtime executable will be named `eod-import`.
1. The scheduling of CLI executions with varied arguments will attempt to maximize efficiency of API credits.
1. The CLI will be broken into separate executables (similar to how `git` is organized). **I'm seriously questioning this decision right now after realizing that I'll want to track available call credits and probably preserve that in memory.**
    1. `eod-import` will be the root control program.
    1. `eod-import-auto` will be the configuration/auto mode.
    1. `eod-import-full` will be the "full" import module.
    1. `eod-import-bulk` will be the "bulk" import module.
    1. `eod-import-config` will be the module for configuring details about the other modules.
1. An algorithm (a weak AI) that determines how many API credits are available and how best to use them seems appropriate.

### Security

This is not a legitimate concern for this project - this code base is designed to be run on a local machine.
The reason for this is _cost_; running these processes every day and storing all of this data could be quite expensive.

For this reason, _security_ will essentially be ignored; the only real security issue is the password to your database.
No connection strings will ever be stored in this repo; _user secrets_ are used to manage individualized, private information.

### Functional

1. The CLI must be able to run either _full_ data imports or _bulk_ data imports (but not both in the same run).
1. A `--tickers` argument will trigger the import of securities.
1. A `--prices` argument will trigger the import of either _full_ or _bulk_ end-of-day price information.
1. A `--splits` argument will trigger the import of either _full_ or _bulk_ splits.
1. A `--dividends` arguments will trigger the import of either _full_ or _bulk_ dividends.
1. A `--fundamentals` argument will trigger the import of fundamentals.
1. A `--verbose` argument will write progress information to the standard output.

### Logging

1. The CLI will capture and log any errors from calls to the [EOD Historical Data](https://eodhistoricaldata.com/) API.
1. The CLI will capture informational logs about when it was invoked and how long it took to complete its tasks.
1. Need to see the request, the timestamp, and the cost of each API call so that I can determine the remaining number of possible calls.
    1. I think if this is just based on a 24-hour period, and the max number of calls allowed is clear from the _configuration_, then this should be fine for determining remaining capacity.
    1. **This makes me question the modularized model above.**

### Configuration

1. Need to be able to exclude and/or filter what is queried.
    1. Exchange
    1. Either approved or not disapproved ticker lists ...?
    1. Security Type (e.g., "Common Stock", "ETF", etc.)
    1. Duration security has been available (how much data is available)
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