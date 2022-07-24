using Microsoft.ML.Data;
using System;

namespace CoinTrader.ML
{
    /// <summary>
    /// 캔들 구조
    /// </summary>
    public class ModelInput
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
        /// 캔들 기준 시각(KST 기준)
        /// </summary>
        [LoadColumn(2)]
        public string candle_date_time_kst;
        /// <summary>
        /// 종가
        /// </summary>
        [LoadColumn(6)]
        public float trade_price;
        /// <summary>
        /// 마지막 틱이 저장된 시각
        /// </summary>
        [LoadColumn(7)]
        public double timestamp;
    }

    public class ModelOutput
    {
        /// <summary>
        /// ForecastedRentals: 예측 기간의 예측 값입니다.
        /// </summary>
        public float[] ForecastedTradePrice { get; set; }

        /// <summary>
        /// LowerBoundRentals: 예측 기간의 예측 최소값입니다.
        /// </summary>
        public float[] LowerBoundTradePrice { get; set; }

        /// <summary>
        /// UpperBoundRentals: 예측 기간의 예측 최대값입니다.
        /// </summary>
        public float[] UpperBoundTradePrice { get; set; }
    }
}