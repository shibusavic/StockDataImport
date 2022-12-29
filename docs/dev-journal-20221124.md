# Development Journal

## 2022-11-24

### Progress

Working on ImportsDbContext ...

1. Now saving to the database
    - Symbols
    - Dividends
    - Splits
    - Price Actions
    - Exchanges

On the 20th, I gave myself 6 days to get here; made it in 4, but Exchanges took less than an hour when I had allocated 2 days.

Options are next; gave myself 3 days, plus the 2 I'm ahead . . . gives me 5 days to complete options without falling behind my planned schedule.

### Next Steps

- Might want to consider inserting `exchange` into the db tables currently only using `symbol`. `symbol` works for US, but it may fail across all exchanges.
- Maybe need a test to test all symbols across all exchanges - will be expensive, but probably worth it to know.
- Maybe consider forcing exchange into the API queries where currently only symbol is used.
    - e.g., "AAPL.US" instead of "AAPL"


