using RestSharp;
using System;
using System.Collections.Generic;

namespace Network
{
    /// <summary>
    /// 주문 가능 정보
    /// </summary>
    public class HandlerOrdersChanceRes : iResponse
    {
        /// <summary>
        /// 매도 수수료 비율
        /// </summary>
        public string ask_fee;
        /// <summary>
        /// 마켓에 대한 정보
        /// </summary>
        public object market;
        /// <summary>
        /// 마켓의 유일 키
        /// </summary>
        public string market_id;
        /// <summary>
        /// 마켓 이름
        /// </summary>
        public string market_name;
        /// <summary>
        /// 지원 주문 방식
        /// </summary>
        public string[] market_order_types;
        /// <summary>
        /// 지원 주문 종류
        /// </summary>
        public string[] market_order_sides;
        /// <summary>
        /// 매수 시 제약사항
        /// </summary>
        public object market_bid;
        /// <summary>
        /// 화폐를 의미하는 영문 대문자 코드
        /// </summary>
        public string market_bid_currency;
        /// <summary>
        /// 주문금액 단위
        /// </summary>
        public string market_bid_price_unit;
        /// <summary>
        /// 최소 매도/매수 금액
        /// </summary>
        public string market_bid_min_total;
        /// <summary>
        /// 매도 시 제약사항
        /// </summary>
        public object market_ask;
        /// <summary>
        /// 화폐를 의미하는 영문 대문자 코드
        /// </summary>
        public string market_ask_currency;
        /// <summary>
        /// 주문금액 단위
        /// </summary>
        public string market_ask_price_unit;
        /// <summary>
        /// 최소 매도/매수 금액
        /// </summary>
        public int market_ask_min_total;
        /// <summary>
        /// 최대 매도/매수 금액
        /// </summary>
        public string market_max_total;
        /// <summary>
        /// 마켓 운영 상태
        /// </summary>
        public string market_state;
        /// <summary>
        /// 매수 시 사용하는 화폐의 계좌 상태
        /// </summary>
        public object bid_account;
        /// <summary>
        /// 화폐를 의미하는 영문 대문자 코드	
        /// </summary>
        public string bid_account_currency;
        /// <summary>
        /// 주문가능 금액/수량
        /// </summary>
        public string bid_account_balance;
        /// <summary>
        /// 주문 중 묶여있는 금액/수량
        /// </summary>
        public string bid_account_locked;
        /// <summary>
        /// 매수평균가
        /// </summary>
        public string bid_account_avg_buy_price;
        /// <summary>
        /// 매수평균가 수정 여부
        /// </summary>
        public bool bid_account_avg_buy_price_modified;
        /// <summary>
        /// 평단가 기준 화폐	
        /// </summary>
        public string bid_account_unit_currency;
        /// <summary>
        /// 매도 시 사용하는 화폐의 계좌 상태	
        /// </summary>
        public object ask_account;
        /// <summary>
        /// 화폐를 의미하는 영문 대문자 코드	
        /// </summary>
        public string ask_account_currency;
        /// <summary>
        /// 주문가능 금액/수량
        /// </summary>
        public string ask_account_balance;
        /// <summary>
        /// 주문 중 묶여있는 금액/수량
        /// </summary>
        public string ask_account_locked;
        /// <summary>
        /// 매수평균가
        /// </summary>
        public string ask_account_avg_buy_price;
        /// <summary>
        /// 매수평균가 수정 여부
        /// </summary>
        public bool ask_account_avg_buy_price_modified;
        /// <summary>
        /// 평단가 기준 화폐
        /// </summary>
        public string ask_account_unit_currency;
    }

    public class HandlerOrdersChance : ProtocolHandler
    {
        private List<HandlerOrdersChanceRes> res = null;

        public HandlerOrdersChance()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "orders/chance?");
            this.Method = Method.Get;
        }

        public void Request(Action<bool, List<HandlerOrdersChanceRes>> onFinished = null)
        {
            RestRequest request = new RestRequest(URI, Method);
            request.AddHeader("Accept", "application/json");
            base.RequestProcess(request, (result) => onFinished?.Invoke(result, res));
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<HandlerOrdersChanceRes>(response.Content);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
