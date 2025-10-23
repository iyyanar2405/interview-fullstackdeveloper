# Module 02: Quality Assessment

Analytical tooling for grading LLM responses with heuristics, bias detection, and human feedback capture.

## Features

- **Multi-Dimensional Scoring**: Relevance, coherence, correctness, and style evaluated on a 0-100 scale.
- **Weighted Aggregation**: Tunable weights determine overall quality pass/fail decisions.
- **Bias Heuristics**: Configurable lexicon flags potentially biased phrases with probability scores.
- **Feedback Loop**: Collect user ratings/comments and expose quick aggregates for review dashboards.
- **Rollup Metrics**: Rolling summaries for throughput windows, including pass rate and average scores.
- **Health & Logging**: Serilog pipeline and `/health` endpoint for operational checks.

## Project Layout

```
Module-02-Quality-Assessment/
├── Controllers/
│   └── QualityAssessmentController.cs
├── Models/
│   └── QualityAssessmentModels.cs
├── Services/
│   ├── BiasDetectionService.cs
│   ├── EvaluationPipelineService.cs
│   ├── FeedbackAggregationService.cs
│   ├── QualityMetricsService.cs
│   └── ResponseEvaluationService.cs
├── Module-02-Quality-Assessment.csproj
├── Program.cs
├── appsettings.json
└── README.md
```

## Endpoints

| Method | Route | Description |
| ------ | ----- | ----------- |
| POST | `/api/quality/evaluate` | Evaluate a prompt/response pair and return detailed scores. |
| GET | `/api/quality/summary?minutes=60` | Rolling quality metrics for the selected window. |
| GET | `/api/quality/recent?count=10` | Latest evaluation results (default 10). |
| POST | `/api/quality/feedback` | Submit human rating/comments for a response. |
| GET | `/api/quality/feedback/{responseId}` | Retrieve aggregate feedback for a specific response. |
| GET | `/health` | Health probe for runtime monitoring. |
| GET | `/swagger` | Swagger UI and OpenAPI spec. |

## Configuration Highlights (`appsettings.json`)

- `QualityAssessment.Weights`: Weighting factors for each dimension (total should be 1.0).
- `QualityAssessment.Thresholds`: Quality pass score, bias probability trigger, minimum dimension score.
- `QualityAssessment.CriticalKeywords`: Keywords that must appear in the response (optional).
- `QualityAssessment.BiasLexicon`: Categorized keyword lists for heuristic bias detection.

## Run the API

```powershell
cd Module-02-Quality-Assessment
 dotnet restore
 dotnet run
```

The service listens on the default ASP.NET Core URLs (configurable via `ASPNETCORE_URLS`).

## Example Evaluation Payload

```json
{
  "prompt": "Explain Newton's first law of motion.",
  "response": "Newton's first law says objects keep moving unless an external force acts.",
  "expectedAnswer": "An object remains at rest or in uniform motion unless acted upon by an external force.",
  "guidelines": ["use simple language", "include an example"],
  "metadata": { "scenario": "physics-quiz" }
}
```

---

Use this module with Module 01 (Performance Metrics) to build a comprehensive monitoring suite for AI quality assurance.
