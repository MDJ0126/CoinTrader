using Network;
using System.Collections.Generic;

public class MarketInfo
{
    /// <summary>
    /// 종목 구분 코드
    /// </summary>
    public string name;
    /// <summary>
    /// 한글 이름
    /// </summary>
    public string korean_name;
    /// <summary>
    /// 종가(현재가)
    /// </summary>
    public double trade_price;
    /// <summary>
    /// 전일 종가
    /// </summary>
    public double? yesterday_trade_price;
    /// <summary>
    /// 일(Day) 캔들 데이터 리스트
    /// </summary>
    public List<CandlesDaysRes> candlesDaysRes = null;

    /// <summary>
    /// 변동성 퍼센테이지 Normalize (-1f ~ 1f)
    /// </summary>
    /// <returns></returns>
    public double GetVariabilityNormalize()
    {
        return (trade_price - yesterday_trade_price.Value) / yesterday_trade_price.Value;
    }

    public override string ToString()
    {
        return $"{name}, {korean_name}, 현재가: {trade_price:N0}";
    }
}