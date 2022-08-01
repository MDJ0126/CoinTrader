using RestSharp;
using System;
using System.Collections.Generic;

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
        public void Request(string market, string side, string volume, string price, string ord_type, string identifier, Action<bool, List<HandlerOrdersRes>> onFinished = null)        
        {
            RestRequest request = new RestRequest(URI + $"&market={market}&side={side}&volume={volume}&price={price}&ord_type={ord_type}&identifier={identifier}", Method);
            request.AddHeader("Accept", "application/json");
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
