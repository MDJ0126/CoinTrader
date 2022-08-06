using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Network
{
    /// <summary>
    /// 현재가 정보: 요청 당시 종목의 스냅샷을 반환한다.
    /// </summary>
    public class TickerRes : iResponse
    {
        /// <summary>
        /// 종목 구분 코드
        /// </summary>
        public string market;
        /// <summary>
        /// 최근 거래 일자(UTC)
        /// </summary>
        public string trade_date;
        /// <summary>
        /// 최근 거래 시각(UTC)
        /// </summary>
        public string trade_time;
        /// <summary>
        /// 최근 거래 일자(KST)
        /// </summary>
        public string trade_date_kst;
        /// <summary>
        /// 최근 거래 시각(KST)
        /// </summary>
        public string trade_time_kst;
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
        /// 종가(현재가)
        /// </summary>
        public double trade_price;
        /// <summary>
        /// 전일 종가
        /// </summary>
        public double prev_closing_price;
        /// <summary>
        /// EVEN : 보합
        /// RISE : 상승
        /// FALL : 하락
        /// </summary>
        public string change;
        /// <summary>
        /// 변화액의 절대값
        /// </summary>
        public double change_price;
        /// <summary>
        /// 변화율의 절대값
        /// </summary>
        public double change_rate;
        /// <summary>
        /// 부호가 있는 변화액
        /// </summary>
        public double signed_change_price;
        /// <summary>
        /// 부호가 있는 변화율
        /// </summary>
        public double signed_change_rate;
        /// <summary>
        /// 가장 최근 거래량
        /// </summary>
        public double trade_volume;
        /// <summary>
        /// 누적 거래대금(UTC 0시 기준)
        /// </summary>
        public double acc_trade_price;
        /// <summary>
        /// 24시간 누적 거래대금
        /// </summary>
        public double acc_trade_price_24h;
        /// <summary>
        /// 누적 거래량(UTC 0시 기준)
        /// </summary>
        public double acc_trade_volume;
        /// <summary>
        /// 24시간 누적 거래량
        /// </summary>
        public double acc_trade_volume_24h;
        /// <summary>
        /// 52주 신고가
        /// </summary>
        public double highest_52_week_price;
        /// <summary>
        /// 52주 신고가 달성일
        /// </summary>
        public DateTime highest_52_week_date;
        /// <summary>
        /// 52주 신저가
        /// </summary>
        public double lowest_52_week_price;
        /// <summary>
        /// 52주 신저가 달성일
        /// </summary>
        public DateTime lowest_52_week_date;
        /// <summary>
        /// 타임스탬프
        /// </summary>
        public long timestamp;

        /// <summary>
        /// 거래 DateTime 가져오기
        /// </summary>
        /// <param name="timeType">표준 시간 기준</param>
        /// <returns>거래 시간</returns>
        public DateTime GetTradeDateTime(eTimeType timeType)
        {
            string strDate = string.Empty;
            string strTime = string.Empty;

            switch (timeType)
            {
                case eTimeType.UTC:
                    strDate = trade_date.ToString();
                    strTime = trade_time.ToString();
                    break;
                case eTimeType.KST:
                    strDate = trade_date_kst.ToString();
                    strTime = trade_time_kst.ToString();
                    break;
                default:
                    break;
            }

            int year = int.Parse(strDate.Substring(0, 4));
            int month = int.Parse(strDate.Substring(4, 2));
            int day = int.Parse(strDate.Substring(6, 2));
            int hour = int.Parse(strTime.Substring(0, 2));
            int minute = int.Parse(strTime.Substring(2, 2));
            int second = int.Parse(strTime.Substring(4, 2));
            return new DateTime(year, month, day, hour, minute, second);
        }
    }

    public class HandlerTicker : ProtocolHandler
    {
        private List<TickerRes> res = null;

        public HandlerTicker()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "ticker?");
            this.Method = Method.Get;
        }

        /// <summary>
        /// 요청
        /// </summary>
        /// <param name="market">반점으로 구분되는 마켓 코드 (ex. KRW-BTC, BTC-ETH)</param>
        /// <param name="onFinished"></param>
        public async Task<List<TickerRes>> Request(eMarketType marketType)
        {
            string marketStr = ModelCenter.Market.GetAllMarketStr(marketType);
            RestRequest request = new RestRequest(URI + $"markets={marketStr}", Method);
            request.AddHeader("Accept", "application/json");
            await base.RequestProcess(request);
            return res;
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<TickerRes>(response.Content);
                ModelCenter.Market.UpdateTickers(res);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
