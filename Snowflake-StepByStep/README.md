# Snowflake — Step by Step (00–13)

A practical Snowflake track with runnable SQL scripts and sample CSV data for an e-commerce scenario.

## Quickstart

You can run scripts via the Snowflake Web UI (Snowsight Worksheets) or the SnowSQL CLI.

- Order of execution: run the SQL files under `scripts/` from 01 to 13.
- Data files are under `data/` and are referenced by staging/COPY commands in `03`.

### Option A — Snowsight (Web UI)
1) Open Worksheets and set your context (role, warehouse)
2) Paste and execute each script from `scripts/01-setup-warehouse-db-schema.sql` onward
3) For `03` data loading: Create a named stage (done in `01`) and use Snowsight to upload CSVs to that stage, then run the COPY commands

### Option B — SnowSQL (CLI)
- Install SnowSQL and authenticate to your account
- Then execute scripts in order (replace placeholders with your values):

```powershell
# Example (edit account, user, role, and warehouse)
$ACCOUNT = "<account_identifier>"   # e.g., ab12345.us-east-1
$USER    = "<username>"
$ROLE    = "ACCOUNTADMIN"
$WH      = "ECOM_WH"

snowsql -a $ACCOUNT -u $USER -r $ROLE -w $WH -f "Snowflake-StepByStep/scripts/01-setup-warehouse-db-schema.sql"
snowsql -a $ACCOUNT -u $USER -r $ROLE -w $WH -f "Snowflake-StepByStep/scripts/02-create-tables.sql"
# For 03, first PUT CSVs into the stage using SnowSQL, then run the script (see script comments)
# Example PUTs (from repo root; adjust account & auth in your session):
# snowsql -a $ACCOUNT -u $USER -r $ROLE -w $WH -q "PUT file://Snowflake-StepByStep/data/csvs/customers.csv @ECOM_STAGE auto_compress=false"
# snowsql -a $ACCOUNT -u $USER -r $ROLE -w $WH -q "PUT file://Snowflake-StepByStep/data/csvs/products.csv  @ECOM_STAGE auto_compress=false"
# snowsql -a $ACCOUNT -u $USER -r $ROLE -w $WH -q "PUT file://Snowflake-StepByStep/data/csvs/orders.csv    @ECOM_STAGE auto_compress=false"
# snowsql -a $ACCOUNT -u $USER -r $ROLE -w $WH -q "PUT file://Snowflake-StepByStep/data/csvs/order_items.csv @ECOM_STAGE auto_compress=false"
snowsql -a $ACCOUNT -u $USER -r $ROLE -w $WH -f "Snowflake-StepByStep/scripts/03-create-stage-and-copy.sql"
# ...continue with 04–13 similarly
```

## Contents
- 00 — Orientation & Tools
- 01 — Warehouse, Database & Schema
- 02 — Tables & Constraints
- 03 — Load Data (Stages & COPY)
- 04 — Querying & Joins
- 05 — Views & Materialized Views
- 06 — Procedures, Tasks & Streams
- 07 — Semi-Structured Data (JSON)
- 08 — Time Travel & Cloning
- 09 — Clustering & Micro-Partitions
- 10 — Security, Roles & Masking
- 11 — UDF/UDTF & External Functions
- 12 — Warehouse Scaling & Cost
- 13 — Capstone: Analytics & Dashboards

Each chapter has a markdown doc and companion `.sql` scripts under `scripts/`.
