using Microsoft.ML;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// '시계열' 예제: https://docs.microsoft.com/ko-kr/dotnet/machine-learning/tutorials/time-series-demand-forecasting

namespace CoinTrader.ML
{
    public static class MachineLearning
    {
        static MachineLearning()
        {
            Initialize();
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public static void Initialize()
        {
            var path = Utils.CSV_DATA_PATH;
            Utils.DeleteDirectory(path);
        }

        /// <summary>
        /// 학습 CSV 파일 만들기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        public static bool CreateTrainCSV<T>(System.Collections.Generic.ICollection<T> collection, string name, string type)
        {
            string path = Path.Combine(Utils.CSV_DATA_PATH, type, name);
            return Utils.CreateCSVFile(collection, path, overwrite: false);
        }

        /// <summary>
        /// 학습 CSV 파일 이어쓰기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool AppendTrainCSV<T>(System.Collections.Generic.ICollection<T> collection, string name, string type)
        {
            string path = Path.Combine(Utils.CSV_DATA_PATH, type, name);
            return Utils.AppendCSVFile(collection, path);
        }

        /// <summary>
        /// 예상 가격 가져오기
        /// </summary>
        /// <param name="market">마켓 이름</param>
        /// <param name="date">예상 날짜</param>
        /// <returns>예상 가격</returns>
        public static double GetPredictePrice(string market, DateTime date, string type)
        {
            var path = Path.Combine(Utils.CSV_DATA_PATH, type, market + ".csv");
            var modelPath = Path.Combine(Utils.CSV_DATA_PATH, "Models", $"{market}_Model.zip");

            MLContext mlContext = new MLContext(/*seed: 0*/);

            // 데이터뷰 생성
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(path: path, hasHeader: true, separatorChar: ',');

            // 평가
            IDataView firstYearData = mlContext.Data.FilterRowsByColumn(dataView, "timestamp", upperBound: 1);
            IDataView secondYearData = mlContext.Data.FilterRowsByColumn(dataView, "timestamp", lowerBound: 1);

            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                                    outputColumnName: "ForecastedTradePrice",
                                    inputColumnName: "trade_price",
                                    windowSize: 7,
                                    seriesLength: 30,
                                    trainSize: 365,
                                    horizon: 7,
                                    confidenceLevel: 0.95f,
                                    confidenceLowerBoundColumn: "LowerBoundTradePrice",
                                    confidenceUpperBoundColumn: "UpperBoundTradePrice");

            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(firstYearData);
            Evaluate(secondYearData, forecaster, mlContext);

            // 모델 저장
            var forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
            Utils.CreatePathFolder(modelPath);
            forecastEngine.CheckPoint(mlContext, modelPath);

            // 예측하기
            Forecast(secondYearData, 7, forecastEngine, mlContext);

            return 0d;
        }

        /// <summary>
        /// 평가
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="model"></param>
        /// <param name="mlContext"></param>
        private static void Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
        {
            IDataView predictions = model.Transform(testData);

            // 실제값 가져오기
            IEnumerable<float> actual = mlContext.Data.CreateEnumerable<ModelInput>(testData, true).Select(observed => observed.trade_price);

            // 예측값 가져오기
            IEnumerable<float> forecast = mlContext.Data.CreateEnumerable<ModelOutput>(predictions, true).Select(prediction => prediction.ForecastedTradePrice[0]);

            // 일반적으로 오류라고 하는 실제 값과 예측 값의 차이를 계산합니다.
            var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

            // 절대 평균 오차 및 제곱 평균 오차 값을 계산하여 성능을 측정합니다.
            // 1) 절대 평균 오차: 예측이 실제 값과 얼마나 근접한지 측정합니다. 이 값의 범위는 0과 무한대 사이입니다. 0에 가까울수록 모델의 품질이 좋습니다.
            var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
            // 2) 제곱 평균 오차: 모델의 오류를 요약합니다. 이 값의 범위는 0과 무한대 사이입니다. 0에 가까울수록 모델의 품질이 좋습니다.
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error

            Console.WriteLine("Evaluation Metrics");
            Console.WriteLine("---------------------");
            Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
            Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");
        }

        /// <summary>
        /// 모델을 사용하여 값 예측하기
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="horizon"></param>
        /// <param name="forecaster"></param>
        /// <param name="mlContext"></param>
        private static void Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecaster, MLContext mlContext)
        {
            ModelOutput forecast = forecaster.Predict();
            IEnumerable<string> forecastOutput = 
                mlContext.Data.CreateEnumerable<ModelInput>(testData, reuseRowObject: false)
                .Take(horizon)
                .Select((ModelInput rental, int index) =>
                {
                    string rentalDate = rental.candle_date_time_utc;
                    float actualRentals = rental.trade_price;
                    float lowerEstimate = Math.Max(0, forecast.LowerBoundTradePrice[index]);
                    float estimate = forecast.ForecastedTradePrice[index];
                    float upperEstimate = forecast.UpperBoundTradePrice[index];
                    return $"Date: {rentalDate}\n" +
                    $"Actual Rentals: {actualRentals}\n" +
                    $"Lower Estimate: {lowerEstimate}\n" +
                    $"Forecast: {estimate}\n" + // <- Predict
                    $"Upper Estimate: {upperEstimate}\n";
                });

            Console.WriteLine("Rental Forecast");
            Console.WriteLine("---------------------");
            foreach (var prediction in forecastOutput)
            {
                Console.WriteLine(prediction);
            }
        }
    }
}
