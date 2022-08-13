/// <summary>
/// 마켓 타입
/// </summary>
public enum eMarketType
{
    /// <summary>
    /// 원화 시장 (주 타겟)
    /// </summary>
    KRW,
    /// <summary>
    /// 비트코인 시장
    /// </summary>
    BTC,
    /// <summary>
    /// 테더 시장
    /// </summary>
    USDT,
}

/// <summary>
/// 표준 시간 구분
/// </summary>
public enum eTimeType
{
    /// <summary>
    /// 세계협정시
    /// </summary>
    UTC,
    /// <summary>
    /// 한국 표준 시간
    /// </summary>
    KST,
}

public enum eOrderState
{
    none,
    /// <summary>
    /// 체결 대기 (default)
    /// </summary>
    wait,
    /// <summary>
    /// 예약주문 대기
    /// </summary>
    watch,
    /// <summary>
    /// 전체 체결 완료
    /// </summary>
    done,
    /// <summary>
    /// 주문 취소
    /// </summary>
    cancel,
}