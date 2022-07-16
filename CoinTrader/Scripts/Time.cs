using System;
using System.Threading;

public static class Time
{
    /// <summary>
    /// 기준 시간
    /// </summary>
    private static DateTime startedTime = DateTime.Now;

    /// <summary>
    /// 프로그램이 시작한 이후 경과 시간(seconds)
    /// </summary>
    private static float realtimeSinceStartup = 0f;

    /// <summary>
    /// 경과 시간 업데이트 간격
    /// </summary>
    private const float realtimeSinceStartup_UpdateInterval = 1f;
    public static DateTime NowTime
    {
        get
        {
            // 현재 로직 기준으로 프로그램이 멈추면 시간 오차가 발생할 수 있음.
            // (기기 시간을 변경되어도 영향 안 받도록 작업한 부분)
            return startedTime.AddSeconds(realtimeSinceStartup);
        }
    }

    public static void Start()
    {
        startedTime = DateTime.Now;
        realtimeSinceStartup = 0f;
        MultiThread.Start(() =>
        {
            const int UPDATE_INTERVAL = (int)(realtimeSinceStartup_UpdateInterval * 1000);
            const float UPDATE_VALUE = UPDATE_INTERVAL / 1000f;
            while (true)
            {
                Thread.Sleep(UPDATE_INTERVAL);
                realtimeSinceStartup += UPDATE_VALUE;
                Logger.Log(realtimeSinceStartup);
            }
        });
    }
}