# Development Journal

## 2023-01-07

### Progress

Been busy. After completing the migration from another repo into this one (because I decided on better names for things), I got to work moving sticky notes across my white board.

`Calendar` items were still missing from the import and the database, so I added them in.

Executed several real-time tests and made a number of changes to the models and the database (changing nullability) as a result.

Reworked `Logs` from the ground up. I simplified the database and then added the `api_response` table for tracking api request/response data for all calls, not just those that crash.
Renamed directory from Logs to AppLogs -> didn't realize that git was ignoring that directory.

Changed price capture to run in parallel - more of this to come I think.

Added SymbolMetaData, SymbolMetaDataRepository, and SymbolsToIgnore for more performant real-time decision making.
The meta data repo will only contain symbols NOT on the symbols-to-ignore list.

Reworked event handling; added a static event publisher.

### Next Steps

1. Rework parts of the yml config and `main` process.
    - Need "Reasons to Ignore." Specifically wanting to see "No fundamentals" on the list.
    - Change "log retention" to "data retention" and review/enhance the data truncate logic in `Program.cs`.
    - Add, fix, or test `Bulk` imports - even though I'm unlikely to use them.
1. Set up Production server
    - Start testing on server.
    - Address defects as they arise.
1. Documentation review and update.
    - Detailed instructions on setting up a yml config file.
    - Issues with the eodhistoricaldata.com api.