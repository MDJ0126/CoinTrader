using System;
using System.Net;

public static class Time
{
    /// <summary>
    /// 클라이언트와 동기화된 시간 차이 (Tick)
    /// </summary>
    private static long diffTick = 0L;

    /// <summary>
    /// 기준 시간
    /// </summary>
    private static DateTime syncTime = DateTime.Now;

    /// <summary>
    /// 현재 시간
    /// </summary>
    public static DateTime NowTime => DateTime.Now.AddTicks(diffTick);

    /// <summary>
    /// UTC 시간 (KST + 9시간)
    /// </summary>
    public static DateTime UtcNowTime => NowTime.AddHours(9f);

    public static void Start()
    {
        Synchronization("https://upbit.com/");
    }

    /// <summary>
    /// 동기화 시간 업데이트
    /// </summary>
    /// <param name="dateStr"></param>
    public static void UpdateDateTime(string dateStr)
    {
        if (DateTime.TryParse(dateStr, out DateTime dateTime))
        {
            syncTime = dateTime;
            diffTick = (syncTime - DateTime.Now).Ticks;
        }
    }

    /// <summary>
    /// 서버 동기화 업데이트
    /// </summary>
    /// <param name="serverURL"></param>
    public static async void Synchronization(string serverURL)
    {
        WebRequest request = WebRequest.Create(serverURL);
        WebResponse response = await request.GetResponseAsync();

        if (Array.Exists(response.Headers.AllKeys, headerKey => headerKey.Equals("Date")))
            UpdateDateTime(response.Headers["Date"]);
    }
}