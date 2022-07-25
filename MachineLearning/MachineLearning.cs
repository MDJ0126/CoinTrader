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
            //Initialize();
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
        /// <param name="market"></param>
        public static bool CreateTrainCSV<T>(ICollection<T> collection, string market)
        {
            string path = Path.Combine(Utils.CSV_DATA_PATH, market);
            return Utils.CreateCSVFile(collection, path, overwrite: false, writeHeader: false);
        }

        /// <summary>
        /// 학습 CSV 파일 이어쓰기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="market"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool AppendTrainCSV<T>(ICollection<T> collection, string market)
        {
            string path = Path.Combine(Utils.CSV_DATA_PATH, market);
            return Utils.AppendCSVFile(collection, path, pasteInFront: true);
        }

        /// <summary>
        /// 학습 가장 오래된 날짜 가져오기
        /// </summary>
        /// <param name="market"></param>
        /// <returns></returns>
        public static DateTime GetFirstDateTime(string market)
        {
            DateTime dateTime = DateTime.MaxValue;
            string path = Path.Combine(Utils.CSV_DATA_PATH, market + ".csv");
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    var line = reader.ReadLine();
                    string date = line.Split(',')[1];
                    DateTime.TryParse(date, out dateTime);
                    reader.Close();
                }
            }
            return dateTime;
        }

        /// <summary>
        /// 학습 가장 최근 날짜 가져오기
        /// </summary>
        /// <param name="market"></param>
        /// <returns></returns>
        public static DateTime GetLastDateTime(string market)
        {
            DateTime dateTime = DateTime.MaxValue;
            string path = Path.Combine(Utils.CSV_DATA_PATH, market + ".csv");
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        string date = line.Split(',')[1];
                        DateTime.TryParse(date, out dateTime);
                    }
                    reader.Close();
                }
            }
            return dateTime;
        }

        /// <summary>
        /// 예상 가격 가져오기
        /// </summary>
        /// <param name="market">마켓 이름</param>
        /// <param name="date">예상 날짜</param>
        /// <returns>예상 가격</returns>
        public static ModelOutput GetPredictePrice(string market)
        {
            var path = Path.Combine(Utils.CSV_DATA_PATH, market + ".csv");
            var modelPath = Path.Combine(Utils.CSV_DATA_PATH, "Models", $"{market}_Model.zip");

            int row = 200;
            if (File.Exists(path))
            {
                string[] strs = File.ReadAllLines(path);
                row = strs.Length;
            }
            MLContext mlContext = new MLContext(/*seed: 0*/);

            // 데이터뷰 생성
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(path: path, hasHeader: false, separatorChar: ',');

            var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                                    inputColumnName: "trade_price", // 추척할 데이터 컬럼
                                    windowSize: 21,  // 예측 전 최종적으로 결정짓는 요소: 이전(최근) '14'개 정보
                                    trainSize: row, // 총 학습할 200개
                                    seriesLength: 24,   // 24개로 분할하여 학습한다.
                                    horizon: 24 * 7,     // 예측 24개을 요청
                                    confidenceLevel: 0.95f, // 신뢰 수준 95%

                                    outputColumnName: "Forecasted",               // 평균 추정치
                                    confidenceLowerBoundColumn: "LowerBound",     // 최상의 경우
                                    confidenceUpperBoundColumn: "UpperBound");    // 최악의 경우
            #region 파이프라인 MSD 메모

            /* forecastingPipeline은 첫 번째 연도와 샘플에 대해 365개의 데이터 요소를 사용하거나 시계열 데이터 세트를 seriesLength 매개 변수에
            지정된 대로 30일(월별) 간격으로 분할합니다. 이러한 각 샘플은 주별 또는 7일 기간을 통해 분석됩니다. 다음 기간에 대한 예측 값을 결정할 때
            이전 7일의 값을 사용하여 예측을 수행합니다. 모델은 horizon 매개 변수로 정의된 대로 향후 7일의 기간을 예측하도록 설정됩니다.
            예측은 추측이므로 항상 100% 정확하지는 않습니다. 따라서 상한 및 하한으로 정의된 최선 및 최악의 시나리오 값 범위를 파악하고 있는 것이 좋습니다.
            이 경우 하한 및 상한에 대한 신뢰 수준은 95%로 설정됩니다. 신뢰 수준을 적절하게 높이거나 줄일 수 있습니다. 값이 높을수록 원하는 수준의 신뢰도를
            얻기 위해 상한 및 하한 간 범위가 넓어집니다. */

            #endregion

            // 모델 학습하기
            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(dataView);

            // 평가해보기 (테스트, 현재 모델과 새로운 데이터와 비교했을 때 얼마나 차이가 발생하는지 알 수 있다.)
            //Evaluate(testData, forecaster, mlContext);

            // 학습된 모델 파일 저장(.zip)
            var forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(mlContext);
            Utils.CreatePathFolder(modelPath);
            forecastEngine.CheckPoint(mlContext, modelPath);

            // 예측하기
            ModelOutput forecast = forecastEngine.Predict();
            return forecast;
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
            IEnumerable<float> forecast = mlContext.Data.CreateEnumerable<ModelOutput>(predictions, true).Select(prediction => prediction.Forecasted[0]);

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
    }
}
