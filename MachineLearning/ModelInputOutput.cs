using Microsoft.ML.Data;

namespace CoinTrader.ML
{
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
        /// 예측 기간의 예측 값입니다.
        /// </summary>
        public float[] Forecasted { get; set; }

        /// <summary>
        /// 예측 기간의 예측 최소값입니다.
        /// </summary>
        public float[] LowerBound { get; set; }

        /// <summary>
        /// 예측 기간의 예측 최대값입니다.
        /// </summary>
        public float[] UpperBound { get; set; }
    }
}