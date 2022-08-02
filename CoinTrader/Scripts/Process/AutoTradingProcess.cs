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
        var myAccounts = ModelCenter.Account.Accounts;
        while (true)
        {
            // 1. 자산을 4등분한다.
            // 2. 매매 타이밍이 되면 4등분한 자산으로 한 번씩 산다.
            // 3. 1퍼센트 이익을 보면 매도를 시도한다.
            // 4. 이익본 금액은 놔두고, 4등분 대기열로 다시 반환한다.
            // 5. 매매했던 코인은 Ignore리스트에 잠시 추가
            // 6. 반복
            // 7. Ignore 리스트는 정시마다 초기화

            await Task.Delay(10);
        }
    }

    /// <summary>
    /// 매수하기
    /// </summary>
    /// <param name="market"></param>
    /// <param name="price"></param>
    /// <param name="onFinished"></param>
    private static void Buy(string market, double price, Action onFinished)
    {
        ProtocolManager.GetHandler<HandlerOrders>().Request(market, "bid", "", price.ToString(), "price", "", (result, res) =>
        {
            onFinished?.Invoke();
        });
    }

    /// <summary>
    /// 매도하기
    /// </summary>
    /// <param name="market"></param>
    /// <param name="volume"></param>
    /// <param name="onFinished"></param>
    private static void Sell(string market, double volume, Action onFinished)
    {
        ProtocolManager.GetHandler<HandlerOrders>().Request(market, "ask", volume.ToString(), "", "market", "", (result, res) =>
        {
            onFinished?.Invoke();
        });
    }

    // 메모
    // 1. 비트코인을 기준으로 팔고 사고를 한다.
    // 비트코인이 현재가가 플러스고, 골든 크로스로 올라가고 있으면
    // 보유하던 코인을 매도하고, 비트코인을 산다.

    // 브릿지 코인 => KRW 하지만 수수료 0.05% 발생

    // 비트코인이 현재가가 마이너스고, 데드 크로스로 떨어지고 있다면
    // 비트코인을 매도하고, 거래량이 높은 골든 크로스 코인을 산다.
}