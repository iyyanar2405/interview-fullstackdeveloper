using AI.StreamProcessing.Models;
using AI.StreamProcessing.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI.StreamProcessing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreamController : ControllerBase
{
    private readonly IKafkaService _kafkaService;
    private readonly IServiceBusService _serviceBusService;
    private readonly IEventHubService _eventHubService;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IRedisStreamService _redisStreamService;
    private readonly IStreamProcessingService _processingService;
    private readonly ILogger<StreamController> _logger;

    public StreamController(
        IKafkaService kafkaService,
        IServiceBusService serviceBusService,
        IEventHubService eventHubService,
        IRabbitMQService rabbitMQService,
        IRedisStreamService redisStreamService,
        IStreamProcessingService processingService,
        ILogger<StreamController> logger)
    {
        _kafkaService = kafkaService;
        _serviceBusService = serviceBusService;
        _eventHubService = eventHubService;
        _rabbitMQService = rabbitMQService;
        _redisStreamService = redisStreamService;
        _processingService = processingService;
        _logger = logger;
    }

    // Kafka Endpoints
    [HttpPost("kafka/produce")]
    public async Task<ActionResult<ApiResponseModel<bool>>> ProduceToKafka(
        [FromBody] KafkaProduceRequest request)
    {
        try
        {
            var result = await _kafkaService.ProduceAsync(request.Topic, request.Message, request.Key);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing to Kafka");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("kafka/produce-batch")]
    public async Task<ActionResult<ApiResponseModel<bool>>> ProduceBatchToKafka(
        [FromBody] KafkaBatchProduceRequest request)
    {
        try
        {
            var result = await _kafkaService.ProduceBatchAsync(request.Topic, request.Messages);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing batch to Kafka");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("kafka/metrics/{topic}")]
    public async Task<ActionResult<ApiResponseModel<StreamMetrics>>> GetKafkaMetrics(string topic)
    {
        try
        {
            var metrics = await _kafkaService.GetMetricsAsync(topic);
            return Ok(ApiResponseModel<StreamMetrics>.SuccessResponse(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Kafka metrics");
            return BadRequest(ApiResponseModel<StreamMetrics>.ErrorResponse(ex.Message));
        }
    }

    // Service Bus Endpoints
    [HttpPost("servicebus/send")]
    public async Task<ActionResult<ApiResponseModel<bool>>> SendToServiceBus(
        [FromBody] ServiceBusSendRequest request)
    {
        try
        {
            var result = await _serviceBusService.SendMessageAsync(request.QueueOrTopic, request.Message);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending to Service Bus");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("servicebus/send-batch")]
    public async Task<ActionResult<ApiResponseModel<bool>>> SendBatchToServiceBus(
        [FromBody] ServiceBusBatchSendRequest request)
    {
        try
        {
            var result = await _serviceBusService.SendBatchAsync(request.QueueOrTopic, request.Messages);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending batch to Service Bus");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("servicebus/metrics")]
    public async Task<ActionResult<ApiResponseModel<StreamMetrics>>> GetServiceBusMetrics()
    {
        try
        {
            var metrics = await _serviceBusService.GetMetricsAsync();
            return Ok(ApiResponseModel<StreamMetrics>.SuccessResponse(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Service Bus metrics");
            return BadRequest(ApiResponseModel<StreamMetrics>.ErrorResponse(ex.Message));
        }
    }

    // Event Hub Endpoints
    [HttpPost("eventhub/send")]
    public async Task<ActionResult<ApiResponseModel<bool>>> SendToEventHub(
        [FromBody] EventHubSendRequest request)
    {
        try
        {
            var result = await _eventHubService.SendEventAsync(request.EventData, request.PartitionKey);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending to Event Hub");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("eventhub/send-batch")]
    public async Task<ActionResult<ApiResponseModel<bool>>> SendBatchToEventHub(
        [FromBody] List<object> events)
    {
        try
        {
            var result = await _eventHubService.SendBatchAsync(events);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending batch to Event Hub");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("eventhub/read/{partitionId}")]
    public async Task<ActionResult<ApiResponseModel<List<EventData>>>> ReadFromEventHub(
        string partitionId,
        [FromQuery] int maxEvents = 100)
    {
        try
        {
            var events = await _eventHubService.ReadEventsAsync(partitionId, maxEvents);
            return Ok(ApiResponseModel<List<EventData>>.SuccessResponse(events));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading from Event Hub");
            return BadRequest(ApiResponseModel<List<EventData>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("eventhub/metrics")]
    public async Task<ActionResult<ApiResponseModel<StreamMetrics>>> GetEventHubMetrics()
    {
        try
        {
            var metrics = await _eventHubService.GetMetricsAsync();
            return Ok(ApiResponseModel<StreamMetrics>.SuccessResponse(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Event Hub metrics");
            return BadRequest(ApiResponseModel<StreamMetrics>.ErrorResponse(ex.Message));
        }
    }

    // RabbitMQ Endpoints
    [HttpPost("rabbitmq/publish")]
    public async Task<ActionResult<ApiResponseModel<bool>>> PublishToRabbitMQ(
        [FromBody] RabbitMQPublishRequest request)
    {
        try
        {
            var result = await _rabbitMQService.PublishAsync(
                request.Exchange,
                request.RoutingKey,
                request.Message);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing to RabbitMQ");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("rabbitmq/publish-batch")]
    public async Task<ActionResult<ApiResponseModel<bool>>> PublishBatchToRabbitMQ(
        [FromBody] RabbitMQBatchPublishRequest request)
    {
        try
        {
            var result = await _rabbitMQService.PublishBatchAsync(
                request.Exchange,
                request.RoutingKey,
                request.Messages);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing batch to RabbitMQ");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("rabbitmq/metrics")]
    public async Task<ActionResult<ApiResponseModel<StreamMetrics>>> GetRabbitMQMetrics()
    {
        try
        {
            var metrics = await _rabbitMQService.GetMetricsAsync();
            return Ok(ApiResponseModel<StreamMetrics>.SuccessResponse(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting RabbitMQ metrics");
            return BadRequest(ApiResponseModel<StreamMetrics>.ErrorResponse(ex.Message));
        }
    }

    // Redis Streams Endpoints
    [HttpPost("redis/add")]
    public async Task<ActionResult<ApiResponseModel<bool>>> AddToRedisStream(
        [FromBody] RedisStreamAddRequest request)
    {
        try
        {
            var result = await _redisStreamService.AddToStreamAsync(request.StreamKey, request.Message);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to Redis stream");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpPost("redis/add-batch")]
    public async Task<ActionResult<ApiResponseModel<bool>>> AddBatchToRedisStream(
        [FromBody] RedisStreamBatchAddRequest request)
    {
        try
        {
            var result = await _redisStreamService.AddBatchToStreamAsync(request.StreamKey, request.Messages);
            return Ok(ApiResponseModel<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding batch to Redis stream");
            return BadRequest(ApiResponseModel<bool>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("redis/read/{streamKey}")]
    public async Task<ActionResult<ApiResponseModel<List<RedisStreamMessage>>>> ReadFromRedisStream(
        string streamKey,
        [FromQuery] int count = 100)
    {
        try
        {
            var messages = await _redisStreamService.ReadStreamAsync(streamKey, count);
            return Ok(ApiResponseModel<List<RedisStreamMessage>>.SuccessResponse(messages));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading from Redis stream");
            return BadRequest(ApiResponseModel<List<RedisStreamMessage>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("redis/metrics")]
    public async Task<ActionResult<ApiResponseModel<StreamMetrics>>> GetRedisMetrics()
    {
        try
        {
            var metrics = await _redisStreamService.GetMetricsAsync();
            return Ok(ApiResponseModel<StreamMetrics>.SuccessResponse(metrics));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Redis metrics");
            return BadRequest(ApiResponseModel<StreamMetrics>.ErrorResponse(ex.Message));
        }
    }

    // Dead Letter Queue Endpoints
    [HttpGet("dlq/messages")]
    public async Task<ActionResult<ApiResponseModel<List<DeadLetterMessage>>>> GetDeadLetterMessages(
        [FromQuery] int count = 100)
    {
        try
        {
            var messages = await _processingService.GetDeadLetterMessagesAsync(count);
            return Ok(ApiResponseModel<List<DeadLetterMessage>>.SuccessResponse(messages));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dead letter messages");
            return BadRequest(ApiResponseModel<List<DeadLetterMessage>>.ErrorResponse(ex.Message));
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "Stream Processing Service"
        });
    }
}

// Request Models
public class KafkaProduceRequest
{
    public string Topic { get; set; } = string.Empty;
    public object Message { get; set; } = new();
    public string? Key { get; set; }
}

public class KafkaBatchProduceRequest
{
    public string Topic { get; set; } = string.Empty;
    public List<object> Messages { get; set; } = new();
}

public class ServiceBusSendRequest
{
    public string QueueOrTopic { get; set; } = string.Empty;
    public object Message { get; set; } = new();
}

public class ServiceBusBatchSendRequest
{
    public string QueueOrTopic { get; set; } = string.Empty;
    public List<object> Messages { get; set; } = new();
}

public class EventHubSendRequest
{
    public object EventData { get; set; } = new();
    public string? PartitionKey { get; set; }
}

public class RabbitMQPublishRequest
{
    public string Exchange { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
    public object Message { get; set; } = new();
}

public class RabbitMQBatchPublishRequest
{
    public string Exchange { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
    public List<object> Messages { get; set; } = new();
}

public class RedisStreamAddRequest
{
    public string StreamKey { get; set; } = string.Empty;
    public object Message { get; set; } = new();
}

public class RedisStreamBatchAddRequest
{
    public string StreamKey { get; set; } = string.Empty;
    public List<object> Messages { get; set; } = new();
}
