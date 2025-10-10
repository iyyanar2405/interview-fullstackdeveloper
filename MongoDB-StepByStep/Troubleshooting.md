# Troubleshooting

- Cannot connect: check service status, port, bindIp, auth
- Permission denied: create users/roles; verify authSource
- Performance: analyze explain(), ensure proper indexes for sort/filter
- Transactions/change streams require replica set — set up rs.initiate()
