// This file was auto-generated by ML.NET Model Builder. 

using System;
using CoinTraderML.Model;

namespace CoinTraderML.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create single instance of sample data from first line of dataset for model input
            ModelInput sampleData = new ModelInput()
            {
                Candle_date_time_utc = @"2022-07-23 토요일 오전 12:00:00",
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            Console.WriteLine("Using model to make single prediction -- Comparing actual Trade_price with predicted Trade_price from sample data...\n\n");
            Console.WriteLine($"Candle_date_time_utc: {sampleData.Candle_date_time_utc}");
            Console.WriteLine($"\n\nPredicted Trade_price: {predictionResult.Score}\n\n");
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }
    }
}