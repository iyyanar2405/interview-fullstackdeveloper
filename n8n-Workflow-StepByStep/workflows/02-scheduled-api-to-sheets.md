# Workflow 02 — Scheduled API → Google Sheets

Goal: Pull an API on a schedule (Cron) and append rows to Google Sheets.

Steps
1) Cron node
- Expression: every 5 minutes (or set as needed)

2) HTTP Request node
- Method: GET; URL: public API (e.g., https://api.github.com/repos/n8n-io/n8n)
- Parse JSON

3) Set node
- Pick fields: full_name, stargazers_count, open_issues, updated_at

4) Google Sheets node
- Operation: Append
- Spreadsheet & sheet: select targets
- Map fields to columns

5) Test & schedule
- Run once to verify, then let Cron schedule it

Export JSON when done and save here.
