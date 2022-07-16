using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Network
{
    /// <summary>
    /// 전체 계좌 조회
    /// </summary>
    public class ApiKeyRes : iResponse
    {
        /// <summary>
        /// 사용 중인 엑세스 키
        /// </summary>
        public string access_key;
        /// <summary>
        /// 사용기한
        /// </summary>
        public DateTime expire_at;
    }

    public class HandlerApiKey : ProtocolHandler
    {
        private List<ApiKeyRes> res = null;

        public HandlerApiKey()
        {
            this.URI = new Uri(ProtocolManager.BASE_URL + "api_keys");
            this.Method = Method.Get;
        }

        public void Request(Action<bool, List<ApiKeyRes>> onFinished = null)
        {
            RestRequest request = new RestRequest(URI, Method);
            request.AddHeader("Authorization", ProtocolManager.GetAuthToken());
            request.AddHeader("Accept", "application/json");
            base.RequestProcess(request, (result) => onFinished?.Invoke(result, res));
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<ApiKeyRes>(response.Content);
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
