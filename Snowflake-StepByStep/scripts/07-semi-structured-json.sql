-- 07: Semi-Structured JSON
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- Create a table with VARIANT column
create or replace table EVENTS (
  EVENT_ID number autoincrement,
  PAYLOAD variant,
  CREATED_AT timestamp_ntz default current_timestamp()
);

-- Insert sample JSON
insert into EVENTS(PAYLOAD) select parse_json('{"event":"click","user":{"id":101,"name":"Ada"},"meta":{"source":"web","tags":["promo","new"]}}');

-- Query JSON fields
select PAYLOAD:event::string as event,
       PAYLOAD:user:id::number as user_id,
       PAYLOAD:meta:source::string as source
from EVENTS;

-- Flatten array
select e.EVENT_ID, t.value::string as tag
from EVENTS e,
     lateral flatten(input => e.PAYLOAD:meta:tags) t;
