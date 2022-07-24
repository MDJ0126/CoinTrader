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
        /// <summary>
        /// 전일 종가(UTC 0시 기준)
        /// </summary>
        [LoadColumn(10)]
        public double prev_closing_price;
        /// <summary>
        /// 전일 종가 대비 변화 금액
        /// </summary>
        [LoadColumn(11)]
        public double change_price;
        /// <summary>
        /// 전일 종가 대비 변화량
        /// </summary>
        [LoadColumn(12)]
        public double change_rate;
        /// <summary>
        /// 종가 환산 화폐 단위로 환산된 가격(요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.)
        /// </summary>
        [LoadColumn(13)]
        public double converted_trade_price;
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