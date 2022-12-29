# Development Journal

## 2022-11-20

### Progress

1. Got the YAML config / action items sorted (at least enough to move on).

### Next Steps

This effort is one component of a larger back-testing and analysis tool, so I'm motivated to get this wrapped up and moved on to some more substantive challenges.

1. Migrate the existing import logic into this new (YAML-based) model. This covers symbols, prices, splits, and dividends.
    1. Since this code already exists and has worked successfully in "production" for several months, I think this is a relatively light lift.
    1. 4 days - this should work out all the kinks in the YAML/action connection(s).
1. Add Exchanges
    1. 2 days.
1. Add Options.
    1. 3 days
1. Add Fundamentals
    1. 5 days
1. Put into production use; watch and review error logs; polish code and write remaining documentation.
    1. 5 days 
1. Set SDK version to 1.0. Release 1.0.
    1. 1 day

So, 20 days, plus 20% = **24 days**. Today is November 20 - that puts the release in the **week of December 12**.