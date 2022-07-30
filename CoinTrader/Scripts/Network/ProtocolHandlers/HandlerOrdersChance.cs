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
        
    }

    public class HandlerOrdersChance : ProtocolHandler
    {
        private List<HandlerOrdersChanceRes> res = null;

        public HandlerOrdersChance()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "orders/chance");
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
