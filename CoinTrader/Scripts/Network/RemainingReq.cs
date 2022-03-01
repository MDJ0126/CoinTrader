using System;

namespace Network
{
    /// <summary>
    /// 남은 요청 수 카운트 클래스
    /// </summary>
    public class RemainingReq
    {
        public static string HEADER_NAME = "Remaining-Req";
        /// <summary>
        /// 그룹
        /// </summary>
        public string group;
        /// <summary>
        /// 남은 1분간 요청 가능 수
        /// </summary>
        public int minutes;
        /// <summary>
        /// 해당 초에 요청 가능 수
        /// </summary>
        public int seconds;
        /// <summary>
        /// 갱신 시간
        /// </summary>
        public DateTime refreshedTime;

        public void Update(string value)
        {
            refreshedTime = Time.NowTime;
            this.group = Utils.GetHeaderValue(value, "group");
            int.TryParse(Utils.GetHeaderValue(value, "min"), out minutes);
            int.TryParse(Utils.GetHeaderValue(value, "sec"), out seconds);
        }

        public bool IsCanRequest()
        {
            DateTime refreshed = new DateTime(refreshedTime.Year, refreshedTime.Month, refreshedTime.Day, refreshedTime.Hour, refreshedTime.Minute, refreshedTime.Second);
            var nowTime = Time.NowTime;
            DateTime now = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, nowTime.Second);
            if (refreshed != now)
            {
                // 시간이 '초' 단위로 다르면 통과
                return true;
            }
            else
            {
                // 시간이 같다면,
                // 남은 요청 횟수가 있는지?
                if (seconds >= 0 /*&& minutes >= 0*/)
                    return true;
            }
            return false;
        }

        public void UseRequestCount()
        {
            --seconds;
        }

        public override string ToString()
        {
            return $"group: {group}, min: {minutes}, sec: {seconds}";
        }
    }
}
