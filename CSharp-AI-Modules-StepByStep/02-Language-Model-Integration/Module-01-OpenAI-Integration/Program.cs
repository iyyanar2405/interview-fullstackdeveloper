using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AI.OpenAI.Integration.Services;

namespace AI.OpenAI.Integration;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🤖 OpenAI Integration Demo");
        Console.WriteLine("==========================\n");

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

        // Build host with dependency injection
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Configure OpenAI settings
                services.Configure<OpenAISettings>(configuration.GetSection("OpenAI"));

                // Register services
                services.AddSingleton<ITokenCountingService, TokenCountingService>();
                services.AddScoped<IOpenAIService, OpenAIService>();

                // Add logging
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });

                // Add HTTP client
                services.AddHttpClient();
            })
            .Build();

        // Run examples
        using var scope = host.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        try
        {
            await RunExamples(serviceProvider);
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred running the examples");
            Console.WriteLine($"❌ Error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static async Task RunExamples(IServiceProvider serviceProvider)
    {
        var openAIService = serviceProvider.GetRequiredService<IOpenAIService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Starting OpenAI integration examples");

        // Example 1: Basic Chat
        await BasicChatExample(openAIService);

        // Example 2: Chat with Context
        await ChatWithContextExample(openAIService);

        // Example 3: Streaming Chat
        await StreamingChatExample(openAIService);

        // Example 4: Text Completion
        await TextCompletionExample(openAIService);

        // Example 5: Model Information
        await ModelInformationExample(openAIService);

        // Example 6: Token Counting
        await TokenCountingExample(serviceProvider);

        logger.LogInformation("All examples completed successfully");
    }

    static async Task BasicChatExample(IOpenAIService openAIService)
    {
        Console.WriteLine("1. 💬 Basic Chat Example");
        Console.WriteLine("========================");

        try
        {
            var response = await openAIService.ChatAsync(
                "Hello! Can you explain what artificial intelligence is in simple terms?");

            Console.WriteLine($"🤖 AI Response: {response.Message}");
            Console.WriteLine($"📊 Tokens used: {response.TokensUsed}");
            Console.WriteLine($"⏱️ Processing time: {response.ProcessingTime.TotalMilliseconds:F0}ms");
            Console.WriteLine($"🔧 Model: {response.Model}");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in basic chat: {ex.Message}");
        }
    }

    static async Task ChatWithContextExample(IOpenAIService openAIService)
    {
        Console.WriteLine("2. 🧠 Chat with Context Example");
        Console.WriteLine("===============================");

        try
        {
            var messages = new List<ChatMessage>
            {
                new() { Role = "system", Content = "You are a helpful programming assistant. Be concise and practical." },
                new() { Role = "user", Content = "How do I create a simple REST API in C#?" },
                new() { Role = "assistant", Content = "You can create a REST API in C# using ASP.NET Core. Start by creating a new Web API project with 'dotnet new webapi'." },
                new() { Role = "user", Content = "What's the next step after creating the project?" }
            };

            var options = new ChatOptions
            {
                Temperature = 0.7,
                MaxTokens = 500
            };

            var response = await openAIService.ChatWithContextAsync(messages, options);

            Console.WriteLine($"🤖 AI Response: {response.Message}");
            Console.WriteLine($"📊 Tokens used: {response.TokensUsed}");
            Console.WriteLine($"⏱️ Processing time: {response.ProcessingTime.TotalMilliseconds:F0}ms");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in context chat: {ex.Message}");
        }
    }

    static async Task StreamingChatExample(IOpenAIService openAIService)
    {
        Console.WriteLine("3. 🌊 Streaming Chat Example");
        Console.WriteLine("============================");

        try
        {
            Console.Write("🤖 AI Response (streaming): ");

            var options = new ChatOptions
            {
                Temperature = 0.8,
                MaxTokens = 200
            };

            await foreach (var chunk in openAIService.ChatStreamAsync(
                "Tell me a short story about a robot learning to paint.", options))
            {
                Console.Write(chunk.Content);
                await Task.Delay(50); // Simulate reading speed
            }

            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in streaming chat: {ex.Message}");
        }
    }

    static async Task TextCompletionExample(IOpenAIService openAIService)
    {
        Console.WriteLine("4. ✍️ Text Completion Example");
        Console.WriteLine("==============================");

        try
        {
            var options = new CompletionOptions
            {
                MaxTokens = 100,
                Temperature = 0.9
            };

            var response = await openAIService.CompleteTextAsync(
                "The future of artificial intelligence in healthcare will", options);

            Console.WriteLine($"📝 Original prompt: \"The future of artificial intelligence in healthcare will\"");
            Console.WriteLine($"🤖 Completed text: {response.CompletedText}");
            Console.WriteLine($"📊 Tokens used: {response.TokensUsed}");
            Console.WriteLine($"⏱️ Processing time: {response.ProcessingTime.TotalMilliseconds:F0}ms");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in text completion: {ex.Message}");
        }
    }

    static async Task ModelInformationExample(IOpenAIService openAIService)
    {
        Console.WriteLine("5. 🔍 Model Information Example");
        Console.WriteLine("===============================");

        try
        {
            var models = await openAIService.GetAvailableModelsAsync();
            
            Console.WriteLine($"📋 Available models: {models.Count}");
            
            var relevantModels = models.Where(m => m.Id.Contains("gpt")).Take(5).ToList();
            foreach (var model in relevantModels)
            {
                Console.WriteLine($"  • {model.Id} (owned by: {model.OwnedBy})");
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error retrieving models: {ex.Message}");
        }
    }

    static async Task TokenCountingExample(IServiceProvider serviceProvider)
    {
        Console.WriteLine("6. 🔢 Token Counting Example");
        Console.WriteLine("=============================");

        try
        {
            var tokenService = serviceProvider.GetRequiredService<ITokenCountingService>();
            
            var testTexts = new[]
            {
                "Hello, world!",
                "This is a longer sentence with more words to test token counting.",
                "The quick brown fox jumps over the lazy dog. This pangram contains every letter of the English alphabet at least once."
            };

            foreach (var text in testTexts)
            {
                var tokenCount = tokenService.CountTokens(text, "gpt-3.5-turbo");
                var cost = tokenService.EstimateCost(tokenCount, 0, "gpt-3.5-turbo");
                var exceedsLimit = tokenService.ExceedsContextLimit(text, "gpt-3.5-turbo");

                Console.WriteLine($"📝 Text: \"{text.Substring(0, Math.Min(50, text.Length))}...\"");
                Console.WriteLine($"🔢 Estimated tokens: {tokenCount}");
                Console.WriteLine($"💰 Estimated cost: ${cost:F6}");
                Console.WriteLine($"⚠️ Exceeds limit: {(exceedsLimit ? "Yes" : "No")}");
                Console.WriteLine();
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in token counting: {ex.Message}");
        }
    }
}