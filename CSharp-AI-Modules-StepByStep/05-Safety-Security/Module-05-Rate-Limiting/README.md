# Module 05: Rate Limiting & DDoS Protection

Comprehensive rate limiting, quota management, DDoS protection, and resource throttling for API applications.

## Features

### 🚦 Rate Limiting
- **5 Algorithms**: Token Bucket, Sliding Window, Fixed Window, Leaky Bucket, Concurrent Requests
- **Multi-Level Limiting**: IP-based, Client-based, Endpoint-based
- **Flexible Configuration**: Per-endpoint and per-client rules
- **Whitelist Support**: Bypass limits for trusted clients/IPs

### 📊 Quota Management
- **5 Subscription Tiers**: Free, Basic, Professional, Enterprise, Unlimited
- **Daily & Monthly Limits**: Track usage across time periods
- **Overage Handling**: Allow or block requests exceeding quota
- **Cost Tracking**: Calculate API usage costs
- **Auto-Reset**: Daily and monthly quota resets

### 🛡️ DDoS Protection
- **Traffic Analysis**: Real-time RPS/RPM monitoring
- **Threat Detection**: 5 threat levels (None/Low/Medium/High/Critical)
- **Auto-Blocking**: Automatic IP blocking for suspicious activity
- **Bot Detection**: Identify and handle bot traffic
- **IP Blacklisting**: Permanent and temporary blocks

### ⚡ Throttling
- **Resource Management**: Control concurrent requests
- **Priority Queuing**: Handle requests based on priority
- **Backoff Strategies**: 5 strategies (Fixed, Linear, Exponential, Fibonacci, Decorrelated)
- **Adaptive Throttling**: Adjust limits based on load
- **Queue Management**: FIFO queue with priority support

### 📈 Fair Usage Policies
- **Burst Protection**: Limit short-term request spikes
- **Endpoint Weighting**: Different costs for different endpoints
- **Violation Tracking**: Monitor and penalize abuse
- **Dynamic Adjustment**: Adapt limits based on behavior

## API Endpoints

### Rate Limiting (3 endpoints)
```
GET    /api/ratelimit/status              # Get rate limit status
POST   /api/ratelimit/rules               # Add custom rate limit rule
DELETE /api/ratelimit/reset               # Reset rate limit
```

### Quota Management (5 endpoints)
```
GET    /api/ratelimit/quota               # Get quota usage
GET    /api/ratelimit/quota/allocation    # Get quota allocation
POST   /api/ratelimit/quota/check         # Check quota availability
PUT    /api/ratelimit/quota/update        # Update quota/tier
DELETE /api/ratelimit/quota/reset         # Reset quota
```

### DDoS Protection (5 endpoints)
```
GET    /api/ratelimit/ddos/traffic        # Get traffic analysis
POST   /api/ratelimit/ddos/block          # Block IP address
DELETE /api/ratelimit/ddos/unblock        # Unblock IP address
GET    /api/ratelimit/ddos/blocked        # List blocked IPs
GET    /api/ratelimit/ddos/check          # Check if IP is blocked
```

### Throttling (1 endpoint)
```
GET    /api/ratelimit/throttle/status     # Get throttle status
```

### Testing & Demo (3 endpoints)
```
GET    /api/ratelimit/test/simple         # Simple test endpoint
POST   /api/ratelimit/test/load           # Load test (100ms delay)
GET    /api/ratelimit/test/heavy          # Heavy test (1s delay)
```

### Utility (2 endpoints)
```
GET    /api/ratelimit/health              # Health check
GET    /api/ratelimit/stats               # Get statistics
```

**Total: 19 REST API Endpoints**

## Quick Start

### 1. Run the API
```bash
cd Module-05-Rate-Limiting
dotnet restore
dotnet run
```

API runs at: `https://localhost:7005`  
Swagger UI: `https://localhost:7005`

### 2. Test Rate Limiting (C#)
```csharp
using System.Net.Http.Json;

var client = new HttpClient { BaseAddress = new Uri("https://localhost:7005") };
client.DefaultRequestHeaders.Add("X-Client-Id", "client123");

// Make multiple requests to test rate limiting
for (int i = 0; i < 10; i++)
{
    var response = await client.GetAsync("/api/ratelimit/test/simple");
    
    Console.WriteLine($"Request {i + 1}:");
    Console.WriteLine($"  Status: {response.StatusCode}");
    Console.WriteLine($"  Remaining: {response.Headers.GetValues("X-RateLimit-Remaining").FirstOrDefault()}");
    Console.WriteLine($"  Reset: {response.Headers.GetValues("X-RateLimit-Reset").FirstOrDefault()}");
    
    if (!response.IsSuccessStatusCode)
    {
        var error = await response.Content.ReadFromJsonAsync<dynamic>();
        Console.WriteLine($"  Error: {error}");
        
        var retryAfter = response.Headers.GetValues("Retry-After").FirstOrDefault();
        Console.WriteLine($"  Retry After: {retryAfter}s");
        break;
    }
    
    await Task.Delay(100);
}
```

### 3. Check Quota Usage (JavaScript)
```javascript
const response = await fetch('https://localhost:7005/api/ratelimit/quota?clientId=client123', {
  headers: {
    'X-Client-Id': 'client123'
  }
});

const usage = await response.json();
console.log('Quota Usage:');
console.log(`  Daily: ${usage.dailyUsed}/${usage.dailyUsed + usage.dailyRemaining}`);
console.log(`  Monthly: ${usage.monthlyUsed}/${usage.monthlyUsed + usage.monthlyRemaining}`);
console.log(`  Cost: $${usage.currentCost.toFixed(4)}`);
console.log(`  Over Quota: ${usage.isOverQuota}`);
```

### 4. Update Subscription Tier (Python)
```python
import requests

response = requests.put('https://localhost:7005/api/ratelimit/quota/update',
    json={
        'clientId': 'client123',
        'newTier': 'Professional',
        'customDailyLimit': None,
        'customMonthlyLimit': None
    },
    headers={'X-Client-Id': 'client123'})

result = response.json()
print(f"Quota Update: {result['message']}")

# Check new allocation
allocation_response = requests.get(
    'https://localhost:7005/api/ratelimit/quota/allocation',
    params={'clientId': 'client123'})

allocation = allocation_response.json()
print(f"New Tier: {allocation['tier']}")
print(f"Daily Quota: {allocation['dailyQuota']}")
print(f"Monthly Quota: {allocation['monthlyQuota']}")
```

### 5. Block Suspicious IP (C#)
```csharp
var blockRequest = new
{
    ipAddress = "192.168.1.100",
    reason = "DDoSDetected",
    isPermanent = false,
    durationMinutes = 60,
    notes = "Excessive requests detected"
};

var response = await client.PostAsJsonAsync("/api/ratelimit/ddos/block", blockRequest);
var rule = await response.Content.ReadFromJsonAsync<IpBlockRule>();

Console.WriteLine($"IP Blocked: {rule.IpAddress}");
Console.WriteLine($"Reason: {rule.Reason}");
Console.WriteLine($"Expires: {rule.ExpiresAt}");
```

### 6. Monitor Traffic (JavaScript)
```javascript
const response = await fetch(
  'https://localhost:7005/api/ratelimit/ddos/traffic?ipAddress=192.168.1.50');

const analysis = await response.json();
console.log('Traffic Analysis:');
console.log(`  RPS: ${analysis.requestsPerSecond}`);
console.log(`  RPM: ${analysis.requestsPerMinute}`);
console.log(`  Total Requests: ${analysis.totalRequests}`);
console.log(`  Failure Rate: ${(analysis.failureRate * 100).toFixed(2)}%`);
console.log(`  Unique Endpoints: ${analysis.uniqueEndpoints}`);
console.log(`  Is Bot: ${analysis.isBot}`);
```

## Rate Limiting Algorithms

### 1. Token Bucket
**Best for**: Bursty traffic with consistent average rate

```
Capacity: 100 tokens
Refill Rate: 10 tokens/second
```

- Allows bursts up to capacity
- Tokens refill at constant rate
- Good balance between strictness and flexibility

### 2. Sliding Window
**Best for**: Smooth rate limiting without edge cases

```
Limit: 100 requests
Window: 60 seconds (rolling)
```

- Most accurate algorithm
- No edge cases at window boundaries
- Higher memory usage

### 3. Fixed Window
**Best for**: Simple, low-overhead limiting

```
Limit: 100 requests
Window: 60 seconds (fixed)
```

- Lowest memory usage
- Edge case: 200 requests possible across window boundary
- Fastest performance

### 4. Leaky Bucket
**Best for**: Smoothing traffic to constant rate

```
Capacity: 100
Leak Rate: 10 requests/second
```

- Forces constant output rate
- Good for protecting downstream services
- Queue behavior

### 5. Concurrent Requests
**Best for**: Limiting simultaneous operations

```
Max Concurrent: 50
```

- Controls resource usage
- Prevents resource exhaustion
- Good for expensive operations

## Subscription Tiers

| Tier | Daily Limit | Monthly Limit | Concurrent | Cost/Request | Overage |
|------|-------------|---------------|------------|--------------|---------|
| **Free** | 1,000 | 10,000 | 5 | $0.000 | Not Allowed |
| **Basic** | 10,000 | 250,000 | 20 | $0.001 | $0.002 |
| **Professional** | 100,000 | 2,500,000 | 50 | $0.0008 | $0.0015 |
| **Enterprise** | 1,000,000 | 25,000,000 | 200 | $0.0005 | $0.001 |
| **Unlimited** | ∞ | ∞ | 1,000 | $0.000 | N/A |

### Cost Examples

**Basic Tier (250k monthly requests)**:
- Base cost: 250,000 × $0.001 = $250/month
- With 10% overage: 275,000 requests
  - Base: 250,000 × $0.001 = $250
  - Overage: 25,000 × $0.002 = $50
  - Total: $300/month

**Professional Tier (2.5M monthly requests)**:
- Base cost: 2,500,000 × $0.0008 = $2,000/month
- With 20% overage: 3,000,000 requests
  - Base: 2,500,000 × $0.0008 = $2,000
  - Overage: 500,000 × $0.0015 = $750
  - Total: $2,750/month

## DDoS Detection

### Threat Levels

#### None (0)
- Normal traffic
- Action: Allow

#### Low (1)
- RPS: 50-74
- Action: Monitor
- No blocking

#### Medium (2)
- RPS: 75-99
- Action: Challenge (CAPTCHA)
- Temporary throttling

#### High (3)
- RPS: 100-199
- Action: Aggressive throttling
- Auto-block for 60 minutes

#### Critical (4)
- RPS: 200+
- Action: Immediate block
- Permanent blacklist consideration

### Detection Patterns

1. **High RPS**: > 100 requests/second
2. **High RPM**: > 1000 requests/minute
3. **High Failure Rate**: > 50% failed requests
4. **Scanning Behavior**: > 50 unique endpoints
5. **Bot Pattern**: Bot user agent detected

### Auto-Blocking

When threat level ≥ High:
- IP automatically blocked for 60 minutes
- Logged to audit trail
- Alert triggered (if configured)

```csharp
// Example: Traffic that triggers auto-block
// Client makes 150 requests in 1 second
// Threat Level: High
// Action: Auto-block for 60 minutes
```

## Throttling Strategies

### Backoff Strategies

#### Fixed
```
Retry 1: Wait 5s
Retry 2: Wait 5s
Retry 3: Wait 5s
```

#### Linear
```
Retry 1: Wait 5s
Retry 2: Wait 10s
Retry 3: Wait 15s
```

#### Exponential (Recommended)
```
Retry 1: Wait 2s
Retry 2: Wait 4s
Retry 3: Wait 8s
Retry 4: Wait 16s
```

#### Fibonacci
```
Retry 1: Wait 1s
Retry 2: Wait 1s
Retry 3: Wait 2s
Retry 4: Wait 3s
Retry 5: Wait 5s
```

#### Decorrelated (Random)
```
Retry 1: Wait 1-2s (random)
Retry 2: Wait 1-4s (random)
Retry 3: Wait 1-8s (random)
```

## Configuration

### Rate Limiting
```json
{
  "RateLimiting": {
    "EnableRateLimiting": true,
    "DefaultAlgorithm": "SlidingWindow",
    "DefaultLimit": 100,
    "DefaultPeriodSeconds": 60,
    "EnableIpRateLimiting": true,
    "EnableClientRateLimiting": true,
    "WhitelistedIps": ["127.0.0.1"],
    "WhitelistedClients": []
  }
}
```

### DDoS Protection
```json
{
  "DDoSProtection": {
    "EnableDDoSProtection": true,
    "RequestsPerSecondThreshold": 100,
    "RequestsPerMinuteThreshold": 1000,
    "AutoBlockSuspiciousIps": true,
    "AutoBlockDurationMinutes": 60,
    "AutoBlockThreshold": "High"
  }
}
```

### Redis (Production)
```json
{
  "Redis": {
    "ConnectionString": "your-redis-server:6379",
    "InstanceName": "RateLimit_",
    "Enabled": true
  }
}
```

## Response Headers

### Rate Limit Headers
```
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 2024-10-22T12:34:56Z
```

### Quota Headers
```
X-Quota-Remaining-Daily: 950
X-Quota-Remaining-Monthly: 9500
```

### Retry Header
```
Retry-After: 45
```

### HTTP Status Codes

- `200 OK`: Request successful
- `429 Too Many Requests`: Rate limit exceeded
- `403 Forbidden`: IP blocked
- `503 Service Unavailable`: Throttled/queued

## Load Testing

### Apache Bench
```bash
# Test rate limiting
ab -n 1000 -c 10 https://localhost:7005/api/ratelimit/test/simple

# Test with client ID
ab -n 1000 -c 10 -H "X-Client-Id: client123" https://localhost:7005/api/ratelimit/test/simple
```

### k6 (Load Testing Tool)
```javascript
import http from 'k6/http';
import { check } from 'k6';

export const options = {
  stages: [
    { duration: '30s', target: 50 },
    { duration: '1m', target: 100 },
    { duration: '30s', target: 0 },
  ],
};

export default function () {
  const response = http.get('https://localhost:7005/api/ratelimit/test/simple', {
    headers: { 'X-Client-Id': 'load-test-client' },
  });

  check(response, {
    'status is 200 or 429': (r) => r.status === 200 || r.status === 429,
    'has rate limit headers': (r) => r.headers['X-RateLimit-Remaining'] !== undefined,
  });
}
```

## Best Practices

### 1. Rate Limiting
- Use **Sliding Window** for most accurate limiting
- Use **Fixed Window** for best performance
- Use **Token Bucket** for bursty APIs
- Whitelist trusted clients/IPs
- Set per-endpoint limits for expensive operations

### 2. Quota Management
- Start with **Free tier**, upgrade based on usage
- Monitor quota usage regularly
- Set up alerts for quota thresholds (80%, 90%)
- Allow overage for business-critical clients
- Review and optimize API usage patterns

### 3. DDoS Protection
- Enable auto-blocking for High+ threats
- Monitor traffic patterns daily
- Review blocked IPs weekly
- Whitelist known good bots (Googlebot, etc.)
- Set up webhooks for critical alerts

### 4. Throttling
- Use **Exponential backoff** for retries
- Enable priority queues for critical operations
- Monitor resource utilization
- Set appropriate timeouts (30s recommended)
- Release resources immediately after completion

### 5. Production Deployment
- Use Redis for distributed caching
- Enable all monitoring and alerts
- Set conservative thresholds initially
- Log all rate limit violations
- Review and adjust thresholds monthly

## Monitoring & Alerts

### Metrics to Monitor

1. **Rate Limiting**
   - Requests allowed vs. denied
   - Rate limit hit rate
   - Most limited endpoints

2. **Quotas**
   - Quota usage by tier
   - Overage frequency
   - Top consumers

3. **DDoS**
   - Blocked IPs count
   - Threat level distribution
   - Traffic patterns

4. **Throttling**
   - Resource utilization
   - Queue lengths
   - Wait times

### Alert Thresholds

```json
{
  "AlertThresholds": {
    "HighTraffic": 1000,
    "BlockedIps": 10,
    "QuotaExceeded": 100,
    "ResourceUtilization": 90
  }
}
```

## Technology Stack

- **Framework**: .NET 8.0
- **Rate Limiting**: AspNetCoreRateLimit 5.0.0
- **Caching**: Memory Cache / Redis (StackExchange.Redis 2.7.10)
- **Metrics**: App.Metrics 4.3.0, Prometheus
- **Logging**: Serilog 8.0.0
- **Testing**: xUnit, k6, Apache Bench

## Project Structure

```
Module-05-Rate-Limiting/
├── Controllers/
│   └── RateLimitController.cs           # 19 REST endpoints
├── Services/
│   ├── RateLimitingService.cs           # 5 algorithms
│   ├── QuotaManagementService.cs        # 5 tiers
│   ├── DDoSProtectionService.cs         # Threat detection
│   └── ThrottlingService.cs             # Resource throttling
├── Middleware/
│   └── RateLimitingMiddleware.cs        # Automatic rate limiting
├── Models/
│   └── RateLimitingModels.cs            # 50+ models
├── Program.cs                            # Service registration
├── appsettings.json                      # Configuration
└── Module-05-Rate-Limiting.csproj       # Dependencies
```

## Testing

### Unit Tests
```bash
dotnet test --filter Category=RateLimiting
```

### Integration Tests
```bash
dotnet test --filter Category=Integration
```

### Load Tests
```bash
k6 run load-test.js
```

## License

MIT License - See LICENSE file for details

## Support

For issues, questions, or contributions:
- GitHub Issues: [your-repo/issues]
- Email: support@yourcompany.com
- Documentation: [your-docs-site]

---

**Module Status**: ✅ Complete  
**Endpoints**: 19  
**Services**: 4  
**Algorithms**: 5  
**Tiers**: 5  
**Lines of Code**: 2,000+
