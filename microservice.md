# Microservices Interview Questions

## Basic Concepts

### 1. What are microservices and how do they differ from monolithic architecture?

**Expected Answer:**
- Microservices are an architectural approach where applications are built as a collection of small, independent services
- Each service runs in its own process and communicates via APIs
- Differs from monolithic where all components are tightly coupled in a single deployable unit

**Follow-up:** What are the main advantages and disadvantages of microservices?

### 2. What are the key characteristics of microservices?

**Expected Answer:**
- Single responsibility
- Decentralized
- Technology agnostic
- Independently deployable
- Fault tolerant
- Highly maintainable and testable

### 3. When would you choose microservices over a monolithic architecture?

**Expected Answer:**
- Large, complex applications
- Multiple teams working independently
- Need for different technologies/languages
- Frequent deployments required
- Scalability requirements vary by component

## Design Patterns and Best Practices

### 4. Explain the Database per Service pattern

**Expected Answer:**
- Each microservice has its own private database
- No direct database access between services
- Data consistency maintained through APIs and events
- Enables independent scaling and technology choices

### 5. What is the API Gateway pattern and why is it important?

**Expected Answer:**
- Single entry point for all client requests
- Routes requests to appropriate microservices
- Handles cross-cutting concerns (authentication, logging, rate limiting)
- Simplifies client complexity

### 6. Describe the Circuit Breaker pattern

**Expected Answer:**
- Prevents cascading failures in distributed systems
- Monitors service calls and "opens" when failure threshold reached
- Provides fallback mechanisms
- Helps maintain system resilience

### 7. What is the Saga pattern and when would you use it?

**Expected Answer:**
- Manages distributed transactions across multiple services
- Two types: Choreography and Orchestration
- Used when ACID transactions span multiple services
- Ensures eventual consistency

## Communication Patterns

### 8. What are the different ways microservices can communicate?

**Expected Answer:**
- **Synchronous:** HTTP/REST, gRPC
- **Asynchronous:** Message queues, Event streaming
- **Request-Response vs Event-driven**

### 9. Explain the difference between REST and gRPC for microservice communication

**Expected Answer:**
- **REST:** HTTP-based, human-readable, widely supported, stateless
- **gRPC:** Binary protocol, faster, built-in load balancing, strong typing
- **Use cases:** REST for public APIs, gRPC for internal service communication

### 10. What is Event Sourcing and how does it work with microservices?

**Expected Answer:**
- Stores events rather than current state
- Events are immutable and ordered
- Enables audit trails and temporal queries
- Works well with event-driven microservices

## Data Management

### 11. How do you handle data consistency across microservices?

**Expected Answer:**
- **Eventual consistency** over strong consistency
- **Saga pattern** for distributed transactions
- **Event-driven architecture** for data synchronization
- **CQRS** (Command Query Responsibility Segregation)

### 12. What is CQRS and how does it relate to microservices?

**Expected Answer:**
- Separates read and write operations
- Different models for commands and queries
- Enables independent scaling of read/write workloads
- Often used with Event Sourcing

### 13. How do you handle database migrations in a microservices environment?

**Expected Answer:**
- **Backward compatible changes** first
- **Blue-green deployments** for schema changes
- **Database versioning** strategies
- **Feature flags** for gradual rollouts

## Service Discovery and Load Balancing

### 14. What is service discovery and why is it needed?

**Expected Answer:**
- Mechanism for services to find and communicate with each other
- Handles dynamic service locations in cloud environments
- Types: Client-side vs Server-side discovery
- Examples: Consul, Eureka, Kubernetes DNS

### 15. Explain different load balancing strategies for microservices

**Expected Answer:**
- **Round Robin:** Sequential distribution
- **Weighted Round Robin:** Based on capacity
- **Least Connections:** Route to least busy service
- **Health-based:** Avoid unhealthy instances

## Security

### 16. How do you handle authentication and authorization in microservices?

**Expected Answer:**
- **JWT tokens** for stateless authentication
- **OAuth 2.0/OpenID Connect** for authorization
- **Service-to-service authentication** (mTLS, service accounts)
- **API Gateway** for centralized security

### 17. What is Zero Trust in microservices architecture?

**Expected Answer:**
- Never trust, always verify
- Service-to-service authentication required
- Network segmentation
- Principle of least privilege
- Continuous monitoring and validation

## Monitoring and Observability

### 18. What are the three pillars of observability?

**Expected Answer:**
- **Logs:** Discrete events and messages
- **Metrics:** Numerical measurements over time
- **Traces:** Request flow across services

### 19. How do you implement distributed tracing?

**Expected Answer:**
- **Correlation IDs** across service calls
- **OpenTelemetry** or similar standards
- **Trace context propagation**
- Tools like Jaeger, Zipkin

### 20. What metrics are important to monitor in microservices?

**Expected Answer:**
- **Business metrics:** User actions, revenue
- **Application metrics:** Response time, error rate
- **Infrastructure metrics:** CPU, memory, disk
- **RED metrics:** Rate, Errors, Duration

## Deployment and DevOps

### 21. How do you deploy microservices?

**Expected Answer:**
- **Containerization** (Docker)
- **Container orchestration** (Kubernetes)
- **CI/CD pipelines** for each service
- **Blue-green** or **canary deployments**

### 22. What is the difference between blue-green and canary deployments?

**Expected Answer:**
- **Blue-green:** Complete environment switch
- **Canary:** Gradual traffic shift to new version
- **Rolling:** Sequential instance updates
- **A/B testing:** Feature comparison

### 23. How do you handle configuration management in microservices?

**Expected Answer:**
- **Externalized configuration**
- **Configuration servers** (Spring Cloud Config)
- **Environment variables**
- **Secrets management** (Vault, Kubernetes secrets)

## Testing

### 24. What testing strategies do you use for microservices?

**Expected Answer:**
- **Unit tests** for individual services
- **Integration tests** for service interactions
- **Contract testing** (Pact, Spring Cloud Contract)
- **End-to-end tests** (minimal, focused on critical paths)

### 25. What is contract testing and why is it important?

**Expected Answer:**
- Tests the contract between service consumer and provider
- Enables independent deployment
- Catches breaking changes early
- Tools: Pact, Spring Cloud Contract

## Practical Scenarios

### 26. How would you break down a monolith into microservices?

**Expected Answer:**
- **Domain-driven design** to identify boundaries
- **Strangler fig pattern** for gradual migration
- **Database decomposition** strategies
- **Start with read-only services**

### 27. How do you handle a service that's frequently failing?

**Expected Answer:**
- **Circuit breaker** to prevent cascading failures
- **Retry mechanisms** with exponential backoff
- **Fallback responses**
- **Health checks** and monitoring

### 28. Design a microservices architecture for an e-commerce platform

**Expected Answer:**
- **User Service:** Authentication, profiles
- **Product Service:** Catalog management
- **Order Service:** Order processing
- **Payment Service:** Payment processing
- **Inventory Service:** Stock management
- **Notification Service:** Emails, SMS

## Advanced Topics

### 29. What is a service mesh and when would you use it?

**Expected Answer:**
- Infrastructure layer for service-to-service communication
- Handles security, observability, traffic management
- Examples: Istio, Linkerd, Consul Connect
- Use when you have many services and complex networking needs

### 30. How do you handle data consistency in event-driven architectures?

**Expected Answer:**
- **Eventually consistent** approach
- **Idempotent operations**
- **Event replay** capabilities
- **Compensation actions** for failures

## Code Example Questions

### 31. Show how you would implement a simple circuit breaker

```csharp
public class CircuitBreaker
{
    private int failureThreshold;
    private TimeSpan timeout;
    private int failureCount;
    private DateTime lastFailureTime;
    private CircuitBreakerState state;

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        if (state == CircuitBreakerState.Open)
        {
            if (DateTime.UtcNow - lastFailureTime > timeout)
            {
                state = CircuitBreakerState.HalfOpen;
            }
            else
            {
                throw new CircuitBreakerOpenException();
            }
        }

        try
        {
            var result = await operation();
            OnSuccess();
            return result;
        }
        catch (Exception ex)
        {
            OnFailure();
            throw;
        }
    }
}
```

### 32. Implement a simple service registry

```csharp
public class ServiceRegistry
{
    private readonly ConcurrentDictionary<string, List<ServiceInstance>> services = new();

    public void RegisterService(string serviceName, ServiceInstance instance)
    {
        services.AddOrUpdate(serviceName, 
            new List<ServiceInstance> { instance },
            (key, existing) => { existing.Add(instance); return existing; });
    }

    public ServiceInstance DiscoverService(string serviceName)
    {
        if (services.TryGetValue(serviceName, out var instances))
        {
            return instances.FirstOrDefault(i => i.IsHealthy);
        }
        return null;
    }
}
```

## Real-world Scenarios

### 33. Your microservice is experiencing high latency. How do you troubleshoot?

**Expected Answer:**
- Check **distributed traces** to identify bottlenecks
- Review **database query performance**
- Analyze **network latency** between services
- Check for **resource constraints** (CPU, memory)
- Review **third-party service dependencies**

### 34. How do you handle versioning of microservices APIs?

**Expected Answer:**
- **Semantic versioning** (major.minor.patch)
- **Backward compatibility** for minor versions
- **API gateway** routing based on version
- **Gradual migration** strategies
- **Deprecation policies**

### 35. Describe your approach to microservices security

**Expected Answer:**
- **Defense in depth** strategy
- **mTLS** for service-to-service communication
- **API Gateway** for external security
- **Secret management** systems
- **Regular security audits**
- **Zero trust architecture**

## Assessment Criteria

**Scoring Guide:**
- **Expert (4-5 points):** Demonstrates deep understanding, provides examples, discusses trade-offs
- **Proficient (3 points):** Good understanding, can explain concepts clearly
- **Developing (2 points):** Basic understanding, may need prompting
- **Beginner (1 point):** Limited understanding, requires significant guidance

**Red Flags:**
- Cannot explain basic microservices concepts
- Suggests microservices for all scenarios
- No understanding of distributed system challenges
- Cannot discuss trade-offs between patterns