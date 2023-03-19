-- earnings - calendar_trends
select "date",period, growth, earnings_estimate_avg, earnings_estimate_low, earnings_estimate_high, earnings_estimate_growth from calendar_trends where symbol = 'A';

-- 'A' = 'e4abf062-8bd5-4b34-a12c-ab32108656a2'

select "date", period, growth, earnings_estimate_avg, earnings_estimate_low, earnings_estimate_high, earnings_estimate_growth from company_earnings_trends where company_id = 'e4abf062-8bd5-4b34-a12c-ab32108656a2';

select most_recent_quarter, earnings_share, eps_estimate_current_year, eps_estimate_next_year from company_highlights where company_id = 'e4abf062-8bd5-4b34-a12c-ab32108656a2';

select * from company_earnings_annual where company_id = 'e4abf062-8bd5-4b34-a12c-ab32108656a2';

