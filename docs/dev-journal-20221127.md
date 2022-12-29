# Development Journal

## 2022-11-27

### Progress

Made good progress in the last few days:

- Actions are getting processed.
- Got API call limits working as expected.
    - Cost calculations are made before calls are made - preventing exceeding the specified limit.
- Added "Fixes" to the import configuration and a fix to hydrate `has_options` on the `public.symbols` table.

Roughly 10 days remaining to be "code complete" - if I want to have a week of field testing and I want to hit my **December 12 deadline**.
But, there's been some scope creep.
I missed the `calendar` endpoints in my db design, but these are not urgent, so I'm going to move that task to post-production-release.
Likewise, I'm going to move the bulk import tasks to post-production-release.

### Plans to Production Deployment

- Import Fundamentals.
    - This is going to involve building a complex object before saving (I think). This is going to take me all of my free time this coming week.
- Review and improve logs and action logs.
    - Action logs are not currently be preserved.
    - Make sure logs are verbose - to correctly capture production errors.
- Set up Production Server (next weekend).
- Deploy to production.

### Post-Deployment Tasks

- Bulk Imports
- Calendar API Endpoint processing
- Bug fixes from production test.
- Beta deployment - some time in the week of **December 12**
- Get it running smoothly.
- Polish the code.
- Release 1.0 -- by end of year.