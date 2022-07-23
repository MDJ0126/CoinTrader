using Network;
using System.Collections.Generic;

public class MarketInfo
{
    public enum eDay
    {
        Today,
        D1day,
        D2day,
    }

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
    /// 금일 예상 종가
    /// </summary>
    public double? predictePrice;
    /// <summary>
    /// D+1 예상 종가
    /// </summary>
    public double? predictePrice_D1;
    /// <summary>
    /// D+2 예상 종가
    /// </summary>
    public double? predictePrice_D2;
    /// <summary>
    /// 전일 종가
    /// </summary>
    public double? yesterday_trade_price;

    /// <summary>
    /// 변동성 퍼센테이지 Normalize (-1f ~ 1f)
    /// </summary>
    /// <returns></returns>
    public double GetVariabilityNormalize()
    {
        return (trade_price - yesterday_trade_price.Value) / yesterday_trade_price.Value;
    }

    /// <summary>
    /// 금일 예상 종가 퍼센테이지 Normalize (-1f ~ 1f)
    /// </summary>
    /// <returns></returns>
    public double GetTodayPredicteNormalize(eDay day = eDay.Today)
    {
        double price = 0f;
        switch (day)
        {
            case eDay.Today:
                price = predictePrice.Value;
                break;
            case eDay.D1day:
                price = predictePrice_D1.Value;
                break;
            case eDay.D2day:
                price = predictePrice_D2.Value;
                break;
            default:
                break;
        }
        return (price - yesterday_trade_price.Value) / yesterday_trade_price.Value;
    }

    public override string ToString()
    {
        return $"{name}, {korean_name}, 현재가: {trade_price:N0}";
    }
}