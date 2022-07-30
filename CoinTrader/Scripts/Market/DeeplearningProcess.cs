using CoinTrader.ML;
using Network;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public static class DeeplearningProcess
{
    public static Action<MarketInfo> onUpdateMarketInfo = null;

    private static bool isStarted = false;

    private static List<string> completedOldDataMarketNames = new List<string>();

    public static void Start()
    {
        if (!isStarted)
        {
            isStarted = true;
            Process();
        }
    }

    private static async void Process()
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
                        await Task.Run(async () =>
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
                                        MachineLearning.AddOld(res[0].market, ConvertDatas(res));
                                    }
                                    else
                                    {
                                        completedOldDataMarketNames.Add(marketInfo.name);
                                    }
                                    isFinished = true;
                                });

                                while (!isFinished)
                                {
                                    await Task.Delay(100);
                                }
                            }

                            // 최신 데이터들 불러오기
                            DateTime lastTime = MachineLearning.GetLatestDateTime(marketInfo.name);
                            if (lastTime == DateTime.MinValue)
                                lastTime = Time.NowTime;
                            TimeSpan ts = Time.NowTime - lastTime;
                            int addHours = (int)ts.TotalHours;
                            if (addHours > 200) // 200개 초과하면 줄인다
                                addHours = 200;
                            lastTime = lastTime.AddHours(addHours);

                            if (addHours > 0f)
                            {
                                string to = lastTime.ToString("yyyy-MM-dd HH:mm:ss");
                                bool isFinished = false;
                                ProtocolManager.GetHandler<HandlerCandlesMinutes>().Request(60, marketInfos[i].name, to: to, count: addHours - 1, onFinished: (result, res) =>
                                {
                                    if (res != null && res.Count > 0)
                                    {
                                        MachineLearning.AddLatest(res[0].market, ConvertDatas(res));
                                    }
                                    isFinished = true;
                                });

                                while (!isFinished)
                                {
                                    await Task.Delay(100);
                                }
                            }

                            // 예상 종가 도출
                            marketInfo.SetPredictPrices(Time.NowTime, MachineLearning.GetPredictePrice(marketInfo.name));

                        });
                        
                        onUpdateMarketInfo?.Invoke(marketInfo);
                    }
                }
            }

            await Task.Delay(100);
        }
    }

    private static CandlesData ConvertData(CandlesMinutesRes res)
    {
        return new CandlesData
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

    private static List<CandlesData> ConvertDatas(List<CandlesMinutesRes> res)
    {
        List<CandlesData> datas = new List<CandlesData>();
        for (int i = 0; i < res.Count; i++)
        {
            datas.Add(ConvertData(res[i]));
        }
        return datas;
    }
}