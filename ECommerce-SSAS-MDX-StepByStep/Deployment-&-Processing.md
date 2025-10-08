# Deployment & Processing — SSAS

## Processing Types
- ProcessFull: rebuilds dimension/cube data
- ProcessAdd: adds fact rows to a partition
- ProcessUpdate: updates dimension attributes

## Incremental Strategy
- Partition by Year and use ProcessAdd with a staging table or view
- Keep watermarks for last processed key/date

## Scheduling
- SQL Agent jobs for night processing
- Error handling and alerting via Agent and logs

## Automation Notes
- Consider SSIS or Azure Data Factory for upstream loads
- Cache warm-up query after processing to reduce first-hit latency
