-- 03: Stage + COPY INTO
use role ACCOUNTADMIN;
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- Staging CSVs (Option A: Snowsight upload to ECOM_STAGE)
-- Option B: SnowSQL PUT from local (edit local path):
-- PUT file://Snowflake-StepByStep/data/csvs/customers.csv @ECOM_STAGE auto_compress=false;
-- PUT file://Snowflake-StepByStep/data/csvs/products.csv  @ECOM_STAGE auto_compress=false;
-- PUT file://Snowflake-StepByStep/data/csvs/orders.csv    @ECOM_STAGE auto_compress=false;
-- PUT file://Snowflake-StepByStep/data/csvs/order_items.csv @ECOM_STAGE auto_compress=false;

-- Create a file format explicitly (if not done in stage)
create or replace file format CSV_FF type = csv field_delimiter = ',' skip_header = 1 null_if=('NULL');

-- Create named stages per table (optional, or reuse ECOM_STAGE)
create or replace stage STG_CUSTOMERS file_format = CSV_FF;
create or replace stage STG_PRODUCTS  file_format = CSV_FF;
create or replace stage STG_ORDERS    file_format = CSV_FF;
create or replace stage STG_ORDER_ITEMS file_format = CSV_FF;

-- Copy from ECOM_STAGE to per-table stages (optional, if you uploaded there)
-- COPY INTO @STG_CUSTOMERS FROM @ECOM_STAGE/customers.csv OVERWRITE = TRUE;
-- COPY INTO @STG_PRODUCTS  FROM @ECOM_STAGE/products.csv  OVERWRITE = TRUE;
-- COPY INTO @STG_ORDERS    FROM @ECOM_STAGE/orders.csv    OVERWRITE = TRUE;
-- COPY INTO @STG_ORDER_ITEMS FROM @ECOM_STAGE/order_items.csv OVERWRITE = TRUE;

-- Load tables directly from ECOM_STAGE files
copy into CUSTOMERS(FIRST_NAME, LAST_NAME, EMAIL, CREATED_AT)
from (
  select t.$1, t.$2, t.$3, t.$4::timestamp_ntz
  from @ECOM_STAGE/customers.csv t
);

copy into PRODUCTS(SKU, NAME, PRICE, CREATED_AT)
from (
  select t.$1, t.$2, t.$3::number(10,2), t.$4::timestamp_ntz
  from @ECOM_STAGE/products.csv t
);

copy into ORDERS(CUSTOMER_ID, ORDER_DATE, STATUS)
from (
  select t.$1::number, to_date(t.$2, 'YYYY-MM-DD'), t.$3
  from @ECOM_STAGE/orders.csv t
);

copy into ORDER_ITEMS(ORDER_ID, PRODUCT_ID, QUANTITY, UNIT_PRICE)
from (
  select t.$1::number, t.$2::number, t.$3::number, t.$4::number(10,2)
  from @ECOM_STAGE/order_items.csv t
);
