using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// '시계열' 예제: https://docs.microsoft.com/ko-kr/dotnet/machine-learning/tutorials/time-series-demand-forecasting

namespace CoinTrader.ML
{
    public static class MachineLearning
    {
        /// <summary>
        /// 데이터 Dictionary
        /// </summary>
        private static Dictionary<string, List<CandlesData>> CandleDatas = new Dictionary<string, List<CandlesData>>();

        /// <summary>
        /// 로드
        /// </summary>
        public static void Initialize(Action<float> onProgress = null)
        {
            var dirs = Directory.GetDirectories(Utils.CSV_DATA_PATH);
            for (int i = 0; i < dirs.Length; i++)
            {
                var market = Path.GetFileName(dirs[i]);
                if (!CandleDatas.TryGetValue(market, out List<CandlesData> datas))
                {
                    datas = LoadData(market);
                    CandleDatas.Add(market, datas);
                }
                onProgress?.Invoke((float)i / dirs.Length);
            }
        }

        /// <summary>
        /// 데이터 로드
        /// </summary>
        /// <param name="market"></param>
        /// <returns></returns>
        private static List<CandlesData> LoadData(string market)
        {
            List<CandlesData> modelInputs = new List<CandlesData>();
            string path = Path.Combine(Utils.CSV_DATA_PATH, market, market);
            var strs = Utils.OpenCSVFile(path);
            try
            {
                if (strs != null)
                {
                    for (int i = 0; i < strs.GetLength(0); i++)
                    {
                        CandlesData modelInput = new CandlesData();
                        modelInput.market = strs[i, (int)eModelInput.market];
                        DateTime.TryParse(strs[i, (int)eModelInput.candle_date_time_utc], out modelInput.candle_date_time_utc);
                        DateTime.TryParse(strs[i, (int)eModelInput.candle_date_time_kst], out modelInput.candle_date_time_kst);
                        double.TryParse(strs[i, (int)eModelInput.opening_price], out modelInput.opening_price);
                        double.TryParse(strs[i, (int)eModelInput.high_price], out modelInput.high_price);
                        double.TryParse(strs[i, (int)eModelInput.low_price], out modelInput.low_price);
                        float.TryParse(strs[i, (int)eModelInput.trade_price], out modelInput.trade_price);
                        double.TryParse(strs[i, (int)eModelInput.timestamp], out modelInput.timestamp);
                        double.TryParse(strs[i, (int)eModelInput.candle_acc_trade_price], out modelInput.candle_acc_trade_price);
                        double.TryParse(strs[i, (int)eModelInput.candle_acc_trade_volume], out modelInput.candle_acc_trade_volume);
                        modelInputs.Add(modelInput);
                    }
                }
            }
            catch { }
            return modelInputs;
        }

        private static Mutex mutex_GetDatas = new Mutex();
        /// <summary>
        /// 캔들 리스트 가져오기
        /// </summary>
        /// <param name="market"></param>
        /// <returns></returns>
        private static List<CandlesData> GetDatas(string market)
        {
            mutex_GetDatas.WaitOne();
            if (!CandleDatas.TryGetValue(market, out List<CandlesData> datas))
            {
                datas = LoadData(market);
                CandleDatas.Add(market, datas);
            }
            mutex_GetDatas.ReleaseMutex();
            return datas;
        }

        /// <summary>
        /// 정보 추가
        /// </summary>
        /// <param name="market"></param>
        /// <param name="input"></param>
        public static void Add(string market, CandlesData input)
        {
            var datas = GetDatas(market);
            if (datas != null)
            {
                if (!datas.Exists(data => data.candle_date_time_utc == input.candle_date_time_utc))
                {
                    datas.Add(input);
                    Save(market);
                }
            }
        }

        /// <summary>
        /// 정보 추가
        /// </summary>
        /// <param name="market"></param>
        /// <param name="inputs"></param>
        public static void Add(string market, List<CandlesData> inputs)
        {
            var datas = GetDatas(market);
            if (datas != null)
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    CandlesData input = inputs[i];
                    if (input != null)
                    {
                        if (!datas.Exists(data => data.candle_date_time_utc == input.candle_date_time_utc))
                        {
                            datas.Add(input);
                        }
                    }
                }
                Save(market);
            }
        }

        /// <summary>
        /// 저장
        /// </summary>
        private static void Save(string market)
        {
            var datas = GetDatas(market);
            if (datas != null)
            {
                // 정렬
                datas.Sort((a, b) =>
                {
                    DateTime A = a.candle_date_time_utc;
                    DateTime B = b.candle_date_time_utc;
                    return A.CompareTo(B);
                });

                string path = Path.Combine(Utils.CSV_DATA_PATH, market, market);
                bool result = Utils.CreateCSVFile(datas, path, writeHeader: false);
            }
        }

        /// <summary>
        /// 전날 변동성 k 배수로 가져오기
        /// </summary>
        /// <param name="date"></param>
        /// <param name="k">배수 세팅 0f ~ 1f</param>
        /// <returns></returns>
        public static double GetTargetPrice(string market, DateTime date, float k)
        {
            k = Utils.Clamp(k, 0f, 1f);
            var datas = GetDatas(market);
            if (datas != null)
            {
                // 일자화
                date = date.Date;
                var candleData = datas.Find(data => data.candle_date_time_kst == date.AddDays(-1f));
                if (candleData != null)
                    return (candleData.high_price - candleData.low_price) * k;
            }
            return double.MaxValue;
        }

        /// <summary>
        /// 이동평균값 가져오기
        /// </summary>
        public static double GetMovingAverage(string market, DateTime date, int days)
        {
            var datas = GetDatas(market);
            if (datas != null)
            {
                // 일자화
                date = date.Date;
                var candleData = datas.Find(data => data.candle_date_time_kst == date.AddDays(-1f));
                if (candleData != null)
                {
                    int index = datas.IndexOf(candleData);
                    double total = 0f;
                    int i = 0;
                    int day = 0;
                    while (true)
                    {
                        int currentIndex = index - i;
                        if (currentIndex > 0)
                        {
                            if (datas[currentIndex].candle_date_time_kst.Hour == 0 && datas[currentIndex].candle_date_time_kst.Minute == 0)
                            {
                                total += datas[currentIndex].trade_price;
                                ++day;
                            }
                        }
                        else
                        {
                            total = 0f;
                            break;
                        }

                        if (day >= days)
                            break;

                        ++i;
                    }
                    double average = total / days;
                    return average;
                }
            }
            return 0f;
        }

        /// <summary>
        /// 학습 가장 오래된 날짜 가져오기
        /// </summary>
        public static DateTime GetOldDateTime(string market)
        {
            var datas = GetDatas(market);
            if (datas != null)
            {
                if (datas.Count > 0)
                {
                    return datas[0].candle_date_time_kst;
                }
            }
            return DateTime.MaxValue;
        }

        /// <summary>
        /// 학습 가장 최근 날짜 가져오기
        /// </summary>
        public static DateTime GetLastDateTime(string market)
        {
            var datas = GetDatas(market);
            if (datas != null)
            {
                if (datas.Count > 0)
                {
                    return datas[datas.Count - 1].candle_date_time_kst;
                }
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// 예상 가격 가져오기
        /// </summary>
        /// <param name="market">마켓 이름</param>
        /// <param name="date">예상 날짜</param>
        /// <returns>예상 가격</returns>
        public static ModelOutput GetPredictePrice(string market)
        {
            var path = Path.Combine(Utils.CSV_DATA_PATH, market, market + ".csv");
            var modelPath = Path.Combine(Utils.CSV_DATA_PATH, market, $"{market}.zip");

            var datas = GetDatas(market);
            if (datas.Count > 0)
            {
                if (File.Exists(path))
                {
                    string[] strs = File.ReadAllLines(path);
                    int row = strs.Length;
                    MLContext mlContext = new MLContext(/*seed: 0*/);

                    // 데이터뷰 생성
                    IDataView dataView = mlContext.Data.LoadFromTextFile<CandlesData>(path: path, hasHeader: false, separatorChar: ',');
                    var forecastingPipeline = mlContext.Forecasting.ForecastBySsa(
                                            inputColumnName: "trade_price", // 추척할 데이터 컬럼
                                            windowSize: 21,  // 예측 전 최종적으로 결정짓는 요소: 이전(최근) '14'개 정보
                                            trainSize: row, // 총 학습할 데이터 길이
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

                    // 평가해보기 (정확도 테스트, 현재 모델과 새로운 데이터와 비교했을 때 얼마나 차이가 발생하는지 알 수 있다.)
                    //Evaluate(testData, forecaster, mlContext);

                    // 학습된 모델 파일 저장(.zip)
                    var forecastEngine = forecaster.CreateTimeSeriesEngine<CandlesData, ModelOutput>(mlContext);
                    Utils.CreatePathFolder(modelPath);
                    forecastEngine.CheckPoint(mlContext, modelPath);

                    // 예측하기
                    ModelOutput forecast = forecastEngine.Predict();
                    return forecast;
                }
            }
            return null;
        }

        #region Evaluate

        /// <summary>
        /// 평가
        /// </summary>
        /// <param name="testData"></param>
        /// <param name="model"></param>
        /// <param name="mlContext"></param>
        //private static void Evaluate(IDataView testData, ITransformer model, MLContext mlContext)
        //{
        //    IDataView predictions = model.Transform(testData);

        //    // 실제값 가져오기
        //    IEnumerable<float> actual = mlContext.Data.CreateEnumerable<CandlesDaysRes>(testData, true).Select(observed => observed.trade_price);

        //    // 예측값 가져오기
        //    IEnumerable<float> forecast = mlContext.Data.CreateEnumerable<ModelOutput>(predictions, true).Select(prediction => prediction.Forecasted[0]);

        //    // 일반적으로 오류라고 하는 실제 값과 예측 값의 차이를 계산합니다.
        //    var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

        //    // 절대 평균 오차 및 제곱 평균 오차 값을 계산하여 성능을 측정합니다.
        //    // 1) 절대 평균 오차: 예측이 실제 값과 얼마나 근접한지 측정합니다. 이 값의 범위는 0과 무한대 사이입니다. 0에 가까울수록 모델의 품질이 좋습니다.
        //    var MAE = metrics.Average(error => Math.Abs(error)); // Mean Absolute Error
        //    // 2) 제곱 평균 오차: 모델의 오류를 요약합니다. 이 값의 범위는 0과 무한대 사이입니다. 0에 가까울수록 모델의 품질이 좋습니다.
        //    var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(error, 2))); // Root Mean Squared Error

        //    Console.WriteLine("Evaluation Metrics");
        //    Console.WriteLine("---------------------");
        //    Console.WriteLine($"Mean Absolute Error: {MAE:F3}");
        //    Console.WriteLine($"Root Mean Squared Error: {RMSE:F3}\n");
        //}

        #endregion
    }
}
