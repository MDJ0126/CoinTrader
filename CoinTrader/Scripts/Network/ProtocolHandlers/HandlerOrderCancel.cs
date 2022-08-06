using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Network
{
    /// <summary>
    /// 주문 취소 접수
    /// </summary>
    public class HandlerOrderCancelRes : iResponse
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

    public class HandlerOrderCancel : ProtocolHandler
    {
        private List<HandlerOrderCancelRes> res = null;

        public HandlerOrderCancel()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "order?");
            this.Method = Method.Delete;
        }

        public async Task<List<HandlerOrderCancelRes>> Request(string uuid, string identifier)
        {
            RestRequest request = new RestRequest(URI + $"&uuid={uuid}&identifier={identifier}", Method);
            request.AddHeader("Accept", "application/json");
            await base.RequestProcess(request);
            return res;
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<HandlerOrderCancelRes>(response.Content);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
