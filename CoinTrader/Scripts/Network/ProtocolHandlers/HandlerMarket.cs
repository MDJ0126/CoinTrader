using RestSharp;
using System;
using System.Collections.Generic;

namespace Network
{
    /// <summary>
    /// 마켓 코드 조회
    /// </summary>
    public class MarketRes : iResponse
    {
        public enum eWarning
        {
            /// <summary>
            /// 해당 사항 없음
            /// </summary>
            NONE,
            /// <summary>
            /// 투자유의
            /// </summary>
            CAUTION,
        }

        /// <summary>
        /// 업비트에서 제공중인 시장 정보
        /// </summary>
        public string market;
        /// <summary>
        /// 거래 대상 암호화폐 한글명
        /// </summary>
        public string korean_name;
        /// <summary>
        /// 거래 대상 암호화폐 영문명
        /// </summary>
        public string english_name;
        /// <summary>
        /// 유의 종목 여부
        /// NONE(해당 사항 없음), CAUTION(투자유의)
        /// </summary>
        public string market_warning;

        /// <summary>
        /// 유의 종목 여부
        /// </summary>
        /// <returns></returns>
        public eWarning GetWarning()
        {
            if (!string.IsNullOrEmpty(market_warning))
            {
                if (market_warning == "CAUTION")
                {
                    return eWarning.CAUTION;
                }
            }
            return eWarning.NONE;
        }
    }

    public class HandlerMarket : ProtocolHandler
    {
        private List<MarketRes> res = null;

        public HandlerMarket()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "market/all");
            this.Method = Method.Get;
        }

        public void Request(Action<bool, List<MarketRes>> onFinished = null)
        {
            RestRequest request = new RestRequest(URI, Method);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("isDetails", "true");
            base.RequestProcess(request, (result) => onFinished?.Invoke(result, res));
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<MarketRes>(response.Content);
                ModelCenter.Market.SetBaseInfo(res);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
