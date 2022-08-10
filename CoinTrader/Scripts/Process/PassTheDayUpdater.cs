using Network;
using System;
using System.Threading.Tasks;

public static class DataManager
{
    public delegate void UpdateProgressBar(float total, float current, string text);
    public static UpdateProgressBar updateProgressBar;

    public static async Task Updater(UpdateProgressBar onUpdateProgressBar = null)
    {
        updateProgressBar = onUpdateProgressBar;
        await CheckAPIKeyState();
        await SetAccount();
        await SetMarketList();
        await RequestTicker();
        await SetCandleDays();
        OnUpdateProgressBar(1f, 1f, "데이터 갱신이 완료되었습니다.");
        updateProgressBar = null;
    }

    /// <summary>
    /// API Key 만료기한 확인하기
    /// </summary>
    /// <param name="onFinished"></param>
    private static async Task CheckAPIKeyState()
    {
        await ProtocolManager.GetHandler<HandlerApiKey>().Request();
    }

    /// <summary>
    /// 잔고 세팅
    /// </summary>
    /// <param name="onFinished"></param>
    private static async Task SetAccount()
    {
        await ProtocolManager.GetHandler<HandlerAccount>().Request();
    }

    /// <summary>
    /// 기본 마켓 정보 요청
    /// </summary>
    /// <param name="onFinished"></param>
    private static async Task SetMarketList()
    {
        OnUpdateProgressBar(1f, 1f, "마켓 리스트를 요청합니다.");
        await ProtocolManager.GetHandler<HandlerMarket>().Request();
    }

    /// <summary>
    /// 현재가 한 번 갱신
    /// </summary>
    /// <param name="onFinished"></param>
    private static async Task RequestTicker()
    {
        OnUpdateProgressBar(1f, 1f, "모든 현재가를 갱신합니다.");
        await ProtocolManager.GetHandler<HandlerTicker>().Request(eMarketType.KRW);
    }

    /// <summary>
    /// 기본 30일 캔들 데이터 세팅
    /// </summary>
    /// <param name="onFinished"></param>
    private static async Task SetCandleDays()
    {
        if (ModelCenter.Market.markets.TryGetValue(eMarketType.KRW, out var marketInfos))
        {
            for (int i = 0; i < marketInfos.Count; i++)
            {
                var marketInfo = marketInfos[i];
                OnUpdateProgressBar(marketInfos.Count, i + 1, $"'{marketInfo.name}'의 30일 캔들 데이터를 요청합니다.");
                var res = await ProtocolManager.GetHandler<HandlerCandlesDays>().Request(marketInfo.name, "", 30);
                marketInfo.SetCandleDaysRes(res);
                await Task.Delay(100);
            }
        }
    }

    private static void OnUpdateProgressBar(float total, float current, string text)
    {
        Logger.Log($"{text}.. {current}/{total}");
        updateProgressBar?.Invoke(total, current, text);
    }
}