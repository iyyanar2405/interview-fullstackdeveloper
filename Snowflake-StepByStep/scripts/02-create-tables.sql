-- 02: Create tables
use role ACCOUNTADMIN;
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

create or replace table CUSTOMERS (
  CUSTOMER_ID number autoincrement start 1 increment 1,
  FIRST_NAME  string,
  LAST_NAME   string,
  EMAIL       string,
  CREATED_AT  timestamp_ntz default current_timestamp(),
  primary key (CUSTOMER_ID)
);

create or replace table PRODUCTS (
  PRODUCT_ID number autoincrement,
  SKU        string,
  NAME       string,
  PRICE      number(10,2),
  CREATED_AT timestamp_ntz default current_timestamp(),
  primary key (PRODUCT_ID)
);

create or replace table ORDERS (
  ORDER_ID    number autoincrement,
  CUSTOMER_ID number,
  ORDER_DATE  date,
  STATUS      string,
  primary key (ORDER_ID)
);

create or replace table ORDER_ITEMS (
  ORDER_ITEM_ID number autoincrement,
  ORDER_ID      number,
  PRODUCT_ID    number,
  QUANTITY      number,
  UNIT_PRICE    number(10,2),
  primary key (ORDER_ITEM_ID)
);

-- Basic referential constraints (not enforced, informational)
-- Snowflake supports declarative constraints but may treat some as not enforced
alter table ORDERS add constraint fk_orders_customer
  foreign key (CUSTOMER_ID) references CUSTOMERS(CUSTOMER_ID);

alter table ORDER_ITEMS add constraint fk_items_order
  foreign key (ORDER_ID) references ORDERS(ORDER_ID);

alter table ORDER_ITEMS add constraint fk_items_product
  foreign key (PRODUCT_ID) references PRODUCTS(PRODUCT_ID);

-- Add unique constraints (note: informational/not enforced)
alter table PRODUCTS add constraint UQ_PRODUCTS_SKU unique (SKU);
alter table CUSTOMERS add constraint UQ_CUSTOMERS_EMAIL unique (EMAIL);
