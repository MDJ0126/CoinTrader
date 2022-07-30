using RestSharp;
using System;
using System.Collections.Generic;

namespace Network
{
    /// <summary>
    /// 주문 취소 접수
    /// </summary>
    public class HandlerOrderCancelRes : iResponse
    {
        
    }

    public class HandlerOrderCancel : ProtocolHandler
    {
        private List<HandlerOrderCancelRes> res = null;

        public HandlerOrderCancel()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "order");
            this.Method = Method.Delete;
        }

        public void Request(Action<bool, List<HandlerOrderCancelRes>> onFinished = null)
        {
            RestRequest request = new RestRequest(URI, Method);
            request.AddHeader("Accept", "application/json");
            base.RequestProcess(request, (result) => onFinished?.Invoke(result, res));
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
