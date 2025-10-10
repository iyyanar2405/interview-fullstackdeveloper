-- 04: Querying & Joins
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- Sample joins and windows
select o.ORDER_ID, c.FIRST_NAME, c.LAST_NAME, o.ORDER_DATE, o.STATUS,
       sum(oi.QUANTITY * oi.UNIT_PRICE) as ORDER_TOTAL
from ORDERS o
join CUSTOMERS c on c.CUSTOMER_ID = o.CUSTOMER_ID
left join ORDER_ITEMS oi on oi.ORDER_ID = o.ORDER_ID
group by 1,2,3,4,5
order by o.ORDER_DATE desc
limit 20;

-- Top products by revenue
select p.SKU, p.NAME,
       sum(oi.QUANTITY * oi.UNIT_PRICE) as REVENUE
from ORDER_ITEMS oi
join PRODUCTS p on p.PRODUCT_ID = oi.PRODUCT_ID
group by 1,2
order by REVENUE desc
limit 10;
