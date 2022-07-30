using RestSharp;
using System;
using System.Collections.Generic;

namespace Network
{
    /// <summary>
    /// 개별 주문 조회
    /// </summary>
    public class HandlerOrderCheckRes : iResponse
    {
        
    }

    public class HandlerOrderCheck : ProtocolHandler
    {
        private List<HandlerOrderCheckRes> res = null;

        public HandlerOrderCheck()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "order");
            this.Method = Method.Get;
        }

        public void Request(Action<bool, List<HandlerOrderCheckRes>> onFinished = null)
        {
            RestRequest request = new RestRequest(URI, Method);
            request.AddHeader("Accept", "application/json");
            base.RequestProcess(request, (result) => onFinished?.Invoke(result, res));
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
