# Docker & Kubernetes Complete Guide
## Step-by-Step Real-World Scenarios

### Table of Contents
1. [Docker Fundamentals](#docker-fundamentals)
2. [Docker Real-World Scenarios](#docker-real-world-scenarios)
3. [Kubernetes Fundamentals](#kubernetes-fundamentals)
4. [Kubernetes Real-World Scenarios](#kubernetes-real-world-scenarios)
5. [Complete DevOps Pipeline](#complete-devops-pipeline)
6. [Production Best Practices](#production-best-practices)

---

## Docker Fundamentals

### What is Docker?
Docker is a containerization platform that packages applications and their dependencies into lightweight, portable containers that can run consistently across different environments.

**Key Concepts:**
- **Container**: Lightweight, portable package containing application and dependencies
- **Image**: Read-only template used to create containers
- **Dockerfile**: Instructions to build Docker images
- **Registry**: Storage for Docker images (Docker Hub, AWS ECR, etc.)

### Real-World Scenario 1: Containerizing a .NET Core Web API

**Business Context**: You have a .NET Core Web API that needs to run consistently across development, staging, and production environments.

#### Step 1: Prepare Your .NET Core Application

```csharp
// Program.cs - Simple .NET Core Web API
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure for containerization
builder.WebHost.UseUrls("http://0.0.0.0:5000"); // Bind to all interfaces

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { 
    Status = "Healthy", 
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName,
    Version = "1.0.0"
});

app.Run();

// Controllers/ProductController.cs
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 999.99m },
            new Product { Id = 2, Name = "Phone", Price = 599.99m },
            new Product { Id = 3, Name = "Tablet", Price = 399.99m }
        };
        return Ok(products);
    }

    [HttpGet("{id}")]
    public ActionResult<Product> Get(int id)
    {
        var product = new Product { Id = id, Name = $"Product {id}", Price = 99.99m };
        return Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> Post([FromBody] Product product)
    {
        // In real scenario, save to database
        product.Id = new Random().Next(1000, 9999);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }
}

public record Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
```

#### Step 2: Create Dockerfile

```dockerfile
# Dockerfile - Multi-stage build for .NET Core API
# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy source code and build application
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1001 appgroup && \
    adduser --system --uid 1001 --gid 1001 appuser

# Copy built application from build stage
COPY --from=build /app/out .

# Change ownership to non-root user
RUN chown -R appuser:appgroup /app
USER appuser

# Expose port
EXPOSE 5000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Set entry point
ENTRYPOINT ["dotnet", "ProductApi.dll"]
```

#### Step 3: Build and Run Docker Image

```bash
# Build the Docker image
docker build -t product-api:1.0.0 .

# Tag for different environments
docker tag product-api:1.0.0 product-api:latest
docker tag product-api:1.0.0 product-api:dev
docker tag product-api:1.0.0 product-api:staging

# Run container with environment variables
docker run -d \
  --name product-api-container \
  -p 8080:5000 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Server=db;Database=ProductDB;User=sa;Password=YourPassword123;" \
  --restart unless-stopped \
  product-api:1.0.0

# Check container logs
docker logs product-api-container

# Test the API
curl http://localhost:8080/api/product
curl http://localhost:8080/health
```

#### Step 4: Docker Compose for Multi-Service Setup

```yaml
# docker-compose.yml - Complete application stack
version: '3.8'

services:
  # SQL Server Database
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: product-db
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
      - MSSQL_PID=Express
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    networks:
      - product-network
    restart: unless-stopped

  # Redis Cache
  redis:
    image: redis:7-alpine
    container_name: product-cache
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - product-network
    restart: unless-stopped
    command: redis-server --appendonly yes

  # Product API
  product-api:
    build: 
      context: .
      dockerfile: Dockerfile
    container_name: product-api
    ports:
      - "8080:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=ProductDB;User=sa;Password=YourPassword123!;TrustServerCertificate=true;
      - Redis__ConnectionString=redis:6379
    depends_on:
      - sqlserver
      - redis
    networks:
      - product-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

  # Nginx Reverse Proxy
  nginx:
    image: nginx:alpine
    container_name: product-nginx
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
      - ./ssl:/etc/nginx/ssl:ro
    depends_on:
      - product-api
    networks:
      - product-network
    restart: unless-stopped

volumes:
  sqlserver_data:
  redis_data:

networks:
  product-network:
    driver: bridge
```

```nginx
# nginx.conf - Load balancer and SSL termination
events {
    worker_connections 1024;
}

http {
    upstream product_api {
        server product-api:5000;
        # Add more instances for load balancing
        # server product-api-2:5000;
        # server product-api-3:5000;
    }

    # HTTP to HTTPS redirect
    server {
        listen 80;
        server_name localhost;
        return 301 https://$host$request_uri;
    }

    # HTTPS server
    server {
        listen 443 ssl;
        server_name localhost;

        ssl_certificate /etc/nginx/ssl/cert.pem;
        ssl_certificate_key /etc/nginx/ssl/key.pem;

        location / {
            proxy_pass http://product_api;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            
            # Health check
            proxy_connect_timeout 5s;
            proxy_send_timeout 60s;
            proxy_read_timeout 60s;
        }

        location /health {
            proxy_pass http://product_api/health;
            access_log off;
        }
    }
}
```

#### Step 5: Environment-Specific Configurations

```yaml
# docker-compose.dev.yml - Development environment
version: '3.8'

services:
  product-api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5000
    volumes:
      - .:/app  # Hot reload for development
    command: dotnet watch run
```

```yaml
# docker-compose.prod.yml - Production environment
version: '3.8'

services:
  product-api:
    image: your-registry.com/product-api:${VERSION}
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    deploy:
      replicas: 3
      resources:
        limits:
          cpus: '0.5'
          memory: 512M
        reservations:
          cpus: '0.25'
          memory: 256M
```

```bash
# Commands for different environments
# Development
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d

# Production
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Staging
docker-compose -f docker-compose.yml -f docker-compose.staging.yml up -d
```

---

## Docker Real-World Scenarios

### Scenario 2: Microservices Architecture with Docker

**Business Context**: E-commerce platform with separate services for users, products, orders, and notifications.

#### Project Structure
```
ecommerce-platform/
├── services/
│   ├── user-service/
│   │   ├── Dockerfile
│   │   └── src/
│   ├── product-service/
│   │   ├── Dockerfile
│   │   └── src/
│   ├── order-service/
│   │   ├── Dockerfile
│   │   └── src/
│   └── notification-service/
│       ├── Dockerfile
│       └── src/
├── docker-compose.yml
├── docker-compose.override.yml
└── scripts/
    ├── build.sh
    ├── deploy.sh
    └── cleanup.sh
```

#### User Service (Node.js)

```javascript
// services/user-service/src/app.js
const express = require('express');
const mongoose = require('mongoose');
const jwt = require('jsonwebtoken');
const bcrypt = require('bcrypt');

const app = express();
app.use(express.json());

// MongoDB connection
mongoose.connect(process.env.MONGODB_URL || 'mongodb://mongodb:27017/userdb');

// User schema
const userSchema = new mongoose.Schema({
  username: { type: String, required: true, unique: true },
  email: { type: String, required: true, unique: true },
  password: { type: String, required: true },
  createdAt: { type: Date, default: Date.now }
});

const User = mongoose.model('User', userSchema);

// Routes
app.post('/api/users/register', async (req, res) => {
  try {
    const { username, email, password } = req.body;
    const hashedPassword = await bcrypt.hash(password, 10);
    
    const user = new User({ username, email, password: hashedPassword });
    await user.save();
    
    res.status(201).json({ message: 'User created successfully', userId: user._id });
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
});

app.post('/api/users/login', async (req, res) => {
  try {
    const { email, password } = req.body;
    const user = await User.findOne({ email });
    
    if (!user || !await bcrypt.compare(password, user.password)) {
      return res.status(401).json({ error: 'Invalid credentials' });
    }
    
    const token = jwt.sign({ userId: user._id }, process.env.JWT_SECRET || 'secret', { expiresIn: '1h' });
    res.json({ token, user: { id: user._id, username: user.username, email: user.email } });
  } catch (error) {
    res.status(400).json({ error: error.message });
  }
});

app.get('/health', (req, res) => {
  res.json({ service: 'user-service', status: 'healthy', timestamp: new Date().toISOString() });
});

const PORT = process.env.PORT || 3001;
app.listen(PORT, '0.0.0.0', () => {
  console.log(`User service running on port ${PORT}`);
});
```

```dockerfile
# services/user-service/Dockerfile
FROM node:18-alpine

WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci --only=production

# Copy source code
COPY src/ ./src/

# Create non-root user
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nodejs -u 1001 -G nodejs

USER nodejs

EXPOSE 3001

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:3001/health || exit 1

CMD ["node", "src/app.js"]
```

#### Complete Microservices Docker Compose

```yaml
# docker-compose.yml - Microservices setup
version: '3.8'

services:
  # Databases
  mongodb:
    image: mongo:6
    container_name: mongodb
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: password123
    volumes:
      - mongodb_data:/data/db
    networks:
      - microservices-network
    restart: unless-stopped

  postgres:
    image: postgres:15
    container_name: postgres
    environment:
      POSTGRES_DB: ecommerce
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password123
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - microservices-network
    restart: unless-stopped

  redis:
    image: redis:7-alpine
    container_name: redis
    networks:
      - microservices-network
    restart: unless-stopped

  # Message Queue
  rabbitmq:
    image: rabbitmq:3-management
    container_name: rabbitmq
    environment:
      RABBITMQ_DEFAULT_USER: admin
      RABBITMQ_DEFAULT_PASS: password123
    ports:
      - "5672:5672"
      - "15672:15672"  # Management UI
    networks:
      - microservices-network
    restart: unless-stopped

  # Microservices
  user-service:
    build: ./services/user-service
    container_name: user-service
    environment:
      - MONGODB_URL=mongodb://admin:password123@mongodb:27017/userdb?authSource=admin
      - JWT_SECRET=your-jwt-secret-here
      - RABBITMQ_URL=amqp://admin:password123@rabbitmq:5672
    depends_on:
      - mongodb
      - rabbitmq
    networks:
      - microservices-network
    restart: unless-stopped

  product-service:
    build: ./services/product-service
    container_name: product-service
    environment:
      - DATABASE_URL=postgresql://postgres:password123@postgres:5432/ecommerce
      - REDIS_URL=redis://redis:6379
      - RABBITMQ_URL=amqp://admin:password123@rabbitmq:5672
    depends_on:
      - postgres
      - redis
      - rabbitmq
    networks:
      - microservices-network
    restart: unless-stopped

  order-service:
    build: ./services/order-service
    container_name: order-service
    environment:
      - DATABASE_URL=postgresql://postgres:password123@postgres:5432/ecommerce
      - USER_SERVICE_URL=http://user-service:3001
      - PRODUCT_SERVICE_URL=http://product-service:3002
      - RABBITMQ_URL=amqp://admin:password123@rabbitmq:5672
    depends_on:
      - postgres
      - user-service
      - product-service
      - rabbitmq
    networks:
      - microservices-network
    restart: unless-stopped

  notification-service:
    build: ./services/notification-service
    container_name: notification-service
    environment:
      - RABBITMQ_URL=amqp://admin:password123@rabbitmq:5672
      - EMAIL_SERVICE_API_KEY=${EMAIL_SERVICE_API_KEY}
      - SMS_SERVICE_API_KEY=${SMS_SERVICE_API_KEY}
    depends_on:
      - rabbitmq
    networks:
      - microservices-network
    restart: unless-stopped

  # API Gateway
  api-gateway:
    image: nginx:alpine
    container_name: api-gateway
    ports:
      - "80:80"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
    depends_on:
      - user-service
      - product-service
      - order-service
    networks:
      - microservices-network
    restart: unless-stopped

  # Monitoring
  prometheus:
    image: prom/prometheus
    container_name: prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./monitoring/prometheus.yml:/etc/prometheus/prometheus.yml:ro
    networks:
      - microservices-network
    restart: unless-stopped

  grafana:
    image: grafana/grafana
    container_name: grafana
    ports:
      - "3000:3000"
    environment:
      - GF_SECURITY_ADMIN_PASSWORD=admin123
    volumes:
      - grafana_data:/var/lib/grafana
    networks:
      - microservices-network
    restart: unless-stopped

volumes:
  mongodb_data:
  postgres_data:
  grafana_data:

networks:
  microservices-network:
    driver: bridge
```

#### Deployment Scripts

```bash
#!/bin/bash
# scripts/build.sh - Build all services

echo "Building microservices..."

# Build user service
echo "Building user-service..."
cd services/user-service
docker build -t ecommerce/user-service:latest .
cd ../..

# Build product service
echo "Building product-service..."
cd services/product-service
docker build -t ecommerce/product-service:latest .
cd ../..

# Build order service
echo "Building order-service..."
cd services/order-service
docker build -t ecommerce/order-service:latest .
cd ../..

# Build notification service
echo "Building notification-service..."
cd services/notification-service
docker build -t ecommerce/notification-service:latest .
cd ../..

echo "All services built successfully!"
```

```bash
#!/bin/bash
# scripts/deploy.sh - Deploy to production

set -e

echo "Deploying ecommerce platform..."

# Load environment variables
if [ -f .env.prod ]; then
    source .env.prod
fi

# Pull latest images
docker-compose pull

# Deploy with zero downtime
docker-compose up -d --scale user-service=2 --scale product-service=2 --scale order-service=2

# Wait for services to be healthy
echo "Waiting for services to be healthy..."
sleep 30

# Run health checks
./scripts/health-check.sh

echo "Deployment completed successfully!"
```

```bash
#!/bin/bash
# scripts/health-check.sh - Check service health

services=("user-service:3001" "product-service:3002" "order-service:3003")

for service in "${services[@]}"; do
    IFS=':' read -r name port <<< "$service"
    
    echo "Checking $name..."
    if curl -f "http://localhost:$port/health" > /dev/null 2>&1; then
        echo "✅ $name is healthy"
    else
        echo "❌ $name is not responding"
        exit 1
    fi
done

echo "✅ All services are healthy!"
```

---

## Kubernetes Fundamentals

### What is Kubernetes?
Kubernetes (K8s) is an open-source container orchestration platform that automates deployment, scaling, and management of containerized applications across clusters of machines.

**Key Concepts:**
- **Pod**: Smallest deployable unit containing one or more containers
- **Service**: Network abstraction providing stable endpoint for pods
- **Deployment**: Manages replica sets and rolling updates
- **ConfigMap/Secret**: Configuration and sensitive data management
- **Namespace**: Virtual clusters for resource isolation
- **Ingress**: HTTP/HTTPS routing to services

### Real-World Scenario 3: Migrating Docker Compose to Kubernetes

**Business Context**: Your microservices application running on Docker Compose needs to scale and be highly available across multiple servers.

#### Step 1: Kubernetes Cluster Setup

```bash
# Using minikube for local development
minikube start --driver=docker --memory=4096 --cpus=2

# Enable required addons
minikube addons enable ingress
minikube addons enable dashboard
minikube addons enable metrics-server

# Verify cluster
kubectl cluster-info
kubectl get nodes
```

#### Step 2: Namespace and Resource Quotas

```yaml
# k8s/namespace.yaml
apiVersion: v1
kind: Namespace
metadata:
  name: ecommerce
  labels:
    name: ecommerce
    environment: production
---
apiVersion: v1
kind: ResourceQuota
metadata:
  name: ecommerce-quota
  namespace: ecommerce
spec:
  hard:
    requests.cpu: "10"
    requests.memory: 20Gi
    limits.cpu: "20"
    limits.memory: 40Gi
    persistentvolumeclaims: "10"
    pods: "50"
    services: "20"
```

#### Step 3: ConfigMaps and Secrets

```yaml
# k8s/configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: app-config
  namespace: ecommerce
data:
  # Database configurations
  POSTGRES_DB: "ecommerce"
  POSTGRES_USER: "postgres"
  MONGODB_DATABASE: "userdb"
  
  # Service URLs
  USER_SERVICE_URL: "http://user-service:3001"
  PRODUCT_SERVICE_URL: "http://product-service:3002"
  ORDER_SERVICE_URL: "http://order-service:3003"
  
  # Redis configuration
  REDIS_URL: "redis://redis-service:6379"
  
  # RabbitMQ configuration
  RABBITMQ_URL: "amqp://rabbitmq-service:5672"
  
  # Application settings
  NODE_ENV: "production"
  LOG_LEVEL: "info"
  API_RATE_LIMIT: "100"
---
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
  namespace: ecommerce
type: Opaque
data:
  # Base64 encoded values
  POSTGRES_PASSWORD: cGFzc3dvcmQxMjM=  # password123
  MONGODB_PASSWORD: cGFzc3dvcmQxMjM=   # password123
  JWT_SECRET: eW91ci1qd3Qtc2VjcmV0LWhlcmU=  # your-jwt-secret-here
  RABBITMQ_PASSWORD: cGFzc3dvcmQxMjM=  # password123
  EMAIL_SERVICE_API_KEY: eW91ci1lbWFpbC1hcGkta2V5  # your-email-api-key
  SMS_SERVICE_API_KEY: eW91ci1zbXMtYXBpLWtleQ==    # your-sms-api-key
```

#### Step 4: Persistent Volumes

```yaml
# k8s/persistent-volumes.yaml
apiVersion: v1
kind: PersistentVolume
metadata:
  name: postgres-pv
spec:
  capacity:
    storage: 10Gi
  volumeMode: Filesystem
  accessModes:
    - ReadWriteOnce
  persistentVolumeReclaimPolicy: Retain
  storageClassName: local-storage
  local:
    path: /mnt/data/postgres
  nodeAffinity:
    required:
      nodeSelectorTerms:
      - matchExpressions:
        - key: kubernetes.io/hostname
          operator: In
          values:
          - minikube
---
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: postgres-pvc
  namespace: ecommerce
spec:
  accessModes:
    - ReadWriteOnce
  volumeMode: Filesystem
  resources:
    requests:
      storage: 10Gi
  storageClassName: local-storage
---
apiVersion: v1
kind: PersistentVolume
metadata:
  name: mongodb-pv
spec:
  capacity:
    storage: 10Gi
  volumeMode: Filesystem
  accessModes:
    - ReadWriteOnce
  persistentVolumeReclaimPolicy: Retain
  storageClassName: local-storage
  local:
    path: /mnt/data/mongodb
  nodeAffinity:
    required:
      nodeSelectorTerms:
      - matchExpressions:
        - key: kubernetes.io/hostname
          operator: In
          values:
          - minikube
---
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: mongodb-pvc
  namespace: ecommerce
spec:
  accessModes:
    - ReadWriteOnce
  volumeMode: Filesystem
  resources:
    requests:
      storage: 10Gi
  storageClassName: local-storage
```

#### Step 5: Database Deployments

```yaml
# k8s/postgres-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: postgres-deployment
  namespace: ecommerce
  labels:
    app: postgres
spec:
  replicas: 1
  selector:
    matchLabels:
      app: postgres
  template:
    metadata:
      labels:
        app: postgres
    spec:
      containers:
      - name: postgres
        image: postgres:15
        ports:
        - containerPort: 5432
        env:
        - name: POSTGRES_DB
          valueFrom:
            configMapKeyRef:
              name: app-config
              key: POSTGRES_DB
        - name: POSTGRES_USER
          valueFrom:
            configMapKeyRef:
              name: app-config
              key: POSTGRES_USER
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: POSTGRES_PASSWORD
        volumeMounts:
        - name: postgres-storage
          mountPath: /var/lib/postgresql/data
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          exec:
            command:
            - pg_isready
            - -U
            - postgres
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          exec:
            command:
            - pg_isready
            - -U
            - postgres
          initialDelaySeconds: 5
          periodSeconds: 5
      volumes:
      - name: postgres-storage
        persistentVolumeClaim:
          claimName: postgres-pvc
---
apiVersion: v1
kind: Service
metadata:
  name: postgres-service
  namespace: ecommerce
spec:
  selector:
    app: postgres
  ports:
  - protocol: TCP
    port: 5432
    targetPort: 5432
  type: ClusterIP
```

```yaml
# k8s/mongodb-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mongodb-deployment
  namespace: ecommerce
  labels:
    app: mongodb
spec:
  replicas: 1
  selector:
    matchLabels:
      app: mongodb
  template:
    metadata:
      labels:
        app: mongodb
    spec:
      containers:
      - name: mongodb
        image: mongo:6
        ports:
        - containerPort: 27017
        env:
        - name: MONGO_INITDB_ROOT_USERNAME
          value: "admin"
        - name: MONGO_INITDB_ROOT_PASSWORD
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: MONGODB_PASSWORD
        volumeMounts:
        - name: mongodb-storage
          mountPath: /data/db
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          exec:
            command:
            - mongo
            - --eval
            - "db.adminCommand('ping')"
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          exec:
            command:
            - mongo
            - --eval
            - "db.adminCommand('ping')"
          initialDelaySeconds: 5
          periodSeconds: 5
      volumes:
      - name: mongodb-storage
        persistentVolumeClaim:
          claimName: mongodb-pvc
---
apiVersion: v1
kind: Service
metadata:
  name: mongodb-service
  namespace: ecommerce
spec:
  selector:
    app: mongodb
  ports:
  - protocol: TCP
    port: 27017
    targetPort: 27017
  type: ClusterIP
```

#### Step 6: Microservice Deployments

```yaml
# k8s/user-service-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: user-service-deployment
  namespace: ecommerce
  labels:
    app: user-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: user-service
  template:
    metadata:
      labels:
        app: user-service
    spec:
      containers:
      - name: user-service
        image: ecommerce/user-service:latest
        ports:
        - containerPort: 3001
        env:
        - name: MONGODB_URL
          value: "mongodb://admin:$(MONGODB_PASSWORD)@mongodb-service:27017/userdb?authSource=admin"
        - name: JWT_SECRET
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: JWT_SECRET
        - name: MONGODB_PASSWORD
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: MONGODB_PASSWORD
        - name: RABBITMQ_URL
          valueFrom:
            configMapKeyRef:
              name: app-config
              key: RABBITMQ_URL
        resources:
          requests:
            memory: "128Mi"
            cpu: "100m"
          limits:
            memory: "256Mi"
            cpu: "200m"
        livenessProbe:
          httpGet:
            path: /health
            port: 3001
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 3001
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: user-service
  namespace: ecommerce
spec:
  selector:
    app: user-service
  ports:
  - protocol: TCP
    port: 3001
    targetPort: 3001
  type: ClusterIP
```

```yaml
# k8s/product-service-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: product-service-deployment
  namespace: ecommerce
  labels:
    app: product-service
spec:
  replicas: 3
  selector:
    matchLabels:
      app: product-service
  template:
    metadata:
      labels:
        app: product-service
    spec:
      containers:
      - name: product-service
        image: ecommerce/product-service:latest
        ports:
        - containerPort: 3002
        env:
        - name: DATABASE_URL
          value: "postgresql://$(POSTGRES_USER):$(POSTGRES_PASSWORD)@postgres-service:5432/$(POSTGRES_DB)"
        - name: POSTGRES_USER
          valueFrom:
            configMapKeyRef:
              name: app-config
              key: POSTGRES_USER
        - name: POSTGRES_DB
          valueFrom:
            configMapKeyRef:
              name: app-config
              key: POSTGRES_DB
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: POSTGRES_PASSWORD
        - name: REDIS_URL
          valueFrom:
            configMapKeyRef:
              name: app-config
              key: REDIS_URL
        resources:
          requests:
            memory: "128Mi"
            cpu: "100m"
          limits:
            memory: "256Mi"
            cpu: "200m"
        livenessProbe:
          httpGet:
            path: /health
            port: 3002
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 3002
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: product-service
  namespace: ecommerce
spec:
  selector:
    app: product-service
  ports:
  - protocol: TCP
    port: 3002
    targetPort: 3002
  type: ClusterIP
```

#### Step 7: Horizontal Pod Autoscaler

```yaml
# k8s/hpa.yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: user-service-hpa
  namespace: ecommerce
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: user-service-deployment
  minReplicas: 2
  maxReplicas: 10
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
  behavior:
    scaleDown:
      stabilizationWindowSeconds: 300
      policies:
      - type: Percent
        value: 10
        periodSeconds: 60
    scaleUp:
      stabilizationWindowSeconds: 0
      policies:
      - type: Percent
        value: 100
        periodSeconds: 15
      - type: Pods
        value: 4
        periodSeconds: 60
      selectPolicy: Max
---
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: product-service-hpa
  namespace: ecommerce
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: product-service-deployment
  minReplicas: 2
  maxReplicas: 15
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

#### Step 8: Ingress Configuration

```yaml
# k8s/ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: ecommerce-ingress
  namespace: ecommerce
  annotations:
    nginx.ingress.kubernetes.io/rewrite-target: /
    nginx.ingress.kubernetes.io/ssl-redirect: "false"
    nginx.ingress.kubernetes.io/use-regex: "true"
    nginx.ingress.kubernetes.io/rate-limit: "100"
    nginx.ingress.kubernetes.io/rate-limit-window: "1m"
    nginx.ingress.kubernetes.io/proxy-body-size: "10m"
    nginx.ingress.kubernetes.io/proxy-connect-timeout: "5"
    nginx.ingress.kubernetes.io/proxy-send-timeout: "60"
    nginx.ingress.kubernetes.io/proxy-read-timeout: "60"
spec:
  ingressClassName: nginx
  rules:
  - host: ecommerce.local
    http:
      paths:
      - path: /api/users
        pathType: Prefix
        backend:
          service:
            name: user-service
            port:
              number: 3001
      - path: /api/products
        pathType: Prefix
        backend:
          service:
            name: product-service
            port:
              number: 3002
      - path: /api/orders
        pathType: Prefix
        backend:
          service:
            name: order-service
            port:
              number: 3003
      - path: /health
        pathType: Prefix
        backend:
          service:
            name: user-service
            port:
              number: 3001
  # TLS configuration
  tls:
  - hosts:
    - ecommerce.local
    secretName: ecommerce-tls
```

#### Step 9: Deployment Scripts

```bash
#!/bin/bash
# k8s/deploy.sh - Deploy to Kubernetes

set -e

echo "Deploying to Kubernetes..."

# Apply namespace and RBAC
kubectl apply -f namespace.yaml

# Apply persistent volumes
kubectl apply -f persistent-volumes.yaml

# Apply config maps and secrets
kubectl apply -f configmap.yaml
kubectl apply -f secrets.yaml

# Deploy databases
kubectl apply -f postgres-deployment.yaml
kubectl apply -f mongodb-deployment.yaml
kubectl apply -f redis-deployment.yaml
kubectl apply -f rabbitmq-deployment.yaml

# Wait for databases to be ready
echo "Waiting for databases to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/postgres-deployment -n ecommerce
kubectl wait --for=condition=available --timeout=300s deployment/mongodb-deployment -n ecommerce

# Deploy services
kubectl apply -f user-service-deployment.yaml
kubectl apply -f product-service-deployment.yaml
kubectl apply -f order-service-deployment.yaml
kubectl apply -f notification-service-deployment.yaml

# Wait for services to be ready
echo "Waiting for services to be ready..."
kubectl wait --for=condition=available --timeout=300s deployment/user-service-deployment -n ecommerce
kubectl wait --for=condition=available --timeout=300s deployment/product-service-deployment -n ecommerce

# Apply autoscaling
kubectl apply -f hpa.yaml

# Apply ingress
kubectl apply -f ingress.yaml

# Display status
echo "Deployment completed!"
kubectl get pods -n ecommerce
kubectl get services -n ecommerce
kubectl get ingress -n ecommerce

echo "Access the application at: http://ecommerce.local"
echo "Add '$(minikube ip) ecommerce.local' to your /etc/hosts file"
```

```bash
#!/bin/bash
# k8s/monitor.sh - Monitor cluster health

echo "Cluster Information:"
kubectl cluster-info

echo -e "\nNodes:"
kubectl get nodes

echo -e "\nNamespaces:"
kubectl get namespaces

echo -e "\nPods in ecommerce namespace:"
kubectl get pods -n ecommerce -o wide

echo -e "\nServices in ecommerce namespace:"
kubectl get svc -n ecommerce

echo -e "\nHorizontal Pod Autoscalers:"
kubectl get hpa -n ecommerce

echo -e "\nIngress:"
kubectl get ingress -n ecommerce

echo -e "\nPersistent Volumes:"
kubectl get pv

echo -e "\nPersistent Volume Claims:"
kubectl get pvc -n ecommerce

echo -e "\nTop Pods (resource usage):"
kubectl top pods -n ecommerce

echo -e "\nTop Nodes (resource usage):"
kubectl top nodes
```

---

## Kubernetes Real-World Scenarios

### Scenario 4: Blue-Green Deployment Strategy

**Business Context**: Zero-downtime deployment for critical e-commerce application during peak hours.

#### Blue-Green Deployment Implementation

```yaml
# k8s/blue-green/blue-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: product-service-blue
  namespace: ecommerce
  labels:
    app: product-service
    version: blue
spec:
  replicas: 5
  selector:
    matchLabels:
      app: product-service
      version: blue
  template:
    metadata:
      labels:
        app: product-service
        version: blue
    spec:
      containers:
      - name: product-service
        image: ecommerce/product-service:v1.0.0
        ports:
        - containerPort: 3002
        env:
        - name: VERSION
          value: "1.0.0"
        - name: ENVIRONMENT
          value: "production"
        resources:
          requests:
            memory: "256Mi"
            cpu: "200m"
          limits:
            memory: "512Mi"
            cpu: "400m"
        livenessProbe:
          httpGet:
            path: /health
            port: 3002
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 3002
          initialDelaySeconds: 5
          periodSeconds: 5
```

```yaml
# k8s/blue-green/green-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: product-service-green
  namespace: ecommerce
  labels:
    app: product-service
    version: green
spec:
  replicas: 0  # Initially zero replicas
  selector:
    matchLabels:
      app: product-service
      version: green
  template:
    metadata:
      labels:
        app: product-service
        version: green
    spec:
      containers:
      - name: product-service
        image: ecommerce/product-service:v2.0.0
        ports:
        - containerPort: 3002
        env:
        - name: VERSION
          value: "2.0.0"
        - name: ENVIRONMENT
          value: "production"
        resources:
          requests:
            memory: "256Mi"
            cpu: "200m"
          limits:
            memory: "512Mi"
            cpu: "400m"
        livenessProbe:
          httpGet:
            path: /health
            port: 3002
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 3002
          initialDelaySeconds: 5
          periodSeconds: 5
```

```yaml
# k8s/blue-green/service.yaml
apiVersion: v1
kind: Service
metadata:
  name: product-service
  namespace: ecommerce
spec:
  selector:
    app: product-service
    version: blue  # Initially pointing to blue
  ports:
  - protocol: TCP
    port: 3002
    targetPort: 3002
  type: ClusterIP
```

#### Blue-Green Deployment Script

```bash
#!/bin/bash
# k8s/blue-green/deploy.sh - Blue-Green deployment script

set -e

NAMESPACE="ecommerce"
SERVICE_NAME="product-service"
NEW_VERSION=${1:-"v2.0.0"}
CURRENT_COLOR=""
NEW_COLOR=""

# Function to get current active color
get_current_color() {
    kubectl get service $SERVICE_NAME -n $NAMESPACE -o jsonpath='{.spec.selector.version}'
}

# Function to get the opposite color
get_opposite_color() {
    if [ "$1" == "blue" ]; then
        echo "green"
    else
        echo "blue"
    fi
}

# Function to wait for deployment to be ready
wait_for_deployment() {
    local deployment_name=$1
    echo "Waiting for deployment $deployment_name to be ready..."
    kubectl wait --for=condition=available --timeout=300s deployment/$deployment_name -n $NAMESPACE
}

# Function to run health checks
run_health_checks() {
    local color=$1
    echo "Running health checks for $color deployment..."
    
    # Get pod names for the color
    pods=$(kubectl get pods -n $NAMESPACE -l app=$SERVICE_NAME,version=$color -o jsonpath='{.items[*].metadata.name}')
    
    for pod in $pods; do
        echo "Health check for pod $pod..."
        kubectl exec -n $NAMESPACE $pod -- curl -f http://localhost:3002/health || {
            echo "Health check failed for pod $pod"
            return 1
        }
    done
    
    echo "All health checks passed for $color deployment"
}

# Function to run smoke tests
run_smoke_tests() {
    local color=$1
    echo "Running smoke tests against $color deployment..."
    
    # Port forward to test the deployment
    kubectl port-forward -n $NAMESPACE deployment/$SERVICE_NAME-$color 8002:3002 &
    PORT_FORWARD_PID=$!
    
    sleep 5
    
    # Run tests
    curl -f http://localhost:8002/api/products || {
        kill $PORT_FORWARD_PID
        echo "Smoke tests failed"
        return 1
    }
    
    # Test load
    for i in {1..10}; do
        curl -f http://localhost:8002/api/products/$i > /dev/null || {
            kill $PORT_FORWARD_PID
            echo "Load test failed on iteration $i"
            return 1
        }
    done
    
    kill $PORT_FORWARD_PID
    echo "Smoke tests passed"
}

# Main deployment logic
main() {
    echo "Starting Blue-Green deployment for $SERVICE_NAME with version $NEW_VERSION"
    
    # Get current active color
    CURRENT_COLOR=$(get_current_color)
    NEW_COLOR=$(get_opposite_color $CURRENT_COLOR)
    
    echo "Current active color: $CURRENT_COLOR"
    echo "Deploying to color: $NEW_COLOR"
    
    # Update the new color deployment with new image
    kubectl set image deployment/$SERVICE_NAME-$NEW_COLOR \
        $SERVICE_NAME=ecommerce/$SERVICE_NAME:$NEW_VERSION \
        -n $NAMESPACE
    
    # Scale up the new color deployment
    kubectl scale deployment/$SERVICE_NAME-$NEW_COLOR --replicas=5 -n $NAMESPACE
    
    # Wait for new deployment to be ready
    wait_for_deployment "$SERVICE_NAME-$NEW_COLOR"
    
    # Run health checks
    run_health_checks $NEW_COLOR
    
    # Run smoke tests
    run_smoke_tests $NEW_COLOR
    
    # Switch traffic to new color
    echo "Switching traffic to $NEW_COLOR..."
    kubectl patch service $SERVICE_NAME -n $NAMESPACE -p '{"spec":{"selector":{"version":"'$NEW_COLOR'"}}}'
    
    # Wait a bit for traffic to settle
    sleep 30
    
    # Run final verification
    echo "Running final verification..."
    run_smoke_tests $NEW_COLOR
    
    # Scale down old color deployment
    echo "Scaling down $CURRENT_COLOR deployment..."
    kubectl scale deployment/$SERVICE_NAME-$CURRENT_COLOR --replicas=0 -n $NAMESPACE
    
    echo "Blue-Green deployment completed successfully!"
    echo "Active color: $NEW_COLOR"
    echo "Version: $NEW_VERSION"
}

# Rollback function
rollback() {
    echo "Rolling back to $CURRENT_COLOR..."
    kubectl patch service $SERVICE_NAME -n $NAMESPACE -p '{"spec":{"selector":{"version":"'$CURRENT_COLOR'"}}}'
    kubectl scale deployment/$SERVICE_NAME-$CURRENT_COLOR --replicas=5 -n $NAMESPACE
    kubectl scale deployment/$SERVICE_NAME-$NEW_COLOR --replicas=0 -n $NAMESPACE
    echo "Rollback completed"
}

# Trap errors and rollback
trap 'echo "Deployment failed, rolling back..."; rollback; exit 1' ERR

# Run main deployment
main

echo "Deployment successful!"
```

### Scenario 5: Canary Deployment with Istio

**Business Context**: Gradual rollout of new features to minimize risk and gather user feedback.

#### Istio Setup

```bash
# Install Istio
curl -L https://istio.io/downloadIstio | sh -
cd istio-*
export PATH=$PWD/bin:$PATH

# Install Istio on cluster
istioctl install --set values.defaultRevision=default -y

# Enable sidecar injection
kubectl label namespace ecommerce istio-injection=enabled
```

#### Canary Deployment Configuration

```yaml
# k8s/canary/destination-rule.yaml
apiVersion: networking.istio.io/v1beta1
kind: DestinationRule
metadata:
  name: product-service-destination
  namespace: ecommerce
spec:
  host: product-service
  subsets:
  - name: v1
    labels:
      version: v1
  - name: v2
    labels:
      version: v2
```

```yaml
# k8s/canary/virtual-service.yaml
apiVersion: networking.istio.io/v1beta1
kind: VirtualService
metadata:
  name: product-service-vs
  namespace: ecommerce
spec:
  http:
  - match:
    - headers:
        canary:
          exact: "true"
    route:
    - destination:
        host: product-service
        subset: v2
      weight: 100
  - route:
    - destination:
        host: product-service
        subset: v1
      weight: 90  # 90% to stable version
    - destination:
        host: product-service
        subset: v2
      weight: 10  # 10% to canary version
```

#### Canary Deployment Script

```bash
#!/bin/bash
# k8s/canary/deploy.sh - Canary deployment script

set -e

NAMESPACE="ecommerce"
SERVICE_NAME="product-service"
NEW_VERSION=${1:-"v2.0.0"}
CANARY_PERCENTAGE=${2:-10}

# Function to update traffic split
update_traffic_split() {
    local v1_weight=$1
    local v2_weight=$2
    
    echo "Updating traffic split: v1=$v1_weight%, v2=$v2_weight%"
    
    kubectl patch virtualservice $SERVICE_NAME-vs -n $NAMESPACE --type='json' -p="[
        {
            \"op\": \"replace\",
            \"path\": \"/spec/http/1/route/0/weight\",
            \"value\": $v1_weight
        },
        {
            \"op\": \"replace\",
            \"path\": \"/spec/http/1/route/1/weight\",
            \"value\": $v2_weight
        }
    ]"
}

# Function to monitor metrics
monitor_metrics() {
    local duration=$1
    echo "Monitoring metrics for $duration seconds..."
    
    # Monitor error rate, response time, etc.
    # This would integrate with your monitoring system
    sleep $duration
    
    # Simulate metrics check
    error_rate=$(( RANDOM % 5 ))  # Random error rate 0-4%
    if [ $error_rate -gt 2 ]; then
        echo "High error rate detected: $error_rate%"
        return 1
    fi
    
    echo "Metrics look good. Error rate: $error_rate%"
    return 0
}

# Main canary deployment
main() {
    echo "Starting canary deployment for $SERVICE_NAME with version $NEW_VERSION"
    
    # Deploy v2 with zero traffic
    kubectl set image deployment/$SERVICE_NAME-v2 \
        $SERVICE_NAME=ecommerce/$SERVICE_NAME:$NEW_VERSION \
        -n $NAMESPACE
    
    kubectl scale deployment/$SERVICE_NAME-v2 --replicas=2 -n $NAMESPACE
    
    # Wait for deployment
    kubectl wait --for=condition=available --timeout=300s deployment/$SERVICE_NAME-v2 -n $NAMESPACE
    
    # Gradual traffic increase
    percentages=(10 25 50 75 100)
    
    for percentage in "${percentages[@]}"; do
        v1_weight=$((100 - percentage))
        v2_weight=$percentage
        
        # Update traffic split
        update_traffic_split $v1_weight $v2_weight
        
        # Monitor for issues
        if ! monitor_metrics 60; then
            echo "Issues detected, rolling back..."
            update_traffic_split 100 0
            exit 1
        fi
        
        echo "Canary at $percentage% - metrics stable"
        
        if [ $percentage -lt 100 ]; then
            echo "Waiting before next increment..."
            sleep 30
        fi
    done
    
    # Complete the rollout
    kubectl scale deployment/$SERVICE_NAME-v1 --replicas=0 -n $NAMESPACE
    
    echo "Canary deployment completed successfully!"
}

# Run deployment
main
```

## Complete DevOps Pipeline

### Scenario 6: CI/CD Pipeline with Docker and Kubernetes

**Business Context**: Automated pipeline from code commit to production deployment with quality gates and rollback capabilities.

#### GitLab CI/CD Pipeline

```yaml
# .gitlab-ci.yml
stages:
  - test
  - build
  - security-scan
  - deploy-staging
  - integration-tests
  - deploy-production
  - monitoring

variables:
  DOCKER_REGISTRY: "registry.gitlab.com/company/ecommerce"
  KUBERNETES_NAMESPACE: "ecommerce"
  HELM_CHART_PATH: "./helm/ecommerce"

# Testing Stage
unit-tests:
  stage: test
  image: node:18
  services:
    - postgres:15
    - redis:7
  variables:
    POSTGRES_DB: test_db
    POSTGRES_USER: test
    POSTGRES_PASSWORD: test123
  script:
    - npm ci
    - npm run test:unit
    - npm run test:integration
  coverage: '/Coverage: \d+\.\d+%/'
  artifacts:
    reports:
      coverage: coverage/cobertura-coverage.xml
      junit: test-results.xml
    paths:
      - coverage/
    expire_in: 1 week

code-quality:
  stage: test
  image: sonarsource/sonar-scanner-cli
  script:
    - sonar-scanner
      -Dsonar.projectKey=${CI_PROJECT_ID}
      -Dsonar.sources=src
      -Dsonar.host.url=${SONAR_HOST_URL}
      -Dsonar.login=${SONAR_TOKEN}
  only:
    - merge_requests
    - main

# Build Stage
build-docker-images:
  stage: build
  image: docker:20.10.16
  services:
    - docker:20.10.16-dind
  variables:
    DOCKER_TLS_CERTDIR: "/certs"
  before_script:
    - echo $CI_REGISTRY_PASSWORD | docker login -u $CI_REGISTRY_USER --password-stdin $CI_REGISTRY
  script:
    # Build all microservices
    - |
      for service in user-service product-service order-service notification-service; do
        echo "Building $service..."
        cd services/$service
        
        # Multi-arch build
        docker buildx create --use --name multi-arch-builder
        docker buildx build \
          --platform linux/amd64,linux/arm64 \
          --tag $DOCKER_REGISTRY/$service:$CI_COMMIT_SHA \
          --tag $DOCKER_REGISTRY/$service:latest \
          --push .
        
        cd ../..
      done
  only:
    - main
    - develop
    - merge_requests

# Security Scanning
security-scan:
  stage: security-scan
  image: aquasec/trivy:latest
  script:
    - |
      for service in user-service product-service order-service; do
        echo "Scanning $service for vulnerabilities..."
        trivy image \
          --exit-code 1 \
          --severity HIGH,CRITICAL \
          --format json \
          --output $service-security-report.json \
          $DOCKER_REGISTRY/$service:$CI_COMMIT_SHA
      done
  artifacts:
    paths:
      - "*-security-report.json"
    expire_in: 1 week
  allow_failure: false

# Deploy to Staging
deploy-staging:
  stage: deploy-staging
  image: alpine/helm:latest
  environment:
    name: staging
    url: https://staging.ecommerce.company.com
  before_script:
    - kubectl config use-context $KUBE_CONTEXT_STAGING
  script:
    - |
      helm upgrade --install ecommerce-staging $HELM_CHART_PATH \
        --namespace $KUBERNETES_NAMESPACE-staging \
        --create-namespace \
        --set image.tag=$CI_COMMIT_SHA \
        --set environment=staging \
        --set ingress.host=staging.ecommerce.company.com \
        --set replicas.userService=2 \
        --set replicas.productService=2 \
        --set replicas.orderService=1 \
        --wait --timeout=10m
  only:
    - main
    - develop

# Integration Tests
integration-tests:
  stage: integration-tests
  image: postman/newman:latest
  dependencies:
    - deploy-staging
  script:
    - newman run tests/integration/ecommerce-api.postman_collection.json \
        --environment tests/integration/staging.postman_environment.json \
        --reporters cli,junit \
        --reporter-junit-export newman-results.xml
  artifacts:
    reports:
      junit: newman-results.xml
    paths:
      - newman-results.xml
    expire_in: 1 week

# Performance Tests
performance-tests:
  stage: integration-tests
  image: grafana/k6:latest
  script:
    - k6 run --out json=performance-results.json tests/performance/load-test.js
  artifacts:
    paths:
      - performance-results.json
    expire_in: 1 week

# Production Deployment
deploy-production:
  stage: deploy-production
  image: alpine/helm:latest
  environment:
    name: production
    url: https://ecommerce.company.com
  before_script:
    - kubectl config use-context $KUBE_CONTEXT_PRODUCTION
  script:
    - |
      # Blue-Green deployment using Helm
      CURRENT_COLOR=$(helm get values ecommerce-prod --namespace $KUBERNETES_NAMESPACE | grep color | cut -d: -f2 | tr -d ' ')
      NEW_COLOR=$([ "$CURRENT_COLOR" = "blue" ] && echo "green" || echo "blue")
      
      echo "Current color: $CURRENT_COLOR, New color: $NEW_COLOR"
      
      # Deploy to new color
      helm upgrade --install ecommerce-prod-$NEW_COLOR $HELM_CHART_PATH \
        --namespace $KUBERNETES_NAMESPACE \
        --set image.tag=$CI_COMMIT_SHA \
        --set environment=production \
        --set color=$NEW_COLOR \
        --set ingress.host=ecommerce.company.com \
        --set replicas.userService=5 \
        --set replicas.productService=5 \
        --set replicas.orderService=3 \
        --wait --timeout=15m
      
      # Run health checks
      kubectl wait --for=condition=available \
        --timeout=300s \
        deployment/user-service-$NEW_COLOR \
        -n $KUBERNETES_NAMESPACE
      
      # Switch traffic
      kubectl patch service user-service -n $KUBERNETES_NAMESPACE \
        -p '{"spec":{"selector":{"color":"'$NEW_COLOR'"}}}'
      
      # Clean up old deployment after successful switch
      sleep 60
      helm uninstall ecommerce-prod-$CURRENT_COLOR --namespace $KUBERNETES_NAMESPACE || true
  when: manual
  only:
    - main
```

#### Helm Charts for Kubernetes Deployment

```yaml
# helm/ecommerce/Chart.yaml
apiVersion: v2
name: ecommerce
description: E-commerce microservices platform
version: 1.0.0
appVersion: "1.0.0"

dependencies:
  - name: postgresql
    version: 12.1.9
    repository: https://charts.bitnami.com/bitnami
    condition: postgresql.enabled
  - name: redis
    version: 17.8.7
    repository: https://charts.bitnami.com/bitnami
    condition: redis.enabled
  - name: rabbitmq
    version: 11.9.1
    repository: https://charts.bitnami.com/bitnami
    condition: rabbitmq.enabled
```

```yaml
# helm/ecommerce/values.yaml
# Global settings
global:
  imageRegistry: "registry.gitlab.com/company/ecommerce"
  imagePullPolicy: IfNotPresent
  storageClass: "fast-ssd"

# Environment
environment: production
color: blue

# Image settings
image:
  tag: "latest"
  pullPolicy: IfNotPresent

# Replica counts
replicas:
  userService: 3
  productService: 3
  orderService: 2
  notificationService: 1

# Resource limits
resources:
  userService:
    requests:
      memory: "256Mi"
      cpu: "200m"
    limits:
      memory: "512Mi"
      cpu: "500m"
  productService:
    requests:
      memory: "256Mi"
      cpu: "200m"
    limits:
      memory: "512Mi"
      cpu: "500m"

# Autoscaling
autoscaling:
  userService:
    enabled: true
    minReplicas: 2
    maxReplicas: 10
    targetCPUUtilizationPercentage: 70
  productService:
    enabled: true
    minReplicas: 2
    maxReplicas: 15
    targetCPUUtilizationPercentage: 70

# Ingress
ingress:
  enabled: true
  className: "nginx"
  host: "ecommerce.company.com"
  tls:
    enabled: true
    secretName: "ecommerce-tls"

# Database dependencies
postgresql:
  enabled: true
  auth:
    postgresPassword: "secretpassword"
    database: "ecommerce"
  primary:
    persistence:
      enabled: true
      size: 20Gi
      storageClass: "fast-ssd"

redis:
  enabled: true
  auth:
    enabled: false
  master:
    persistence:
      enabled: true
      size: 5Gi

rabbitmq:
  enabled: true
  auth:
    username: "admin"
    password: "secretpassword"
  persistence:
    enabled: true
    size: 10Gi

# Monitoring
monitoring:
  enabled: true
  serviceMonitor:
    enabled: true
    interval: 30s
```

```yaml
# helm/ecommerce/templates/user-service.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: user-service-{{ .Values.color }}
  namespace: {{ .Values.namespace | default "ecommerce" }}
  labels:
    app: user-service
    color: {{ .Values.color }}
    version: {{ .Values.image.tag }}
spec:
  replicas: {{ .Values.replicas.userService }}
  selector:
    matchLabels:
      app: user-service
      color: {{ .Values.color }}
  template:
    metadata:
      labels:
        app: user-service
        color: {{ .Values.color }}
        version: {{ .Values.image.tag }}
      annotations:
        prometheus.io/scrape: "true"
        prometheus.io/port: "3001"
        prometheus.io/path: "/metrics"
    spec:
      containers:
      - name: user-service
        image: {{ .Values.global.imageRegistry }}/user-service:{{ .Values.image.tag }}
        imagePullPolicy: {{ .Values.image.pullPolicy }}
        ports:
        - containerPort: 3001
        env:
        - name: NODE_ENV
          value: {{ .Values.environment }}
        - name: COLOR
          value: {{ .Values.color }}
        - name: VERSION
          value: {{ .Values.image.tag }}
        - name: MONGODB_URL
          valueFrom:
            secretKeyRef:
              name: mongodb-credentials
              key: connection-string
        - name: JWT_SECRET
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: jwt-secret
        resources:
          {{- toYaml .Values.resources.userService | nindent 10 }}
        livenessProbe:
          httpGet:
            path: /health
            port: 3001
          initialDelaySeconds: 30
          periodSeconds: 10
          timeoutSeconds: 5
          failureThreshold: 3
        readinessProbe:
          httpGet:
            path: /ready
            port: 3001
          initialDelaySeconds: 5
          periodSeconds: 5
          timeoutSeconds: 3
          failureThreshold: 3
        volumeMounts:
        - name: config
          mountPath: /app/config
          readOnly: true
      volumes:
      - name: config
        configMap:
          name: user-service-config
---
apiVersion: v1
kind: Service
metadata:
  name: user-service
  namespace: {{ .Values.namespace | default "ecommerce" }}
  labels:
    app: user-service
spec:
  selector:
    app: user-service
    color: {{ .Values.color }}
  ports:
  - protocol: TCP
    port: 3001
    targetPort: 3001
  type: ClusterIP
---
{{- if .Values.autoscaling.userService.enabled }}
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: user-service-hpa
  namespace: {{ .Values.namespace | default "ecommerce" }}
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: user-service-{{ .Values.color }}
  minReplicas: {{ .Values.autoscaling.userService.minReplicas }}
  maxReplicas: {{ .Values.autoscaling.userService.maxReplicas }}
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: {{ .Values.autoscaling.userService.targetCPUUtilizationPercentage }}
{{- end }}
```

### Scenario 7: Multi-Cluster Setup and Disaster Recovery

**Business Context**: High availability across multiple regions with automated failover and disaster recovery.

#### Multi-Cluster Architecture

```yaml
# clusters/primary/cluster-config.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: cluster-config
  namespace: kube-system
data:
  cluster-name: "primary-us-east-1"
  region: "us-east-1"
  role: "primary"
  backup-schedule: "0 2 * * *"  # Daily at 2 AM
---
apiVersion: v1
kind: ConfigMap
metadata:
  name: disaster-recovery-config
  namespace: ecommerce
data:
  dr-enabled: "true"
  replication-target: "secondary-us-west-2"
  rto: "300"  # Recovery Time Objective: 5 minutes
  rpo: "60"   # Recovery Point Objective: 1 minute
```

#### Cross-Cluster Service Mesh with Istio

```yaml
# istio/multi-cluster-setup.yaml
apiVersion: install.istio.io/v1alpha1
kind: IstioOperator
metadata:
  name: primary-cluster
spec:
  values:
    pilot:
      env:
        EXTERNAL_ISTIOD: false
    global:
      meshID: mesh1
      multiCluster:
        clusterName: primary-us-east-1
      network: network1
---
apiVersion: networking.istio.io/v1alpha3
kind: Gateway
metadata:
  name: cross-network-gateway
  namespace: istio-system
spec:
  selector:
    istio: eastwestgateway
  servers:
    - port:
        number: 15021
        name: status-port
        protocol: TLS
      tls:
        mode: ISTIO_MUTUAL
      hosts:
        - cross-network.local
```

#### Automated Backup Strategy

```yaml
# backup/velero-backup.yaml
apiVersion: velero.io/v1
kind: Schedule
metadata:
  name: ecommerce-daily-backup
  namespace: velero
spec:
  schedule: "0 2 * * *"  # Daily at 2 AM
  template:
    includedNamespaces:
    - ecommerce
    - istio-system
    includedResources:
    - "*"
    excludedResources:
    - events
    - events.events.k8s.io
    storageLocation: primary-backup-location
    volumeSnapshotLocations:
    - primary-volume-snapshots
    ttl: 720h0m0s  # 30 days retention
---
apiVersion: velero.io/v1
kind: BackupStorageLocation
metadata:
  name: primary-backup-location
  namespace: velero
spec:
  provider: aws
  objectStorage:
    bucket: ecommerce-k8s-backups
    prefix: primary-cluster
  config:
    region: us-east-1
    s3ForcePathStyle: "false"
```

#### Disaster Recovery Script

```bash
#!/bin/bash
# disaster-recovery/failover.sh - Automated disaster recovery

set -e

PRIMARY_CLUSTER="primary-us-east-1"
SECONDARY_CLUSTER="secondary-us-west-2"
NAMESPACE="ecommerce"
MONITORING_ENDPOINT="https://monitoring.company.com/api/v1/query"

# Function to check cluster health
check_cluster_health() {
    local cluster=$1
    local context="kubernetes-admin@$cluster"
    
    echo "Checking health of cluster $cluster..."
    
    # Switch to cluster context
    kubectl config use-context $context
    
    # Check API server
    if ! kubectl get nodes >/dev/null 2>&1; then
        echo "❌ API server not responding in $cluster"
        return 1
    fi
    
    # Check critical pods
    unhealthy_pods=$(kubectl get pods -n $NAMESPACE --field-selector=status.phase!=Running --no-headers 2>/dev/null | wc -l)
    if [ $unhealthy_pods -gt 0 ]; then
        echo "❌ $unhealthy_pods unhealthy pods in $cluster"
        return 1
    fi
    
    # Check service endpoints
    services=("user-service" "product-service" "order-service")
    for service in "${services[@]}"; do
        if ! kubectl get endpoints $service -n $NAMESPACE >/dev/null 2>&1; then
            echo "❌ Service $service has no endpoints in $cluster"
            return 1
        fi
    done
    
    echo "✅ Cluster $cluster is healthy"
    return 0
}

# Function to check application health
check_application_health() {
    local cluster=$1
    echo "Checking application health in $cluster..."
    
    # Get ingress IP
    ingress_ip=$(kubectl get service istio-ingressgateway -n istio-system -o jsonpath='{.status.loadBalancer.ingress[0].ip}')
    
    # Health check endpoints
    endpoints=("/health" "/api/users/health" "/api/products/health" "/api/orders/health")
    
    for endpoint in "${endpoints[@]}"; do
        if ! curl -f -m 10 "http://$ingress_ip$endpoint" >/dev/null 2>&1; then
            echo "❌ Health check failed for $endpoint in $cluster"
            return 1
        fi
    done
    
    echo "✅ Application is healthy in $cluster"
    return 0
}

# Function to get metrics from monitoring system
get_error_rate() {
    local cluster=$1
    local query="sum(rate(http_requests_total{cluster=\"$cluster\",status=~\"5..\"}[5m])) / sum(rate(http_requests_total{cluster=\"$cluster\"}[5m])) * 100"
    
    error_rate=$(curl -s "$MONITORING_ENDPOINT?query=$query" | jq -r '.data.result[0].value[1] // 0')
    echo $error_rate
}

# Function to trigger failover
trigger_failover() {
    echo "🚨 TRIGGERING FAILOVER FROM $PRIMARY_CLUSTER TO $SECONDARY_CLUSTER"
    
    # Switch to secondary cluster
    kubectl config use-context "kubernetes-admin@$SECONDARY_CLUSTER"
    
    # Update DNS to point to secondary cluster
    echo "Updating DNS records..."
    # This would use your DNS provider's API
    # aws route53 change-resource-record-sets --hosted-zone-id $HOSTED_ZONE_ID --change-batch file://dns-failover.json
    
    # Scale up secondary cluster services
    echo "Scaling up services in secondary cluster..."
    kubectl scale deployment user-service --replicas=5 -n $NAMESPACE
    kubectl scale deployment product-service --replicas=5 -n $NAMESPACE
    kubectl scale deployment order-service --replicas=3 -n $NAMESPACE
    
    # Wait for pods to be ready
    kubectl wait --for=condition=available --timeout=300s deployment/user-service -n $NAMESPACE
    kubectl wait --for=condition=available --timeout=300s deployment/product-service -n $NAMESPACE
    kubectl wait --for=condition=available --timeout=300s deployment/order-service -n $NAMESPACE
    
    # Update monitoring alerts
    echo "Updating monitoring configuration..."
    # Update alert manager configuration to point to secondary cluster
    
    # Send notifications
    echo "Sending failover notifications..."
    # Send to Slack, PagerDuty, etc.
    
    echo "✅ Failover completed successfully"
}

# Function to restore from backup
restore_from_backup() {
    local target_cluster=$1
    local backup_name=$2
    
    echo "Restoring from backup $backup_name to cluster $target_cluster..."
    
    kubectl config use-context "kubernetes-admin@$target_cluster"
    
    # Create restore job
    cat <<EOF | kubectl apply -f -
apiVersion: velero.io/v1
kind: Restore
metadata:
  name: disaster-recovery-restore-$(date +%s)
  namespace: velero
spec:
  backupName: $backup_name
  includedNamespaces:
  - ecommerce
  restorePVs: true
  preserveNodePorts: false
EOF
    
    # Wait for restore to complete
    echo "Waiting for restore to complete..."
    kubectl wait --for=condition=completed --timeout=600s restore/disaster-recovery-restore-* -n velero
    
    echo "✅ Restore completed"
}

# Main disaster recovery logic
main() {
    echo "Starting disaster recovery assessment..."
    
    # Check primary cluster health
    if check_cluster_health $PRIMARY_CLUSTER && check_application_health $PRIMARY_CLUSTER; then
        echo "✅ Primary cluster is healthy, no action needed"
        exit 0
    fi
    
    echo "❌ Primary cluster issues detected"
    
    # Get current error rate
    error_rate=$(get_error_rate $PRIMARY_CLUSTER)
    echo "Current error rate: $error_rate%"
    
    # Check if error rate exceeds threshold
    if (( $(echo "$error_rate > 5.0" | bc -l) )); then
        echo "🚨 Error rate exceeds threshold (5%), initiating failover"
    fi
    
    # Check secondary cluster readiness
    if ! check_cluster_health $SECONDARY_CLUSTER; then
        echo "❌ Secondary cluster not available, attempting restore"
        
        # Get latest backup
        latest_backup=$(kubectl get backups -n velero --sort-by=.metadata.creationTimestamp -o jsonpath='{.items[-1].metadata.name}')
        restore_from_backup $SECONDARY_CLUSTER $latest_backup
    fi
    
    # Perform failover
    trigger_failover
    
    # Verify failover success
    sleep 30
    if check_application_health $SECONDARY_CLUSTER; then
        echo "✅ Failover successful, secondary cluster is serving traffic"
    else
        echo "❌ Failover failed, manual intervention required"
        exit 1
    fi
}

# Monitor mode - continuous health checking
monitor_mode() {
    echo "Starting continuous monitoring mode..."
    
    while true; do
        if ! check_cluster_health $PRIMARY_CLUSTER || ! check_application_health $PRIMARY_CLUSTER; then
            echo "Issues detected, running full disaster recovery assessment"
            main
            break
        fi
        
        echo "All systems healthy - $(date)"
        sleep 60
    done
}

# Handle command line arguments
case "${1:-monitor}" in
    "monitor")
        monitor_mode
        ;;
    "failover")
        trigger_failover
        ;;
    "restore")
        restore_from_backup $2 $3
        ;;
    "check")
        check_cluster_health $PRIMARY_CLUSTER && check_application_health $PRIMARY_CLUSTER
        ;;
    *)
        echo "Usage: $0 {monitor|failover|restore|check}"
        exit 1
        ;;
esac
```

---

## Production Best Practices

### Security Best Practices

#### 1. Container Security

```dockerfile
# Security-hardened Dockerfile
FROM node:18-alpine AS base

# Create app directory and non-root user
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nodejs -u 1001 -G nodejs && \
    mkdir -p /app && \
    chown -R nodejs:nodejs /app

WORKDIR /app

# Install security updates
RUN apk update && apk upgrade && \
    apk add --no-cache dumb-init && \
    rm -rf /var/cache/apk/*

# Development stage
FROM base AS development
USER nodejs
COPY --chown=nodejs:nodejs package*.json ./
RUN npm ci --only=development
COPY --chown=nodejs:nodejs . .
CMD ["dumb-init", "npm", "run", "dev"]

# Production build stage
FROM base AS build
USER nodejs
COPY --chown=nodejs:nodejs package*.json ./
RUN npm ci --only=production && npm cache clean --force
COPY --chown=nodejs:nodejs . .
RUN npm run build

# Production runtime stage
FROM node:18-alpine AS production

# Security: Run as non-root user
RUN addgroup -g 1001 -S nodejs && \
    adduser -S nodejs -u 1001 -G nodejs

# Security: Install only necessary packages and security updates
RUN apk update && apk upgrade && \
    apk add --no-cache dumb-init && \
    rm -rf /var/cache/apk/*

WORKDIR /app

# Copy built application
COPY --from=build --chown=nodejs:nodejs /app/dist ./dist
COPY --from=build --chown=nodejs:nodejs /app/node_modules ./node_modules
COPY --from=build --chown=nodejs:nodejs /app/package*.json ./

# Security: Remove unnecessary files
RUN rm -rf /tmp/* /var/tmp/* /usr/share/man /usr/share/doc

# Security: Set proper permissions
RUN chmod -R 755 /app && \
    chmod 644 /app/package*.json

USER nodejs

# Security: Use dumb-init to handle signals properly
ENTRYPOINT ["dumb-init", "--"]
CMD ["node", "dist/index.js"]

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:3000/health || exit 1

# Security: Expose only necessary port
EXPOSE 3000
```

#### 2. Kubernetes Security Policies

```yaml
# security/network-policies.yaml
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: ecommerce-network-policy
  namespace: ecommerce
spec:
  podSelector: {}
  policyTypes:
  - Ingress
  - Egress
  ingress:
  - from:
    - namespaceSelector:
        matchLabels:
          name: istio-system
    - namespaceSelector:
        matchLabels:
          name: ecommerce
  - from: []
    ports:
    - protocol: TCP
      port: 3000
    - protocol: TCP
      port: 3001
    - protocol: TCP
      port: 3002
  egress:
  - to:
    - namespaceSelector:
        matchLabels:
          name: kube-system
    ports:
    - protocol: UDP
      port: 53  # DNS
  - to:
    - namespaceSelector:
        matchLabels:
          name: ecommerce
  - to: []
    ports:
    - protocol: TCP
      port: 443  # HTTPS
    - protocol: TCP
      port: 5432  # PostgreSQL
    - protocol: TCP
      port: 6379  # Redis
---
apiVersion: policy/v1beta1
kind: PodSecurityPolicy
metadata:
  name: ecommerce-psp
spec:
  privileged: false
  allowPrivilegeEscalation: false
  requiredDropCapabilities:
    - ALL
  volumes:
    - 'configMap'
    - 'emptyDir'
    - 'projected'
    - 'secret'
    - 'downwardAPI'
    - 'persistentVolumeClaim'
  runAsUser:
    rule: 'MustRunAsNonRoot'
  seLinux:
    rule: 'RunAsAny'
  fsGroup:
    rule: 'RunAsAny'
```

#### 3. Secret Management with External Secrets Operator

```yaml
# security/external-secrets.yaml
apiVersion: external-secrets.io/v1beta1
kind: SecretStore
metadata:
  name: aws-secrets-manager
  namespace: ecommerce
spec:
  provider:
    aws:
      service: SecretsManager
      region: us-east-1
      auth:
        jwt:
          serviceAccountRef:
            name: external-secrets-sa
---
apiVersion: external-secrets.io/v1beta1
kind: ExternalSecret
metadata:
  name: database-credentials
  namespace: ecommerce
spec:
  refreshInterval: 1h
  secretStoreRef:
    name: aws-secrets-manager
    kind: SecretStore
  target:
    name: database-secret
    creationPolicy: Owner
  data:
  - secretKey: postgres-password
    remoteRef:
      key: ecommerce/database
      property: password
  - secretKey: mongodb-password
    remoteRef:
      key: ecommerce/mongodb
      property: password
```

### Monitoring and Observability

#### 1. Prometheus Monitoring Setup

```yaml
# monitoring/prometheus-config.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: prometheus-config
  namespace: monitoring
data:
  prometheus.yml: |
    global:
      scrape_interval: 15s
      evaluation_interval: 15s
    
    rule_files:
      - "/etc/prometheus/rules/*.yml"
    
    alerting:
      alertmanagers:
        - static_configs:
            - targets:
              - alertmanager:9093
    
    scrape_configs:
      # Kubernetes API server
      - job_name: 'kubernetes-apiservers'
        kubernetes_sd_configs:
        - role: endpoints
        scheme: https
        tls_config:
          ca_file: /var/run/secrets/kubernetes.io/serviceaccount/ca.crt
        bearer_token_file: /var/run/secrets/kubernetes.io/serviceaccount/token
        relabel_configs:
        - source_labels: [__meta_kubernetes_namespace, __meta_kubernetes_service_name, __meta_kubernetes_endpoint_port_name]
          action: keep
          regex: default;kubernetes;https
      
      # Kubernetes nodes
      - job_name: 'kubernetes-nodes'
        kubernetes_sd_configs:
        - role: node
        scheme: https
        tls_config:
          ca_file: /var/run/secrets/kubernetes.io/serviceaccount/ca.crt
        bearer_token_file: /var/run/secrets/kubernetes.io/serviceaccount/token
        relabel_configs:
        - action: labelmap
          regex: __meta_kubernetes_node_label_(.+)
      
      # Application services
      - job_name: 'ecommerce-services'
        kubernetes_sd_configs:
        - role: endpoints
          namespaces:
            names:
            - ecommerce
        relabel_configs:
        - source_labels: [__meta_kubernetes_service_annotation_prometheus_io_scrape]
          action: keep
          regex: true
        - source_labels: [__meta_kubernetes_service_annotation_prometheus_io_path]
          action: replace
          target_label: __metrics_path__
          regex: (.+)
        - source_labels: [__address__, __meta_kubernetes_service_annotation_prometheus_io_port]
          action: replace
          regex: ([^:]+)(?::\d+)?;(\d+)
          replacement: $1:$2
          target_label: __address__
```

#### 2. Custom Metrics and Alerts

```yaml
# monitoring/alerts.yaml
apiVersion: monitoring.coreos.com/v1
kind: PrometheusRule
metadata:
  name: ecommerce-alerts
  namespace: ecommerce
spec:
  groups:
  - name: ecommerce.rules
    rules:
    # High error rate alert
    - alert: HighErrorRate
      expr: |
        (
          sum(rate(http_requests_total{job="ecommerce-services",status=~"5.."}[5m])) /
          sum(rate(http_requests_total{job="ecommerce-services"}[5m]))
        ) * 100 > 5
      for: 2m
      labels:
        severity: critical
      annotations:
        summary: "High error rate detected"
        description: "Error rate is {{ $value }}% for the last 5 minutes"
    
    # High response time alert
    - alert: HighResponseTime
      expr: |
        histogram_quantile(0.95,
          sum(rate(http_request_duration_seconds_bucket{job="ecommerce-services"}[5m])) by (le)
        ) > 1
      for: 5m
      labels:
        severity: warning
      annotations:
        summary: "High response time detected"
        description: "95th percentile response time is {{ $value }}s"
    
    # Pod restart alert
    - alert: PodRestartingTooMuch
      expr: |
        increase(kube_pod_container_status_restarts_total{namespace="ecommerce"}[1h]) > 3
      for: 0m
      labels:
        severity: warning
      annotations:
        summary: "Pod restarting too much"
        description: "Pod {{ $labels.pod }} in namespace {{ $labels.namespace }} has restarted {{ $value }} times in the last hour"
    
    # Database connection alert
    - alert: DatabaseConnectionHigh
      expr: |
        sum(database_connections_active) by (database) > 80
      for: 5m
      labels:
        severity: warning
      annotations:
        summary: "High database connections"
        description: "Database {{ $labels.database }} has {{ $value }} active connections"
```

#### 3. Grafana Dashboards

```json
{
  "dashboard": {
    "id": null,
    "title": "E-commerce Microservices Dashboard",
    "tags": ["ecommerce", "microservices"],
    "timezone": "browser",
    "panels": [
      {
        "id": 1,
        "title": "Request Rate",
        "type": "graph",
        "targets": [
          {
            "expr": "sum(rate(http_requests_total{job=\"ecommerce-services\"}[5m])) by (service)",
            "legendFormat": "{{service}}"
          }
        ],
        "yAxes": [
          {
            "label": "Requests per second"
          }
        ]
      },
      {
        "id": 2,
        "title": "Error Rate",
        "type": "singlestat",
        "targets": [
          {
            "expr": "(sum(rate(http_requests_total{job=\"ecommerce-services\",status=~\"5..\"}[5m])) / sum(rate(http_requests_total{job=\"ecommerce-services\"}[5m]))) * 100"
          }
        ],
        "valueName": "current",
        "format": "percent",
        "thresholds": [
          {
            "value": 1,
            "color": "green"
          },
          {
            "value": 5,
            "color": "yellow"
          },
          {
            "value": 10,
            "color": "red"
          }
        ]
      },
      {
        "id": 3,
        "title": "Response Time (95th percentile)",
        "type": "graph",
        "targets": [
          {
            "expr": "histogram_quantile(0.95, sum(rate(http_request_duration_seconds_bucket{job=\"ecommerce-services\"}[5m])) by (service, le))",
            "legendFormat": "{{service}}"
          }
        ]
      },
      {
        "id": 4,
        "title": "Pod Status",
        "type": "table",
        "targets": [
          {
            "expr": "kube_pod_status_phase{namespace=\"ecommerce\"}",
            "format": "table"
          }
        ]
      }
    ],
    "time": {
      "from": "now-1h",
      "to": "now"
    },
    "refresh": "30s"
  }
}
```

### Performance Optimization

#### 1. Resource Management

```yaml
# performance/resource-quotas.yaml
apiVersion: v1
kind: ResourceQuota
metadata:
  name: ecommerce-quota
  namespace: ecommerce
spec:
  hard:
    # Compute resources
    requests.cpu: "20"
    requests.memory: 40Gi
    limits.cpu: "40"
    limits.memory: 80Gi
    
    # Storage resources
    persistentvolumeclaims: "10"
    requests.storage: 100Gi
    
    # Object counts
    pods: "50"
    services: "20"
    secrets: "20"
    configmaps: "20"
    
    # Load balancers
    services.loadbalancers: "5"
    services.nodeports: "10"
---
apiVersion: v1
kind: LimitRange
metadata:
  name: ecommerce-limits
  namespace: ecommerce
spec:
  limits:
  - type: Container
    default:
      cpu: "500m"
      memory: "512Mi"
    defaultRequest:
      cpu: "100m"
      memory: "128Mi"
    max:
      cpu: "2"
      memory: "2Gi"
    min:
      cpu: "50m"
      memory: "64Mi"
  - type: Pod
    max:
      cpu: "4"
      memory: "4Gi"
```

#### 2. Caching Strategy

```yaml
# performance/redis-cluster.yaml
apiVersion: apps/v1
kind: StatefulSet
metadata:
  name: redis-cluster
  namespace: ecommerce
spec:
  serviceName: redis-cluster
  replicas: 6
  selector:
    matchLabels:
      app: redis-cluster
  template:
    metadata:
      labels:
        app: redis-cluster
    spec:
      containers:
      - name: redis
        image: redis:7-alpine
        ports:
        - containerPort: 6379
        - containerPort: 16379
        command:
        - redis-server
        args:
        - /etc/redis/redis.conf
        - --cluster-enabled
        - "yes"
        - --cluster-config-file
        - /data/nodes.conf
        - --cluster-node-timeout
        - "5000"
        - --appendonly
        - "yes"
        volumeMounts:
        - name: redis-data
          mountPath: /data
        - name: redis-config
          mountPath: /etc/redis
        resources:
          requests:
            memory: "256Mi"
            cpu: "200m"
          limits:
            memory: "512Mi"
            cpu: "500m"
      volumes:
      - name: redis-config
        configMap:
          name: redis-config
  volumeClaimTemplates:
  - metadata:
      name: redis-data
    spec:
      accessModes: ["ReadWriteOnce"]
      resources:
        requests:
          storage: 5Gi
```

This comprehensive Docker and Kubernetes guide provides real-world scenarios, step-by-step implementations, and production-ready best practices for containerization and orchestration. Each section includes practical examples that you can adapt to your specific requirements.

---

*End of Docker & Kubernetes Complete Guide*