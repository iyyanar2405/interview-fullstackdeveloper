using Microsoft.ML;
using Microsoft.ML.Data;

namespace HelloMLNet
{
    // Define data structure
    public class HouseData
    {
        public float Size { get; set; }
        public float Price { get; set; }
    }

    public class HousePrediction
    {
        [ColumnName("Score")]
        public float Price { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ML.NET Hello World Example ===\n");

            // Create ML Context
            var mlContext = new MLContext(seed: 0);
            Console.WriteLine("✓ ML.NET Context Created");

            // Create sample training data
            var trainingData = new[]
            {
                new HouseData { Size = 1000, Price = 100000 },
                new HouseData { Size = 1500, Price = 150000 },
                new HouseData { Size = 2000, Price = 200000 },
                new HouseData { Size = 2500, Price = 250000 },
                new HouseData { Size = 3000, Price = 300000 }
            };

            // Load data into IDataView
            IDataView dataView = mlContext.Data.LoadFromEnumerable(trainingData);
            Console.WriteLine("✓ Training Data Loaded");

            // Define ML pipeline
            var pipeline = mlContext.Transforms.Concatenate("Features", "Size")
                .Append(mlContext.Regression.Trainers.Sdca(
                    labelColumnName: "Price",
                    featureColumnName: "Features"));

            // Train the model
            Console.WriteLine("\nTraining model...");
            var model = pipeline.Fit(dataView);
            Console.WriteLine("✓ Model Trained Successfully");

            // Create prediction engine
            var predictionEngine = mlContext.Model
                .CreatePredictionEngine<HouseData, HousePrediction>(model);

            // Test predictions
            Console.WriteLine("\n=== Making Predictions ===\n");

            var testCases = new[]
            {
                new HouseData { Size = 1200 },
                new HouseData { Size = 1800 },
                new HouseData { Size = 2200 },
                new HouseData { Size = 2800 }
            };

            foreach (var testCase in testCases)
            {
                var prediction = predictionEngine.Predict(testCase);
                Console.WriteLine($"House Size: {testCase.Size:N0} sqft → Predicted Price: ${prediction.Price:N2}");
            }

            Console.WriteLine("\n✓ ML.NET Hello World Complete!");
        }
    }
}
