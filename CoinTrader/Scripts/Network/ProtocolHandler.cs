using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Network
{
    public abstract class ProtocolHandler
    {
        protected static Dictionary<string, RemainingReq> RemainingReqs { get; } = new Dictionary<string, RemainingReq>();

        public delegate void RestRequestDelegate(ProtocolHandler protocolHandler, RestRequest request, Action<RestResponse> onResponse);
        public RestRequestDelegate restRequest = null;

        protected Uri URI { get; set; }
        protected Method Method { get; set; }

        /// <summary>
        /// 통신 요청 정보 캐싱
        /// </summary>
        private Action<bool> onFinished = null;

        /// <summary>
        /// Request 요청 그룹
        /// </summary>
        private string group = string.Empty;    // => Unknown

        /// <summary>
        /// Rest 요청 프로세스
        /// </summary>
        /// <param name="onFinished"></param>
        protected void RequestProcess(RestRequest req, Action<bool> onFinished)
        {
            this.onFinished = onFinished;
            restRequest?.Invoke(this, req, (res) =>
            {
                if (res.IsSuccessful)
                {
                    // 남은 요청 수 갱신
                    UpdateRemainingReq(res);

                    // 표준 수신 처리
                    Response(req, res);

                    // 수신 처리 완료 콜백
                    onFinished?.Invoke(true);
                    onFinished = null;
                }
                else
                {
                    onFinished?.Invoke(false);
                    onFinished = null;
                }
            });
        }

        /// <summary>
        /// 요청 가능 횟수 차감
        /// </summary>
        public void UseReuqestCount()
        {
            if (RemainingReqs.TryGetValue(group, out var remainingReq))
                remainingReq.UseRequestCount();
        }

        /// <summary>
        /// 요청 가능한지 체크
        /// </summary>
        /// <returns></returns>
        public bool IsCanRequest()
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
                                    {
                                        fieldInfo.SetValue(convertRes, jToken.ToObject<DateTime>());
                                    }
                                    break;
                                case "String":
                                    fieldInfo.SetValue(convertRes, jToken.ToString());
                                    break;
                                default:
                                    break;
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
    }
}