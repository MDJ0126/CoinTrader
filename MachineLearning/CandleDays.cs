using Microsoft.ML.Data;

namespace CoinTrader.ML
{
    public class CandleDays
    {
        /// <summary>
        /// 마켓명
        /// </summary>
        [LoadColumn(0)]
        public string market;
        /// <summary>
        /// 캔들 기준 시각(UTC 기준)
        /// </summary>
        [LoadColumn(1)]
        public string candle_date_time_utc;
        /// <summary>
        /// 종가
        /// </summary>
        [LoadColumn(6)]
        public float trade_price;
    }

    public class CandleDaysPrediction
    {
        [ColumnName("Score")]
        public float trade_price;
    }
}