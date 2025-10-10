-- 12: Warehouse Scaling & Cost
use role ACCOUNTADMIN;
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- Resize warehouse
alter warehouse ECOM_WH set warehouse_size = 'SMALL';

-- Multi-cluster example (may require specific editions)
alter warehouse ECOM_WH set max_cluster_count = 2, min_cluster_count = 1, scaling_policy = 'STANDARD';

-- Auto-suspend/resume tuning
alter warehouse ECOM_WH set auto_suspend = 60, auto_resume = true;

-- Inspect query history & profile via Snowsight; here’s an example system view
select * from table(information_schema.query_history()) order by start_time desc limit 50;
