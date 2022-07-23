using Microsoft.ML;
using System;
using System.IO;

namespace CoinTrader.ML
{
    public static class MachineLearning
    {
        public static void CreateCSV<T>(System.Collections.Generic.ICollection<T> collection, string name)
        {
            Utils.CreateCSVFile(collection, Path.Combine(Environment.CurrentDirectory, "Data", name));
        }

        /// <summary>
        /// 예상 가격 가져오기
        /// </summary>
        /// <param name="market">마켓 이름</param>
        /// <param name="date">예상 날짜</param>
        /// <returns>예상 가격</returns>
        public static double GetPredictePrice(string market, DateTime date)
        {
            MLContext mlContext = new MLContext(seed: 0);

            var path = Path.Combine(Utils.CSV_DATA_PATH, market + ".csv");

            // 학습하기
            var model = Train(mlContext, path);

            // 평가하기
            Evaluate(mlContext, model, path);

            // 예상 가격 추출
            var predictionFunction = mlContext.Model.CreatePredictionEngine<CandleDays, CandleDaysPrediction>(model);
            var candleDaysSample = new CandleDays()
            {
                market = market,
                candle_date_time_utc = date.ToString()
            };
            var prediction = predictionFunction.Predict(candleDaysSample);
            return prediction.trade_price;
        }

        /// <summary>
        /// 학습하기
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="dataPath"></param>
        /// <returns></returns>
        public static ITransformer Train(MLContext mlContext, string dataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<CandleDays>(dataPath, hasHeader: true, separatorChar: ',');

            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(CandleDaysPrediction.trade_price))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedMarket", inputColumnName: nameof(CandleDays.market)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedCandle_date_time_utc", inputColumnName: nameof(CandleDays.candle_date_time_utc)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedCandle_date_time_kst", inputColumnName: nameof(CandleDays.candle_date_time_kst)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedOpening_price", inputColumnName: nameof(CandleDays.opening_price)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedHigh_price", inputColumnName: nameof(CandleDays.high_price)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedLow_price", inputColumnName: nameof(CandleDays.low_price)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedTimestamp", inputColumnName: nameof(CandleDays.timestamp)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedCandle_acc_trade_price", inputColumnName: nameof(CandleDays.candle_acc_trade_price)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedCandle_acc_trade_volume", inputColumnName: nameof(CandleDays.candle_acc_trade_volume)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedPrev_closing_price", inputColumnName: nameof(CandleDays.prev_closing_price)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedChange_price", inputColumnName: nameof(CandleDays.change_price)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedChange_rate", inputColumnName: nameof(CandleDays.change_rate)))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "EncodedConverted_trade_price", inputColumnName: nameof(CandleDays.converted_trade_price)))
                    .Append(mlContext.Transforms.Concatenate("Features", "EncodedMarket", "EncodedCandle_date_time_utc", "EncodedCandle_date_time_kst", "EncodedOpening_price", 
                    "EncodedHigh_price", "EncodedLow_price", "trade_price", "EncodedTimestamp", "EncodedCandle_acc_trade_price", "EncodedCandle_acc_trade_volume",
                    "EncodedPrev_closing_price", "EncodedChange_price", "EncodedChange_rate", "EncodedConverted_trade_price"))
                    .Append(mlContext.Regression.Trainers.FastTree());

            // 모델 학습
            var model = pipeline.Fit(dataView);
            return model;
        }

        /// <summary>
        /// 평가하기
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        private static void Evaluate(MLContext mlContext, ITransformer model, string dataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<CandleDays>(dataPath, hasHeader: true, separatorChar: ',');
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
        }
    }
}
