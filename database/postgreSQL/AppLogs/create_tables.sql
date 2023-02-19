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