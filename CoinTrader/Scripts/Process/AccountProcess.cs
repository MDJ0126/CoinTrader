﻿using System.Threading.Tasks;

/// <summary>
/// 잔고 업데이터
/// </summary>
public static class AccountUpdater
{
    public delegate void UpdateAccount(double price);
    public static UpdateAccount updateAccount;

    private static bool isStarted = false;
    private static bool isRequestStop = false;

    public static void Start()
    {
        if (!isStarted)
        {
            isStarted = true;
            isRequestStop = false;
            Process();
        }
    }

    public static void Stop()
    {
        isStarted = false;
        isRequestStop = true;
    }

    private static async void Process()
    {
        var myAccounts = ModelCenter.Account.Accounts;
        while (!isRequestStop)
        {
            // 나의 현재 자산 총 평가
            double avg_price_KRW = 0d;

            for (int i = 0; i < myAccounts.Count; i++)
            {
                if (!myAccounts[i].currency.Equals("KRW"))
                {
                    var marketInfo = ModelCenter.Market.GetMarketInfo(myAccounts[i].currency);
                    if (marketInfo != null)
                    {
                        if (marketInfo.trade_price != 0d)
                            avg_price_KRW += myAccounts[i].balance * marketInfo.trade_price;
                    }
                }
            }

            updateAccount?.Invoke(avg_price_KRW);
            await Task.Delay(100);
        }
    }
}