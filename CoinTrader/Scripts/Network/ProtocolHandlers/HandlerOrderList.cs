using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    /// <summary>
    /// 주문 리스트 조회
    /// </summary>
    public class HandlerOrderListRes : iResponse
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

    public class HandlerOrderList : ProtocolHandler
    {
        private StringBuilder sb = new StringBuilder();
        private List<HandlerOrderListRes> res = null;

        public HandlerOrderList()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "orders?");
            this.Method = Method.Get;
        }

        /// <summary>
        /// 요청
        /// </summary>
        /// <param name="market">마켓 아이디</param>
        /// <param name="uuids">주문 UUID의 목록</param>
        /// <param name="identifiers">주문 identifier의 목록</param>
        /// <param name="state">주문 상태</param>
        //- wait : 체결 대기(default)
        //- watch : 예약주문 대기
        //- done : 전체 체결 완료
        //- cancel : 주문 취소
        /// <param name="states">주문 상태의 목록</param>
        //* 미체결 주문(wait, watch)과 완료 주문(done, cancel)은 혼합하여 조회하실 수 없습니다.
        /// <param name="page">페이지 수, default: 1</param>
        /// <param name="limit">요청 개수, default: 100</param>
        /// <param name="order_by">정렬 방식</param>
        //- asc : 오름차순
        //- desc : 내림차순(default)
        /// <param name="onFinished"></param>
        public void Request(string market, string[] uuids, string[] identifiers, string state, string[] states, int page = 1, int limit = 100, string order_by = "desc", Action<bool, List<HandlerOrderListRes>> onFinished = null)
        {
            sb.Length = 0;
            sb.Append(URI);
            if (!string.IsNullOrEmpty(market))
                sb.Append($"&market={market}");
            if (uuids.Length > 0)
            {
                for (int i = 0; i < uuids.Length; i++)
                {
                    sb.Append($"&uuids={uuids[i]}");
                }
            }
            if (identifiers.Length > 0)
            {
                for (int i = 0; i < identifiers.Length; i++)
                {
                    sb.Append($"&identifiers={identifiers[i]}");
                }
            }
            if (!string.IsNullOrEmpty(state))
                sb.Append($"&state={state}");
            if (states.Length > 0)
            {
                for (int i = 0; i < states.Length; i++)
                {
                    sb.Append($"&states={states[i]}");
                }
            }
            sb.Append($"page={page}");
            sb.Append($"limit={limit}");
            sb.Append($"order_by={order_by}");
            RestRequest request = new RestRequest(sb.ToString(), Method);
            request.AddHeader("Accept", "application/json");
            base.RequestProcess(request, (result) => onFinished?.Invoke(result, res));
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<HandlerOrderListRes>(response.Content);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
