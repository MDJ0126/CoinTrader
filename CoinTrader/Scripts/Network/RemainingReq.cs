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

        public void Update(string value)
        {
            this.group = Utils.GetHeaderValue(value, "group");
            int.TryParse(Utils.GetHeaderValue(value, "min"), out minutes);
            int.TryParse(Utils.GetHeaderValue(value, "sec"), out seconds);
        }

        public override string ToString()
        {
            return $"group: {group}, min: {minutes}, sec: {seconds}";
        }
    }
}
