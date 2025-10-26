# ML.NET Examples

This directory contains runnable examples for each module.

## Structure

```
examples/
├── data/                   # Sample datasets
├── models/                 # Trained model files (.zip)
└── src/
    ├── HelloMLNet/        # Getting started example
    ├── Regression/        # Regression examples
    ├── Classification/    # Classification examples
    ├── Clustering/        # Clustering examples
    ├── AnomalyDetection/  # Anomaly detection examples
    ├── Recommendation/    # Recommendation examples
    ├── TimeSeries/        # Time series examples
    ├── ComputerVision/    # Computer vision examples
    ├── NLP/               # NLP examples
    ├── WebAPI/            # API deployment examples
    └── Capstone/          # E-commerce capstone project
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Run Examples

```bash
# Navigate to an example
cd src/HelloMLNet

# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

## Examples by Module

### Module 00-01: Hello ML.NET
- `src/HelloMLNet` - Basic ML.NET setup and first model

### Module 03: Regression
- House price prediction
- Sales forecasting

### Module 04: Classification
- Sentiment analysis
- Email spam detection

### Module 05: Clustering
- Customer segmentation

### Module 06: Anomaly Detection
- Fraud detection

### Module 07: Recommendation
- Product recommendations

### Module 08: Time Series
- Sales forecasting

### Module 09: Computer Vision
- Image classification

### Module 10: NLP
- Text classification

### Module 11: Deployment
- Web API examples

### Module 13: Capstone
- Complete e-commerce ML system

## Sample Datasets

Sample datasets are provided in the `data/` directory:
- `housing.csv` - House prices
- `reviews.csv` - Product reviews
- `customers.csv` - Customer data
- `transactions.csv` - Transaction data
- `sales.csv` - Sales data

## Notes

- Models are saved in the `models/` directory as `.zip` files
- All examples use ML.NET 3.0+
- Examples are self-contained and can be run independently
