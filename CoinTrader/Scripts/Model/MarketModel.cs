using Network;
using System;
using System.Collections.Generic;

public class MarketModel
{
    public delegate void UpdateTicker(MarketInfo marketInfo);
    private event UpdateTicker onUpdateTicker = null;
    public event UpdateTicker OnUpdateTicker
    {
        add
        {
            onUpdateTicker -= value;
            onUpdateTicker += value;
        }
        remove
        {
            onUpdateTicker -= value;
        }
    }
    public List<MarketInfo> marketInfos = new List<MarketInfo>();
    public Dictionary<eMarketType, MarketInfo> markets = new Dictionary<eMarketType, MarketInfo>();
    public string allMarketNames = string.Empty;

    /// <summary>
    /// 마켓 기본 정보 세팅
    /// </summary>
    /// <param name="res"></param>
    public void SetBaseInfo(List<MarketRes> res)
    {
        marketInfos.Clear();
        if (res != null)
        {
            allMarketNames = string.Empty;
            for (int i = 0; i < res.Count; i++)
            {
                // 마켓 단위처리 해야할 듯 수정 필요 'markets' 방식으로 수정할 것!!

                var marketRes = res[i];
                marketInfos.Add(new MarketInfo
                {
                    name = marketRes.market,
                    korean_name = marketRes.korean_name,
                    trade_price = 0,
                });

                if (allMarketNames.Length > 0)
                {
                    allMarketNames += $", {marketRes.market}";
                }
                else
                    allMarketNames += marketRes.market;
            }
        }
    }

    /// <summary>
    /// 현재가 갱신
    /// </summary>
    /// <param name="res"></param>
    public void UpdateTickers(List<TickerRes> res)
    {
        if (res != null)
        {
            for (int i = 0; i < res.Count; i++)
            {
                var tickerRes = res[i];
                var marketInfo = marketInfos.Find(info => info.name == tickerRes.market);
                if (marketInfo != null)
                {
                    if (marketInfo.trade_price != tickerRes.trade_price)
                    {
                        marketInfo.trade_price = tickerRes.trade_price;
                        onUpdateTicker?.Invoke(marketInfo);
                    }
                }
            }
        }
    }
}