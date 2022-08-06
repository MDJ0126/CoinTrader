using CoinTrader.ML;
using Network;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoinTrader.Forms
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
            this.StartCoroutine(StartProcess());
        }
        private IEnumerator StartProcess()
        {
            yield return new WaitForSeconds(0.5f);
            WaterfallProcess wfp = new WaterfallProcess();
            wfp.Add(CheckAPIKeyState);
            wfp.Add(SetAccount);
            wfp.Add(SetMarketList);
            wfp.Add(RequestTicker);
            wfp.Add(SetCandleDays);
            wfp.Start(result =>
            {
                Completed();
            });
        }

        /// <summary>
        /// API Key 만료기한 확인하기
        /// </summary>
        /// <param name="onFinished"></param>
        private async void CheckAPIKeyState(Action<bool> onFinished)
        {
            var res = await ProtocolManager.GetHandler<HandlerApiKey>().Request();
            onFinished.Invoke(true);
        }

        /// <summary>
        /// 잔고 세팅
        /// </summary>
        /// <param name="onFinished"></param>
        private async void SetAccount(Action<bool> onFinished)
        {
            var res = await ProtocolManager.GetHandler<HandlerAccount>().Request();
            onFinished.Invoke(true);
        }

        /// <summary>
        /// 기본 마켓 정보 요청
        /// </summary>
        /// <param name="onFinished"></param>
        private async void SetMarketList(Action<bool> onFinished)
        {
            UpdateProgressBar(1f, 1f, "마켓 리스트를 요청합니다.");
            var res = await ProtocolManager.GetHandler<HandlerMarket>().Request();
            onFinished.Invoke(true);
        }

        /// <summary>
        /// 현재가 한 번 갱신
        /// </summary>
        /// <param name="onFinished"></param>
        private void RequestTicker(Action<bool> onFinished)
        {
            UpdateProgressBar(1f, 1f, "모든 현재가를 갱신합니다.");
            var res = ProtocolManager.GetHandler<HandlerTicker>().Request(eMarketType.KRW);
            onFinished.Invoke(true);
        }

        /// <summary>
        /// 기본 30일 캔들 데이터 세팅
        /// </summary>
        /// <param name="onFinished"></param>
        private async void SetCandleDays(Action<bool> onFinished)
        {
            if (ModelCenter.Market.markets.TryGetValue(eMarketType.KRW, out var marketInfos))
            {
                for (int i = 0; i < marketInfos.Count; i++)
                {
                    var marketInfo = marketInfos[i];
                    UpdateProgressBar(marketInfos.Count, i + 1, $"'{marketInfo.name}'의 30일 캔들 데이터를 요청합니다.");
                    var res = await ProtocolManager.GetHandler<HandlerCandlesDays>().Request(marketInfo.name, "", 30);
                    marketInfo.SetCandleDaysRes(res);
                    await Task.Delay(100);
                }
            }
            onFinished.Invoke(true);
        }

        private void Completed()
        {
            this.StopAllCoroutine();
            this.Hide();
            var mainForm = new MainForm();
            mainForm.Show();
        }

        private void UpdateProgressBar(float total, float current, string text)
        {
            loadingProgressBar.Value = (int)(current / total * 100f);
            loadingProgressBar.Update();
            loadingLabel.Text = $"{text} ({current}/{total})";
            loadingLabel.Update();
        }
    }
}
