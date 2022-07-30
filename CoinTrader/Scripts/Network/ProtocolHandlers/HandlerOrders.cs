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
        
    }

    public class HandlerOrders : ProtocolHandler
    {
        private List<HandlerOrdersRes> res = null;

        public HandlerOrders()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "orders");
            this.Method = Method.Post;
        }

        public void Request(Action<bool, List<HandlerOrdersRes>> onFinished = null)
        {
            RestRequest request = new RestRequest(URI, Method);
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
