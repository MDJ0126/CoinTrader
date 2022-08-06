using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    /// <summary>
    /// 주문하기
    /// </summary>
    public class HandlerOrdersRes : iResponse
    {
        /// <summary>
        /// 주문의 고유 아이디
        /// </summary>
        public string uuid;
        /// <summary>
        /// 주문 종류
        /// </summary>
        public string side;
        /// <summary>
        /// 주문 방식
        /// </summary>
        public string ord_type;
        /// <summary>
        /// 주문 당시 화폐 가격
        /// </summary>
        public string price;
        /// <summary>
        /// 주문 상태
        /// </summary>
        public string state;
        /// <summary>
        /// 마켓의 유일키
        /// </summary>
        public string market;
        /// <summary>
        /// 주문 생성 시간
        /// </summary>
        public string created_at;
        /// <summary>
        /// 사용자가 입력한 주문 양	
        /// </summary>
        public string volume;
        /// <summary>
        /// 체결 후 남은 주문 양
        /// </summary>
        public string remaining_volume;
        /// <summary>
        /// 수수료로 예약된 비용
        /// </summary>
        public string reserved_fee;
        /// <summary>
        /// 남은 수수료
        /// </summary>
        public string remaining_fee;
        /// <summary>
        /// 사용된 수수료
        /// </summary>
        public string paid_fee;
        /// <summary>
        /// 거래에 사용중인 비용
        /// </summary>
        public string locked;
        /// <summary>
        /// 체결된 양
        /// </summary>
        public string executed_volume;
        /// <summary>
        /// 해당 주문에 걸린 체결 수
        /// </summary>
        public int trades_count;
    }

    public class HandlerOrders : ProtocolHandler
    {
        private List<HandlerOrdersRes> res = null;

        public HandlerOrders()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "orders?");
            this.Method = Method.Post;
        }

        /// <summary>
        /// 요청
        /// </summary>
        /// <param name="market">마켓 ID (필수)</param>
        /// <param name="side">주문 종류 (필수)</param>
            //- bid : 매수
            //- ask : 매도
        /// <param name="volume">주문량 (지정가, 시장가 매도 시 필수)</param>
        /// <param name="price">주문 가격. (지정가, 시장가 매수 시 필수)</param>
            //ex) KRW-BTC 마켓에서 1BTC당 1,000 KRW로 거래할 경우, 값은 1000 이 된다.
            //ex) KRW-BTC 마켓에서 1BTC당 매도 1호가가 500 KRW 인 경우, 시장가 매수 시 값을 1000으로 세팅하면 2BTC가 매수된다.
            //(수수료가 존재하거나 매도 1호가의 수량에 따라 상이할 수 있음)
        /// <param name="ord_type">주문 타입 (필수)</param>
            //- limit : 지정가 주문
            //- price : 시장가 주문(매수)
            //- market : 시장가 주문(매도)
        /// <param name="identifier">조회용 사용자 지정값 (선택)</param>
        /// <param name="onFinished"></param>
        public void Request(string market, string side, string volume, string price, string ord_type, string identifier = "", Action<bool, List<HandlerOrdersRes>> onFinished = null)        
        {
            // 참고 : https://docs.upbit.com/docs/market-info-trade-price-detail
            var marketInfo = ModelCenter.Market.GetMarketInfo(market);
            if (!string.IsNullOrEmpty(price))
            {
                double filter = Convert.ToDouble(price);
                float unit = 0f;
                if (marketInfo.trade_price >= 2000000) unit = 1000f;
                else if (marketInfo.trade_price >= 1000000 && marketInfo.trade_price < 2000000) unit = 500f;
                else if (marketInfo.trade_price >= 500000 && marketInfo.trade_price < 1000000) unit = 100f;
                else if (marketInfo.trade_price >= 100000 && marketInfo.trade_price < 500000) unit = 50f;
                else if (marketInfo.trade_price >= 10000 && marketInfo.trade_price < 100000) unit = 10f;
                else if (marketInfo.trade_price >= 1000 && marketInfo.trade_price < 10000) unit = 5f;
                else if (marketInfo.trade_price >= 100 && marketInfo.trade_price < 1000) unit = 1f;
                else if (marketInfo.trade_price >= 10 && marketInfo.trade_price < 100) unit = 0.1f;
                else if (marketInfo.trade_price >= 1 && marketInfo.trade_price < 10) unit = 0.01f;
                else if (marketInfo.trade_price >= 0.1 && marketInfo.trade_price < 1) unit = 0.001f;
                else if (marketInfo.trade_price >= 0 && marketInfo.trade_price < 0.1) unit = 0.0001f;
                price = (filter - (filter % unit)).ToString();
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(market))
                parameters.Add("market", market);

            if (!string.IsNullOrEmpty(side))
                parameters.Add("side", side);

            if (!string.IsNullOrEmpty(volume))
                parameters.Add("volume", volume);

            if (!string.IsNullOrEmpty(price))
                parameters.Add("price", price);

            if (!string.IsNullOrEmpty(ord_type))
                parameters.Add("ord_type", ord_type);

            if (!string.IsNullOrEmpty(identifier))
                parameters.Add("identifier", identifier);

            var queryString = ProtocolManager.GetQueryString(parameters);
            RestRequest request = new RestRequest(URI + queryString, Method);
            request.AddHeader("Authorization", ProtocolManager.GetAuthToken(queryString));
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(JsonConvert.SerializeObject(parameters));
            base.RequestProcess(request, (result) => onFinished?.Invoke(result, res));
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<HandlerOrdersRes>(response.Content);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
