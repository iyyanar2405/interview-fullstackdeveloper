-- 05: Views & Materialized Views
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

create or replace view VW_ORDER_TOTALS as
select o.ORDER_ID, o.ORDER_DATE, o.STATUS,
       sum(oi.QUANTITY * oi.UNIT_PRICE) as ORDER_TOTAL
from ORDERS o
left join ORDER_ITEMS oi on oi.ORDER_ID = o.ORDER_ID
group by 1,2,3;

-- Materialized example (requires enterprise features; costs apply)
create or replace materialized view MV_TOP_PRODUCTS as
select p.PRODUCT_ID, p.SKU, p.NAME,
       sum(oi.QUANTITY * oi.UNIT_PRICE) as REVENUE
from ORDER_ITEMS oi
join PRODUCTS p on p.PRODUCT_ID = oi.PRODUCT_ID
group by 1,2,3;

-- Query the views
select * from VW_ORDER_TOTALS limit 20;
select * from MV_TOP_PRODUCTS order by REVENUE desc limit 10;
