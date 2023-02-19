CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS public.exchanges
(
  name TEXT NULL,
  code TEXT NOT NULL,
  operating_mic TEXT NULL,
  country TEXT NULL,
  currency TEXT NULL,
  country_iso_2 TEXT NULL,
  country_iso_3 TEXT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (code)
);

CREATE TABLE IF NOT EXISTS public.symbols
(
  code TEXT NOT NULL,
  symbol TEXT NULL,
  exchange TEXT NULL,
  name TEXT NULL,
  country TEXT NULL,
  currency TEXT NULL,
  type TEXT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (code)
);

CREATE TABLE IF NOT EXISTS public.splits
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  date_of_split DATE NOT NULL,
  before_split DOUBLE PRECISION NULL,
  after_split DOUBLE PRECISION NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, date_of_split)
);

CREATE TABLE IF NOT EXISTS public.dividends
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  "date" DATE NOT NULL,
  value NUMERIC(22,5) NULL,
  unadjusted_value NUMERIC(22,5) NULL,
  currency TEXT NULL,
  declaration_date DATE NULL,
  record_date DATE NULL,
  payment_date DATE NULL,
  period TEXT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, "date")
);

CREATE TABLE IF NOT EXISTS public.price_actions
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  start TIMESTAMP NOT NULL,
  finish TIMESTAMP NOT NULL,
  open NUMERIC(22,2) NOT NULL,
  high NUMERIC(22,2) NOT NULL,
  low NUMERIC(22,2) NOT NULL,
  close NUMERIC(22,2) NOT NULL,
  volume BIGINT NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (exchange, symbol, start, finish)
);

CREATE TABLE IF NOT EXISTS public.companies
(
  global_id UUID NOT NULL,
  symbol TEXT NULL,
  exchange TEXT NULL,
  name TEXT NULL,
  type TEXT NULL,
  currency_code TEXT NULL,
  currency_name TEXT NULL,
  currency_symbol TEXT NULL,
  country_name TEXT NULL,
  country_iso TEXT NULL,
  isin TEXT NULL,
  lei TEXT NULL,
  cusip TEXT NULL,
  cik TEXT NULL,
  employer_id_number TEXT NULL,
  fiscal_year_end TEXT NULL,
  ipo_date DATE NULL,
  international_domestic TEXT NULL,
  sector TEXT NULL,
  industry TEXT NULL,
  gic_sector TEXT NULL,
  gic_group TEXT NULL,
  gic_industry TEXT NULL,
  home_category TEXT NULL,
  is_delisted BOOLEAN NULL,
  description TEXT NULL,
  phone TEXT NULL,
  web_url TEXT NULL,
  logo_url TEXT NULL,
  full_time_employees INTEGER NULL,
  update_at DATE NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (global_id)
);

CREATE TABLE IF NOT EXISTS public.company_addresses
(
  company_id UUID NOT NULL,
  street TEXT NOT NULL,
  city TEXT NULL,
  state TEXT NULL,
  country TEXT NULL,
  postal_code TEXT NOT NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, street, postal_code)
);

CREATE TABLE IF NOT EXISTS public.company_listings
(
  company_id UUID NOT NULL,
  symbol TEXT NULL,
  exchange TEXT NULL,
  name TEXT NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.company_officers
(
  company_id UUID NOT NULL,
  name TEXT NULL,
  title TEXT NULL,
  year_born TEXT NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.company_highlights
(
  company_id UUID NOT NULL,
  market_capitalization NUMERIC(22,2) NULL,
  market_capitalization_mln NUMERIC(12,2) NULL,
  ebitda NUMERIC(22,2) NULL,
  pe_ratio DOUBLE PRECISION NULL,
  peg_ratio DOUBLE PRECISION NULL,
  wall_street_target_price NUMERIC(22,2) NULL,
  book_value DOUBLE PRECISION NULL,
  dividend_share DOUBLE PRECISION NULL,
  dividend_yield DOUBLE PRECISION NULL,
  earnings_share NUMERIC(22,2) NULL,
  eps_estimate_current_year NUMERIC(22,2) NULL,
  eps_estimate_next_year NUMERIC(22,2) NULL,
  eps_estimate_next_quarter NUMERIC(22,2) NULL,
  eps_estimate_current_quarter NUMERIC(22,2) NULL,
  most_recent_quarter DATE NULL,
  profit_margin DOUBLE PRECISION NULL,
  operating_margin_ttm DOUBLE PRECISION NULL,
  return_on_assets_ttm DOUBLE PRECISION NULL,
  return_on_equity_ttm DOUBLE PRECISION NULL,
  revenue_ttm NUMERIC(22,2) NULL,
  revenue_per_share_ttm NUMERIC(22,2) NULL,
  quarterly_revenue_growth_yoy DOUBLE PRECISION NULL,
  gross_profit_ttm NUMERIC(22,2) NULL,
  diluted_eps_ttm NUMERIC(22,2) NULL,
  quarterly_earnings_growth_yoy DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.company_valuations
(
  company_id UUID NOT NULL,
  trailing_pe DOUBLE PRECISION NULL,
  forward_pe DOUBLE PRECISION NULL,
  price_sales_ttm DOUBLE PRECISION NULL,
  price_book_mrq DOUBLE PRECISION NULL,
  enterprise_value NUMERIC(22,2) NULL,
  enterprise_value_revenue DOUBLE PRECISION NULL,
  enterprise_value_ebitda DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.company_shares_stats
(
  company_id UUID NOT NULL,
  shares_outstanding DOUBLE PRECISION NULL,
  shares_float DOUBLE PRECISION NULL,
  percent_insiders DOUBLE PRECISION NULL,
  percent_institutions DOUBLE PRECISION NULL,
  shares_short DOUBLE PRECISION NULL,
  shares_short_prior_month DOUBLE PRECISION NULL,
  short_ratio DOUBLE PRECISION NULL,
  short_percent_outstanding DOUBLE PRECISION NULL,
  short_percent_float DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.company_technicals
(
  company_id UUID NOT NULL,
  beta DOUBLE PRECISION NULL,
  fifty_two_week_high NUMERIC (18,2) NULL,
  fifty_two_week_low NUMERIC (18,2) NULL,
  fifty_day_ma NUMERIC (18,2) NULL,
  two_hundred_day_ma NUMERIC (18,2) NULL,
  shares_short NUMERIC (18,2) NULL,
  shares_short_prior_month NUMERIC (18,2) NULL,
  short_ratio DOUBLE PRECISION NULL,
  short_percent DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.company_dividends
(
  company_id UUID NOT NULL,
  forward_annual_dividend_rate DOUBLE PRECISION NULL,
  forward_annual_dividend_yield DOUBLE PRECISION NULL,
  payout_ratio DOUBLE PRECISION NULL,
  dividend_date DATE NULL,
  ex_dividend_date DATE NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.company_dividends_by_year
(
  company_id UUID NOT NULL,
  year INTEGER NOT NULL,
  count INTEGER NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, year)
);

CREATE TABLE IF NOT EXISTS public.company_analyst_ratings
(
  company_id UUID NOT NULL,
  rating DOUBLE PRECISION NULL,
  target_price NUMERIC(22,2) NULL,
  strong_buy INTEGER NULL,
  buy INTEGER NULL,
  hold INTEGER NULL,
  sell INTEGER NULL,
  strong_sell INTEGER NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.company_holders
(
  company_id UUID NOT NULL,
  holder_type TEXT NOT NULL,
  name TEXT NOT NULL,
  "date" DATE NOT NULL,
  total_shares DOUBLE PRECISION NULL,
  total_assets DOUBLE PRECISION NULL,
  current_shares INTEGER NULL,
  change INTEGER NULL,
  change_percentage DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, holder_type, name, "date")
);

CREATE TABLE IF NOT EXISTS public.company_insider_transactions
(
  company_id UUID NOT NULL,
  "date" DATE NOT NULL,
  owner_cik TEXT NULL,
  owner_name TEXT NULL,
  transaction_date DATE NULL,
  transaction_code TEXT NULL,
  transaction_amount INTEGER NULL,
  transaction_price NUMERIC(22,2) NULL,
  transaction_acquired_disposed TEXT NULL,
  post_transaction_amount INTEGER NOT NULL,
  sec_link TEXT NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date, post_transaction_amount)
);

CREATE TABLE IF NOT EXISTS public.company_esg_scores
(
  company_id UUID NOT NULL,
  rating_date DATE NOT NULL,
  total_esg DOUBLE PRECISION NULL,
  total_esg_percentile DOUBLE PRECISION NULL,
  environment_score DOUBLE PRECISION NULL,
  environment_score_percentile DOUBLE PRECISION NULL,
  social_score DOUBLE PRECISION NULL,
  social_score_percentile DOUBLE PRECISION NULL,
  governance_score DOUBLE PRECISION NULL,
  governance_score_percentile DOUBLE PRECISION NULL,
  controversy_level DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, rating_date)
);

CREATE TABLE IF NOT EXISTS public.company_esg_activities
(
  company_id UUID NOT NULL,
  activity TEXT NOT NULL,
  involved BOOLEAN NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, created_timestamp, activity)
);

CREATE TABLE IF NOT EXISTS public.company_outstanding_shares
(
  company_id UUID NOT NULL,
  "date" DATE NOT NULL,
  shares_mln NUMERIC(14,4) NULL,
  shares BIGINT NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, "date")
);

CREATE TABLE IF NOT EXISTS public.company_earnings_history
(
  company_id UUID NOT NULL,
  report_date DATE NOT NULL,
  "date" DATE NULL,
  before_after_market TEXT NULL,
  currency TEXT NULL,
  eps_actual NUMERIC(22,2) NULL,
  eps_estimate NUMERIC(22,2) NULL,
  eps_difference NUMERIC(22,2) NULL,
  surprise_percent DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, report_date)
);

CREATE TABLE IF NOT EXISTS public.company_earnings_trends
(
  company_id UUID NOT NULL,
  "date" DATE NOT NULL,
  period TEXT NULL,
  growth DOUBLE PRECISION NULL,
  earnings_estimate_avg NUMERIC(22,2) NULL,
  earnings_estimate_low NUMERIC(22,2) NULL,
  earnings_estimate_high NUMERIC(22,2) NULL,
  earnings_estimate_year_ago_eps NUMERIC(22,2) NULL,
  earnings_estimate_number_of_analysts NUMERIC(7,2) NULL,
  earnings_estimate_growth DOUBLE PRECISION NULL,
  revenue_estimate_avg NUMERIC(22,2) NULL,
  revenue_estimate_low NUMERIC(22,2) NULL,
  revenue_estimate_high NUMERIC(22,2) NULL,
  revenue_estimate_year_ago_eps NUMERIC(22,2) NULL,
  revenue_estimate_number_of_analysts NUMERIC(22,2) NULL,
  revenue_estimate_growth DOUBLE PRECISION NULL,
  eps_trend_current NUMERIC(22,2) NULL,
  eps_trend7days_ago NUMERIC(22,2) NULL,
  eps_trend30days_ago NUMERIC(22,2) NULL,
  eps_trend60days_ago NUMERIC(22,2) NULL,
  eps_trend90days_ago NUMERIC(22,2) NULL,
  eps_revisions_up_last7days NUMERIC(22,2) NULL,
  eps_revisions_up_last30days NUMERIC(22,2) NULL,
  eps_revisions_down_last7days NUMERIC(22,2) NULL,
  eps_revisions_down_last30days NUMERIC(22,2) NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(company_id, "date")
);

CREATE TABLE IF NOT EXISTS public.company_earnings_annual
(
  company_id UUID NOT NULL,
  "date" DATE NOT NULL,
  eps_actual NUMERIC(22,2) NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, "date")
);

CREATE TABLE IF NOT EXISTS public.company_balance_sheets
(
  company_id UUID NOT NULL,
  type TEXT NOT NULL,
  "date" DATE NOT NULL,
  filing_date DATE NULL,
  currency_symbol TEXT NULL,
  total_assets NUMERIC(22,2) NULL,
  intangible_assets NUMERIC(22,2) NULL,
  earning_assets NUMERIC(22,2) NULL,
  other_current_assets NUMERIC(22,2) NULL,
  total_liab NUMERIC(22,2) NULL,
  total_stockholder_equity NUMERIC(22,2) NULL,
  deferred_long_term_liab NUMERIC(22,2) NULL,
  other_current_liab NUMERIC(22,2) NULL,
  common_stock NUMERIC(22,2) NULL,
  capital_stock NUMERIC(22,2) NULL,
  retained_earnings NUMERIC(22,2) NULL,
  other_liab NUMERIC(22,2) NULL,
  good_will NUMERIC(22,2) NULL,
  other_assets NUMERIC(22,2) NULL,
  cash NUMERIC(22,2) NULL,
  cash_and_equivalents NUMERIC(22,2) NULL,
  total_current_liabilities NUMERIC(22,2) NULL,
  current_deferred_revenue NUMERIC(22,2) NULL,
  net_debt NUMERIC(22,2) NULL,
  short_term_debt NUMERIC(22,2) NULL,
  short_long_term_debt NUMERIC(22,2) NULL,
  short_long_term_debt_total NUMERIC(22,2) NULL,
  other_stockholder_equity NUMERIC(22,2) NULL,
  property_plant_equipment NUMERIC(22,2) NULL,
  total_current_assets NUMERIC(22,2) NULL,
  long_term_investments NUMERIC(22,2) NULL,
  net_tangible_assets NUMERIC(22,2) NULL,
  short_term_investments NUMERIC(22,2) NULL,
  net_receivables NUMERIC(22,2) NULL,
  long_term_debt NUMERIC(22,2) NULL,
  inventory NUMERIC(22,2) NULL,
  accounts_payable NUMERIC(22,2) NULL,
  total_permanent_equity NUMERIC(22,2) NULL,
  noncontrolling_interest_in_consolidated_entity NUMERIC(22,2) NULL,
  temporary_equity_redeemable_noncontrolling_interests NUMERIC(22,2) NULL,
  accumulated_other_comprehensive_income NUMERIC(22,2) NULL,
  additional_paid_in_capital NUMERIC(22,2) NULL,
  common_stock_total_equity NUMERIC(22,2) NULL,
  preferred_stock_total_equity NUMERIC(22,2) NULL,
  retained_earnings_total_equity NUMERIC(22,2) NULL,
  treasury_stock NUMERIC(22,2) NULL,
  accumulated_amortization NUMERIC(22,2) NULL,
  non_currrent_assets_other NUMERIC(22,2) NULL,
  deferred_long_term_asset_charges NUMERIC(22,2) NULL,
  non_current_assets_total NUMERIC(22,2) NULL,
  capital_lease_obligations NUMERIC(22,2) NULL,
  long_term_debt_total NUMERIC(22,2) NULL,
  non_current_liabilities_other NUMERIC(22,2) NULL,
  non_current_liabilities_total NUMERIC(22,2) NULL,
  negative_goodwill NUMERIC(22,2) NULL,
  warrants NUMERIC(22,2) NULL,
  preferred_stock_redeemable NUMERIC(22,2) NULL,
  capital_surpluses NUMERIC(22,2) NULL,
  liabilities_and_stockholders_equity NUMERIC(22,2) NULL,
  cash_and_short_term_investments NUMERIC(22,2) NULL,
  property_plant_and_equipment_gross NUMERIC(22,2) NULL,
  property_plant_and_equipment_net NUMERIC(22,2) NULL,
  accumulated_depreciation NUMERIC(22,2) NULL,
  net_working_capital NUMERIC(22,2) NULL,
  net_invested_capital NUMERIC(22,2) NULL,
  common_stock_shares_outstanding NUMERIC(22,2) NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, type, "date")
);

CREATE TABLE IF NOT EXISTS public.company_cash_flows
(
  company_id UUID NOT NULL,
  type TEXT NOT NULL,
  "date" DATE NOT NULL,
  filing_date DATE NULL,
  currency_symbol TEXT NULL,
  investments NUMERIC(22,2) NULL,
  change_to_liabilities NUMERIC(22,2) NULL,
  total_cashflows_from_investing_activities NUMERIC(22,2) NULL,
  net_borrowings NUMERIC(22,2) NULL,
  total_cash_from_financing_activities NUMERIC(22,2) NULL,
  change_to_operating_activities NUMERIC(22,2) NULL,
  net_income NUMERIC(22,2) NULL,
  change_in_cash NUMERIC(22,2) NULL,
  begin_period_cash_flow NUMERIC(22,2) NULL,
  end_period_cash_flow NUMERIC(22,2) NULL,
  total_cash_from_operating_activities NUMERIC(22,2) NULL,
  issuance_of_capital_stock NUMERIC(22,2) NULL,
  depreciation NUMERIC(22,2) NULL,
  other_cashflows_from_investing_activities NUMERIC(22,2) NULL,
  dividends_paid NUMERIC(22,2) NULL,
  change_to_inventory NUMERIC(22,2) NULL,
  change_to_account_receivables NUMERIC(22,2) NULL,
  sale_purchase_of_stock NUMERIC(22,2) NULL,
  other_cashflows_from_financing_activities NUMERIC(22,2) NULL,
  change_to_netincome NUMERIC(22,2) NULL,
  capital_expenditures NUMERIC(22,2) NULL,
  change_receivables NUMERIC(22,2) NULL,
  cash_flows_other_operating NUMERIC(22,2) NULL,
  exchange_rate_changes NUMERIC(22,2) NULL,
  cash_and_cash_equivalents_changes NUMERIC(22,2) NULL,
  change_in_working_capital NUMERIC(22,2) NULL,
  other_non_cash_items NUMERIC(22,2) NULL,
  free_cash_flow NUMERIC(22,2) NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, type, "date")
);

CREATE TABLE IF NOT EXISTS public.company_income_statements
(
  company_id UUID NOT NULL,
  type TEXT NOT NULL,
  "date" DATE NOT NULL,
  filing_date DATE NULL,
  currency_symbol TEXT NULL,
  research_development NUMERIC(22,2) NULL,
  effect_of_accounting_charges NUMERIC(22,2) NULL,
  income_before_tax NUMERIC(22,2) NULL,
  minority_interest NUMERIC(22,2) NULL,
  net_income NUMERIC(22,2) NULL,
  selling_general_administrative NUMERIC(22,2) NULL,
  selling_and_marketing_expenses NUMERIC(22,2) NULL,
  gross_profit NUMERIC(22,2) NULL,
  reconciled_depreciation NUMERIC(22,2) NULL,
  ebit NUMERIC(22,2) NULL,
  ebitda NUMERIC(22,2) NULL,
  depreciation_and_amortization NUMERIC(22,2) NULL,
  non_operating_income_net_other NUMERIC(22,2) NULL,
  operating_income NUMERIC(22,2) NULL,
  other_operating_expenses NUMERIC(22,2) NULL,
  interest_expense NUMERIC(22,2) NULL,
  tax_provision NUMERIC(22,2) NULL,
  interest_income NUMERIC(22,2) NULL,
  net_interest_income NUMERIC(22,2) NULL,
  extraordinary_items NUMERIC(22,2) NULL,
  non_recurring NUMERIC(22,2) NULL,
  other_items NUMERIC(22,2) NULL,
  income_tax_expense NUMERIC(22,2) NULL,
  total_revenue NUMERIC(22,2) NULL,
  total_operating_expenses NUMERIC(22,2) NULL,
  cost_of_revenue NUMERIC(22,2) NULL,
  total_other_income_expense_net NUMERIC(22,2) NULL,
  discontinued_operations NUMERIC(22,2) NULL,
  net_income_from_continuing_ops NUMERIC(22,2) NULL,
  net_income_applicable_to_common_shares NUMERIC(22,2) NULL,
  preferred_stock_and_other_adjustments NUMERIC(22,2) NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, type, "date")
);

CREATE TABLE IF NOT EXISTS public.etfs
(
  global_id UUID NOT NULL,
  symbol TEXT NULL,
  exchange TEXT NULL,
  name TEXT NULL,
  type TEXT NULL,
  currency_code TEXT NULL,
  currency_name TEXT NULL,
  currency_symbol TEXT NULL,
  country_name TEXT NULL,
  country_iso TEXT NULL,
  description TEXT NULL,
  category TEXT NULL,
  update_at DATE NULL,
  isin TEXT NULL,
  company_name TEXT NULL,
  company_url TEXT NULL,
  etf_url TEXT NULL,
  domicile TEXT NULL,
  index_name TEXT NULL,
  yield DOUBLE PRECISION NULL,
  dividend_paying_frequency TEXT NULL,
  inception_date DATE NULL,
  max_annual_mgmt_charge NUMERIC(22,2) NULL,
  ongoing_charge NUMERIC(22,2) NULL,
  date_ongoing_charge DATE NULL,
  net_expense_ratio DOUBLE PRECISION NULL,
  annual_holdings_turnover DOUBLE PRECISION NULL,
  total_assets NUMERIC(22,2) NULL,
  average_mkt_cap_mln NUMERIC(14,4) NULL,
  holdings_count INTEGER NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (global_id)
);

CREATE TABLE IF NOT EXISTS public.etf_technicals
(
  etf_id UUID NOT NULL,
  beta DOUBLE PRECISION NULL,
  fifty_two_week_high NUMERIC (18,2) NULL,
  fifty_two_week_low NUMERIC (18,2) NULL,
  fifty_day_ma NUMERIC (18,2) NULL,
  two_hundred_day_ma NUMERIC (18,2) NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_market_capitalization
(
  etf_id UUID NOT NULL,
  mega DOUBLE PRECISION NULL,
  big DOUBLE PRECISION NULL,
  medium DOUBLE PRECISION NULL,
  small DOUBLE PRECISION NULL,
  micro DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_asset_allocations
(
  etf_id UUID NOT NULL,
  category TEXT NULL,
  long_percentage DOUBLE PRECISION NULL,
  short_percentage DOUBLE PRECISION NULL,
  net_assets_percentage DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_world_regions
(
  etf_id UUID NOT NULL,
  region TEXT NULL,
  equity_percentage DOUBLE PRECISION NULL,
  relative_to_category DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_sector_weights
(
  etf_id UUID NOT NULL,
  sector TEXT NULL,
  equity_percentage DOUBLE PRECISION NULL,
  relative_to_category DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_fixed_incomes
(
  etf_id UUID NOT NULL,
  category TEXT NULL,
  fund_percentage DOUBLE PRECISION NULL,
  relative_to_category DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_top_ten_holdings
(
  etf_id UUID NOT NULL,
  symbol TEXT NULL,
  exchange TEXT NULL,
  name TEXT NULL,
  sector TEXT NULL,
  industry TEXT NULL,
  country TEXT NULL,
  region TEXT NULL,
  assets_percentage DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_holdings
(
  etf_id UUID NOT NULL,
  symbol TEXT NULL,
  exchange TEXT NULL,
  name TEXT NULL,
  sector TEXT NULL,
  industry TEXT NULL,
  country TEXT NULL,
  region TEXT NULL,
  assets_percentage DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_valuation_growths
(
  etf_id UUID NOT NULL,
  category TEXT NULL,
  price_prospective_earnings DOUBLE PRECISION NULL,
  price_book DOUBLE PRECISION NULL,
  price_sales DOUBLE PRECISION NULL,
  price_cash_flow DOUBLE PRECISION NULL,
  dividend_yield_factor DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_morning_star
(
  etf_id UUID NOT NULL,
  ratio INTEGER NULL,
  category_benchmark TEXT NULL,
  sustainability_ratio INTEGER NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.etf_performance
(
  etf_id UUID NOT NULL,
  one_year_volatility DOUBLE PRECISION NULL,
  three_year_volatility DOUBLE PRECISION NULL,
  three_year_exp_return DOUBLE PRECISION NULL,
  three_year_sharp_ratio DOUBLE PRECISION NULL,
  returns_ytd DOUBLE PRECISION NULL,
  returns_1_y DOUBLE PRECISION NULL,
  returns_3_y DOUBLE PRECISION NULL,
  returns_5_y DOUBLE PRECISION NULL,
  returns_10_y DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.options
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  last_trade_date TIMESTAMP NOT NULL,
  last_trade_price NUMERIC(22,2) NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, last_trade_date)
);

CREATE TABLE IF NOT EXISTS public.option_data
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  expiration_date DATE NOT NULL,
  implied_volatility DOUBLE PRECISION NULL,
  put_volume INTEGER NULL,
  call_volume INTEGER NULL,
  put_call_volume_ratio DOUBLE PRECISION NULL,
  put_open_interest INTEGER NULL,
  call_open_interest INTEGER NULL,
  put_call_open_interest_ratio DOUBLE PRECISION NULL,
  options_count INTEGER NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, expiration_date)
);

CREATE TABLE IF NOT EXISTS public.option_contracts
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  expiration_date DATE NOT NULL,
  option_type TEXT NOT NULL,
  contract_name TEXT NULL,
  contract_size TEXT NULL,
  contract_period TEXT NULL,
  currency TEXT NULL,
  in_the_money BOOLEAN NULL,
  last_trade_date DATE NULL,  
  strike NUMERIC(22,2) NULL,
  last_price NUMERIC(22,2) NULL,
  bid NUMERIC(22,2) NULL,
  ask NUMERIC(22,2) NULL,
  change NUMERIC(22,2) NULL,
  change_percent DOUBLE PRECISION NULL,
  volume INTEGER NULL,
  open_interest INTEGER NULL,
  implied_volatility DOUBLE PRECISION NULL,
  delta DOUBLE PRECISION NULL,
  gamma DOUBLE PRECISION NULL,
  theta DOUBLE PRECISION NULL,
  vega DOUBLE PRECISION NULL,
  rho DOUBLE PRECISION NULL,
  theoretical NUMERIC(22,2) NULL,
  intrinsic_value NUMERIC(22,2) NULL,
  time_value NUMERIC(22,2) NULL,
  updated_at DATE NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, expiration_date, option_type)
);

CREATE TABLE IF NOT EXISTS public.calendar_ipos
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  name TEXT NULL,
  currency TEXT NULL,
  start_date DATE NULL,
  filing_date DATE NULL,
  amended_date DATE NULL,
  price_from NUMERIC(22,2) NULL,
  price_to NUMERIC(22,2) NULL,
  offer_price NUMERIC(22,2) NULL,
  shares BIGINT NULL,
  deal_type TEXT NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, created_timestamp)
);

CREATE TABLE IF NOT EXISTS public.calendar_earnings
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  report_date DATE NULL,
  ending_date DATE NOT NULL,
  before_after_market TEXT NULL,
  currency TEXT NULL,
  actual NUMERIC(22,2) NULL,
  estimate NUMERIC(22,2) NULL,
  difference NUMERIC(22,2) NULL,
  percent DOUBLE PRECISION NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, ending_date)
);

CREATE TABLE IF NOT EXISTS public.calendar_trends
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  "date" DATE NOT NULL,
  period TEXT NULL,
  growth DOUBLE PRECISION NULL,
  earnings_estimate_avg NUMERIC(22,2) NULL,
  earnings_estimate_low NUMERIC(22,2) NULL,
  earnings_estimate_high NUMERIC(22,2) NULL,
  earnings_estimate_number_analysts INT NULL,
  earnings_estimate_growth DOUBLE PRECISION NULL,
  revenue_estimate_avg NUMERIC(22,2) NULL,
  revenue_estimate_low NUMERIC(22,2) NULL,
  revenue_estimate_high NUMERIC(22,2) NULL,
  revenue_estimate_year_ago_eps  NUMERIC(22,2) NULL,
  revenue_estimate_number_analysts INT NULL,
  revenue_estimate_growth DOUBLE PRECISION NULL,
  eps_trend_current NUMERIC(22,2) NULL,
  eps_trend_7days_ago NUMERIC(22,2) NULL,
  eps_trend_30days_ago NUMERIC(22,2) NULL,
  eps_trend_60days_ago NUMERIC(22,2) NULL,
  eps_trend_90days_ago NUMERIC(22,2) NULL,
  eps_revisions_up_last7_days NUMERIC(22,2) NULL,
  eps_revisions_up_last30_days NUMERIC(22,2) NULL,
  eps_revisions_down_last30_days NUMERIC(22,2) NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, "date")
);

CREATE TABLE IF NOT EXISTS public.symbols_to_ignore
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  reason TEXT NULL,
  created_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange)
);

CREATE TABLE IF NOT EXISTS public.exchange_checks
(
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL,
  PRIMARY KEY (utc_timestamp)
)