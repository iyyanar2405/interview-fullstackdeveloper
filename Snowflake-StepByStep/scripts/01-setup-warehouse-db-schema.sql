-- 01: Warehouse, Database & Schema
-- Variables (replace names as needed)
use role ACCOUNTADMIN;

create or replace warehouse ECOM_WH with
  warehouse_size = 'XSMALL'
  auto_suspend = 60
  auto_resume = true
  initially_suspended = true;

create or replace database ECOM_TRAINING;
use database ECOM_TRAINING;
create or replace schema CORE;
use schema CORE;

-- Named internal stage for loading CSVs
create or replace stage ECOM_STAGE
  file_format = (type = csv field_delimiter = ',' skip_header = 1 null_if=('NULL'));
