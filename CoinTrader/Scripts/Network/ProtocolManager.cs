using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Network
{
    public class ProtocolManager : Singleton<ProtocolManager>
    {
        public static readonly string BASE_URL = "https://api.upbit.com/v1/";

        private List<ProtocolHandler> handlers = new List<ProtocolHandler>();

        protected override void Install()
        {

        }

        protected override void Release()
        {
            this.handlers.Clear();
            this.handlers = null;
        }

        /// <summary>
        /// 요청 등록
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onResponse"></param>
        private async void Request(ProtocolHandler protocolHandler, RestRequest request, Action<RestResponse> onResponse)
        {
            if (protocolHandler.CanRequest())
            {
                // 요청 횟수 차감
                protocolHandler.UseReuqestCount();

                // Request
                RestClient restClient = new RestClient(protocolHandler.URI);
                RestResponse response = await restClient.ExecuteAsync(request);

                // Reponse
                onResponse?.Invoke(response);
            }
            else
            {
                Logger.Warning($"요청이 너무 빠릅니다. ({protocolHandler})");

                // Reponse
                onResponse?.Invoke(null);
            }
        }

        /// <summary>
        /// 핸들러 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetHandler<T>() where T : ProtocolHandler, new()
        {
            T protocolHandler;

            var enumerator = Instance.handlers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                protocolHandler = enumerator.Current as T;
                if (protocolHandler != null)
                    return protocolHandler;
            }

            // if handler is null
            protocolHandler = new T();
            protocolHandler.restRequest = Instance.Request;
            Instance.handlers.Add(protocolHandler);
            return protocolHandler;
        }

        /// <summary>
        /// Authorization
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetAuthToken(string parameter = "")
        {
            var payload = new JwtPayload
            {
                { "access_key", Config.ACCESS_KEY },
                { "nonce", Guid.NewGuid().ToString() },
                { "query_hash", parameter },
                { "query_hash_alg", "SHA512" }
            };

            byte[] keyBytes = Encoding.Default.GetBytes(Config.SECRET_KEY);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            return "Bearer " + jwtToken;
        }

        public static string GetAuthToken(Dictionary<string, string> parameters)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                builder.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
            }
            string queryString = builder.ToString().TrimEnd('&');

            SHA512 sha512 = SHA512.Create();
            byte[] queryHashByteArray = sha512.ComputeHash(Encoding.UTF8.GetBytes(queryString));
            string queryHash = BitConverter.ToString(queryHashByteArray).Replace("-", "").ToLower();

            var payload = new JwtPayload
        {
            { "access_key", Config.ACCESS_KEY },
            { "nonce", Guid.NewGuid().ToString() },
            { "query_hash", queryHash },
            { "query_hash_alg", "SHA512" }
        };

            byte[] keyBytes = Encoding.Default.GetBytes(Config.SECRET_KEY);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, payload);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            return "Bearer " + jwtToken;
        }
    }
};