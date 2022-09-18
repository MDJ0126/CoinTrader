using System;

public static class Config
{
    /// <summary>
    /// API 엑세스 키
    /// </summary>
    public static string ACCESS_KEY = "nHlwYxbvnH0xkfQFqCKF5GmM0vy4vNIuZSxjyjx2";   // 예제키가 미리 입력되어있습니다.
    /// <summary>
    /// 시크릿 키
    /// </summary>
    public static string SECRET_KEY = "fxuOGjxVlgTSWlRVeadTvnpE74XpAREIDGdzrRNp";   // 예제키가 미리 입력되어있습니다.
    /// <summary>
    /// API 엑세스 키 만료 날짜
    /// </summary>
    public static DateTime ExpireAt = DateTime.MinValue;
    /// <summary>
    /// 자동 거래 시작 여부
    /// </summary>
    public static bool isStartedAutoTrade = false;
}