using CoinTrader.ML;
using Network;
using System;
using System.Threading;

public static class DeeplearningProcess
{
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
                    if (marketInfo != null && !marketInfo.isCandleOldDataSuccess)
                    {

                        /// 현재부터 과거 데이터까지 받는건 처리됨
                        /// 그럼 이후 최신 시간은 어떻게 업데이트할지 고려할 것!!







                        DateTime nowTime = MachineLearning.GetFirstDateTime(marketInfo.name);
                        if (nowTime == DateTime.MaxValue)
                            nowTime = Time.NowTime;
                        DateTime dateTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, 0, 0);
                        string to = dateTime.ToString("yyyy-MM-dd HH:mm:ss");

                        bool isFinished = false;
                        ProtocolManager.GetHandler<HandlerCandlesMinutes>().Request(60, marketInfos[i].name, to: to, onFinished: (result, res) =>
                        {
                            if (res != null && res.Count > 0)
                            {
                                var first = res[0];
                                bool create = MachineLearning.CreateTrainCSV(res, first.market);
                                var text = MachineLearning.GetFirstDateTime(first.market);
                                if (!create)
                                    MachineLearning.AppendTrainCSV(res, first.market);
                            }
                            else
                            {
                                marketInfo.isCandleOldDataSuccess = true;
                            }
                            isFinished = true;
                        });

                        while (!isFinished)
                        {
                            Thread.Sleep(100);
                        }

                        // 예상 종가 도출
                        marketInfo.SetPredictPrices(Time.NowTime, MachineLearning.GetPredictePrice(marketInfo.name));
                    }
                }
            }

            Thread.Sleep(100);
        }
    }
}