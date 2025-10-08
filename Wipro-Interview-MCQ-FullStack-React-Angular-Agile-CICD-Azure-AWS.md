# Wipro Interview MCQs — .NET Full Stack, React, Angular, Agile, CI/CD, Azure, AWS

Format: Single best answer. Each question includes a brief rationale.

---

## .NET Full Stack (6)

1) In ASP.NET Core, where should `UseExceptionHandler()` be placed to ensure it wraps downstream middleware?
A. After `UseEndpoints()`
B. Before `UseRouting()` and as early as possible
C. Only inside controllers
D. After `UseAuthentication()` but before `UseAuthorization()`

Correct: B — Exception handling should be registered early so it can catch exceptions from later middleware and endpoints.

2) What is the recommended lifetime for `DbContext` in ASP.NET Core DI?
A. Singleton
B. Scoped
C. Transient
D. Pooled singleton

Correct: B — `DbContext` should be scoped to the request to ensure unit-of-work semantics and avoid threading issues.

3) Best way to improve read-only query performance in EF Core:
A. Call `ToList()` twice to warm cache
B. Use `AsNoTracking()` on queries
C. Disable change tracker globally
D. Use `Include()` on every navigation

Correct: B — `AsNoTracking()` avoids tracking cost for read-only queries and is the standard performance optimization.

4) When running multiple app instances, which cache is appropriate for shared cache state?
A. `IMemoryCache`
B. Static dictionary
C. `IDistributedCache` (e.g., Redis)
D. Thread-local storage

Correct: C — `IDistributedCache` supports multi-instance deployments; `IMemoryCache` is per-process.

5) Which practice prevents thread starvation and deadlocks in ASP.NET Core code?
A. Use `.Result` on tasks inside controllers
B. Nest `Task.Run` for awaited calls
C. Use async all the way; avoid blocking calls
D. Always call `ConfigureAwait(false)`

Correct: C — Avoid blocking on async; ASP.NET Core is free of legacy SynchronizationContext, but blocking still causes thread pool issues.

6) Minimal APIs vs Controllers: a pragmatic advantage of Minimal APIs is:
A. They support filters and model binding only via MVC attributes
B. Lower ceremony and faster startup for small services
C. They can only return strings
D. They can’t use DI

Correct: B — Minimal APIs reduce ceremony and are great for small, focused HTTP services with DI support.

---

## React (6)

1) `useCallback` vs `useMemo`:
A. `useMemo` memoizes functions; `useCallback` memoizes values
B. Both memoize values
C. `useCallback` memoizes functions; `useMemo` memoizes computed values
D. Neither helps with re-renders

Correct: C — `useCallback(fn, deps)` memoizes a function; `useMemo(calc, deps)` memoizes a value.

2) Preventing re-render thrash with child components receiving handlers:
A. Inline arrow functions always
B. Wrap handlers with `useCallback` and pass stable dependencies
C. Use `useMemo` for handlers
D. Use `useEffect` with empty deps

Correct: B — Stable function references reduce needless child re-renders when combined with `React.memo`.

3) Correct dependency handling for `useEffect` fetching data on prop change:
A. Omit deps to run once
B. Include only state, not props
C. Include all values used inside effect (e.g., the prop)
D. Add a random key to force rerun

Correct: C — The effect should depend on every reactive value it reads.

4) Keys for lists should be:
A. Array index
B. Random UUID per render
C. Stable unique ID from data
D. The object reference

Correct: C — Stable IDs preserve item identity across updates; indexes break reordering.

5) Controlled vs uncontrolled inputs:
A. Controlled inputs use React state as the source of truth
B. Uncontrolled inputs can’t have default values
C. Controlled inputs can’t validate
D. Uncontrolled inputs trigger more renders

Correct: A — Controlled components are driven by state; uncontrolled rely on the DOM value.

6) React 18 `useTransition` is used to:
A. Defer critical updates behind timeouts
B. Mark non-urgent updates so urgent ones stay responsive
C. Abort fetches
D. Replace Suspense

Correct: B — `useTransition` labels state updates as non-urgent to keep UI responsive.

---

## Angular (6)

1) `ChangeDetectionStrategy.OnPush` triggers change detection when:
A. Any async task completes globally
B. Input reference changes, events, or observable emissions via `async` pipe
C. Only on manual `detectChanges()`
D. Every 16ms tick

Correct: B — OnPush updates on input reference changes, events, and observable emissions consumed in template.

2) Benefit of `async` pipe in templates:
A. Prevents change detection
B. Auto-subscribes and unsubscribes from observables
C. Works only with Promises
D. Improves bundle size

Correct: B — The `async` pipe manages subscriptions safely and simplifies templates.

3) Standalone components (Angular 14+) are:
A. Declared only in NgModules
B. Declared with `standalone: true` and can import providers directly
C. Deprecated
D. Server-side only

Correct: B — Standalone components remove the need for NgModule declarations.

4) Tree-shakable providers are configured by:
A. `providedIn: 'root'`
B. Adding to every module’s providers
C. Using `forwardRef`
D. Using `Injector.create()`

Correct: A — `providedIn: 'root'` registers a service with the root injector, enabling tree-shaking.

5) Intercepting all HTTP requests to add auth headers:
A. Use a route guard
B. Use an `HttpInterceptor` registered with multi providers
C. Patch `HttpClient` prototype
D. Use a resolver

Correct: B — Interceptors manipulate requests/responses across the app.

6) Reactive forms are generally preferred when:
A. The form is tiny and static
B. You need dynamic controls and complex validation
C. You don’t need validation
D. You can’t use TypeScript

Correct: B — Reactive forms scale better for complex scenarios and testing.

---

## Agile (6)

1) The Sprint Review primarily focuses on:
A. Team performance ratings
B. Inspecting the increment and gathering stakeholder feedback
C. Re-estimating the entire backlog
D. Extending the sprint

Correct: B — It’s about the product, not performance management.

2) Definition of Done (DoD) is:
A. Personal to each developer
B. A shared quality bar for increments, enabling releasable work
C. Only for production releases
D. Optional in Scrum

Correct: B — DoD ensures quality and transparency for completed items.

3) Velocity should be used to:
A. Compare teams
B. Judge individual performance
C. Forecast within a team over time
D. Replace prioritization

Correct: C — Velocity helps a team forecast its own capacity; not for cross-team comparison.

4) Good story slicing is:
A. Horizontal (DB only)
B. Vertical thin slices delivering user value
C. Only spike tasks
D. Based on component boundaries

Correct: B — Vertical slices deliver end-to-end value and feedback.

5) The Product Owner is accountable for:
A. Test strategy
B. Maximizing product value and ordering backlog
C. Facilitating ceremonies
D. Hiring developers

Correct: B — PO maximizes value via backlog ownership.

6) Story points represent:
A. Exact hours
B. Calendar days
C. Relative effort considering complexity and uncertainty
D. Number of lines of code

Correct: C — Points capture relative effort and risk, not time.

---

## CI/CD (6)

1) Blue/Green vs Canary:
A. Blue/Green shifts a small percent; Canary flips all at once
B. Both identical
C. Blue/Green swaps full environments; Canary gradually shifts traffic
D. Canary requires feature flags; Blue/Green doesn’t

Correct: C — Blue/Green swaps environments; Canary ramps traffic in stages.

2) Trunk-based development encourages:
A. Long-lived feature branches
B. Daily small merges to main and feature flags
C. Monthly release branches
D. No code reviews

Correct: B — Small, frequent merges reduce merge debt and speed feedback.

3) Secrets in pipelines should be:
A. Committed to repo with base64
B. Stored in secure secret stores (e.g., Key Vault/Secrets Manager) and referenced as masked vars
C. Passed on the command line
D. Emailed to the team

Correct: B — Use managed secret storage and masked variables.

4) Reproducible builds are improved by:
A. Skipping lock files
B. Pinning dependencies and using lockfiles
C. Building on random base images
D. Disabling caches

Correct: B — Deterministic builds rely on pinned versions and lockfiles.

5) Faster CI without flakiness:
A. Run everything in one job
B. Parallelize independent stages and cache dependencies with keyed caches
C. Disable tests on PRs
D. Use `sleep` between steps

Correct: B — Parallelization + proper caching speeds pipelines safely.

6) IaC in deployment ensures:
A. Manual parity across envs
B. Drift between staging and prod
C. Versioned, reviewable, consistent environments
D. Faster hotfixes by editing consoles

Correct: C — IaC gives consistency and traceability across environments.

---

## Azure (6)

1) Best fit for short-running, event-driven tasks:
A. Azure App Service
B. Azure Functions (Consumption)
C. Azure Virtual Machines
D. Azure Kubernetes Service (AKS)

Correct: B — Serverless Functions suit bursty, event-driven workloads.

2) Cosmos DB partition key selection should prioritize:
A. Low-cardinality keys to reduce partitions
B. High-cardinality, evenly distributed access patterns
C. Random GUID per item, always
D. Using `/id` for all cases

Correct: B — Even distribution and scalable RUs require a good partition strategy.

3) Azure Storage tiers for rarely accessed, long-term data:
A. Hot
B. Cool
C. Archive
D. Premium

Correct: C — Archive minimizes cost for rarely accessed objects (higher retrieval latency).

4) Managing secrets for apps on Azure:
A. Store in appsettings.json
B. Use Azure Key Vault with managed identities
C. Environment variables in code
D. Hardcode in source

Correct: B — Key Vault + managed identities is the recommended approach.

5) End-to-end telemetry with correlation across services:
A. Azure Monitor + Application Insights with distributed tracing
B. Blob logs
C. Azure Maps
D. Traffic Manager

Correct: A — App Insights supports traces, metrics, and correlation IDs.

6) To host containerized microservices with full control over networking and scaling:
A. App Service (Web Apps for Containers)
B. Azure Container Instances
C. AKS
D. Azure Functions

Correct: C — AKS provides full Kubernetes control for microservice fleets.

---

## AWS (6)

1) Stateless, event-driven compute with per-request billing:
A. EC2 Auto Scaling
B. AWS Lambda
C. Amazon ECS on EC2
D. Amazon EKS

Correct: B — Lambda is serverless with per-invocation billing.

2) S3 storage class that automatically optimizes cost based on access patterns:
A. S3 Standard
B. S3 Intelligent-Tiering
C. S3 Glacier Deep Archive
D. S3 One Zone-IA

Correct: B — Intelligent-Tiering moves objects across tiers based on usage.

3) IAM best practice for applications on EC2/ECS/EKS:
A. Embed long-lived access keys in AMIs
B. Use instance/task roles with least privilege
C. Share admin user among services
D. Use root credentials

Correct: B — Roles provide short-lived credentials and least privilege.

4) API Gateway integrating with Lambda commonly uses:
A. HTTP proxy to EC2
B. Lambda proxy integration for pass-through requests/responses
C. Direct DynamoDB integration only
D. SNS fanout

Correct: B — Lambda proxy integration forwards the full HTTP request context to the function.

5) Achieve high availability for relational databases:
A. Single-AZ RDS
B. RDS Multi-AZ for failover; Read Replicas for scale-out reads
C. Use S3 instead of RDS
D. Only Aurora Serverless

Correct: B — Multi-AZ provides HA; Read Replicas increase read throughput.

6) Tracing and metrics for distributed apps:
A. CloudWatch Logs only
B. X-Ray for tracing; CloudWatch Metrics/Logs for metrics and logs
C. S3 event notifications
D. AWS Config

Correct: B — X-Ray provides traces; CloudWatch captures metrics/logs.

---

## Answer key summary
- All answers are marked per question with a short rationale.

Tips for Wipro interviews:
- Justify answers briefly (performance, scalability, security, or operational reasons)
- Call out trade-offs when relevant (e.g., Blue/Green vs Canary)
- Tie choices to enterprise practices (governance, observability, cost, reliability)

---

## Databases — MongoDB & SQL Server (6)

1) MongoDB schema design should primarily optimize for:
A. Normalization to 5NF
B. Join-heavy relational patterns
C. Query patterns and document shape used by the app
D. Avoiding arrays in documents at all costs

Correct: C — Model documents around read/write patterns to minimize joins/aggregations and leverage embedded documents where it fits.

2) To ensure atomicity of related updates across multiple MongoDB documents:
A. Use unordered bulk writes only
B. Use multi-document ACID transactions on replica sets
C. Rely on eventual consistency
D. Split across databases

Correct: B — MongoDB supports multi-document transactions on replica sets/sharded clusters for ACID semantics when needed.

3) Efficient pagination in MongoDB with stable ordering should prefer:
A. `skip/limit` on large collections
B. Range-based pagination using an indexed field (e.g., `_id`)
C. Sorting on non-indexed fields
D. Aggregation `$sample`

Correct: B — Range (cursor) pagination is more scalable than `skip/limit` for large datasets and uses index order.

4) SQL Server: improving read-heavy workload with minimal write impact:
A. Add many nonclustered indexes on every column
B. Add targeted nonclustered indexes and use included columns
C. Force NOLOCK everywhere
D. Use only a clustered index

Correct: B — Targeted nonclustered indexes with INCLUDE columns cover queries and reduce lookups; avoid over-indexing and NOLOCK side effects.

5) SQL Server transaction isolation that avoids dirty reads but allows non-repeatable reads:
A. READ UNCOMMITTED
B. READ COMMITTED
C. REPEATABLE READ
D. SERIALIZABLE

Correct: B — READ COMMITTED prevents dirty reads, but non-repeatable reads and phantom reads may still occur.

6) Scaling SQL Server reads with high availability:
A. Single primary only
B. Transactional replication or Always On availability groups with readable secondaries
C. Partition tables only
D. Use tempdb for reporting

Correct: B — Replication/Azure SQL/AG readable secondaries scale read workloads and add HA; partitioning is for manageability, not HA alone.

---

## Appendix — Quick Justification Stems

Use these one-line justifications after your answer to signal trade-off thinking.

- Performance: "Reduces hot path latency by avoiding unnecessary allocations/round-trips and leveraging indexes/caching."
- Scalability: "Removes single-instance bottlenecks and supports horizontal scale via stateless services and distributed backends."
- Security: "Eliminates secret sprawl using managed identities and enforces least privilege with role-scoped access."
- Operational: "Improves reliability with idempotent runbooks, health checks, and observable pipelines for faster rollback and MTTR."
- Cost: "Pins dependencies and right-sizes resources; leverages tiering/caching to cut storage and egress."
- Maintainability: "Simplifies codepaths with clear contracts and templates; reduces coupling for safer changes."
