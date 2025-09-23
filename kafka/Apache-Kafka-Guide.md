# Apache Kafka Complete Guide
## Step-by-Step Real-World Scenarios & Implementation

### Table of Contents
1. [Kafka Fundamentals](#kafka-fundamentals)
2. [Setting Up Kafka Cluster](#setting-up-kafka-cluster)
3. [Real-World Scenario 1: E-commerce Order Processing](#real-world-scenario-1-e-commerce-order-processing)
4. [Real-World Scenario 2: User Activity Tracking](#real-world-scenario-2-user-activity-tracking)
5. [Real-World Scenario 3: Microservices Communication](#real-world-scenario-3-microservices-communication)
6. [Real-World Scenario 4: Real-time Analytics & Event Streaming](#real-world-scenario-4-real-time-analytics--event-streaming)
7. [Kafka with .NET Core Implementation](#kafka-with-net-core-implementation)
8. [Production Deployment & Monitoring](#production-deployment--monitoring)
9. [Performance Optimization & Best Practices](#performance-optimization--best-practices)

---

## Kafka Fundamentals

### What is Apache Kafka?
Apache Kafka is a distributed event streaming platform designed for high-throughput, fault-tolerant, real-time data processing. It acts as a message broker that can handle millions of events per second.

**Key Concepts:**
- **Producer**: Applications that send/publish data to Kafka topics
- **Consumer**: Applications that read/subscribe to data from Kafka topics
- **Topic**: Category or feed name where records are published
- **Partition**: Topics are split into partitions for scalability and parallelism
- **Broker**: Kafka server that stores data and serves clients
- **Cluster**: Group of Kafka brokers working together
- **Offset**: Unique sequential ID assigned to each record in a partition
- **Consumer Group**: Group of consumers that work together to consume a topic

### Kafka Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Kafka Cluster                           │
│                                                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │   Broker 1  │  │   Broker 2  │  │   Broker 3  │         │
│  │             │  │             │  │             │         │
│  │ Topic A     │  │ Topic A     │  │ Topic B     │         │
│  │ Partition 0 │  │ Partition 1 │  │ Partition 0 │         │
│  │ Partition 1 │  │ Partition 2 │  │ Partition 1 │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
└─────────────────────────────────────────────────────────────┘
         ▲                                    │
         │ Publish                            │ Subscribe
    ┌─────────┐                         ┌──────────┐
    │Producer │                         │Consumer  │
    │         │                         │Group     │
    └─────────┘                         └──────────┘
```

---

## Setting Up Kafka Cluster

### Step 1: Docker Compose Setup for Development

**Business Context**: Setting up a complete Kafka environment for development and testing with multiple brokers, ZooKeeper, and management tools.

```yaml
# docker-compose.yml - Complete Kafka development environment
version: '3.8'

services:
  # ZooKeeper - Coordination service for Kafka cluster
  zookeeper:
    image: confluentinc/cp-zookeeper:7.4.0
    hostname: zookeeper
    container_name: zookeeper
    ports:
      - "2181:2181"  # ZooKeeper client port
    environment:
      # ZooKeeper server ID - unique in the ensemble
      ZOOKEEPER_SERVER_ID: 1
      # Client port for ZooKeeper connections
      ZOOKEEPER_CLIENT_PORT: 2181
      # How often to take snapshots of in-memory database
      ZOOKEEPER_TICK_TIME: 2000
      # Time ZooKeeper will wait for initial connection from followers
      ZOOKEEPER_INIT_LIMIT: 5
      # Time between sending a request and getting acknowledgment from follower
      ZOOKEEPER_SYNC_LIMIT: 2
    volumes:
      - zookeeper-data:/var/lib/zookeeper/data
      - zookeeper-logs:/var/lib/zookeeper/log
    networks:
      - kafka-network
    restart: unless-stopped

  # Kafka Broker 1
  kafka-broker-1:
    image: confluentinc/cp-kafka:7.4.0
    hostname: kafka-broker-1
    container_name: kafka-broker-1
    depends_on:
      - zookeeper
    ports:
      - "9092:9092"  # External listener port
      - "19092:19092"  # JMX port for monitoring
    environment:
      # Unique broker ID in the cluster
      KAFKA_BROKER_ID: 1
      
      # ZooKeeper connection string
      KAFKA_ZOOKEEPER_CONNECT: 'zookeeper:2181'
      
      # Listener configuration for different network interfaces
      # PLAINTEXT - unencrypted communication
      # PLAINTEXT_HOST - for external connections from host machine
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka-broker-1:29092,PLAINTEXT_HOST://localhost:9092
      KAFKA_LISTENERS: PLAINTEXT://0.0.0.0:29092,PLAINTEXT_HOST://0.0.0.0:9092
      
      # Inter-broker communication listener
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
      
      # Replication settings
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 3  # Minimum 3 for production
      KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR: 3
      KAFKA_TRANSACTION_STATE_LOG_MIN_ISR: 2  # In-Sync Replicas minimum
      
      # Log settings
      KAFKA_LOG_RETENTION_HOURS: 168  # 7 days retention
      KAFKA_LOG_RETENTION_BYTES: 1073741824  # 1GB per partition
      KAFKA_LOG_SEGMENT_BYTES: 1073741824  # 1GB segment size
      
      # Performance settings
      KAFKA_NUM_NETWORK_THREADS: 8  # Network threads for handling requests
      KAFKA_NUM_IO_THREADS: 8  # I/O threads for disk operations
      KAFKA_SOCKET_SEND_BUFFER_BYTES: 102400  # 100KB send buffer
      KAFKA_SOCKET_RECEIVE_BUFFER_BYTES: 102400  # 100KB receive buffer
      KAFKA_SOCKET_REQUEST_MAX_BYTES: 104857600  # 100MB max request size
      
      # JMX monitoring
      KAFKA_JMX_PORT: 19092
      KAFKA_JMX_HOSTNAME: localhost
      
      # Group coordinator settings
      KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS: 0
      
      # Auto topic creation (disable in production)
      KAFKA_AUTO_CREATE_TOPICS_ENABLE: 'true'
      KAFKA_DEFAULT_REPLICATION_FACTOR: 3
      KAFKA_NUM_PARTITIONS: 6  # Default partitions for auto-created topics
      
    volumes:
      - kafka-broker-1-data:/var/lib/kafka/data
    networks:
      - kafka-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "kafka-broker-api-versions", "--bootstrap-server", "localhost:9092"]
      start_period: 60s
      interval: 30s
      timeout: 10s
      retries: 3

  # Kafka Broker 2
  kafka-broker-2:
    image: confluentinc/cp-kafka:7.4.0
    hostname: kafka-broker-2
    container_name: kafka-broker-2
    depends_on:
      - zookeeper
    ports:
      - "9093:9093"
      - "19093:19093"
    environment:
      KAFKA_BROKER_ID: 2  # Different broker ID
      KAFKA_ZOOKEEPER_CONNECT: 'zookeeper:2181'
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka-broker-2:29093,PLAINTEXT_HOST://localhost:9093
      KAFKA_LISTENERS: PLAINTEXT://0.0.0.0:29093,PLAINTEXT_HOST://0.0.0.0:9093
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 3
      KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR: 3
      KAFKA_TRANSACTION_STATE_LOG_MIN_ISR: 2
      KAFKA_LOG_RETENTION_HOURS: 168
      KAFKA_LOG_RETENTION_BYTES: 1073741824
      KAFKA_LOG_SEGMENT_BYTES: 1073741824
      KAFKA_NUM_NETWORK_THREADS: 8
      KAFKA_NUM_IO_THREADS: 8
      KAFKA_SOCKET_SEND_BUFFER_BYTES: 102400
      KAFKA_SOCKET_RECEIVE_BUFFER_BYTES: 102400
      KAFKA_SOCKET_REQUEST_MAX_BYTES: 104857600
      KAFKA_JMX_PORT: 19093
      KAFKA_JMX_HOSTNAME: localhost
      KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS: 0
      KAFKA_AUTO_CREATE_TOPICS_ENABLE: 'true'
      KAFKA_DEFAULT_REPLICATION_FACTOR: 3
      KAFKA_NUM_PARTITIONS: 6
    volumes:
      - kafka-broker-2-data:/var/lib/kafka/data
    networks:
      - kafka-network
    restart: unless-stopped

  # Kafka Broker 3
  kafka-broker-3:
    image: confluentinc/cp-kafka:7.4.0
    hostname: kafka-broker-3
    container_name: kafka-broker-3
    depends_on:
      - zookeeper
    ports:
      - "9094:9094"
      - "19094:19094"
    environment:
      KAFKA_BROKER_ID: 3  # Third unique broker ID
      KAFKA_ZOOKEEPER_CONNECT: 'zookeeper:2181'
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://kafka-broker-3:29094,PLAINTEXT_HOST://localhost:9094
      KAFKA_LISTENERS: PLAINTEXT://0.0.0.0:29094,PLAINTEXT_HOST://0.0.0.0:9094
      KAFKA_INTER_BROKER_LISTENER_NAME: PLAINTEXT
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 3
      KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR: 3
      KAFKA_TRANSACTION_STATE_LOG_MIN_ISR: 2
      KAFKA_LOG_RETENTION_HOURS: 168
      KAFKA_LOG_RETENTION_BYTES: 1073741824
      KAFKA_LOG_SEGMENT_BYTES: 1073741824
      KAFKA_NUM_NETWORK_THREADS: 8
      KAFKA_NUM_IO_THREADS: 8
      KAFKA_SOCKET_SEND_BUFFER_BYTES: 102400
      KAFKA_SOCKET_RECEIVE_BUFFER_BYTES: 102400
      KAFKA_SOCKET_REQUEST_MAX_BYTES: 104857600
      KAFKA_JMX_PORT: 19094
      KAFKA_JMX_HOSTNAME: localhost
      KAFKA_GROUP_INITIAL_REBALANCE_DELAY_MS: 0
      KAFKA_AUTO_CREATE_TOPICS_ENABLE: 'true'
      KAFKA_DEFAULT_REPLICATION_FACTOR: 3
      KAFKA_NUM_PARTITIONS: 6
    volumes:
      - kafka-broker-3-data:/var/lib/kafka/data
    networks:
      - kafka-network
    restart: unless-stopped

  # Kafka UI for management and monitoring
  kafka-ui:
    image: provectuslabs/kafka-ui:latest
    container_name: kafka-ui
    depends_on:
      - kafka-broker-1
      - kafka-broker-2
      - kafka-broker-3
    ports:
      - "8080:8080"
    environment:
      # Kafka UI configuration
      KAFKA_CLUSTERS_0_NAME: local-cluster
      KAFKA_CLUSTERS_0_BOOTSTRAPSERVERS: kafka-broker-1:29092,kafka-broker-2:29093,kafka-broker-3:29094
      KAFKA_CLUSTERS_0_ZOOKEEPER: zookeeper:2181
      # Enable JMX monitoring
      KAFKA_CLUSTERS_0_JMXPORT: 19092
      # Authentication (optional)
      AUTH_TYPE: "disabled"
      # Logging level
      LOGGING_LEVEL_COM_PROVECTUS: DEBUG
    networks:
      - kafka-network
    restart: unless-stopped

  # Schema Registry for Avro/JSON schema management
  schema-registry:
    image: confluentinc/cp-schema-registry:7.4.0
    hostname: schema-registry
    container_name: schema-registry
    depends_on:
      - kafka-broker-1
      - kafka-broker-2
      - kafka-broker-3
    ports:
      - "8081:8081"
    environment:
      # Schema Registry host name
      SCHEMA_REGISTRY_HOST_NAME: schema-registry
      # Kafka brokers for storing schemas
      SCHEMA_REGISTRY_KAFKASTORE_BOOTSTRAP_SERVERS: 'kafka-broker-1:29092,kafka-broker-2:29093,kafka-broker-3:29094'
      # Listeners for client connections
      SCHEMA_REGISTRY_LISTENERS: http://0.0.0.0:8081
      # Topic for storing schemas
      SCHEMA_REGISTRY_KAFKASTORE_TOPIC: _schemas
      SCHEMA_REGISTRY_KAFKASTORE_TOPIC_REPLICATION_FACTOR: 3
      # Debug settings
      SCHEMA_REGISTRY_DEBUG: 'true'
    networks:
      - kafka-network
    restart: unless-stopped

  # Kafka Connect for data integration
  kafka-connect:
    image: confluentinc/cp-kafka-connect:7.4.0
    hostname: kafka-connect
    container_name: kafka-connect
    depends_on:
      - kafka-broker-1
      - kafka-broker-2
      - kafka-broker-3
      - schema-registry
    ports:
      - "8083:8083"
    environment:
      # Connect configuration
      CONNECT_BOOTSTRAP_SERVERS: 'kafka-broker-1:29092,kafka-broker-2:29093,kafka-broker-3:29094'
      CONNECT_REST_ADVERTISED_HOST_NAME: kafka-connect
      CONNECT_REST_PORT: 8083
      CONNECT_GROUP_ID: compose-connect-group
      
      # Topic configurations
      CONNECT_CONFIG_STORAGE_TOPIC: docker-connect-configs
      CONNECT_OFFSET_STORAGE_TOPIC: docker-connect-offsets
      CONNECT_STATUS_STORAGE_TOPIC: docker-connect-status
      
      # Replication factors
      CONNECT_CONFIG_STORAGE_REPLICATION_FACTOR: 3
      CONNECT_OFFSET_STORAGE_REPLICATION_FACTOR: 3
      CONNECT_STATUS_STORAGE_REPLICATION_FACTOR: 3
      
      # Converters for data serialization
      CONNECT_KEY_CONVERTER: org.apache.kafka.connect.storage.StringConverter
      CONNECT_VALUE_CONVERTER: io.confluent.connect.avro.AvroConverter
      CONNECT_VALUE_CONVERTER_SCHEMA_REGISTRY_URL: http://schema-registry:8081
      
      # Plugin path for connectors
      CONNECT_PLUGIN_PATH: "/usr/share/java,/usr/share/confluent-hub-components"
      
      # Log settings
      CONNECT_LOG4J_LOGGERS: org.apache.zookeeper=ERROR,org.I0Itec.zkclient=ERROR,org.reflections=ERROR
    volumes:
      - kafka-connect-data:/usr/share/confluent-hub-components
    networks:
      - kafka-network
    restart: unless-stopped

# Named volumes for data persistence
volumes:
  zookeeper-data:
    driver: local
  zookeeper-logs:
    driver: local
  kafka-broker-1-data:
    driver: local
  kafka-broker-2-data:
    driver: local
  kafka-broker-3-data:
    driver: local
  kafka-connect-data:
    driver: local

# Custom network for service communication
networks:
  kafka-network:
    driver: bridge
    ipam:
      config:
        - subnet: 172.20.0.0/16
```

### Step 2: Starting the Kafka Cluster

```bash
# Step 1: Create the environment
echo "Starting Kafka cluster setup..."

# Create project directory
mkdir kafka-demo
cd kafka-demo

# Copy the docker-compose.yml file above to this directory

# Step 2: Start the cluster
echo "Starting Kafka cluster with 3 brokers..."
docker-compose up -d

# Step 3: Wait for services to be ready
echo "Waiting for services to initialize..."
sleep 60

# Step 4: Verify cluster health
echo "Checking cluster health..."
docker-compose ps

# Step 5: Check Kafka broker logs
echo "Checking Kafka broker logs..."
docker-compose logs kafka-broker-1 | tail -20

# Step 6: Test cluster connectivity
echo "Testing cluster connectivity..."
docker exec kafka-broker-1 kafka-topics --bootstrap-server localhost:9092 --list
```

### Step 3: Basic Kafka Operations

```bash
# Create a topic with multiple partitions and replication
echo "Creating topics for our demo..."

# E-commerce order topic
docker exec kafka-broker-1 kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic ecommerce-orders \
  --partitions 6 \
  --replication-factor 3 \
  --config cleanup.policy=compact \
  --config retention.ms=604800000

# User activity tracking topic
docker exec kafka-broker-1 kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic user-activity \
  --partitions 12 \
  --replication-factor 3 \
  --config retention.ms=259200000

# Payment processing topic
docker exec kafka-broker-1 kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic payment-events \
  --partitions 3 \
  --replication-factor 3 \
  --config cleanup.policy=delete \
  --config retention.ms=2592000000

# Inventory updates topic
docker exec kafka-broker-1 kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic inventory-updates \
  --partitions 4 \
  --replication-factor 3

# Email notifications topic
docker exec kafka-broker-1 kafka-topics --create \
  --bootstrap-server localhost:9092 \
  --topic email-notifications \
  --partitions 2 \
  --replication-factor 3

# List all topics
echo "Listing all topics..."
docker exec kafka-broker-1 kafka-topics --bootstrap-server localhost:9092 --list

# Describe a topic to see its configuration
echo "Describing ecommerce-orders topic..."
docker exec kafka-broker-1 kafka-topics --bootstrap-server localhost:9092 --describe --topic ecommerce-orders
```

---

## Real-World Scenario 1: E-commerce Order Processing

### Business Context
An e-commerce platform needs to process orders through multiple stages: order placement, inventory check, payment processing, shipping, and notifications. Each stage should be decoupled and scalable.

### Architecture Overview

```
Order Placement → [order-events] → Order Service
                ↓
         [inventory-check] → Inventory Service
                ↓
         [payment-events] → Payment Service
                ↓
         [shipping-events] → Shipping Service
                ↓
         [notification-events] → Notification Service
```

### Step 1: Order Event Producer (.NET Core)

```csharp
// Models/OrderModels.cs
using System.Text.Json.Serialization;

namespace EcommerceKafka.Models
{
    // Order placed event
    public class OrderPlacedEvent
    {
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;
        
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; } = string.Empty;
        
        [JsonPropertyName("items")]
        public List<OrderItem> Items { get; set; } = new();
        
        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }
        
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "USD";
        
        [JsonPropertyName("shippingAddress")]
        public Address ShippingAddress { get; set; } = new();
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = "OrderPlaced";
        
        [JsonPropertyName("version")]
        public string Version { get; set; } = "1.0";
    }

    public class OrderItem
    {
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;
        
        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;
        
        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;
        
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
        
        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }
    }

    public class Address
    {
        [JsonPropertyName("street")]
        public string Street { get; set; } = string.Empty;
        
        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;
        
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;
        
        [JsonPropertyName("zipCode")]
        public string ZipCode { get; set; } = string.Empty;
        
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;
    }

    // Inventory check event
    public class InventoryCheckEvent
    {
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;
        
        [JsonPropertyName("items")]
        public List<InventoryItem> Items { get; set; } = new();
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = "InventoryCheck";
    }

    public class InventoryItem
    {
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;
        
        [JsonPropertyName("sku")]
        public string Sku { get; set; } = string.Empty;
        
        [JsonPropertyName("requestedQuantity")]
        public int RequestedQuantity { get; set; }
        
        [JsonPropertyName("availableQuantity")]
        public int AvailableQuantity { get; set; }
        
        [JsonPropertyName("reserved")]
        public bool Reserved { get; set; }
    }

    // Payment processing event
    public class PaymentEvent
    {
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;
        
        [JsonPropertyName("paymentId")]
        public string PaymentId { get; set; } = string.Empty;
        
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "USD";
        
        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty; // Pending, Approved, Declined
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = "PaymentProcessed";
    }
}
```

```csharp
// Services/KafkaProducerService.cs
using Confluent.Kafka;
using System.Text.Json;
using EcommerceKafka.Models;

namespace EcommerceKafka.Services
{
    public interface IKafkaProducerService
    {
        Task PublishOrderPlacedAsync(OrderPlacedEvent orderEvent);
        Task PublishInventoryCheckAsync(InventoryCheckEvent inventoryEvent);
        Task PublishPaymentEventAsync(PaymentEvent paymentEvent);
    }

    public class KafkaProducerService : IKafkaProducerService, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
        {
            _logger = logger;
            
            // Configure JSON serialization options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            // Kafka producer configuration
            var config = new ProducerConfig
            {
                // Bootstrap servers - can be multiple for redundancy
                BootstrapServers = configuration.GetConnectionString("Kafka") ?? "localhost:9092,localhost:9093,localhost:9094",
                
                // Client identification
                ClientId = $"ecommerce-producer-{Environment.MachineName}-{Guid.NewGuid():N}",
                
                // Acknowledgment settings for reliability
                Acks = Acks.All, // Wait for all in-sync replicas to acknowledge
                
                // Retry settings
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 1000,
                
                // Batch settings for performance
                BatchSize = 16384, // 16KB batch size
                LingerMs = 5, // Wait 5ms to batch messages
                
                // Compression for efficiency
                CompressionType = CompressionType.Snappy,
                
                // Buffer settings
                QueueBufferingMaxMessages = 100000,
                QueueBufferingMaxKbytes = 1048576, // 1GB buffer
                
                // Timeout settings
                MessageTimeoutMs = 30000, // 30 seconds
                RequestTimeoutMs = 30000,
                
                // Idempotence for exactly-once semantics
                EnableIdempotence = true,
                MaxInFlight = 5, // Maximum unacknowledged requests
                
                // Error handling
                DeliveryReportFields = "all"
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => 
                {
                    _logger.LogError("Kafka producer error: {Error}", e.Reason);
                })
                .SetLogHandler((_, logMessage) => 
                {
                    var logLevel = logMessage.Level switch
                    {
                        SyslogLevel.Emergency or SyslogLevel.Alert or SyslogLevel.Critical or SyslogLevel.Error => LogLevel.Error,
                        SyslogLevel.Warning => LogLevel.Warning,
                        SyslogLevel.Notice or SyslogLevel.Info => LogLevel.Information,
                        SyslogLevel.Debug => LogLevel.Debug,
                        _ => LogLevel.Information
                    };
                    _logger.Log(logLevel, "Kafka producer log: {Message}", logMessage.Message);
                })
                .Build();
        }

        public async Task PublishOrderPlacedAsync(OrderPlacedEvent orderEvent)
        {
            try
            {
                // Serialize the event to JSON
                var message = JsonSerializer.Serialize(orderEvent, _jsonOptions);
                
                // Use order ID as key for partitioning - ensures all events for same order go to same partition
                var kafkaMessage = new Message<string, string>
                {
                    Key = orderEvent.OrderId,
                    Value = message,
                    Headers = new Headers
                    {
                        // Add metadata headers for message tracking and routing
                        { "eventType", System.Text.Encoding.UTF8.GetBytes(orderEvent.EventType) },
                        { "version", System.Text.Encoding.UTF8.GetBytes(orderEvent.Version) },
                        { "source", System.Text.Encoding.UTF8.GetBytes("order-service") },
                        { "contentType", System.Text.Encoding.UTF8.GetBytes("application/json") },
                        { "timestamp", System.Text.Encoding.UTF8.GetBytes(orderEvent.Timestamp.ToString("O")) }
                    }
                };

                // Publish to Kafka topic
                var deliveryResult = await _producer.ProduceAsync("ecommerce-orders", kafkaMessage);
                
                _logger.LogInformation(
                    "Order placed event published successfully. OrderId: {OrderId}, Topic: {Topic}, Partition: {Partition}, Offset: {Offset}",
                    orderEvent.OrderId,
                    deliveryResult.Topic,
                    deliveryResult.Partition.Value,
                    deliveryResult.Offset.Value
                );

                // Trigger inventory check after order is placed
                var inventoryCheckEvent = new InventoryCheckEvent
                {
                    OrderId = orderEvent.OrderId,
                    Items = orderEvent.Items.Select(item => new InventoryItem
                    {
                        ProductId = item.ProductId,
                        Sku = item.Sku,
                        RequestedQuantity = item.Quantity
                    }).ToList()
                };

                await PublishInventoryCheckAsync(inventoryCheckEvent);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, 
                    "Failed to publish order placed event. OrderId: {OrderId}, Error: {Error}",
                    orderEvent.OrderId, ex.Error.Reason);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Unexpected error publishing order placed event. OrderId: {OrderId}",
                    orderEvent.OrderId);
                throw;
            }
        }

        public async Task PublishInventoryCheckAsync(InventoryCheckEvent inventoryEvent)
        {
            try
            {
                var message = JsonSerializer.Serialize(inventoryEvent, _jsonOptions);
                
                var kafkaMessage = new Message<string, string>
                {
                    Key = inventoryEvent.OrderId, // Same key as order for consistency
                    Value = message,
                    Headers = new Headers
                    {
                        { "eventType", System.Text.Encoding.UTF8.GetBytes(inventoryEvent.EventType) },
                        { "source", System.Text.Encoding.UTF8.GetBytes("order-service") },
                        { "contentType", System.Text.Encoding.UTF8.GetBytes("application/json") },
                        { "timestamp", System.Text.Encoding.UTF8.GetBytes(inventoryEvent.Timestamp.ToString("O")) },
                        { "correlationId", System.Text.Encoding.UTF8.GetBytes(inventoryEvent.OrderId) }
                    }
                };

                var deliveryResult = await _producer.ProduceAsync("inventory-check", kafkaMessage);
                
                _logger.LogInformation(
                    "Inventory check event published. OrderId: {OrderId}, Partition: {Partition}, Offset: {Offset}",
                    inventoryEvent.OrderId,
                    deliveryResult.Partition.Value,
                    deliveryResult.Offset.Value
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to publish inventory check event. OrderId: {OrderId}",
                    inventoryEvent.OrderId);
                throw;
            }
        }

        public async Task PublishPaymentEventAsync(PaymentEvent paymentEvent)
        {
            try
            {
                var message = JsonSerializer.Serialize(paymentEvent, _jsonOptions);
                
                var kafkaMessage = new Message<string, string>
                {
                    Key = paymentEvent.OrderId,
                    Value = message,
                    Headers = new Headers
                    {
                        { "eventType", System.Text.Encoding.UTF8.GetBytes(paymentEvent.EventType) },
                        { "source", System.Text.Encoding.UTF8.GetBytes("payment-service") },
                        { "contentType", System.Text.Encoding.UTF8.GetBytes("application/json") },
                        { "timestamp", System.Text.Encoding.UTF8.GetBytes(paymentEvent.Timestamp.ToString("O")) },
                        { "correlationId", System.Text.Encoding.UTF8.GetBytes(paymentEvent.OrderId) }
                    }
                };

                var deliveryResult = await _producer.ProduceAsync("payment-events", kafkaMessage);
                
                _logger.LogInformation(
                    "Payment event published. OrderId: {OrderId}, PaymentId: {PaymentId}, Status: {Status}, Partition: {Partition}, Offset: {Offset}",
                    paymentEvent.OrderId,
                    paymentEvent.PaymentId,
                    paymentEvent.Status,
                    deliveryResult.Partition.Value,
                    deliveryResult.Offset.Value
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to publish payment event. OrderId: {OrderId}, PaymentId: {PaymentId}",
                    paymentEvent.OrderId, paymentEvent.PaymentId);
                throw;
            }
        }

        public void Dispose()
        {
            // Flush any pending messages
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }
    }
}
```

### Step 2: Order Controller - API Endpoint

```csharp
// Controllers/OrderController.cs
using Microsoft.AspNetCore.Mvc;
using EcommerceKafka.Models;
using EcommerceKafka.Services;

namespace EcommerceKafka.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IKafkaProducerService kafkaProducer, ILogger<OrderController> logger)
        {
            _kafkaProducer = kafkaProducer;
            _logger = logger;
        }

        // POST api/order/place
        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                // Validate request
                if (request == null || !request.Items.Any())
                {
                    return BadRequest("Order must contain at least one item");
                }

                // Generate unique order ID
                var orderId = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}";
                
                // Calculate total amount
                var totalAmount = request.Items.Sum(item => item.Quantity * item.UnitPrice);

                // Create order placed event
                var orderEvent = new OrderPlacedEvent
                {
                    OrderId = orderId,
                    CustomerId = request.CustomerId,
                    Items = request.Items.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Sku = item.Sku,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.Quantity * item.UnitPrice
                    }).ToList(),
                    TotalAmount = totalAmount,
                    Currency = request.Currency ?? "USD",
                    ShippingAddress = request.ShippingAddress,
                    Timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("Processing order placement. OrderId: {OrderId}, CustomerId: {CustomerId}, Total: {Total}",
                    orderId, request.CustomerId, totalAmount);

                // Publish order placed event to Kafka
                await _kafkaProducer.PublishOrderPlacedAsync(orderEvent);

                // Return order confirmation
                var response = new PlaceOrderResponse
                {
                    OrderId = orderId,
                    Status = "Placed",
                    Message = "Order has been placed successfully and is being processed",
                    TotalAmount = totalAmount,
                    EstimatedDelivery = DateTime.UtcNow.AddDays(5) // Business logic for delivery estimation
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order for customer {CustomerId}", request?.CustomerId);
                return StatusCode(500, "An error occurred while placing the order");
            }
        }

        // GET api/order/{orderId}/status
        [HttpGet("{orderId}/status")]
        public IActionResult GetOrderStatus(string orderId)
        {
            // In a real application, this would query a database or cache
            // For demo purposes, returning a static response
            var response = new OrderStatusResponse
            {
                OrderId = orderId,
                Status = "Processing",
                StatusHistory = new List<OrderStatusEvent>
                {
                    new() { Status = "Placed", Timestamp = DateTime.UtcNow.AddMinutes(-10) },
                    new() { Status = "Inventory Checked", Timestamp = DateTime.UtcNow.AddMinutes(-8) },
                    new() { Status = "Payment Processing", Timestamp = DateTime.UtcNow.AddMinutes(-5) }
                }
            };

            return Ok(response);
        }
    }

    // Request/Response DTOs
    public class PlaceOrderRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public List<OrderItemRequest> Items { get; set; } = new();
        public string? Currency { get; set; }
        public Address ShippingAddress { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class PlaceOrderResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime EstimatedDelivery { get; set; }
    }

    public class OrderStatusResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<OrderStatusEvent> StatusHistory { get; set; } = new();
    }

    public class OrderStatusEvent
    {
        public string Status { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
```

### Step 3: Inventory Service Consumer

```csharp
// Services/InventoryConsumerService.cs
using Confluent.Kafka;
using System.Text.Json;
using EcommerceKafka.Models;

namespace EcommerceKafka.Services
{
    public class InventoryConsumerService : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IKafkaProducerService _producer;
        private readonly ILogger<InventoryConsumerService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public InventoryConsumerService(
            IConfiguration configuration, 
            IKafkaProducerService producer,
            ILogger<InventoryConsumerService> logger)
        {
            _producer = producer;
            _logger = logger;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var config = new ConsumerConfig
            {
                // Kafka cluster connection
                BootstrapServers = configuration.GetConnectionString("Kafka") ?? "localhost:9092,localhost:9093,localhost:9094",
                
                // Consumer group configuration
                GroupId = "inventory-service-group",
                
                // Auto offset reset strategy
                AutoOffsetReset = AutoOffsetReset.Earliest,
                
                // Commit strategy - manual commit for better control
                EnableAutoCommit = false,
                
                // Session and heartbeat settings
                SessionTimeoutMs = 30000,
                HeartbeatIntervalMs = 10000,
                
                // Fetch settings for performance
                FetchMinBytes = 1024,
                FetchMaxWaitMs = 500,
                
                // Partition assignment strategy
                PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky,
                
                // Security settings (if needed)
                SecurityProtocol = SecurityProtocol.Plaintext,
                
                // Client identification
                ClientId = $"inventory-consumer-{Environment.MachineName}",
                
                // Error handling
                LogConnectionClose = false
            };

            _consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => 
                {
                    _logger.LogError("Kafka consumer error: {Error}", e.Reason);
                })
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    _logger.LogInformation("Partitions assigned: {Partitions}", 
                        string.Join(", ", partitions.Select(p => $"{p.Topic}-{p.Partition}")));
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    _logger.LogInformation("Partitions revoked: {Partitions}", 
                        string.Join(", ", partitions.Select(p => $"{p.Topic}-{p.Partition}")));
                })
                .SetPartitionsLostHandler((c, partitions) =>
                {
                    _logger.LogWarning("Partitions lost: {Partitions}", 
                        string.Join(", ", partitions.Select(p => $"{p.Topic}-{p.Partition}")));
                })
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield(); // Ensure this method returns a Task immediately
            
            _logger.LogInformation("Inventory consumer service started");
            
            // Subscribe to the inventory check topic
            _consumer.Subscribe("inventory-check");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        // Poll for messages with timeout
                        var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(1000));
                        
                        if (consumeResult == null)
                            continue;

                        _logger.LogInformation(
                            "Received inventory check message. Key: {Key}, Partition: {Partition}, Offset: {Offset}",
                            consumeResult.Message.Key,
                            consumeResult.Partition.Value,
                            consumeResult.Offset.Value
                        );

                        // Process the inventory check event
                        await ProcessInventoryCheckAsync(consumeResult.Message);

                        // Manually commit the offset after successful processing
                        _consumer.Commit(consumeResult);

                        _logger.LogDebug("Successfully processed and committed message at offset {Offset}", 
                            consumeResult.Offset.Value);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming message: {Error}", ex.Error.Reason);
                        
                        // Continue processing other messages
                        continue;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error processing inventory check message");
                        
                        // In production, you might want to:
                        // 1. Send to dead letter queue
                        // 2. Retry with backoff
                        // 3. Alert monitoring systems
                        
                        // For now, continue processing
                        continue;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Inventory consumer service stopping due to cancellation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in inventory consumer service");
            }
            finally
            {
                _consumer.Close();
                _consumer.Dispose();
                _logger.LogInformation("Inventory consumer service stopped");
            }
        }

        private async Task ProcessInventoryCheckAsync(Message<string, string> message)
        {
            try
            {
                // Deserialize the inventory check event
                var inventoryCheckEvent = JsonSerializer.Deserialize<InventoryCheckEvent>(message.Value, _jsonOptions);
                
                if (inventoryCheckEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize inventory check event");
                    return;
                }

                _logger.LogInformation("Processing inventory check for order: {OrderId}", inventoryCheckEvent.OrderId);

                // Simulate inventory checking logic
                var inventoryResults = new List<InventoryItem>();
                
                foreach (var item in inventoryCheckEvent.Items)
                {
                    // Simulate database lookup for inventory
                    var availableQuantity = await GetAvailableInventoryAsync(item.ProductId);
                    
                    var inventoryItem = new InventoryItem
                    {
                        ProductId = item.ProductId,
                        Sku = item.Sku,
                        RequestedQuantity = item.RequestedQuantity,
                        AvailableQuantity = availableQuantity,
                        Reserved = availableQuantity >= item.RequestedQuantity
                    };

                    inventoryResults.Add(inventoryItem);

                    if (inventoryItem.Reserved)
                    {
                        // Reserve the inventory
                        await ReserveInventoryAsync(item.ProductId, item.RequestedQuantity);
                        _logger.LogInformation(
                            "Reserved inventory. ProductId: {ProductId}, Quantity: {Quantity}",
                            item.ProductId, item.RequestedQuantity);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Insufficient inventory. ProductId: {ProductId}, Requested: {Requested}, Available: {Available}",
                            item.ProductId, item.RequestedQuantity, availableQuantity);
                    }
                }

                // Check if all items are available
                var allItemsAvailable = inventoryResults.All(item => item.Reserved);

                if (allItemsAvailable)
                {
                    // All items available - trigger payment processing
                    var paymentEvent = new PaymentEvent
                    {
                        OrderId = inventoryCheckEvent.OrderId,
                        PaymentId = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}",
                        Amount = CalculateOrderTotal(inventoryResults), // You'd calculate this properly
                        Currency = "USD",
                        PaymentMethod = "CreditCard", // This would come from the order
                        Status = "Pending",
                        Timestamp = DateTime.UtcNow
                    };

                    await _producer.PublishPaymentEventAsync(paymentEvent);
                    
                    _logger.LogInformation(
                        "Inventory check passed. Triggered payment processing. OrderId: {OrderId}",
                        inventoryCheckEvent.OrderId);
                }
                else
                {
                    // Some items not available - handle out of stock
                    await HandleOutOfStockAsync(inventoryCheckEvent.OrderId, inventoryResults);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize inventory check event");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing inventory check");
                throw;
            }
        }

        private async Task<int> GetAvailableInventoryAsync(string productId)
        {
            // Simulate database call to get available inventory
            await Task.Delay(50); // Simulate network latency
            
            // Return random inventory for demo (in production, this would be a real database call)
            var random = new Random();
            return random.Next(0, 100);
        }

        private async Task ReserveInventoryAsync(string productId, int quantity)
        {
            // Simulate database call to reserve inventory
            await Task.Delay(30);
            
            _logger.LogDebug("Reserved {Quantity} units of product {ProductId}", quantity, productId);
        }

        private decimal CalculateOrderTotal(List<InventoryItem> inventoryResults)
        {
            // In a real scenario, you'd calculate based on product prices
            // For demo, return a fixed amount
            return inventoryResults.Sum(item => item.RequestedQuantity * 29.99m);
        }

        private async Task HandleOutOfStockAsync(string orderId, List<InventoryItem> inventoryResults)
        {
            // Handle out of stock scenario
            var outOfStockItems = inventoryResults.Where(item => !item.Reserved).ToList();
            
            _logger.LogWarning(
                "Order {OrderId} has out of stock items: {Items}",
                orderId,
                string.Join(", ", outOfStockItems.Select(item => item.ProductId))
            );

            // In production, you might:
            // 1. Send notification to customer
            // 2. Create backorder
            // 3. Cancel the order
            // 4. Offer alternatives
            
            await Task.CompletedTask;
        }
    }
}
```

### Step 4: Program.cs Configuration

```csharp
// Program.cs
using EcommerceKafka.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Kafka services
builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.AddHostedService<InventoryConsumerService>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName,
    Version = "1.0.0"
});

app.Run();
```

### Step 5: Configuration Files

```json
// appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "EcommerceKafka.Services": "Debug"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Kafka": "localhost:9092,localhost:9093,localhost:9094"
  },
  "Kafka": {
    "ProducerConfig": {
      "BootstrapServers": "localhost:9092,localhost:9093,localhost:9094",
      "Acks": "All",
      "Retries": 3,
      "BatchSize": 16384,
      "LingerMs": 5,
      "CompressionType": "Snappy",
      "EnableIdempotence": true,
      "MaxInFlight": 5
    },
    "ConsumerConfig": {
      "BootstrapServers": "localhost:9092,localhost:9093,localhost:9094",
      "GroupId": "ecommerce-service-group",
      "AutoOffsetReset": "Earliest",
      "EnableAutoCommit": false,
      "SessionTimeoutMs": 30000,
      "HeartbeatIntervalMs": 10000
    }
  }
}
```

### Step 6: Testing the E-commerce Order Flow

```bash
# Test script for e-commerce order processing
echo "Testing E-commerce Order Processing with Kafka..."

# 1. Start the application
echo "Make sure your .NET application is running..."
echo "dotnet run"

# 2. Place an order via API
echo "Placing a test order..."
curl -X POST "https://localhost:7000/api/order/place" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "CUST-12345",
    "currency": "USD",
    "items": [
      {
        "productId": "PROD-001",
        "productName": "Gaming Laptop",
        "sku": "GL-2024-001",
        "quantity": 1,
        "unitPrice": 1299.99
      },
      {
        "productId": "PROD-002",
        "productName": "Wireless Mouse",
        "sku": "WM-2024-002",
        "quantity": 2,
        "unitPrice": 49.99
      }
    ],
    "shippingAddress": {
      "street": "123 Main St",
      "city": "New York",
      "state": "NY",
      "zipCode": "10001",
      "country": "USA"
    }
  }'

# 3. Check Kafka topics for messages
echo "Checking Kafka topics..."

# Check order events
docker exec kafka-broker-1 kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic ecommerce-orders \
  --from-beginning \
  --max-messages 1

# Check inventory events
docker exec kafka-broker-1 kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic inventory-check \
  --from-beginning \
  --max-messages 1

# Check payment events
docker exec kafka-broker-1 kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic payment-events \
  --from-beginning \
  --max-messages 1

# 4. Monitor consumer lag
docker exec kafka-broker-1 kafka-consumer-groups \
  --bootstrap-server localhost:9092 \
  --describe \
  --group inventory-service-group

echo "Order processing test completed!"
```

## Real-World Scenario 2: User Activity Tracking

### Business Context
A social media or e-commerce platform needs to track user activities in real-time for analytics, personalization, and fraud detection. This includes page views, clicks, searches, purchases, and user interactions.

### Architecture Overview

```
Web App → User Activity Events → [user-activity] → Multiple Consumers:
                                                   ├── Analytics Service
                                                   ├── Recommendation Engine
                                                   ├── Fraud Detection
                                                   └── Personalization Service
```

### Step 1: User Activity Event Models

```csharp
// Models/UserActivityModels.cs
using System.Text.Json.Serialization;

namespace EcommerceKafka.Models
{
    // Base user activity event
    public abstract class UserActivityEvent
    {
        [JsonPropertyName("eventId")]
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;
        
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; } = string.Empty;
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [JsonPropertyName("eventType")]
        public abstract string EventType { get; }
        
        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; } = string.Empty;
        
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; } = string.Empty;
        
        [JsonPropertyName("deviceType")]
        public string DeviceType { get; set; } = string.Empty; // Mobile, Desktop, Tablet
        
        [JsonPropertyName("location")]
        public UserLocation? Location { get; set; }
        
        [JsonPropertyName("referrer")]
        public string? Referrer { get; set; }
    }

    // Page view event
    public class PageViewEvent : UserActivityEvent
    {
        public override string EventType => "PageView";
        
        [JsonPropertyName("pageUrl")]
        public string PageUrl { get; set; } = string.Empty;
        
        [JsonPropertyName("pageTitle")]
        public string PageTitle { get; set; } = string.Empty;
        
        [JsonPropertyName("loadTime")]
        public double LoadTimeMs { get; set; }
        
        [JsonPropertyName("previousPage")]
        public string? PreviousPage { get; set; }
    }

    // Product view event
    public class ProductViewEvent : UserActivityEvent
    {
        public override string EventType => "ProductView";
        
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;
        
        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;
        
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
        
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("viewDurationSeconds")]
        public int ViewDurationSeconds { get; set; }
        
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty; // Search, Category, Recommendation
    }

    // Search event
    public class SearchEvent : UserActivityEvent
    {
        public override string EventType => "Search";
        
        [JsonPropertyName("searchQuery")]
        public string SearchQuery { get; set; } = string.Empty;
        
        [JsonPropertyName("resultsCount")]
        public int ResultsCount { get; set; }
        
        [JsonPropertyName("filters")]
        public Dictionary<string, string> Filters { get; set; } = new();
        
        [JsonPropertyName("sortBy")]
        public string SortBy { get; set; } = string.Empty;
        
        [JsonPropertyName("responseTimeMs")]
        public double ResponseTimeMs { get; set; }
    }

    // Add to cart event
    public class AddToCartEvent : UserActivityEvent
    {
        public override string EventType => "AddToCart";
        
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;
        
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("cartTotal")]
        public decimal CartTotal { get; set; }
        
        [JsonPropertyName("cartItemCount")]
        public int CartItemCount { get; set; }
    }

    // Purchase event
    public class PurchaseEvent : UserActivityEvent
    {
        public override string EventType => "Purchase";
        
        [JsonPropertyName("orderId")]
        public string OrderId { get; set; } = string.Empty;
        
        [JsonPropertyName("totalAmount")]
        public decimal TotalAmount { get; set; }
        
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "USD";
        
        [JsonPropertyName("items")]
        public List<PurchaseItem> Items { get; set; } = new();
        
        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [JsonPropertyName("discountAmount")]
        public decimal DiscountAmount { get; set; }
        
        [JsonPropertyName("taxAmount")]
        public decimal TaxAmount { get; set; }
    }

    // Login/Logout events
    public class AuthenticationEvent : UserActivityEvent
    {
        public override string EventType => "Authentication";
        
        [JsonPropertyName("action")]
        public string Action { get; set; } = string.Empty; // Login, Logout, Failed
        
        [JsonPropertyName("method")]
        public string Method { get; set; } = string.Empty; // Email, Social, SMS
        
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        
        [JsonPropertyName("failureReason")]
        public string? FailureReason { get; set; }
    }

    // Support classes
    public class UserLocation
    {
        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;
        
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;
        
        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;
        
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }
        
        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }

    public class PurchaseItem
    {
        [JsonPropertyName("productId")]
        public string ProductId { get; set; } = string.Empty;
        
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
        
        [JsonPropertyName("totalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
```

### Step 2: User Activity Tracking Service

```csharp
// Services/UserActivityService.cs
using Confluent.Kafka;
using System.Text.Json;
using EcommerceKafka.Models;

namespace EcommerceKafka.Services
{
    public interface IUserActivityService
    {
        Task TrackPageViewAsync(PageViewEvent pageView);
        Task TrackProductViewAsync(ProductViewEvent productView);
        Task TrackSearchAsync(SearchEvent searchEvent);
        Task TrackAddToCartAsync(AddToCartEvent addToCart);
        Task TrackPurchaseAsync(PurchaseEvent purchase);
        Task TrackAuthenticationAsync(AuthenticationEvent auth);
    }

    public class UserActivityService : IUserActivityService
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<UserActivityService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string TOPIC_NAME = "user-activity";

        public UserActivityService(IConfiguration configuration, ILogger<UserActivityService> logger)
        {
            _logger = logger;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            var config = new ProducerConfig
            {
                BootstrapServers = configuration.GetConnectionString("Kafka") ?? "localhost:9092,localhost:9093,localhost:9094",
                ClientId = $"activity-tracker-{Environment.MachineName}-{Guid.NewGuid():N}",
                
                // High throughput settings for activity tracking
                Acks = Acks.Leader, // Faster than Acks.All for high-volume events
                MessageSendMaxRetries = 2, // Less retries for activity data
                RetryBackoffMs = 100,
                
                // Batch for performance - activity events can be batched
                BatchSize = 65536, // 64KB batches
                LingerMs = 10, // Wait 10ms to batch more messages
                
                CompressionType = CompressionType.Snappy,
                QueueBufferingMaxMessages = 500000, // Large buffer for high volume
                QueueBufferingMaxKbytes = 2097152, // 2GB buffer
                
                // Shorter timeouts for activity events
                MessageTimeoutMs = 10000, // 10 seconds
                RequestTimeoutMs = 10000,
                
                // Enable idempotence
                EnableIdempotence = true,
                MaxInFlight = 5
            };

            _producer = new ProducerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => 
                {
                    _logger.LogError("Activity tracker producer error: {Error}", e.Reason);
                })
                .Build();
        }

        public async Task TrackPageViewAsync(PageViewEvent pageView)
        {
            await PublishActivityEventAsync(pageView, pageView.UserId);
        }

        public async Task TrackProductViewAsync(ProductViewEvent productView)
        {
            await PublishActivityEventAsync(productView, productView.UserId);
        }

        public async Task TrackSearchAsync(SearchEvent searchEvent)
        {
            await PublishActivityEventAsync(searchEvent, searchEvent.UserId);
        }

        public async Task TrackAddToCartAsync(AddToCartEvent addToCart)
        {
            await PublishActivityEventAsync(addToCart, addToCart.UserId);
        }

        public async Task TrackPurchaseAsync(PurchaseEvent purchase)
        {
            await PublishActivityEventAsync(purchase, purchase.UserId);
        }

        public async Task TrackAuthenticationAsync(AuthenticationEvent auth)
        {
            await PublishActivityEventAsync(auth, auth.UserId);
        }

        private async Task PublishActivityEventAsync<T>(T activityEvent, string userId) where T : UserActivityEvent
        {
            try
            {
                var message = JsonSerializer.Serialize(activityEvent, _jsonOptions);
                
                var kafkaMessage = new Message<string, string>
                {
                    // Use userId as key for partitioning - keeps user events in order
                    Key = userId,
                    Value = message,
                    Headers = new Headers
                    {
                        { "eventType", System.Text.Encoding.UTF8.GetBytes(activityEvent.EventType) },
                        { "userId", System.Text.Encoding.UTF8.GetBytes(userId) },
                        { "sessionId", System.Text.Encoding.UTF8.GetBytes(activityEvent.SessionId) },
                        { "timestamp", System.Text.Encoding.UTF8.GetBytes(activityEvent.Timestamp.ToString("O")) },
                        { "eventId", System.Text.Encoding.UTF8.GetBytes(activityEvent.EventId) },
                        { "source", System.Text.Encoding.UTF8.GetBytes("web-app") },
                        { "version", System.Text.Encoding.UTF8.GetBytes("1.0") }
                    }
                };

                // Use fire-and-forget for better performance with activity tracking
                _producer.Produce(TOPIC_NAME, kafkaMessage, (deliveryReport) =>
                {
                    if (deliveryReport.Error.IsError)
                    {
                        _logger.LogError("Failed to publish activity event: {Error}, EventType: {EventType}, UserId: {UserId}",
                            deliveryReport.Error.Reason, activityEvent.EventType, userId);
                    }
                    else
                    {
                        _logger.LogDebug("Activity event published: {EventType}, UserId: {UserId}, Partition: {Partition}, Offset: {Offset}",
                            activityEvent.EventType, userId, deliveryReport.Partition.Value, deliveryReport.Offset.Value);
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing activity event: {EventType}, UserId: {UserId}",
                    activityEvent.EventType, userId);
                
                // Don't throw - activity tracking shouldn't break the main flow
            }
        }

        public void Dispose()
        {
            _producer?.Flush(TimeSpan.FromSeconds(5));
            _producer?.Dispose();
        }
    }
}
```

### Step 3: Activity Tracking Controller

```csharp
// Controllers/ActivityController.cs
using Microsoft.AspNetCore.Mvc;
using EcommerceKafka.Models;
using EcommerceKafka.Services;

namespace EcommerceKafka.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly IUserActivityService _activityService;
        private readonly ILogger<ActivityController> _logger;

        public ActivityController(IUserActivityService activityService, ILogger<ActivityController> logger)
        {
            _activityService = activityService;
            _logger = logger;
        }

        // POST api/activity/pageview
        [HttpPost("pageview")]
        public async Task<IActionResult> TrackPageView([FromBody] TrackPageViewRequest request)
        {
            try
            {
                var pageViewEvent = new PageViewEvent
                {
                    UserId = request.UserId,
                    SessionId = request.SessionId,
                    PageUrl = request.PageUrl,
                    PageTitle = request.PageTitle,
                    LoadTimeMs = request.LoadTimeMs,
                    PreviousPage = request.PreviousPage,
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IpAddress = GetClientIpAddress(),
                    DeviceType = DetermineDeviceType(Request.Headers["User-Agent"].ToString()),
                    Referrer = Request.Headers["Referer"].ToString()
                };

                await _activityService.TrackPageViewAsync(pageViewEvent);
                return Ok(new { Success = true, EventId = pageViewEvent.EventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking page view for user {UserId}", request?.UserId);
                return Ok(new { Success = false }); // Return success to not break user experience
            }
        }

        // POST api/activity/productview
        [HttpPost("productview")]
        public async Task<IActionResult> TrackProductView([FromBody] TrackProductViewRequest request)
        {
            try
            {
                var productViewEvent = new ProductViewEvent
                {
                    UserId = request.UserId,
                    SessionId = request.SessionId,
                    ProductId = request.ProductId,
                    ProductName = request.ProductName,
                    Category = request.Category,
                    Price = request.Price,
                    ViewDurationSeconds = request.ViewDurationSeconds,
                    Source = request.Source,
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IpAddress = GetClientIpAddress(),
                    DeviceType = DetermineDeviceType(Request.Headers["User-Agent"].ToString())
                };

                await _activityService.TrackProductViewAsync(productViewEvent);
                return Ok(new { Success = true, EventId = productViewEvent.EventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking product view for user {UserId}, product {ProductId}", 
                    request?.UserId, request?.ProductId);
                return Ok(new { Success = false });
            }
        }

        // POST api/activity/search
        [HttpPost("search")]
        public async Task<IActionResult> TrackSearch([FromBody] TrackSearchRequest request)
        {
            try
            {
                var searchEvent = new SearchEvent
                {
                    UserId = request.UserId,
                    SessionId = request.SessionId,
                    SearchQuery = request.SearchQuery,
                    ResultsCount = request.ResultsCount,
                    Filters = request.Filters ?? new Dictionary<string, string>(),
                    SortBy = request.SortBy ?? string.Empty,
                    ResponseTimeMs = request.ResponseTimeMs,
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IpAddress = GetClientIpAddress(),
                    DeviceType = DetermineDeviceType(Request.Headers["User-Agent"].ToString())
                };

                await _activityService.TrackSearchAsync(searchEvent);
                return Ok(new { Success = true, EventId = searchEvent.EventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking search for user {UserId}, query {Query}", 
                    request?.UserId, request?.SearchQuery);
                return Ok(new { Success = false });
            }
        }

        // POST api/activity/addtocart
        [HttpPost("addtocart")]
        public async Task<IActionResult> TrackAddToCart([FromBody] TrackAddToCartRequest request)
        {
            try
            {
                var addToCartEvent = new AddToCartEvent
                {
                    UserId = request.UserId,
                    SessionId = request.SessionId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Price = request.Price,
                    CartTotal = request.CartTotal,
                    CartItemCount = request.CartItemCount,
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IpAddress = GetClientIpAddress(),
                    DeviceType = DetermineDeviceType(Request.Headers["User-Agent"].ToString())
                };

                await _activityService.TrackAddToCartAsync(addToCartEvent);
                return Ok(new { Success = true, EventId = addToCartEvent.EventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking add to cart for user {UserId}, product {ProductId}", 
                    request?.UserId, request?.ProductId);
                return Ok(new { Success = false });
            }
        }

        // POST api/activity/purchase
        [HttpPost("purchase")]
        public async Task<IActionResult> TrackPurchase([FromBody] TrackPurchaseRequest request)
        {
            try
            {
                var purchaseEvent = new PurchaseEvent
                {
                    UserId = request.UserId,
                    SessionId = request.SessionId,
                    OrderId = request.OrderId,
                    TotalAmount = request.TotalAmount,
                    Currency = request.Currency ?? "USD",
                    Items = request.Items?.Select(item => new PurchaseItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    }).ToList() ?? new List<PurchaseItem>(),
                    PaymentMethod = request.PaymentMethod ?? string.Empty,
                    DiscountAmount = request.DiscountAmount,
                    TaxAmount = request.TaxAmount,
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    IpAddress = GetClientIpAddress(),
                    DeviceType = DetermineDeviceType(Request.Headers["User-Agent"].ToString())
                };

                await _activityService.TrackPurchaseAsync(purchaseEvent);
                return Ok(new { Success = true, EventId = purchaseEvent.EventId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking purchase for user {UserId}, order {OrderId}", 
                    request?.UserId, request?.OrderId);
                return Ok(new { Success = false });
            }
        }

        // Batch tracking endpoint for better performance
        [HttpPost("batch")]
        public async Task<IActionResult> TrackBatch([FromBody] List<ActivityEventRequest> requests)
        {
            var results = new List<object>();
            
            foreach (var request in requests)
            {
                try
                {
                    switch (request.EventType.ToLower())
                    {
                        case "pageview":
                            var pageViewRequest = JsonSerializer.Deserialize<TrackPageViewRequest>(request.Data.ToString());
                            await TrackPageView(pageViewRequest);
                            results.Add(new { EventType = request.EventType, Success = true });
                            break;
                            
                        case "productview":
                            var productViewRequest = JsonSerializer.Deserialize<TrackProductViewRequest>(request.Data.ToString());
                            await TrackProductView(productViewRequest);
                            results.Add(new { EventType = request.EventType, Success = true });
                            break;
                            
                        // Add more event types as needed
                        
                        default:
                            results.Add(new { EventType = request.EventType, Success = false, Error = "Unknown event type" });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing batch event: {EventType}", request.EventType);
                    results.Add(new { EventType = request.EventType, Success = false, Error = ex.Message });
                }
            }

            return Ok(new { Results = results });
        }

        // Helper methods
        private string GetClientIpAddress()
        {
            return Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                ?? Request.Headers["X-Real-IP"].FirstOrDefault()
                ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                ?? "Unknown";
        }

        private string DetermineDeviceType(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            userAgent = userAgent.ToLower();
            
            if (userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone"))
                return "Mobile";
            
            if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
                return "Tablet";
            
            return "Desktop";
        }
    }

    // DTOs for API requests
    public class TrackPageViewRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string PageUrl { get; set; } = string.Empty;
        public string PageTitle { get; set; } = string.Empty;
        public double LoadTimeMs { get; set; }
        public string? PreviousPage { get; set; }
    }

    public class TrackProductViewRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int ViewDurationSeconds { get; set; }
        public string Source { get; set; } = string.Empty;
    }

    public class TrackSearchRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string SearchQuery { get; set; } = string.Empty;
        public int ResultsCount { get; set; }
        public Dictionary<string, string>? Filters { get; set; }
        public string? SortBy { get; set; }
        public double ResponseTimeMs { get; set; }
    }

    public class TrackAddToCartRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal CartTotal { get; set; }
        public int CartItemCount { get; set; }
    }

    public class TrackPurchaseRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? Currency { get; set; }
        public List<PurchaseItemRequest>? Items { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
    }

    public class PurchaseItemRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class ActivityEventRequest
    {
        public string EventType { get; set; } = string.Empty;
        public object Data { get; set; } = new();
    }
}
```

### Step 4: Real-time Analytics Consumer

```csharp
// Services/AnalyticsConsumerService.cs
using Confluent.Kafka;
using System.Text.Json;
using EcommerceKafka.Models;

namespace EcommerceKafka.Services
{
    public class AnalyticsConsumerService : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly ILogger<AnalyticsConsumerService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        // In-memory analytics cache (in production, use Redis or database)
        private readonly Dictionary<string, UserAnalytics> _userAnalytics = new();
        private readonly Dictionary<string, ProductAnalytics> _productAnalytics = new();
        private readonly object _analyticsLock = new object();

        public AnalyticsConsumerService(IConfiguration configuration, ILogger<AnalyticsConsumerService> logger)
        {
            _logger = logger;
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var config = new ConsumerConfig
            {
                BootstrapServers = configuration.GetConnectionString("Kafka") ?? "localhost:9092,localhost:9093,localhost:9094",
                GroupId = "analytics-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Latest, // Start from latest for real-time analytics
                EnableAutoCommit = true, // Auto commit for analytics (some data loss is acceptable)
                AutoCommitIntervalMs = 5000, // Commit every 5 seconds
                SessionTimeoutMs = 30000,
                HeartbeatIntervalMs = 10000,
                FetchMinBytes = 1024,
                FetchMaxWaitMs = 100, // Low latency for real-time processing
                MaxPollIntervalMs = 300000,
                ClientId = $"analytics-consumer-{Environment.MachineName}"
            };

            _consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => 
                {
                    _logger.LogError("Analytics consumer error: {Error}", e.Reason);
                })
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            
            _logger.LogInformation("Analytics consumer service started");
            
            _consumer.Subscribe("user-activity");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(100));
                        
                        if (consumeResult == null)
                            continue;

                        await ProcessActivityEventAsync(consumeResult.Message);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming analytics message: {Error}", ex.Error.Reason);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error in analytics consumer");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Analytics consumer stopping due to cancellation");
            }
            finally
            {
                _consumer.Close();
                _consumer.Dispose();
                _logger.LogInformation("Analytics consumer stopped");
            }
        }

        private async Task ProcessActivityEventAsync(Message<string, string> message)
        {
            try
            {
                // Get event type from headers
                var eventTypeBytes = message.Headers.FirstOrDefault(h => h.Key == "eventType")?.GetValueBytes();
                if (eventTypeBytes == null) return;
                
                var eventType = System.Text.Encoding.UTF8.GetString(eventTypeBytes);
                
                // Process different event types
                switch (eventType)
                {
                    case "PageView":
                        await ProcessPageViewEvent(message.Value);
                        break;
                        
                    case "ProductView":
                        await ProcessProductViewEvent(message.Value);
                        break;
                        
                    case "Search":
                        await ProcessSearchEvent(message.Value);
                        break;
                        
                    case "AddToCart":
                        await ProcessAddToCartEvent(message.Value);
                        break;
                        
                    case "Purchase":
                        await ProcessPurchaseEvent(message.Value);
                        break;
                        
                    case "Authentication":
                        await ProcessAuthenticationEvent(message.Value);
                        break;
                        
                    default:
                        _logger.LogDebug("Unknown event type: {EventType}", eventType);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing activity event for analytics");
            }
        }

        private async Task ProcessPageViewEvent(string messageValue)
        {
            var pageViewEvent = JsonSerializer.Deserialize<PageViewEvent>(messageValue, _jsonOptions);
            if (pageViewEvent == null) return;

            lock (_analyticsLock)
            {
                // Update user analytics
                if (!_userAnalytics.ContainsKey(pageViewEvent.UserId))
                {
                    _userAnalytics[pageViewEvent.UserId] = new UserAnalytics { UserId = pageViewEvent.UserId };
                }

                var userAnalytics = _userAnalytics[pageViewEvent.UserId];
                userAnalytics.PageViews++;
                userAnalytics.LastActivity = pageViewEvent.Timestamp;
                userAnalytics.SessionIds.Add(pageViewEvent.SessionId);
            }

            _logger.LogDebug("Processed page view for user {UserId}: {PageUrl}", 
                pageViewEvent.UserId, pageViewEvent.PageUrl);
            
            await Task.CompletedTask;
        }

        private async Task ProcessProductViewEvent(string messageValue)
        {
            var productViewEvent = JsonSerializer.Deserialize<ProductViewEvent>(messageValue, _jsonOptions);
            if (productViewEvent == null) return;

            lock (_analyticsLock)
            {
                // Update user analytics
                if (!_userAnalytics.ContainsKey(productViewEvent.UserId))
                {
                    _userAnalytics[productViewEvent.UserId] = new UserAnalytics { UserId = productViewEvent.UserId };
                }

                var userAnalytics = _userAnalytics[productViewEvent.UserId];
                userAnalytics.ProductViews++;
                userAnalytics.LastActivity = productViewEvent.Timestamp;
                userAnalytics.ViewedProducts.Add(productViewEvent.ProductId);

                // Update product analytics
                if (!_productAnalytics.ContainsKey(productViewEvent.ProductId))
                {
                    _productAnalytics[productViewEvent.ProductId] = new ProductAnalytics 
                    { 
                        ProductId = productViewEvent.ProductId,
                        ProductName = productViewEvent.ProductName,
                        Category = productViewEvent.Category,
                        Price = productViewEvent.Price
                    };
                }

                var productAnalytics = _productAnalytics[productViewEvent.ProductId];
                productAnalytics.Views++;
                productAnalytics.UniqueViewers.Add(productViewEvent.UserId);
                productAnalytics.TotalViewDuration += productViewEvent.ViewDurationSeconds;
            }

            await Task.CompletedTask;
        }

        private async Task ProcessSearchEvent(string messageValue)
        {
            var searchEvent = JsonSerializer.Deserialize<SearchEvent>(messageValue, _jsonOptions);
            if (searchEvent == null) return;

            lock (_analyticsLock)
            {
                if (!_userAnalytics.ContainsKey(searchEvent.UserId))
                {
                    _userAnalytics[searchEvent.UserId] = new UserAnalytics { UserId = searchEvent.UserId };
                }

                var userAnalytics = _userAnalytics[searchEvent.UserId];
                userAnalytics.Searches++;
                userAnalytics.LastActivity = searchEvent.Timestamp;
                userAnalytics.SearchQueries.Add(searchEvent.SearchQuery);
            }

            await Task.CompletedTask;
        }

        private async Task ProcessAddToCartEvent(string messageValue)
        {
            var addToCartEvent = JsonSerializer.Deserialize<AddToCartEvent>(messageValue, _jsonOptions);
            if (addToCartEvent == null) return;

            lock (_analyticsLock)
            {
                if (!_userAnalytics.ContainsKey(addToCartEvent.UserId))
                {
                    _userAnalytics[addToCartEvent.UserId] = new UserAnalytics { UserId = addToCartEvent.UserId };
                }

                var userAnalytics = _userAnalytics[addToCartEvent.UserId];
                userAnalytics.CartAdditions++;
                userAnalytics.LastActivity = addToCartEvent.Timestamp;

                // Update product analytics
                if (_productAnalytics.ContainsKey(addToCartEvent.ProductId))
                {
                    _productAnalytics[addToCartEvent.ProductId].CartAdditions++;
                }
            }

            await Task.CompletedTask;
        }

        private async Task ProcessPurchaseEvent(string messageValue)
        {
            var purchaseEvent = JsonSerializer.Deserialize<PurchaseEvent>(messageValue, _jsonOptions);
            if (purchaseEvent == null) return;

            lock (_analyticsLock)
            {
                if (!_userAnalytics.ContainsKey(purchaseEvent.UserId))
                {
                    _userAnalytics[purchaseEvent.UserId] = new UserAnalytics { UserId = purchaseEvent.UserId };
                }

                var userAnalytics = _userAnalytics[purchaseEvent.UserId];
                userAnalytics.Purchases++;
                userAnalytics.TotalSpent += purchaseEvent.TotalAmount;
                userAnalytics.LastActivity = purchaseEvent.Timestamp;

                // Update product analytics
                foreach (var item in purchaseEvent.Items)
                {
                    if (_productAnalytics.ContainsKey(item.ProductId))
                    {
                        var productAnalytics = _productAnalytics[item.ProductId];
                        productAnalytics.Purchases += item.Quantity;
                        productAnalytics.Revenue += item.TotalPrice;
                    }
                }
            }

            await Task.CompletedTask;
        }

        private async Task ProcessAuthenticationEvent(string messageValue)
        {
            var authEvent = JsonSerializer.Deserialize<AuthenticationEvent>(messageValue, _jsonOptions);
            if (authEvent == null) return;

            lock (_analyticsLock)
            {
                if (!_userAnalytics.ContainsKey(authEvent.UserId))
                {
                    _userAnalytics[authEvent.UserId] = new UserAnalytics { UserId = authEvent.UserId };
                }

                var userAnalytics = _userAnalytics[authEvent.UserId];
                if (authEvent.Action == "Login" && authEvent.Success)
                {
                    userAnalytics.Logins++;
                    userAnalytics.LastLogin = authEvent.Timestamp;
                }
                userAnalytics.LastActivity = authEvent.Timestamp;
            }

            await Task.CompletedTask;
        }

        // API to get analytics data
        public UserAnalytics? GetUserAnalytics(string userId)
        {
            lock (_analyticsLock)
            {
                return _userAnalytics.TryGetValue(userId, out var analytics) ? analytics : null;
            }
        }

        public ProductAnalytics? GetProductAnalytics(string productId)
        {
            lock (_analyticsLock)
            {
                return _productAnalytics.TryGetValue(productId, out var analytics) ? analytics : null;
            }
        }

        public Dictionary<string, object> GetOverallAnalytics()
        {
            lock (_analyticsLock)
            {
                return new Dictionary<string, object>
                {
                    ["totalUsers"] = _userAnalytics.Count,
                    ["totalProducts"] = _productAnalytics.Count,
                    ["totalPageViews"] = _userAnalytics.Values.Sum(u => u.PageViews),
                    ["totalProductViews"] = _userAnalytics.Values.Sum(u => u.ProductViews),
                    ["totalSearches"] = _userAnalytics.Values.Sum(u => u.Searches),
                    ["totalPurchases"] = _userAnalytics.Values.Sum(u => u.Purchases),
                    ["totalRevenue"] = _userAnalytics.Values.Sum(u => u.TotalSpent),
                    ["topProducts"] = _productAnalytics.Values
                        .OrderByDescending(p => p.Views)
                        .Take(10)
                        .Select(p => new { p.ProductId, p.ProductName, p.Views, p.Revenue })
                        .ToList()
                };
            }
        }
    }

    // Analytics data models
    public class UserAnalytics
    {
        public string UserId { get; set; } = string.Empty;
        public int PageViews { get; set; }
        public int ProductViews { get; set; }
        public int Searches { get; set; }
        public int CartAdditions { get; set; }
        public int Purchases { get; set; }
        public int Logins { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastActivity { get; set; }
        public DateTime LastLogin { get; set; }
        public HashSet<string> SessionIds { get; set; } = new();
        public HashSet<string> ViewedProducts { get; set; } = new();
        public List<string> SearchQueries { get; set; } = new();
    }

    public class ProductAnalytics
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Views { get; set; }
        public int CartAdditions { get; set; }
        public int Purchases { get; set; }
        public decimal Revenue { get; set; }
        public int TotalViewDuration { get; set; }
        public HashSet<string> UniqueViewers { get; set; } = new();
        public double ConversionRate => Views > 0 ? (double)Purchases / Views * 100 : 0;
        public double AverageViewDuration => Views > 0 ? (double)TotalViewDuration / Views : 0;
    }
}
```

---

*Continuing with more real-world scenarios...*