using CoinTrader.ML;
using Network;
using System;
using System.Collections;
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
        private void CheckAPIKeyState(Action<bool> onFinished)
        {
            ProtocolManager.GetHandler<HandlerApiKey>().Request((result, res) =>
            {
                onFinished.Invoke(true);
            });
        }

        /// <summary>
        /// 잔고 세팅
        /// </summary>
        /// <param name="onFinished"></param>
        private void SetAccount(Action<bool> onFinished)
        {
            ProtocolManager.GetHandler<HandlerAccount>().Request((result, res) =>
            {
                onFinished.Invoke(true);
            });
        }

        /// <summary>
        /// 기본 마켓 정보 요청
        /// </summary>
        /// <param name="onFinished"></param>
        private void SetMarketList(Action<bool> onFinished)
        {
            UpdateProgressBar(1f, 1f, "마켓 리스트를 요청합니다.");
            ProtocolManager.GetHandler<HandlerMarket>().Request((result, res) =>
            {
                onFinished.Invoke(true);
            });
        }

        /// <summary>
        /// 기본 30일 캔들 데이터 세팅
        /// </summary>
        /// <param name="onFinished"></param>
        private void SetCandleDays(Action<bool> onFinished)
        {
            this.StartCoroutine(RequestCandleDays(onFinished));
        }

        private IEnumerator RequestCandleDays(Action<bool> onFinished)
        {
            if (ModelCenter.Market.markets.TryGetValue(eMarketType.KRW, out var marketInfos))
            {
                for (int i = 0; i < marketInfos.Count; i++)
                {
                    var marketInfo = marketInfos[i];
                    bool isFinished = false;
                    ProtocolManager.GetHandler<HandlerCandlesDays>().Request(marketInfo.name, "", 30, onFinished: (result, res) =>
                    {
                        marketInfo.SetCandleDaysRes(res);
                        isFinished = true;
                    });
                    yield return new WaitUntil(() => isFinished);

                    UpdateProgressBar(marketInfos.Count, i + 1, $"'{marketInfo.name}'의 30일 캔들 데이터를 요청합니다.");

                    yield return new WaitForSeconds(0.1f);
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
