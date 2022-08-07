using Network;
using System;
using System.Threading.Tasks;

public static class AutoTradingProcess
{
    private static bool isStarted = false;

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
        const float USE_BALANCE_RATE = 0.5f;

        var myAccounts = ModelCenter.Account.Accounts;
        var marketInfos = ModelCenter.Market.GetMarketInfos(eMarketType.KRW);
        DateTime buyingTime = Time.NowTime;
        while (true)
        {
            // 총 평가 (순수 코인만)
            double only_coin_balance = 0d;
            // 총 보유 자산 (코인 + 원화)
            double total_balance = 0d;
            // 매매에 사용할 투자 금액
            double use_balance = 0d;

            for (int i = 0; i < myAccounts.Count; i++)
            {
                if (!myAccounts[i].currency.Equals("KRW"))
                {
                    // 코인
                    var marketInfo = ModelCenter.Market.GetMarketInfo(myAccounts[i].currency);
                    if (marketInfo != null)
                    {
                        if (marketInfo.trade_price != 0d)
                            only_coin_balance += myAccounts[i].balance * marketInfo.trade_price;
                    }
                }
                else
                {
                    // 원화
                    total_balance += myAccounts[i].balance;
                }
            }
            total_balance += only_coin_balance;

            // 1. 총 자산 중 50%만 투자에 사용한다.
            use_balance = total_balance * USE_BALANCE_RATE;

            use_balance = Math.Truncate(use_balance);

            bool isAvailableKRW = (total_balance - only_coin_balance) * USE_BALANCE_RATE >= use_balance && use_balance > 6000;

            // 2. 예측 결과 6시간 중, 변동성 돌파가 있고, 골든 크로스인 경우 매수 시작
            if (myAccounts.Count < 2 && isAvailableKRW)   // 원화 + 코인이면 잔고 길이가 '2'이다.
            {
                // 매수
                for (int i = 0; i < marketInfos.Count; i++)
                {
                    bool isTargetPriceSuccess = false;
                    bool isGoldenCross = false;

                    var marketInfo = marketInfos[i];
                    if (marketInfo != null && marketInfo.predictPrices != null && marketInfo.predictPrices.Count > 0)
                    {
                        if (marketInfo.buy_target_price > marketInfo.trade_price)
                        {
                            for (int k = 0; k < marketInfo.predictPrices.Count; k++)
                            {
                                if (k < 6)  // 6시간 정도만 예측
                                {
                                    if (marketInfo.buy_target_price < marketInfo.predictPrices[k].forecasted)
                                    {
                                        isTargetPriceSuccess = true;
                                        break;
                                    }
                                }
                                else
                                    break;
                            }
                        }
                        isGoldenCross = marketInfo.IsGoldenCross;
                    }

                    // 조건 충족 매수 시작
                    if (isTargetPriceSuccess && isGoldenCross)
                    {
                        var close_price_rate = (marketInfo.buy_target_price - marketInfo.trade_price) / marketInfo.trade_price;
                        if (close_price_rate > 0.02f)
                        {
                            //Logger.Log($"매수 시도 {marketInfo}");    // 메신저 넣자
                            await Buy(marketInfo.name, use_balance);
                            buyingTime = Time.NowTime;
                            break;
                        }
                    }
                }
            }
            else
            {
                // 매도
                for (int i = 0; i < myAccounts.Count; i++)
                {
                    if (!myAccounts[i].currency.Equals("KRW"))
                    {
                        var marketInfo = ModelCenter.Market.GetMarketInfo(myAccounts[i].currency);
                        if (marketInfo != null)
                        {
                            if (marketInfo.trade_price != 0d)
                            {
                                var avg_buy_price_rate = (marketInfo.trade_price - myAccounts[i].avg_buy_price) / myAccounts[i].avg_buy_price;
                                if (avg_buy_price_rate > 0.02f                      // +2%가 되거나
                                    || avg_buy_price_rate < -0.04f                  // -4%가 되거나
                                    || buyingTime.AddHours(6f) < Time.NowTime)      // 6시간이 지났을 경우 매도
                                {
                                    //Logger.Log($"매도 시도 {marketInfo}");     // 메신저 넣자
                                    await Sell($"KRW-{myAccounts[i].currency}", myAccounts[i].balance);
                                }
                            }
                        }
                    }
                }
            }

            await Task.Delay(10);
        }
    }

    /// <summary>
    /// 매수하기 (잔고에 모두 입금이 완료될떄까지 대기 가능)
    /// </summary>
    /// <param name="market"></param>
    /// <param name="price"></param>
    /// <param name="onFinished"></param>
    private static async Task Buy(string market, double price)
    {
        var res = await ProtocolManager.GetHandler<HandlerOrders>().Request(market, "bid", "", price.ToString(), "price", "");
        if (res != null)
        {
            var res2 = await ProtocolManager.GetHandler<HandlerAccount>().Request();
        }
    }

    /// <summary>
    /// 매도하기
    /// </summary>
    /// <param name="market"></param>
    /// <param name="volume"></param>
    /// <param name="onFinished"></param>
    private static async Task Sell(string market, double volume)
    {
        var res = await ProtocolManager.GetHandler<HandlerOrders>().Request(market, "ask", volume.ToString(), "", "market", "");
        if (res != null)
        {
            var res2 = ProtocolManager.GetHandler<HandlerAccount>().Request();
        }
    }

    // 메모
    // 1. 비트코인을 기준으로 팔고 사고를 한다.
    // 비트코인이 현재가가 플러스고, 골든 크로스로 올라가고 있으면
    // 보유하던 코인을 매도하고, 비트코인을 산다.

    // 브릿지 코인 => KRW 하지만 수수료 0.05% 발생

    // 비트코인이 현재가가 마이너스고, 데드 크로스로 떨어지고 있다면
    // 비트코인을 매도하고, 거래량이 높은 골든 크로스 코인을 산다.
}