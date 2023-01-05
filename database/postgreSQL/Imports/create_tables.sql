CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS public.exchanges
(
  name TEXT NOT NULL,
  code TEXT NOT NULL,
  operating_mic TEXT NULL,
  country TEXT NOT NULL,
  currency TEXT NOT NULL,
  country_iso_2 TEXT NULL,
  country_iso_3 TEXT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (code)
);

CREATE TABLE IF NOT EXISTS public.symbols
(
  code TEXT NOT NULL,
  exchange TEXT NOT NULL,
  name TEXT NOT NULL,
  country TEXT NOT NULL,
  currency TEXT NOT NULL,
  type TEXT NOT NULL,
  has_options BOOLEAN NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (code, exchange)
);

CREATE TABLE IF NOT EXISTS public.splits
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  date_of_split DATE NOT NULL,
  before_split DOUBLE PRECISION NOT NULL,
  after_split DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, date_of_split)
);

CREATE TABLE IF NOT EXISTS public.dividends
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  "date" DATE NOT NULL,
  value NUMERIC(18,5) NOT NULL,
  unadjusted_value NUMERIC(18,5) NOT NULL,
  currency TEXT NOT NULL,
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
  open NUMERIC(18,2) NOT NULL,
  high NUMERIC(18,2) NOT NULL,
  low NUMERIC(18,2) NOT NULL,
  close NUMERIC(18,2) NOT NULL,
  volume BIGINT NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (exchange, symbol, start, finish)
);

CREATE TABLE IF NOT EXISTS public.companies
(
  global_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  name TEXT NOT NULL,
  type TEXT NOT NULL,
  currency_code TEXT NOT NULL,
  currency_name TEXT NOT NULL,
  currency_symbol TEXT NOT NULL,
  country_name TEXT NOT NULL,
  country_iso TEXT NOT NULL,
  isin TEXT NULL,
  lei TEXT NULL,
  cusip TEXT NULL,
  cik TEXT NULL,
  employer_id_number TEXT NULL,
  fiscal_year_end TEXT NULL,
  ipo_date DATE NOT NULL,
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
  update_at DATE NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, type)
);

CREATE TABLE IF NOT EXISTS public.company_addresses
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  street TEXT NOT NULL,
  city TEXT NOT NULL,
  state TEXT NOT NULL,
  country TEXT NOT NULL,
  postal_code TEXT NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, street, postal_code)
);

CREATE TABLE IF NOT EXISTS public.company_listings
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  name TEXT NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.company_officers
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  name TEXT NOT NULL,
  title TEXT NULL,
  year_born TEXT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.company_highlights
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  market_capitalization NUMERIC(18,2) NOT NULL,
  market_capitalization_mln NUMERIC(12,2) NOT NULL,
  ebitda NUMERIC(18,2) NOT NULL,
  pe_ratio DOUBLE PRECISION NOT NULL,
  peg_ratio DOUBLE PRECISION NOT NULL,
  wall_street_target_price NUMERIC(18,2) NOT NULL,
  book_value DOUBLE PRECISION NOT NULL,
  dividend_share DOUBLE PRECISION NOT NULL,
  dividend_yield DOUBLE PRECISION NOT NULL,
  earnings_share NUMERIC(18,2) NOT NULL,
  eps_estimate_current_year NUMERIC(18,2) NOT NULL,
  eps_estimate_next_year NUMERIC(18,2) NOT NULL,
  eps_estimate_next_quarter NUMERIC(18,2) NOT NULL,
  eps_estimate_current_quarter NUMERIC(18,2) NOT NULL,
  most_recent_quarter DATE NOT NULL,
  profit_margin DOUBLE PRECISION NOT NULL,
  operating_margin_ttm DOUBLE PRECISION NOT NULL,
  return_on_assets_ttm DOUBLE PRECISION NOT NULL,
  return_on_equity_ttm DOUBLE PRECISION NOT NULL,
  revenue_ttm NUMERIC(18,2) NOT NULL,
  revenue_per_share_ttm NUMERIC(18,2) NOT NULL,
  quarterly_revenue_growth_yoy DOUBLE PRECISION NOT NULL,
  gross_profit_ttm NUMERIC(18,2) NOT NULL,
  diluted_eps_ttm NUMERIC(18,2) NOT NULL,
  quarterly_earnings_growth_yoy DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.company_valuations
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  trailing_pe DOUBLE PRECISION NOT NULL,
  forward_pe DOUBLE PRECISION NOT NULL,
  price_sales_ttm DOUBLE PRECISION NOT NULL,
  price_book_mrq DOUBLE PRECISION NOT NULL,
  enterprise_value NUMERIC(18,2) NOT NULL,
  enterprise_value_revenue DOUBLE PRECISION NOT NULL,
  enterprise_value_ebitda DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.company_shares_stats
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  shares_outstanding DOUBLE PRECISION NOT NULL,
  shares_float DOUBLE PRECISION NOT NULL,
  percent_insiders DOUBLE PRECISION NOT NULL,
  percent_institutions DOUBLE PRECISION NOT NULL,
  shares_short DOUBLE PRECISION NOT NULL,
  shares_short_prior_month DOUBLE PRECISION NOT NULL,
  short_ratio DOUBLE PRECISION NOT NULL,
  short_percent_outstanding DOUBLE PRECISION NOT NULL,
  short_percent_float DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.company_technicals
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  beta DOUBLE PRECISION NOT NULL,
  fifty_two_week_high NUMERIC (18,2) NOT NULL,
  fifty_two_week_low NUMERIC (18,2) NOT NULL,
  fifty_day_ma NUMERIC (18,2) NOT NULL,
  two_hundred_day_ma NUMERIC (18,2) NOT NULL,
  shares_short NUMERIC (18,2) NOT NULL,
  shares_short_prior_month NUMERIC (18,2) NOT NULL,
  short_ratio DOUBLE PRECISION NOT NULL,
  short_percent DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.company_dividends
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  forward_annual_dividend_rate DOUBLE PRECISION NOT NULL,
  forward_annual_dividend_yield DOUBLE PRECISION NOT NULL,
  payout_ratio DOUBLE PRECISION NOT NULL,
  dividend_date DATE NOT NULL,
  ex_dividend_date DATE NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.company_dividends_by_year
(
  company_id UUID NOT NULL,
  year INTEGER NOT NULL,
  count INTEGER NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, year)
);

CREATE TABLE IF NOT EXISTS public.company_analyst_ratings
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  rating DOUBLE PRECISION NOT NULL,
  target_price NUMERIC(18,2) NOT NULL,
  strong_buy INTEGER NOT NULL,
  buy INTEGER NOT NULL,
  hold INTEGER NOT NULL,
  sell INTEGER NOT NULL,
  strong_sell INTEGER NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.company_holders
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  holder_type TEXT NOT NULL,
  name TEXT NOT NULL,
  "date" DATE NOT NULL,
  total_shares DOUBLE PRECISION NOT NULL,
  total_assets DOUBLE PRECISION NOT NULL,
  current_shares INTEGER NOT NULL,
  change INTEGER NOT NULL,
  change_percentage DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, holder_type, name, "date")
);

CREATE TABLE IF NOT EXISTS public.company_insider_transactions
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  "date" DATE NOT NULL,
  owner_cik TEXT NULL,
  owner_name TEXT NOT NULL,
  transaction_date DATE NOT NULL,
  transaction_code TEXT NOT NULL,
  transaction_amount INTEGER NOT NULL,
  transaction_price NUMERIC(18,2) NOT NULL,
  transaction_acquired_disposed TEXT NOT NULL,
  post_transaction_amount INTEGER NOT NULL,
  sec_link TEXT NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date, post_transaction_amount)
);

CREATE TABLE IF NOT EXISTS public.company_esg_scores
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  rating_date DATE NOT NULL,
  total_esg DOUBLE PRECISION NOT NULL,
  total_esg_percentile DOUBLE PRECISION NOT NULL,
  environment_score DOUBLE PRECISION NOT NULL,
  environment_score_percentile DOUBLE PRECISION NOT NULL,
  social_score DOUBLE PRECISION NOT NULL,
  social_score_percentile DOUBLE PRECISION NOT NULL,
  governance_score DOUBLE PRECISION NOT NULL,
  governance_score_percentile DOUBLE PRECISION NOT NULL,
  controversy_level DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, rating_date)
);

CREATE TABLE IF NOT EXISTS public.company_esg_activities
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  activity TEXT NOT NULL,
  involved BOOLEAN NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, date_captured, activity)
);

CREATE TABLE IF NOT EXISTS public.company_outstanding_shares
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  "date" DATE NOT NULL,
  shares_mln NUMERIC(14,4) NOT NULL,
  shares BIGINT NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, "date")
);

CREATE TABLE IF NOT EXISTS public.company_earnings_history
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  report_date DATE NOT NULL,
  "date" DATE NOT NULL,
  before_after_market TEXT NULL,
  currency TEXT NOT NULL,
  eps_actual NUMERIC(18,2) NULL,
  eps_estimate NUMERIC(18,2) NULL,
  eps_difference NUMERIC(18,2) NULL,
  surprise_percent DOUBLE PRECISION NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, report_date)
);

CREATE TABLE IF NOT EXISTS public.company_earnings_trends
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  "date" DATE NOT NULL,
  period TEXT NOT NULL,
  growth DOUBLE PRECISION NOT NULL,
  earnings_estimate_avg NUMERIC(18,2) NULL,
  earnings_estimate_low NUMERIC(18,2) NULL,
  earnings_estimate_high NUMERIC(18,2) NULL,
  earnings_estimate_year_ago_eps NUMERIC(18,2) NULL,
  earnings_estimate_number_of_analysts NUMERIC(7,2) NULL,
  earnings_estimate_growth DOUBLE PRECISION NULL,
  revenue_estimate_avg NUMERIC(18,2) NULL,
  revenue_estimate_low NUMERIC(18,2) NULL,
  revenue_estimate_high NUMERIC(18,2) NULL,
  revenue_estimate_year_ago_eps NUMERIC(18,2) NULL,
  revenue_estimate_number_of_analysts NUMERIC(18,2) NULL,
  revenue_estimate_growth DOUBLE PRECISION NULL,
  eps_trend_current NUMERIC(18,2) NULL,
  eps_trend7days_ago NUMERIC(18,2) NULL,
  eps_trend30days_ago NUMERIC(18,2) NULL,
  eps_trend60days_ago NUMERIC(18,2) NULL,
  eps_trend90days_ago NUMERIC(18,2) NULL,
  eps_revisions_up_last7days NUMERIC(18,2) NULL,
  eps_revisions_up_last30days NUMERIC(18,2) NULL,
  eps_revisions_down_last7days NUMERIC(18,2) NULL,
  eps_revisions_down_last30days NUMERIC(18,2) NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY(company_id, "date")
);

CREATE TABLE IF NOT EXISTS public.company_earnings_annual
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  "date" DATE NOT NULL,
  eps_actual NUMERIC(18,2) NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, "date")
);

CREATE TABLE IF NOT EXISTS public.company_balance_sheets
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  type TEXT NOT NULL,
  "date" DATE NOT NULL,
  filing_date DATE NOT NULL,
  currency_symbol TEXT NOT NULL,
  total_assets NUMERIC(18,2) NULL,
  intangible_assets NUMERIC(18,2) NULL,
  earning_assets NUMERIC(18,2) NULL,
  other_current_assets NUMERIC(18,2) NULL,
  total_liab NUMERIC(18,2) NULL,
  total_stockholder_equity NUMERIC(18,2) NULL,
  deferred_long_term_liab NUMERIC(18,2) NULL,
  other_current_liab NUMERIC(18,2) NULL,
  common_stock NUMERIC(18,2) NULL,
  capital_stock NUMERIC(18,2) NULL,
  retained_earnings NUMERIC(18,2) NULL,
  other_liab NUMERIC(18,2) NULL,
  good_will NUMERIC(18,2) NULL,
  other_assets NUMERIC(18,2) NULL,
  cash NUMERIC(18,2) NULL,
  cash_and_equivalents NUMERIC(18,2) NULL,
  total_current_liabilities NUMERIC(18,2) NULL,
  current_deferred_revenue NUMERIC(18,2) NULL,
  net_debt NUMERIC(18,2) NULL,
  short_term_debt NUMERIC(18,2) NULL,
  short_long_term_debt NUMERIC(18,2) NULL,
  short_long_term_debt_total NUMERIC(18,2) NULL,
  other_stockholder_equity NUMERIC(18,2) NULL,
  property_plant_equipment NUMERIC(18,2) NULL,
  total_current_assets NUMERIC(18,2) NULL,
  long_term_investments NUMERIC(18,2) NULL,
  net_tangible_assets NUMERIC(18,2) NULL,
  short_term_investments NUMERIC(18,2) NULL,
  net_receivables NUMERIC(18,2) NULL,
  long_term_debt NUMERIC(18,2) NULL,
  inventory NUMERIC(18,2) NULL,
  accounts_payable NUMERIC(18,2) NULL,
  total_permanent_equity NUMERIC(18,2) NULL,
  noncontrolling_interest_in_consolidated_entity NUMERIC(18,2) NULL,
  temporary_equity_redeemable_noncontrolling_interests NUMERIC(18,2) NULL,
  accumulated_other_comprehensive_income NUMERIC(18,2) NULL,
  additional_paid_in_capital NUMERIC(18,2) NULL,
  common_stock_total_equity NUMERIC(18,2) NULL,
  preferred_stock_total_equity NUMERIC(18,2) NULL,
  retained_earnings_total_equity NUMERIC(18,2) NULL,
  treasury_stock NUMERIC(18,2) NULL,
  accumulated_amortization NUMERIC(18,2) NULL,
  non_currrent_assets_other NUMERIC(18,2) NULL,
  deferred_long_term_asset_charges NUMERIC(18,2) NULL,
  non_current_assets_total NUMERIC(18,2) NULL,
  capital_lease_obligations NUMERIC(18,2) NULL,
  long_term_debt_total NUMERIC(18,2) NULL,
  non_current_liabilities_other NUMERIC(18,2) NULL,
  non_current_liabilities_total NUMERIC(18,2) NULL,
  negative_goodwill NUMERIC(18,2) NULL,
  warrants NUMERIC(18,2) NULL,
  preferred_stock_redeemable NUMERIC(18,2) NULL,
  capital_surpluses NUMERIC(18,2) NULL,
  liabilities_and_stockholders_equity NUMERIC(18,2) NULL,
  cash_and_short_term_investments NUMERIC(18,2) NULL,
  property_plant_and_equipment_gross NUMERIC(18,2) NULL,
  property_plant_and_equipment_net NUMERIC(18,2) NULL,
  accumulated_depreciation NUMERIC(18,2) NULL,
  net_working_capital NUMERIC(18,2) NULL,
  net_invested_capital NUMERIC(18,2) NULL,
  common_stock_shares_outstanding NUMERIC(18,2) NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, type, "date")
);

CREATE TABLE IF NOT EXISTS public.company_cash_flows
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  type TEXT NOT NULL,
  "date" DATE NOT NULL,
  filing_date DATE NOT NULL,
  currency_symbol TEXT NOT NULL,
  investments NUMERIC(18,2) NULL,
  change_to_liabilities NUMERIC(18,2) NULL,
  total_cashflows_from_investing_activities NUMERIC(18,2) NULL,
  net_borrowings NUMERIC(18,2) NULL,
  total_cash_from_financing_activities NUMERIC(18,2) NULL,
  change_to_operating_activities NUMERIC(18,2) NULL,
  net_income NUMERIC(18,2) NULL,
  change_in_cash NUMERIC(18,2) NULL,
  begin_period_cash_flow NUMERIC(18,2) NULL,
  end_period_cash_flow NUMERIC(18,2) NULL,
  total_cash_from_operating_activities NUMERIC(18,2) NULL,
  issuance_of_capital_stock NUMERIC(18,2) NULL,
  depreciation NUMERIC(18,2) NULL,
  other_cashflows_from_investing_activities NUMERIC(18,2) NULL,
  dividends_paid NUMERIC(18,2) NULL,
  change_to_inventory NUMERIC(18,2) NULL,
  change_to_account_receivables NUMERIC(18,2) NULL,
  sale_purchase_of_stock NUMERIC(18,2) NULL,
  other_cashflows_from_financing_activities NUMERIC(18,2) NULL,
  change_to_netincome NUMERIC(18,2) NULL,
  capital_expenditures NUMERIC(18,2) NULL,
  change_receivables NUMERIC(18,2) NULL,
  cash_flows_other_operating NUMERIC(18,2) NULL,
  exchange_rate_changes NUMERIC(18,2) NULL,
  cash_and_cash_equivalents_changes NUMERIC(18,2) NULL,
  change_in_working_capital NUMERIC(18,2) NULL,
  other_non_cash_items NUMERIC(18,2) NULL,
  free_cash_flow NUMERIC(18,2) NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, type, "date")
);

CREATE TABLE IF NOT EXISTS public.company_income_statements
(
  company_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  type TEXT NOT NULL,
  "date" DATE NOT NULL,
  filing_date DATE NOT NULL,
  currency_symbol TEXT NOT NULL,
  research_development NUMERIC(18,2) NULL,
  effect_of_accounting_charges NUMERIC(18,2) NULL,
  income_before_tax NUMERIC(18,2) NULL,
  minority_interest NUMERIC(18,2) NULL,
  net_income NUMERIC(18,2) NULL,
  selling_general_administrative NUMERIC(18,2) NULL,
  selling_and_marketing_expenses NUMERIC(18,2) NULL,
  gross_profit NUMERIC(18,2) NULL,
  reconciled_depreciation NUMERIC(18,2) NULL,
  ebit NUMERIC(18,2) NULL,
  ebitda NUMERIC(18,2) NULL,
  depreciation_and_amortization NUMERIC(18,2) NULL,
  non_operating_income_net_other NUMERIC(18,2) NULL,
  operating_income NUMERIC(18,2) NULL,
  other_operating_expenses NUMERIC(18,2) NULL,
  interest_expense NUMERIC(18,2) NULL,
  tax_provision NUMERIC(18,2) NULL,
  interest_income NUMERIC(18,2) NULL,
  net_interest_income NUMERIC(18,2) NULL,
  extraordinary_items NUMERIC(18,2) NULL,
  non_recurring NUMERIC(18,2) NULL,
  other_items NUMERIC(18,2) NULL,
  income_tax_expense NUMERIC(18,2) NULL,
  total_revenue NUMERIC(18,2) NULL,
  total_operating_expenses NUMERIC(18,2) NULL,
  cost_of_revenue NUMERIC(18,2) NULL,
  total_other_income_expense_net NUMERIC(18,2) NULL,
  discontinued_operations NUMERIC(18,2) NULL,
  net_income_from_continuing_ops NUMERIC(18,2) NULL,
  net_income_applicable_to_common_shares NUMERIC(18,2) NULL,
  preferred_stock_and_other_adjustments NUMERIC(18,2) NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (company_id, type, "date")
);

CREATE TABLE IF NOT EXISTS public.etfs
(
  global_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  name TEXT NOT NULL,
  type TEXT NOT NULL,
  currency_code TEXT NOT NULL,
  currency_name TEXT NOT NULL,
  currency_symbol TEXT NOT NULL,
  country_name TEXT NOT NULL,
  country_iso TEXT NOT NULL,
  description TEXT NOT NULL,
  category TEXT NOT NULL,
  update_at DATE NOT NULL,
  isin TEXT NOT NULL,
  company_name TEXT NOT NULL,
  company_url TEXT NOT NULL,
  etf_url TEXT NOT NULL,
  domicile TEXT NOT NULL,
  index_name TEXT NOT NULL,
  yield DOUBLE PRECISION NOT NULL,
  dividend_paying_frequency TEXT NOT NULL,
  inception_date DATE NOT NULL,
  max_annual_mgmt_charge NUMERIC(18,2) NOT NULL,
  ongoing_charge NUMERIC(18,2) NOT NULL,
  date_ongoing_charge DATE NULL,
  net_expense_ratio DOUBLE PRECISION NOT NULL,
  annual_holdings_turnover DOUBLE PRECISION NOT NULL,
  total_assets NUMERIC(18,2) NOT NULL,
  average_mkt_cap_mln NUMERIC(14,4) NOT NULL,
  holdings_count INTEGER NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, type)
);

CREATE TABLE IF NOT EXISTS public.etf_technicals
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  beta DOUBLE PRECISION NOT NULL,
  fifty_two_week_high NUMERIC (18,2) NOT NULL,
  fifty_two_week_low NUMERIC (18,2) NOT NULL,
  fifty_day_ma NUMERIC (18,2) NOT NULL,
  two_hundred_day_ma NUMERIC (18,2) NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_market_capitalization
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  mega DOUBLE PRECISION NOT NULL,
  big DOUBLE PRECISION NOT NULL,
  medium DOUBLE PRECISION NOT NULL,
  small DOUBLE PRECISION NOT NULL,
  micro DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_asset_allocations
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  category TEXT NOT NULL,
  long_percentage DOUBLE PRECISION NOT NULL,
  short_percentage DOUBLE PRECISION NOT NULL,
  net_assets_percentage DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_world_regions
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  region TEXT NOT NULL,
  equity_percentage DOUBLE PRECISION NOT NULL,
  relative_to_category DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_sector_weights
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  sector TEXT NOT NULL,
  equity_percentage DOUBLE PRECISION NOT NULL,
  relative_to_category DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_fixed_incomes
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  category TEXT NOT NULL,
  fund_percentage DOUBLE PRECISION NOT NULL,
  relative_to_category DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_top_ten_holdings
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  name TEXT NOT NULL,
  sector TEXT NOT NULL,
  industry TEXT NOT NULL,
  country TEXT NOT NULL,
  region TEXT NOT NULL,
  assets_percentage DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_holdings
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  name TEXT NOT NULL,
  sector TEXT NOT NULL,
  industry TEXT NOT NULL,
  country TEXT NOT NULL,
  region TEXT NOT NULL,
  assets_percentage DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_valuation_growths
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  category TEXT NOT NULL,
  price_prospective_earnings DOUBLE PRECISION NOT NULL,
  price_book DOUBLE PRECISION NOT NULL,
  price_sales DOUBLE PRECISION NOT NULL,
  price_cash_flow DOUBLE PRECISION NOT NULL,
  dividend_yield_factor DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_morning_star
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  ratio INTEGER NOT NULL,
  category_benchmark TEXT NOT NULL,
  sustainability_ratio INTEGER NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);

CREATE TABLE IF NOT EXISTS public.etf_performance
(
  etf_id UUID NOT NULL,
  date_captured DATE NOT NULL,
  one_year_volatility DOUBLE PRECISION NOT NULL,
  three_year_volatility DOUBLE PRECISION NOT NULL,
  three_year_exp_return DOUBLE PRECISION NOT NULL,
  three_year_sharp_ratio DOUBLE PRECISION NOT NULL,
  returns_ytd DOUBLE PRECISION NOT NULL,
  returns_1_y DOUBLE PRECISION NOT NULL,
  returns_3_y DOUBLE PRECISION NOT NULL,
  returns_5_y DOUBLE PRECISION NOT NULL,
  returns_10_y DOUBLE PRECISION NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (etf_id, date_captured)
);


CREATE TABLE IF NOT EXISTS public.options
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  last_trade_date TIMESTAMP NOT NULL,
  last_trade_price NUMERIC(18,2) NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, last_trade_date)
);

CREATE TABLE IF NOT EXISTS public.option_data
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  expiration_date DATE NOT NULL,
  implied_volatility DOUBLE PRECISION NOT NULL,
  put_volume INTEGER NOT NULL,
  call_volume INTEGER NOT NULL,
  put_call_volume_ratio DOUBLE PRECISION NOT NULL,
  put_open_interest INTEGER NOT NULL,
  call_open_interest INTEGER NOT NULL,
  put_call_open_interest_ratio DOUBLE PRECISION NOT NULL,
  options_count INTEGER NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, expiration_date)
);

CREATE TABLE IF NOT EXISTS public.option_contracts
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  expiration_date DATE NOT NULL,
  option_type TEXT NOT NULL,
  contract_name TEXT NOT NULL,
  contract_size TEXT NOT NULL,
  contract_period TEXT NOT NULL,
  currency TEXT NOT NULL,
  in_the_money BOOLEAN NOT NULL,
  last_trade_date DATE NOT NULL,  
  strike NUMERIC(18,2) NOT NULL,
  last_price NUMERIC(18,2) NOT NULL,
  bid NUMERIC(18,2) NOT NULL,
  ask NUMERIC(18,2) NOT NULL,
  change NUMERIC(18,2) NOT NULL,
  change_percent DOUBLE PRECISION NOT NULL,
  volume INTEGER NOT NULL,
  open_interest INTEGER NOT NULL,
  implied_volatility DOUBLE PRECISION NOT NULL,
  delta DOUBLE PRECISION NOT NULL,
  gamma DOUBLE PRECISION NOT NULL,
  theta DOUBLE PRECISION NOT NULL,
  vega DOUBLE PRECISION NOT NULL,
  rho DOUBLE PRECISION NOT NULL,
  theoretical NUMERIC(18,2) NOT NULL,
  intrinsic_value NUMERIC(18,2) NOT NULL,
  time_value NUMERIC(18,2) NOT NULL,
  updated_at DATE NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, expiration_date, option_type)
);

CREATE TABLE IF NOT EXISTS public.calendar_ipos
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  name TEXT NOT NULL,
  currency TEXT NULL,
  start_date DATE NULL,
  filing_date DATE NULL,
  amended_date DATE NULL,
  price_from NUMERIC(18,2) NOT NULL,
  price_to NUMERIC(18,2) NOT NULL,
  offer_price NUMERIC(18,2) NOT NULL,
  shares BIGINT NOT NULL,
  deal_type TEXT NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange)
);

CREATE TABLE IF NOT EXISTS public.calendar_earnings
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  report_date DATE NOT NULL,
  ending_date DATE NOT NULL,
  before_after_market TEXT NULL,
  currency TEXT NULL,
  actual NUMERIC(18,2) NULL,
  estimate NUMERIC(18,2) NULL,
  difference NUMERIC(18,2) NULL,
  percent DOUBLE PRECISION NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange, ending_date)
);


CREATE TABLE IF NOT EXISTS public.calendar_trends
(
  symbol TEXT NOT NULL,
  "date" DATE NOT NULL,
  period TEXT NOT NULL,
  growth DOUBLE PRECISION NULL,
  earnings_estimate_avg NUMERIC(18,2) NULL,
  earnings_estimate_low NUMERIC(18,2) NULL,
  earnings_estimate_high NUMERIC(18,2) NULL,
  earnings_estimate_number_analysts INT NULL,
  earnings_estimate_growth DOUBLE PRECISION NULL,
  revenue_estimate_avg NUMERIC(18,2) NULL,
  revenue_estimate_low NUMERIC(18,2) NULL,
  revenue_estimate_high NUMERIC(18,2) NULL,
  revenue_estimate_year_ago_eps  NUMERIC(18,2) NULL,
  revenue_estimate_number_analysts INT NULL,
  revenue_estimate_growth DOUBLE PRECISION NULL,
  eps_trend_current NUMERIC(18,2) NULL,
  eps_trend_7days_ago NUMERIC(18,2) NULL,
  eps_trend_30days_ago NUMERIC(18,2) NULL,
  eps_trend_60days_ago NUMERIC(18,2) NULL,
  eps_trend_90days_ago NUMERIC(18,2) NULL,
  eps_revisions_up_last7_days NUMERIC(18,2) NULL,
  eps_revisions_up_last30_days NUMERIC(18,2) NULL,
  eps_revisions_down_last30_days NUMERIC(18,2) NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, "date")
);

CREATE TABLE IF NOT EXISTS public.symbols_to_ignore
(
  symbol TEXT NOT NULL,
  exchange TEXT NOT NULL,
  date_ignored DATE NOT NULL,
  reason TEXT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (symbol, exchange)
);