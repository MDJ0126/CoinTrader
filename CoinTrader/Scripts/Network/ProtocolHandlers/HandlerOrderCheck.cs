using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Network
{
    /// <summary>
    /// 개별 주문 조회
    /// </summary>
    public class HandlerOrderCheckRes : iResponse
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
        /// <summary>
        /// 체결
        /// </summary>
        public object[] trades;
        /// <summary>
        /// 마켓의 유일 키
        /// </summary>
        public string trades_market;
        /// <summary>
        /// 체결의 고유 아이디
        /// </summary>
        public string trades_uuid;
        /// <summary>
        /// 체결 가격
        /// </summary>
        public string trades_price;
        /// <summary>
        /// 체결 양
        /// </summary>
        public string trades_volume;
        /// <summary>
        /// 체결된 총 가격
        /// </summary>
        public string trades_funds;
        /// <summary>
        /// 체결 종류
        /// </summary>
        public string trades_side;
        /// <summary>
        /// 체결 시각
        /// </summary>
        public string trades_created_at;

        public eOrderState GetOrderState()
        {
            return Utils.FindEnumValue<eOrderState>(state);
        }
    }

    public class HandlerOrderCheck : ProtocolHandler
    {
        private List<HandlerOrderCheckRes> res = null;

        public HandlerOrderCheck()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "order?");
            this.Method = Method.Get;
        }

        /// <summary>
        /// 요청 (uuid 혹은 identifier 둘 중 하나의 값이 반드시 포함되어야 합니다.)
        /// </summary>
        /// <param name="uuid">주문 UUID</param>
        /// <param name="identifier">조회용 사용자 지정 값</param>
        /// <param name="onFinished"></param>
        public async Task<List<HandlerOrderCheckRes>> Request(string uuid = "", string identifier = "")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(uuid))
                parameters.Add("uuid", uuid);
            if (!string.IsNullOrEmpty(identifier))
                parameters.Add("identifier", identifier);
            
            var queryString = ProtocolManager.GetQueryString(parameters);
            RestRequest request = new RestRequest(URI + queryString, Method);
            request.AddHeader("Authorization", ProtocolManager.GetAuthToken(queryString));
            request.AddHeader("Accept", "application/json");
            await base.RequestProcess(request);
            return res;
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<HandlerOrderCheckRes>(response.Content);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
