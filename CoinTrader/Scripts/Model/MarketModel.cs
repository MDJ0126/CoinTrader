using CoinTrader.ML;
using Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MarketModel
{
    public delegate void UpdateTicker(MarketInfo marketInfo);
    private event UpdateTicker onUpdateMarketInfo = null;
    public event UpdateTicker OnUpdateMarketInfo
    {
        add
        {
            onUpdateMarketInfo -= value;
            onUpdateMarketInfo += value;
        }
        remove
        {
            onUpdateMarketInfo -= value;
        }
    }

    public Dictionary<eMarketType, List<MarketInfo>> markets = new Dictionary<eMarketType, List<MarketInfo>>();
    public Dictionary<eMarketType, string> marketAllStrs = new Dictionary<eMarketType, string>();

    /// <summary>
    /// 마켓 기본 정보 세팅
    /// </summary>
    /// <param name="res"></param>
    public void SetBaseInfo(List<MarketRes> res)
    {
        markets.Clear();
        marketAllStrs.Clear();
        if (res != null)
        {
            for (int i = 0; i < res.Count; i++)
            {
                var marketRes = res[i];

                if (marketRes.GetWarning() == MarketRes.eWarning.NONE)
                {
                    string market = marketRes.market.Split('-')[0];
                    eMarketType marketType = Utils.FindEnumValue<eMarketType>(market);
                    if (!markets.TryGetValue(marketType, out var list))
                    {
                        list = new List<MarketInfo>();
                        markets.Add(marketType, list);
                    }

                    var marketInfo = new MarketInfo
                    {
                        name = marketRes.market,
                        korean_name = marketRes.korean_name,
                        trade_price = 0,
                    };
                    list.Add(marketInfo);

                    // 스트링 리스트 따로 작성
                    if (!marketAllStrs.TryGetValue(marketType, out var str))
                    {
                        marketAllStrs.Add(marketType, string.Empty);
                    }

                    if (marketAllStrs[marketType].Length > 0)
                    {
                        marketAllStrs[marketType] += $",{marketRes.market}";
                    }
                    else
                        marketAllStrs[marketType] += marketRes.market;
                }
            }
        }
    }

    /// <summary>
    /// 마켓 정보 리스트 반환
    /// </summary>
    /// <param name="marketType"></param>
    /// <returns></returns>
    public List<MarketInfo> GetMarketInfos(eMarketType marketType)
    {
        markets.TryGetValue(marketType, out List<MarketInfo> marketInfos);
        return marketInfos;
    }

    /// <summary>
    /// 마켓 정보 반환
    /// </summary>
    /// <param name="market"></param>
    /// <returns></returns>
    public MarketInfo GetMarketInfo(string market)
    {
        var enumerator = markets.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var list = enumerator.Current.Value;
            var find = list.Find(info => info.name.Equals(market));
            if (find != null)
                return find;
        }
        return null;
    }

    /// <summary>
    /// 마켓 리스트 스트링 반환
    /// </summary>
    /// <param name="marketType">마켓 타입</param>
    /// <returns></returns>
    public string GetAllMarketStr(eMarketType marketType)
    {
        marketAllStrs.TryGetValue(marketType, out string str);
        return str;
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

                var enumerator = markets.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var marketInfos = enumerator.Current.Value;
                    if (marketInfos != null)
                    {
                        var marketInfo = marketInfos.Find(info => info.name == tickerRes.market);
                        if (marketInfo != null)
                        {
                            bool isUpdate = false;

                            if (marketInfo.trade_price != tickerRes.trade_price)
                            {
                                isUpdate = true;
                                // 현재가 세팅
                                marketInfo.trade_price = tickerRes.trade_price;
                            }

                            //if (marketInfo.prev_closing_price != tickerRes.prev_closing_price)
                            //{
                            //    isUpdate = true;
                            //    // 전일 종가 세팅
                            //    marketInfo.prev_closing_price = tickerRes.prev_closing_price;
                            //}

                            if (isUpdate)
                                onUpdateMarketInfo?.Invoke(marketInfo);
                        }
                    }
                }
            }
        }
    }
}