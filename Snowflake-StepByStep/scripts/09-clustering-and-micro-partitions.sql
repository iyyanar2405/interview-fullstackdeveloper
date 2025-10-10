-- 09: Clustering & Micro-Partitions
use role ACCOUNTADMIN;
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- Define clustering key on a large table (example on ORDER_ITEMS)
alter table ORDER_ITEMS cluster by (ORDER_ID);

-- Monitor clustering depth
select system$clustering_information('ECOM_TRAINING.CORE.ORDER_ITEMS');
