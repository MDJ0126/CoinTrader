using System.Threading.Tasks;

public static class AccountProcess
{
    public delegate void UpdateAccount(double price);
    public static UpdateAccount updateAccount;

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
        var myAccounts = ModelCenter.Account.Accounts;
        while (true)
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