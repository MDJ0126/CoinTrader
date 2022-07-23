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
    public class CandlesDaysRes : iResponse
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
        /// 전일 종가(UTC 0시 기준)
        /// </summary>
        public double prev_closing_price;
        /// <summary>
        /// 전일 종가 대비 변화 금액
        /// </summary>
        public double change_price;
        /// <summary>
        /// 전일 종가 대비 변화량
        /// </summary>
        public double change_rate;
        /// <summary>
        /// 종가 환산 화폐 단위로 환산된 가격(요청에 convertingPriceUnit 파라미터 없을 시 해당 필드 포함되지 않음.)
        /// </summary>
        public double converted_trade_price;

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

    public class HandlerCandlesDays : ProtocolHandler
    {
        private List<CandlesDaysRes> res = null;

        public HandlerCandlesDays()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "candles/days?");
            this.Method = Method.Get;
        }

        /// <summary>
        /// 요청
        /// </summary>
        /// <param name="market">마켓 코드 (ex. KRW-BTC)</param>
        /// <param name="to">마지막 캔들 시각 (exclusive). 포맷 : yyyy-MM-dd'T'HH:mm:ss'Z' or yyyy-MM-dd HH:mm:ss. 비워서 요청시 가장 최근 캔들</param>
        /// <param name="count">캔들 개수(최대 200개까지 요청 가능)</param>
        /// <param name="convertingPriceUnit">종가 환산 화폐 단위 (생략 가능, KRW로 명시할 시 원화 환산 가격을 반환.)</param>
        /// <param name="onFinished"></param>
        public void Request(string market, string to = "", int count = 200, string convertingPriceUnit = "KRW", Action<bool, List<CandlesDaysRes>> onFinished = null)
        {
            if (string.IsNullOrEmpty(to))
                to = Time.NowTime.Date.ToString("yyyy-MM-dd HH:mm:ss");
            RestRequest request = new RestRequest(URI + $"market={market}&to={to}&count={count}&convertingPriceUnit={convertingPriceUnit}", Method);
            request.AddHeader("Accept", "application/json");
            base.RequestProcess(request, (result) => onFinished?.Invoke(result, res));
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<CandlesDaysRes>(response.Content);
                ModelCenter.Market.UpdateCandlesDays(res);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
