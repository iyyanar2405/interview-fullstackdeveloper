-- 06: Procedures, Tasks & Streams
use role ACCOUNTADMIN;
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- Simple JavaScript stored procedure to recalc a summary table
create or replace table ORDER_SUMMARY (
  ORDER_ID number,
  ORDER_TOTAL number(10,2)
);

create or replace procedure REFRESH_ORDER_SUMMARY()
returns string
language javascript
as
$$
var cmd = `insert overwrite into ORDER_SUMMARY
select o.ORDER_ID, coalesce(sum(oi.QUANTITY * oi.UNIT_PRICE),0)
from ORDERS o
left join ORDER_ITEMS oi on oi.ORDER_ID = o.ORDER_ID
group by 1;`;
snowflake.execute({sqlText: cmd});
return 'OK';
$$;

-- Stream on ORDER_ITEMS for incremental changes
create or replace stream ORDER_ITEMS_STRM on table ORDER_ITEMS;

-- Task to refresh summary (every 15 minutes)
create or replace task TASK_REFRESH_SUMMARY
  warehouse = ECOM_WH
  schedule = '15 minute'
  when system$stream_has_data('ORDER_ITEMS_STRM')
as
  call REFRESH_ORDER_SUMMARY();

-- To enable/disable task
-- alter task TASK_REFRESH_SUMMARY resume;
-- alter task TASK_REFRESH_SUMMARY suspend;
