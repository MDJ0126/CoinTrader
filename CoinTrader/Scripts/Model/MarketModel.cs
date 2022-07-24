using CoinTrader.ML;
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

                    list.Add(new MarketInfo
                    {
                        name = marketRes.market,
                        korean_name = marketRes.korean_name,
                        trade_price = 0,
                    });

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
                            if (marketInfo.trade_price != tickerRes.trade_price)
                            {
                                marketInfo.trade_price = tickerRes.trade_price;
                                if (marketInfo.yesterday_trade_price == null)
                                    marketInfo.yesterday_trade_price = tickerRes.trade_price;
                                if (marketInfo.predictePrice == null)
                                    marketInfo.predictePrice = tickerRes.trade_price;
                                onUpdateTicker?.Invoke(marketInfo);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 일(Day) 캔들 업데이트
    /// </summary>
    /// <param name="res"></param>
    public void UpdateCandlesDays(List<CandlesDaysRes> res)
    {
        if (res != null)
        {
            var first = res[0];
            
            //res.Insert(0, new CandlesDaysRes { market = first.market, candle_date_time_utc = Time.NowTime.Date.ToString(), candle_date_time_kst = Time.NowTime.Date.AddHours(9).ToString() });
            bool create = MachineLearning.CreateTrainCSV(res, first.market, "Days");
            if (!create)
                MachineLearning.AppendTrainCSV(res, first.market, "Days");

            for (int i = 0; i < res.Count; i++)
            {
                var candlesDays = res[i];
                var enumerator = markets.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var marketInfos = enumerator.Current.Value;
                    if (marketInfos != null)
                    {
                        var marketInfo = marketInfos.Find(info => info.name == candlesDays.market);
                        if (marketInfo != null)
                        {
                            var date = candlesDays.GetTradeDateTime(eTimeType.KST);
                            if (date.Date == Time.NowTime.Date.AddDays(-1))
                            {
                                marketInfo.yesterday_trade_price = candlesDays.trade_price;
                                onUpdateTicker?.Invoke(marketInfo);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 분(Minute) 캔들 업데이트
    /// </summary>
    /// <param name="res"></param>
    public void UpdateCandlesMinutes(List<CandlesMinutesRes> res)
    {
        if (res != null)
        {
            var first = res[0];
            bool create = MachineLearning.CreateTrainCSV(res, first.market, "Minutes");
            if (!create)
                MachineLearning.AppendTrainCSV(res, first.market, "Minutes");
        }
    }

    /// <summary>
    /// 예상 종가 도출
    /// </summary>
    /// <param name="marketType"></param>
    /// <param name="market"></param>
    public void UpdatePredictePrice(eMarketType marketType, string market)
    {
        var marketInfos = GetMarketInfos(marketType);
        if (marketInfos != null)
        {
            var marketInfo = marketInfos.Find(info => info.name == market);
            if (marketInfo != null)
            {
                marketInfo.predictePrice = MachineLearning.GetPredictePrice(market, Time.NowTime.AddDays(1).Date, "Days");
                onUpdateTicker?.Invoke(marketInfo);
            }
        }
    }
}