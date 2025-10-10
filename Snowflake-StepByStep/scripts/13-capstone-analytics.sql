-- 13: Capstone Analytics
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- Simple star-like mart for orders
create or replace view FACT_ORDER_REVENUE as
select o.ORDER_ID, o.ORDER_DATE, o.STATUS,
       c.CUSTOMER_ID, c.FIRST_NAME, c.LAST_NAME,
       sum(oi.QUANTITY * oi.UNIT_PRICE) as ORDER_TOTAL
from ORDERS o
join CUSTOMERS c on c.CUSTOMER_ID = o.CUSTOMER_ID
left join ORDER_ITEMS oi on oi.ORDER_ID = o.ORDER_ID
group by 1,2,3,4,5,6;

-- Example analytics
select date_trunc('month', ORDER_DATE) as month,
       sum(ORDER_TOTAL) as revenue
from FACT_ORDER_REVENUE
group by 1
order by 1;
