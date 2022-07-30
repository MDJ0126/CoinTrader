using RestSharp;
using System;
using System.Collections.Generic;

namespace Network
{
    /// <summary>
    /// 주문 리스트 조회
    /// </summary>
    public class HandlerOrderListRes : iResponse
    {
        
    }

    public class HandlerOrderList : ProtocolHandler
    {
        private List<HandlerOrderListRes> res = null;

        public HandlerOrderList()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "orders");
            this.Method = Method.Get;
        }

        public void Request(Action<bool, List<HandlerOrderListRes>> onFinished = null)
        {
            RestRequest request = new RestRequest(URI, Method);
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
