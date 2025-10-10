-- 10: Security, Roles & Masking
use role ACCOUNTADMIN;
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- Create a reporting role and grants
create or replace role REPORTING_ROLE;
grant usage on warehouse ECOM_WH to role REPORTING_ROLE;
grant usage on database ECOM_TRAINING to role REPORTING_ROLE;
grant usage on schema CORE to role REPORTING_ROLE;
grant select on all tables in schema CORE to role REPORTING_ROLE;

-- Dynamic Data Masking example
create or replace masking policy EMAIL_MASK as (val string) returns string ->
  case
    when current_role() in ('ACCOUNTADMIN') then val
    else regexp_replace(val, '(^.).*(@.*$)', '\\1***\\2')
  end;

alter table CUSTOMERS modify column EMAIL set masking policy EMAIL_MASK;

-- Row Access Policy (example: restrict by customer_id)
create or replace row access policy RAP_CUSTOMERS as (cust_id number) returns boolean ->
  case
    when current_role() in ('ACCOUNTADMIN') then true
    else cust_id % 2 = 0
  end;

alter table CUSTOMERS add row access policy RAP_CUSTOMERS on (CUSTOMER_ID);
