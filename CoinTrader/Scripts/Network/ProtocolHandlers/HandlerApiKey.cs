using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

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

        public async Task<List<ApiKeyRes>> Request()
        {
            RestRequest request = new RestRequest(URI, Method);
            request.AddHeader("Authorization", ProtocolManager.GetAuthToken());
            request.AddHeader("Accept", "application/json");
            await base.RequestProcess(request);
            return res;
        }

        protected override void Response(RestRequest request, RestResponse response)
        {
            if (response.IsSuccessful)
            {
                res = JsonParser<ApiKeyRes>(response.Content);
                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].access_key == Config.ACCESS_KEY)
                        Config.ExpireAt = res[i].expire_at;
                }
            }
            else
            {
                if (response.ErrorMessage != null)
                    Logger.Error(response.ErrorMessage);
            }
        }
    }
}
