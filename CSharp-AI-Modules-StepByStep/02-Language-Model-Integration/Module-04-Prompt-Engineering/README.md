# Module-04-Prompt-Engineering

## Overview

Comprehensive prompt engineering toolkit with template management, few-shot learning, chain-of-thought prompting, and advanced optimization techniques.

## Features

- **📝 Template Management**: Create, manage, and version prompt templates with multiple template engines
- **🎯 Few-Shot Learning**: Dynamic example selection with relevance-based, quality-based, and diverse strategies
- **🧠 Chain-of-Thought**: Step-by-step reasoning prompts with multiple style options
- **⚡ Prompt Optimization**: Automatic optimization for tokens, clarity, and cost
- **🔬 A/B Testing**: Compare prompt variants with statistical analysis
- **✅ Validation**: Template syntax and structure validation
- **📊 Metrics**: Token counting, readability scoring, and cost estimation

## Project Structure

```
Module-04-Prompt-Engineering/
├── Controllers/
│   └── PromptController.cs         # REST API endpoints
├── Extensions/
│   └── ServiceCollectionExtensions.cs # DI configuration
├── Models/
│   └── PromptModels.cs            # Data models
├── Services/
│   ├── PromptTemplateService.cs   # Template engine
│   ├── FewShotAndChainOfThoughtServices.cs # Learning services
│   └── PromptOptimizationService.cs # Optimization engine
├── AI.PromptEngineering.csproj    # Project file
├── Program.cs                     # Application startup
├── appsettings.json              # Configuration
└── README.md                     # This file
```

## Quick Start

### 1. Run the Application

```bash
dotnet restore
dotnet run
```

### 2. Access Swagger UI

Navigate to: `https://localhost:5001/swagger`

## API Endpoints

### Template Management

#### Create Template
```http
POST /api/prompt/templates
Content-Type: application/json

{
  "name": "Code Review",
  "description": "Generate code review comments",
  "content": "Review the following {{language}} code:\n\n{{code}}\n\nProvide feedback on:",
  "engine": "Scriban",
  "variables": [
    {
      "name": "language",
      "type": "string",
      "required": true
    },
    {
      "name": "code",
      "type": "string",
      "required": true
    }
  ],
  "category": "Code Analysis",
  "tags": ["code", "review", "analysis"]
}
```

#### Render Template
```http
POST /api/prompt/templates/{id}/render
Content-Type: application/json

{
  "language": "C#",
  "code": "public class Example { }"
}
```

### Few-Shot Learning

#### Generate Few-Shot Prompt
```http
POST /api/prompt/few-shot
Content-Type: application/json

{
  "taskDescription": "Classify the sentiment of customer reviews",
  "userInput": "The product exceeded my expectations!",
  "examples": [
    {
      "input": "Amazing quality, very satisfied!",
      "output": "Positive",
      "explanation": "Expresses strong satisfaction"
    },
    {
      "input": "Disappointed with the purchase",
      "output": "Negative",
      "explanation": "Clearly indicates dissatisfaction"
    }
  ],
  "config": {
    "maxExamples": 5,
    "selectionStrategy": "MostRelevant"
  }
}
```

### Chain-of-Thought

#### Generate CoT Prompt
```http
POST /api/prompt/chain-of-thought
Content-Type: application/json

{
  "problem": "Calculate the total cost of 15 items at $12.99 each with 8% sales tax",
  "config": {
    "style": "Detailed",
    "includeExplanations": true,
    "showIntermediateSteps": true,
    "guidingQuestions": [
      "What is the subtotal?",
      "How much is the tax?",
      "What is the final total?"
    ]
  }
}
```

### Prompt Optimization

#### Optimize Prompt
```http
POST /api/prompt/optimize
Content-Type: application/json

{
  "prompt": "Please provide a very detailed and comprehensive analysis...",
  "options": {
    "optimizeForTokens": true,
    "optimizeForClarity": true,
    "removeRedundancy": true,
    "maxTokens": 500
  }
}
```

#### Get Improvement Suggestions
```http
POST /api/prompt/optimize/suggestions
Content-Type: application/json

"Your verbose prompt text here..."
```

### A/B Testing

#### Create A/B Test
```http
POST /api/prompt/ab-test
Content-Type: application/json

{
  "name": "Summarization Style Test",
  "description": "Compare concise vs detailed summarization",
  "variantA": "Provide a brief summary in 2-3 sentences",
  "variantB": "Provide a detailed summary covering all key points"
}
```

#### Record Test Result
```http
POST /api/prompt/ab-test/{testId}/record
Content-Type: application/json

{
  "variant": "A",
  "qualityScore": 8.5,
  "responseTime": 1.2,
  "cost": 0.003,
  "success": true
}
```

#### Get Winner
```http
GET /api/prompt/ab-test/{testId}/winner
```

## Usage Examples

### 1. Template Management

```csharp
// Create a template
var template = new PromptTemplate
{
    Name = "Summarization",
    Content = "Summarize the following in {{length}} sentences:\n\n{{text}}",
    Engine = TemplateEngine.Scriban,
    Variables = new List<TemplateVariable>
    {
        new() { Name = "text", Required = true },
        new() { Name = "length", DefaultValue = "3" }
    }
};

var created = await templateService.CreateTemplateAsync(template);

// Render template
var request = new PromptTemplateRequest
{
    TemplateId = created.Id,
    Variables = new Dictionary<string, object>
    {
        ["text"] = "Long article text...",
        ["length"] = "5"
    }
};

var response = await templateService.RenderTemplateAsync(request);
```

### 2. Few-Shot Learning

```csharp
// Generate few-shot prompt
var fewShotRequest = new FewShotPromptRequest
{
    TaskDescription = "Translate English to French",
    UserInput = "Hello, how are you?",
    Examples = new List<FewShotExample>
    {
        new() 
        { 
            Input = "Good morning", 
            Output = "Bonjour",
            Quality = 1.0
        },
        new() 
        { 
            Input = "Thank you", 
            Output = "Merci",
            Quality = 1.0
        }
    },
    Config = new FewShotConfig
    {
        MaxExamples = 3,
        SelectionStrategy = ExampleSelectionStrategy.HighestQuality
    }
};

var response = await fewShotService.GenerateFewShotPromptAsync(fewShotRequest);
Console.WriteLine(response.Prompt);
```

### 3. Chain-of-Thought

```csharp
// Generate chain-of-thought prompt
var cotRequest = new ChainOfThoughtPromptRequest
{
    Problem = "If a train travels 60 miles in 45 minutes, what is its speed in mph?",
    Config = new ChainOfThoughtConfig
    {
        Style = ChainOfThoughtStyle.Structured,
        IncludeExplanations = true,
        MaxSteps = 5
    }
};

var response = await cotService.GenerateChainOfThoughtPromptAsync(cotRequest);
Console.WriteLine(response.Prompt);
```

### 4. Prompt Optimization

```csharp
// Optimize prompt
var options = new PromptOptimizationOptions
{
    OptimizeForTokens = true,
    OptimizeForClarity = true,
    RemoveRedundancy = true,
    MaxTokens = 300
};

var result = await optimizationService.OptimizePromptAsync(longPrompt, options);

Console.WriteLine($"Original tokens: {result.OriginalMetrics.TokenCount}");
Console.WriteLine($"Optimized tokens: {result.OptimizedMetrics.TokenCount}");
Console.WriteLine($"Improvement: {result.ImprovementPercentage:F2}%");
```

### 5. A/B Testing

```csharp
// Create test
var test = await optimizationService.CreateABTestAsync(
    variantA: "Explain in simple terms:",
    variantB: "Provide a detailed technical explanation:",
    testName: "Explanation Style",
    description: "Test simple vs technical explanations"
);

// Record results
await optimizationService.RecordABTestResultAsync(
    test.Id, "A", qualityScore: 8.5, responseTime: 1.2, cost: 0.003, success: true);

await optimizationService.RecordABTestResultAsync(
    test.Id, "B", qualityScore: 7.8, responseTime: 2.1, cost: 0.005, success: true);

// Get winner (after sufficient data)
var winner = await optimizationService.GetABTestWinnerAsync(test.Id);
Console.WriteLine($"Winner: Variant {winner}");
```

## Template Engines

### Scriban (Default)

```scriban
Hello {{name}},

{{if premium}}
Welcome to our premium service!
{{else}}
Welcome to our service!
{{end}}

Your benefits:
{{for benefit in benefits}}
- {{benefit}}
{{end}}
```

### Handlebars

```handlebars
Hello {{name}},

{{#if premium}}
Welcome to our premium service!
{{else}}
Welcome to our service!
{{/if}}

Your benefits:
{{#each benefits}}
- {{this}}
{{/each}}
```

### Simple

```
Hello {{name}},

Welcome to {{service}}!
```

## Example Selection Strategies

1. **MostRelevant**: Uses fuzzy matching to find most similar examples
2. **MostRecent**: Selects newest examples first
3. **HighestQuality**: Prioritizes examples with highest quality scores
4. **Random**: Random selection for diversity
5. **Diverse**: Maximizes diversity while maintaining relevance

## Chain-of-Thought Styles

1. **Concise**: Brief step-by-step with minimal explanation
2. **Detailed**: Comprehensive reasoning with full explanations
3. **Structured**: Formal numbered steps with clear sections
4. **Conversational**: Natural language thinking process

## Optimization Techniques

### Token Optimization
- Removes excessive whitespace
- Eliminates filler words
- Condenses redundant phrases

### Clarity Enhancement
- Adds paragraph breaks
- Structures long content
- Improves readability

### Cost Optimization
- Combines token and redundancy optimization
- Prioritizes brevity while maintaining meaning
- Estimates and minimizes API costs

## Best Practices

### Template Design

1. **Use Clear Variable Names**
   ```
   Good: {{user_query}}, {{max_results}}
   Bad: {{x}}, {{y}}
   ```

2. **Provide Default Values**
   ```json
   {
     "name": "limit",
     "defaultValue": "10"
   }
   ```

3. **Document Variables**
   ```json
   {
     "name": "tone",
     "description": "Response tone (formal/casual)",
     "allowedValues": ["formal", "casual"]
   }
   ```

### Few-Shot Selection

1. **Quality Over Quantity**: 3-5 high-quality examples better than 10 mediocre
2. **Diverse Examples**: Cover different scenarios and edge cases
3. **Similar Context**: Examples should match target task closely
4. **Clear Patterns**: Ensure examples demonstrate consistent patterns

### Chain-of-Thought

1. **Break Down Complex Problems**: Multiple simple steps better than one complex step
2. **Use Guiding Questions**: Help structure the reasoning process
3. **Show Work**: Include intermediate calculations and reasoning
4. **Verify Results**: Include validation steps

### Optimization

1. **Measure First**: Get metrics before optimizing
2. **Iterative Approach**: Optimize incrementally
3. **A/B Test Changes**: Validate improvements with data
4. **Balance Trade-offs**: Consider token cost vs output quality

## Configuration

### appsettings.json

```json
{
  "PromptEngineering": {
    "TemplateStoragePath": "Templates",
    "DefaultTemplateEngine": "Scriban",
    "EnableCaching": true,
    "CacheExpirationMinutes": 60,
    "EnableOptimization": true,
    "EnableVersioning": true
  }
}
```

## Metrics and Analytics

### Prompt Metrics
- **Token Count**: Estimated tokens (4 chars ≈ 1 token)
- **Character Count**: Total characters
- **Word Count**: Total words
- **Readability Score**: Flesch Reading Ease (0-100)
- **Clarity Score**: Structure and complexity rating
- **Estimated Cost**: Based on token count and model pricing

### A/B Test Metrics
- **Quality Score**: Average quality rating
- **Success Rate**: Percentage of successful executions
- **Response Time**: Average processing time
- **Cost**: Average API cost per request
- **Error Count**: Total errors encountered

## Troubleshooting

### Template Not Rendering

**Issue**: Template variables not replaced

**Solution**: 
- Check variable names match exactly (case-sensitive)
- Ensure all required variables provided
- Validate template syntax for chosen engine

### Few-Shot Examples Not Selected

**Issue**: No examples returned

**Solution**:
- Verify examples exist with matching tags
- Check MaxExamples setting
- Ensure examples have valid Input/Output

### Optimization Not Working

**Issue**: Prompt unchanged after optimization

**Solution**:
- Enable optimization options explicitly
- Check if prompt already optimal
- Review optimization settings

## Performance Tips

1. **Enable Caching**: Reduces repeated template parsing
2. **Batch Operations**: Use batch endpoints for multiple prompts
3. **Optimize Templates**: Keep templates concise
4. **Limit Examples**: 3-5 examples usually sufficient
5. **Use Simple Engine**: For basic templates, simple engine is fastest

## License

MIT License

## Contributing

Contributions welcome! Please submit pull requests or open issues.

## Support

For questions and support:
- GitHub Issues
- Documentation
- API Reference

## Version History

### 1.0.0
- Initial release
- Template management with versioning
- Few-shot learning with multiple strategies
- Chain-of-thought prompting
- Prompt optimization
- A/B testing framework
- Comprehensive API endpoints