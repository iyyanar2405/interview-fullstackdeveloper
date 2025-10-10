-- 11: UDF/UDTF & External Functions
use warehouse ECOM_WH;
use database ECOM_TRAINING;
use schema CORE;

-- SQL UDF
create or replace function fn_discount(price number, pct number)
returns number
as 'price * (1 - pct)';

-- JavaScript UDF
create or replace function js_concat(a string, b string)
returns string
language javascript
as $$
return a + b;
$$;

select fn_discount(100, 0.2) as discounted, js_concat('Hello ', 'Snowflake') as greeting;

-- UDTF example (returns table)
create or replace function split_to_rows(s string)
returns table (val string)
language javascript
as $$
  var out = [];
  if (s) {
    var parts = s.split(',');
    for (var i=0; i<parts.length; i++) {
      out.push({VAL: parts[i]});
    }
  }
  return out;
$$;

select * from table(split_to_rows('a,b,c'));

-- External Functions require API integration setup (not included in quickstart)
