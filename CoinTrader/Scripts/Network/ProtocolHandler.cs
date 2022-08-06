using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;

namespace Network
{
    public abstract class ProtocolHandler
    {
        private static RestClient client = new RestClient("https://api.upbit.com");

        protected static Dictionary<string, RemainingReq> RemainingReqs { get; } = new Dictionary<string, RemainingReq>();

        public Uri URI { get; set; }
        protected Method Method { get; set; }

        /// <summary>
        /// 데이터 사용량
        /// </summary>
        public ulong dataUsage = 0;

        /// <summary>
        /// 총 요청 횟수
        /// </summary>
        public int requestCount = 0;

        /// <summary>
        /// Request 요청 그룹
        /// </summary>
        private string group = string.Empty;    // => Unknown

        /// <summary>
        /// Rest 요청 프로세스
        /// </summary>
        /// <param name="onFinished"></param>
        protected async Task<RestResponse> RequestProcess(RestRequest req)
        {
            RestResponse res = null;
            
            // 요청이 가능한지
            if (CanRequest())
            {
                // 요청 카운트 소모 처리
                UseReuqestCount();
                res = await client.ExecuteAsync(req);

                // 데이터 사용량 체크 (미지원)
                CheckDataUsage();

                if (res != null)
                {
                    // 남은 요청 수 갱신
                    UpdateRemainingReq(res);

                    if (res.IsSuccessful)
                    {
                        // 표준 수신 처리
                        Response(req, res);
                    }
                    else
                    {
                        Logger.Warning($"요청에 대한 응답 실패 => ({res.Content})");
                    }
                }
            }
            else
            {
                Logger.Warning($"요청이 너무 빠릅니다 => ({this})");
            }
            return res;
        }

        /// <summary>
        /// 요청 가능 횟수 차감
        /// </summary>
        public void UseReuqestCount()
        {
            if (RemainingReqs.TryGetValue(group, out var remainingReq))
            {
                ++requestCount;
                remainingReq.UseRequestCount();
            }
        }

        /// <summary>
        /// 요청 가능한지 체크
        /// </summary>
        /// <returns></returns>
        public bool CanRequest()
        {
            if (!string.IsNullOrEmpty(group))
            {
                if (RemainingReqs.TryGetValue(group, out var remainingReq))
                {
                    // 그룹에 대한 정보가 있는 경우 체크
                    return remainingReq.IsCanRequest();
                }
                else
                {
                    // 그룹에 대한 정보가 없다는 것은 최초 통신 시도한다는 것으로 간주
                    return true;
                }
            }
            else
            {
                // 그룹을 모르기에 일단 통신 시도할 수 있다고 간주
                return true;
            }
        }

        /// <summary>
        /// 남은 요청 수 갱신
        /// 참고: https://docs.upbit.com/docs/user-request-guide
        /// </summary>
        private void UpdateRemainingReq(RestResponse res)
        {
            var enumerator = res.Headers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var header = enumerator.Current;
                if (header.Name.Equals(RemainingReq.HEADER_NAME))
                {
                    string group = Utils.GetHeaderValue((string)header.Value, "group");
                    if (!RemainingReqs.TryGetValue(group, out var value))
                    {
                        this.group = group;
                        value = new RemainingReq();
                        RemainingReqs.Add(group, value);
                    }
                    value.Update((string)header.Value);
                }
            }
        }

        protected abstract void Response(RestRequest req, RestResponse res);

        /// <summary>
        /// Json 포맷을 Response 클래스로 변환
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        protected List<T> JsonParser<T>(string jsonContent) where T : iResponse, new()
        {
            List<T> list = new List<T>();
            try
            {
                FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

                var array = JArray.Parse(jsonContent);
                foreach (var item in array)
                {
                    T convertRes = new T();
                    var jObject = JObject.Parse(item.ToString());
                    for (int i = 0; i < fieldInfos.Length; i++)
                    {
                        FieldInfo fieldInfo = fieldInfos[i];
                        string name = fieldInfo.Name;
                        if (jObject.TryGetValue(name, out var jToken))
                        {
                            if (jToken.Value<string>() != null)
                            {
                                switch (fieldInfo.FieldType.Name)
                                {
                                    case "Byte":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<Byte>());
                                        break;
                                    case "SByte":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<SByte>());
                                        break;
                                    case "Int16":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<Int16>());
                                        break;
                                    case "Int32":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<Int32>());
                                        break;
                                    case "Int64":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<Int64>());
                                        break;
                                    case "Decimal":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<Decimal>());
                                        break;
                                    case "UInt16":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<UInt16>());
                                        break;
                                    case "UInt32":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<UInt32>());
                                        break;
                                    case "UInt64":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<UInt64>());
                                        break;
                                    case "Single":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<Single>());
                                        break;
                                    case "Double":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<Double>());
                                        break;
                                    case "Boolean":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<Boolean>());
                                        break;
                                    case "DateTime":
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<DateTime>());
                                        break;
                                    case "String":
                                        fieldInfo.SetValue(convertRes, jToken.ToString());
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    list.Add(convertRes);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
            return list;
        }

        /// <summary>
        /// 데이터 사용량 측적하기
        /// </summary>
        public void CheckDataUsage()
        {
            //if (NetworkInterface.GetIsNetworkAvailable())
            //{
            //    NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            //    for (int i = 0; i < interfaces.Length; i++)
            //    {
            //        NetworkInterface ni = interfaces[i];

            //        var statics = ni.GetIPv4Statistics();
            //        Logger.Log($"Bytes Sent: {statics.BytesSent}");
            //        Logger.Log($"Bytes Received: {statics.BytesReceived}");
            //        dataUsage += (ulong)(statics.BytesSent + statics.BytesReceived);
            //    }
            //}
        }
    }
}