# Module 11: Model Deployment & API Integration

Deploy ML.NET models as web APIs and containerized applications.

## Learning Objectives
- Create REST APIs for ML models
- Deploy with Docker
- Model versioning
- Production best practices

## Example: ASP.NET Core API

```csharp
[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly PredictionEnginePool<InputData, OutputData> _predictionEngine;

    [HttpPost("predict")]
    public IActionResult Predict([FromBody] InputData input)
    {
        var prediction = _predictionEngine.Predict(input);
        return Ok(prediction);
    }
}
```

---
**Previous:** [Module 10](./10-NLP-Text-Analysis.md) | **Next:** [Module 12](./12-AI.NET-Azure-Integration.md)
