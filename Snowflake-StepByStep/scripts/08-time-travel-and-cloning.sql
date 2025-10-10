-- 08: Time Travel & Cloning
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- Delete a row, then query historical
delete from PRODUCTS where SKU = 'SKU-2';

-- Query at a timestamp in the past (replace TIMESTAMP => use last query time from history)
-- select * from PRODUCTS at(timestamp => to_timestamp_ntz('2025-10-10 10:00:00')) where SKU='SKU-2';

-- Zero-copy clone schema (for dev/test)
create or replace schema CORE_DEV clone CORE;
select count(*) from CORE_DEV.PRODUCTS;
