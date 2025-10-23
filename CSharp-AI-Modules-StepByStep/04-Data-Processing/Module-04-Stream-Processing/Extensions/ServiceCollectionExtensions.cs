using AI.StreamProcessing.Models;
using AI.StreamProcessing.Services;

namespace AI.StreamProcessing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStreamProcessingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Kafka services
        var kafkaProducerConfig = configuration.GetSection("Kafka:Producer").Get<KafkaProducerConfig>() 
            ?? new KafkaProducerConfig();
        var kafkaConsumerConfig = configuration.GetSection("Kafka:Consumer").Get<KafkaConsumerConfig>() 
            ?? new KafkaConsumerConfig();
        
        services.AddSingleton(kafkaProducerConfig);
        services.AddSingleton(kafkaConsumerConfig);
        services.AddScoped<IKafkaService, KafkaService>();

        // Register Azure Service Bus services
        var serviceBusConfig = configuration.GetSection("ServiceBus").Get<ServiceBusConfig>() 
            ?? new ServiceBusConfig();
        services.AddSingleton(serviceBusConfig);
        services.AddScoped<IServiceBusService, ServiceBusService>();

        // Register Azure Event Hubs services
        var eventHubConfig = configuration.GetSection("EventHubs").Get<EventHubConfig>() 
            ?? new EventHubConfig();
        services.AddSingleton(eventHubConfig);
        services.AddScoped<IEventHubService, EventHubService>();

        // Register RabbitMQ services
        var rabbitMQConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>() 
            ?? new RabbitMQConfig();
        services.AddSingleton(rabbitMQConfig);
        services.AddScoped<IRabbitMQService, RabbitMQService>();

        // Register Redis Streams services
        var redisStreamConfig = configuration.GetSection("RedisStreams").Get<RedisStreamConfig>() 
            ?? new RedisStreamConfig();
        services.AddSingleton(redisStreamConfig);
        services.AddScoped<IRedisStreamService, RedisStreamService>();

        // Register stream processing service
        services.AddScoped<IStreamProcessingService, StreamProcessingService>();

        return services;
    }
}
