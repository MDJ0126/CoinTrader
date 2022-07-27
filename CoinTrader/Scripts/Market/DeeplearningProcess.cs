using CoinTrader.ML;
using Network;
using System;
using System.Collections.Generic;
using System.Threading;

public static class DeeplearningProcess
{
    private static List<MarketInfo> onUpdates = new List<MarketInfo>();

    private static List<string> completedOldDataMarketNames = new List<string>();

    public static MarketInfo DequeueUpdatedMarketInfo()
    {
        MarketInfo marketInfo = null;
        if (onUpdates.Count > 0)
        {
            marketInfo = onUpdates[0];
            onUpdates.RemoveAt(0);
        }
        return marketInfo;
    }

    public static void Start()
    {
        MultiThread.Start(() => Process());
    }

    private static void Process()
    {
        while (true)
        {
            var marketInfos = ModelCenter.Market.GetMarketInfos(eMarketType.KRW);
            if (marketInfos != null)
            {
                DateTime start = Time.NowTime;
                for (int i = 0; i < marketInfos.Count; i++)
                {
                    var marketInfo = marketInfos[i];
                    if (marketInfo != null)
                    {
                        // 과거 데이터들 불러오기
                        if (!completedOldDataMarketNames.Exists(name => name.Equals(marketInfo.name)))
                        {
                            DateTime oldTime = MachineLearning.GetOldDateTime(marketInfo.name);
                            if (oldTime == DateTime.MaxValue)
                                oldTime = Time.NowTime;
                            string to = oldTime.ToString("yyyy-MM-dd HH:mm:ss");

                            bool isFinished = false;
                            ProtocolManager.GetHandler<HandlerCandlesMinutes>().Request(60, marketInfos[i].name, to: to, onFinished: (result, res) =>
                            {
                                if (res != null && res.Count > 0)
                                {
                                    for (int k = 0; k < res.Count; k++)
                                    {
                                        MachineLearning.Add(res[k].market, ConvertData(res[k]));
                                    }
                                }
                                else
                                {
                                    completedOldDataMarketNames.Add(marketInfo.name);
                                }
                                isFinished = true;
                            });

                            while (!isFinished)
                            {
                                Thread.Sleep(100);
                            }
                        }

                        // 최신 데이터들 불러오기
                        DateTime lastTime = MachineLearning.GetLastDateTime(marketInfo.name);
                        if (lastTime == DateTime.MinValue)
                            lastTime = Time.NowTime;
                        TimeSpan ts = Time.NowTime - lastTime;
                        int addHours = (int)ts.TotalHours;
                        if (ts.TotalHours > 200f) // 200개 초과하면 줄인다
                            addHours -= (int)(ts.TotalHours - 200f);
                        lastTime = lastTime.AddHours(addHours);

                        if (addHours > 0f)
                        {
                            string to = lastTime.ToString("yyyy-MM-dd HH:mm:ss");
                            bool isFinished = false;
                            ProtocolManager.GetHandler<HandlerCandlesMinutes>().Request(60, marketInfos[i].name, to: to, count: addHours, onFinished: (result, res) =>
                            {
                                if (res != null && res.Count > 0)
                                {
                                    for (int k = 0; k < res.Count; k++)
                                    {
                                        MachineLearning.Add(res[k].market, ConvertData(res[k]));
                                    }
                                }
                                isFinished = true;
                            });


                            while (!isFinished)
                            {
                                Thread.Sleep(100);
                            }
                        }

                        // 예상 종가 도출
                        marketInfo.SetPredictPrices(Time.NowTime, MachineLearning.GetPredictePrice(marketInfo.name));

                        if (!onUpdates.Exists(info => info.Equals(marketInfo)))
                            onUpdates.Add(marketInfo);
                    }
                }
            }

            Thread.Sleep(100);
        }
    }

    private static CandlesDayData ConvertData(CandlesMinutesRes res)
    {
        return new CandlesDayData
        {
            market = res.market,
            candle_date_time_utc = Convert.ToDateTime(res.candle_date_time_utc),
            candle_date_time_kst = Convert.ToDateTime(res.candle_date_time_kst),
            opening_price = res.opening_price,
            high_price = res.high_price,
            low_price = res.low_price,
            trade_price = (float)res.trade_price,
            timestamp = res.timestamp,
            candle_acc_trade_price = res.candle_acc_trade_price,
            candle_acc_trade_volume = res.candle_acc_trade_volume,
        };
    }
}