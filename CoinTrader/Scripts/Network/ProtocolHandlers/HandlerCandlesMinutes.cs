using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Network
{
    /// <summary>
    /// 일(Day) 캔들
    /// </summary>
    public class CandlesMinutesRes : iResponse
    {
        /// <summary>
        /// 마켓명
        /// </summary>
        public string market;
        /// <summary>
        /// 캔들 기준 시각(UTC 기준)
        /// </summary>
        public string candle_date_time_utc;
        /// <summary>
        /// 캔들 기준 시각(KST 기준)
        /// </summary>
        public string candle_date_time_kst;
        /// <summary>
        /// 시가
        /// </summary>
        public double opening_price;
        /// <summary>
        /// 고가
        /// </summary>
        public double high_price;
        /// <summary>
        /// 저가
        /// </summary>
        public double low_price;
        /// <summary>
        /// 종가
        /// </summary>
        public double trade_price;
        /// <summary>
        /// 마지막 틱이 저장된 시각
        /// </summary>
        public long timestamp;
        /// <summary>
        /// 누적 거래 금액
        /// </summary>
        public double candle_acc_trade_price;
        /// <summary>
        /// 누적 거래량
        /// </summary>
        public double candle_acc_trade_volume;
        /// <summary>
        /// 분 단위(유닛)
        /// </summary>
        public int unit;

        /// <summary>
        /// 표준 시간 구분
        /// </summary>
        public enum eTimeType
        {
            /// <summary>
            /// 세계협정시
            /// </summary>
            UTC,
            /// <summary>
            /// 한국 표준 시간
            /// </summary>
            KST,
        }

        /// <summary>
        /// 거래 DateTime 가져오기
        /// </summary>
        /// <param name="timeType">표준 시간 기준</param>
        /// <returns>거래 시간</returns>
        public DateTime GetTradeDateTime(eTimeType timeType = eTimeType.UTC)
        {
            DateTime dateTime;
            switch (timeType)
            {
                case eTimeType.KST:
                    DateTime.TryParse(candle_date_time_kst, out dateTime);
                    break;
                case eTimeType.UTC:
                default: 
                    DateTime.TryParse(candle_date_time_utc, out dateTime);
                    break;
            }
            return dateTime;
        }
    }

    public class HandlerCandlesMinutes : ProtocolHandler
    {
        private List<CandlesMinutesRes> res = null;

        public HandlerCandlesMinutes()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "candles/minutes/");
            this.Method = Method.Get;
        }

        /// <summary>
        /// 요청
        /// </summary>
        /// <param name="unit">분 단위. 가능한 값 : 1, 3, 5, 15, 10, 30, 60, 240</param>
        /// <param name="market">마켓 코드 (ex. KRW-BTC)</param>
        /// <param name="to">마지막 캔들 시각 (exclusive). 포맷 : yyyy-MM-dd'T'HH:mm:ss'Z' or yyyy-MM-dd HH:mm:ss. 비워서 요청시 가장 최근 캔들</param>
        /// <param name="count">캔들 개수(최대 200개까지 요청 가능)</param>
        /// <param name="onFinished"></param>
        public void Request(int unit, string market, string to = "", int count = 200, Action<bool, List<CandlesMinutesRes>> onFinished = null)
        {
            if (string.IsNullOrEmpty(to))
                to = Time.NowTime.Date.ToString("yyyy-MM-dd HH:mm:ss");
            RestRequest request = new RestRequest(URI + $"{unit}?market={market}&to={to}&count={count}", Method);
            request.AddHeader("Accept", "application/json");
            base.RequestProcess(request, (result) => onFinished?.Invoke(result, res));
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<CandlesMinutesRes>(response.Content);
                ModelCenter.Market.UpdateCandlesMinutes(res);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
