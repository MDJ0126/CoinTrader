using Microsoft.ML.Data;
using System;

namespace CoinTrader.ML
{
    public enum eModelInput
    {
        market,
        candle_date_time_utc,
        candle_date_time_kst,
        opening_price,
        high_price,
        low_price,
        trade_price,
        timestamp,
        candle_acc_trade_price,
        candle_acc_trade_volume,
        prev_closing_price,
        change_price,
        change_rate,
        converted_trade_price,
    }

    public class CandlesDayData
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
        public DateTime candle_date_time_utc;
        /// <summary>
        /// 캔들 기준 시각(KST 기준)
        /// </summary>
        [LoadColumn(2)]
        public DateTime candle_date_time_kst;
        /// <summary>
        /// 시가
        /// </summary>
        [LoadColumn(3)]
        public double opening_price;
        /// <summary>
        /// 고가
        /// </summary>
        [LoadColumn(4)]
        public double high_price;
        /// <summary>
        /// 저가
        /// </summary>
        [LoadColumn(5)]
        public double low_price;
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
        /// <summary>
        /// 누적 거래 금액
        /// </summary>
        [LoadColumn(8)]
        public double candle_acc_trade_price;
        /// <summary>
        /// 누적 거래량
        /// </summary>
        [LoadColumn(9)]
        public double candle_acc_trade_volume;

        public override string ToString()
        {
            return base.ToString();
        }
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