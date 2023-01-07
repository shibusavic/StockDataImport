CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS public.logs
(
  global_id UUID NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL,
  log_level TEXT NOT NULL,
  message TEXT NULL,
  exception TEXT NULL,
  log_scope TEXT NULL,
  event_id INTEGER NULL,
  event_name TEXT NULL,
  PRIMARY KEY (global_id)
);

CREATE TABLE IF NOT EXISTS public.action_items
(
  global_id UUID NOT NULL,
  action_name TEXT NOT NULL,
  target_name TEXT NULL,
  target_scope TEXT NULL,
  target_data_type TEXT NULL,
  priority INTEGER NOT NULL,
  status TEXT NOT NULL,
  utc_created TIMESTAMP NOT NULL,
  utc_started TIMESTAMP NULL,
  utc_completed TIMESTAMP NULL,
  details TEXT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (global_id)
);

CREATE TABLE IF NOT EXISTS public.api_responses
(
  request TEXT NOT NULL,
  response TEXT NOT NULL,
  status_code INTEGER NOT NULL,
  utc_timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (request, utc_timestamp)
)