using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public class ProtocolManager
    {
        public static readonly string BASE_URL = "https://api.upbit.com/v1/";

        private static List<ProtocolHandler> handlers = new List<ProtocolHandler>();

        public void Release()
        {
            handlers.Clear();
            handlers = null;
        }

        /// <summary>
        /// 핸들러 가져오기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetHandler<T>() where T : ProtocolHandler, new()
        {
            T protocolHandler;

            var enumerator = handlers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                protocolHandler = enumerator.Current as T;
                if (protocolHandler != null)
                    return protocolHandler;
            }

            // if handler is null
            protocolHandler = new T();
            handlers.Add(protocolHandler);
            protocolHandler.onRequest = OnRequest;
            protocolHandler.onResponse = OnResponse;
            return protocolHandler;
        }

        private static void OnRequest(RestRequest req)
        {

        }

        private static void OnResponse(RestResponse res)
        {
            // 패킷을 받을 때, 헤더에 있는 시간을 찍어와서 서버 시간으로 동기화 처리
            if (res.Headers != null)
            {
                var enumerator = res.Headers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var header = enumerator.Current;
                    if (header != null && header.Name.Equals("Date"))
                    {
                        Time.UpdateDateTime(header.Value.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Authorization
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetAuthToken()
        {
            var payload = new JwtPayload
            {
                { "access_key", Config.ACCESS_KEY },
                { "nonce", Guid.NewGuid().ToString() },
                { "query_hash", string.Empty },
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

        /// <summary>
        /// Authorization
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string GetAuthToken(string queryString)
        {
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

        /// <summary>
        /// 쿼리 스트링 만들기
        /// key=value&key2=value...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetQueryString(Dictionary<string, string> parameters)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                builder.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1; // 마지막 &를 제거하기 위함.
            }
            return builder.ToString();
        }

        /// <summary>
        /// 쿼리 스트링 만들기
        /// key=value&key2=value...
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetQueryString(List<KeyValuePair<string, string>> parameters)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                builder.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1; // 마지막 &를 제거하기 위함.
            }
            return builder.ToString();
        }
    }
};