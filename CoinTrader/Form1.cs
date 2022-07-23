using Network;
using System;
using MetroFramework.Forms;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace CoinTrader
{
    public partial class Form1 : MetroForm
    {
        private enum eTickerHeader
        {
            Market,
            Name,
            Price,
            YesterDay,
            TargetSell,
            TargetBuy,
        }

        public Form1()
        {
            InitializeComponent();
            this.StyleManager = metroStyleManager;
            //this.StyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            ProtocolManager.GetHandler<HandlerAccount>().Request();
            ProtocolManager.GetHandler<HandlerApiKey>().Request();

            // 계좌 리스트 뿌리기
            // 종목 리스트 뿌리기
            // 매입 리스트 뿌리기
            // 그래프는 시간 날 때
            // 폰트 적용
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("투자에 주의 바랍니다.");
            Initialize();
            Logger.OnLogger += OnLogger;
        }

        /// <summary>
        /// 초기화
        /// </summary>
        private void Initialize()
        {
            WaterfallProcess wfp = new WaterfallProcess();
            wfp.Add(SetCoinList);
            wfp.Add(RequestCandlesDays);
            wfp.Start(result =>
            {
                ModelCenter.Market.OnUpdateTicker += OnUpdateTicker;

                listView1.Items.Clear();
                listView1.DoubleBuffered(true);
                metroListView1.DoubleBuffered(true);
            });
        }

        /// <summary>
        /// 코인 리스트 세팅
        /// </summary>
        private void SetCoinList(Action<bool> onFinished)
        {
            ProtocolManager.GetHandler<HandlerMarket>().Request((result, res) =>
            {
                metroListView1.BeginUpdate();
                if (ModelCenter.Market.markets.TryGetValue(eMarketType.KRW, out var marketInfos))
                {
                    for (int i = 0; i < marketInfos.Count; i++)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = marketInfos[i].name;
                        item.SubItems.Add(marketInfos[i].korean_name);
                        item.SubItems.Add("");
                        item.SubItems.Add("");
                        metroListView1.Items.Add(item);
                    }
                }
                metroListView1.EndUpdate();

                if (tickerCoroutine != null)
                    this.StopCoroutine(tickerCoroutine);
                tickerCoroutine = this.StartCoroutine(RequestTicker());

                onFinished?.Invoke(true);
            });
        }

        private Coroutine tickerCoroutine = null;
        /// <summary>
        /// 주기적으로 현재가 갱신 요청
        /// </summary>
        /// <returns></returns>
        private IEnumerator RequestTicker()
        {
            while (true)
            {
                const float INTERVAL = 0.2f;
                bool isFinished = false;
                ProtocolManager.GetHandler<HandlerTicker>().Request(eMarketType.KRW, (result, res) =>
                {
                    isFinished = true;
                });
                yield return new WaitUntil(() => isFinished);
                yield return new WaitForSeconds(INTERVAL);
            }
        }

        /// <summary>
        /// 캔들 요청
        /// </summary>
        /// <param name="onFinished"></param>
        private void RequestCandlesDays(Action<bool> onFinished)
        {
            if (candlesDaysCoroutine != null)
                this.StopCoroutine(candlesDaysCoroutine);
            candlesDaysCoroutine = this.StartCoroutine(RequestCandlesDays());
            onFinished?.Invoke(true);
        }

        private Coroutine candlesDaysCoroutine = null;
        /// <summary>
        ///  캔들 요청
        /// </summary>
        /// <returns></returns>
        private IEnumerator RequestCandlesDays()
        {
            const float INTERVAL = 0.1f;
            var marketStrs = ModelCenter.Market.GetMarketInfos(eMarketType.KRW);

            DateTime start = Time.NowTime;
            for (int i = 0; i < marketStrs.Count; i++)
            {
                bool isFinished = false;
                ProtocolManager.GetHandler<HandlerCandlesDays>().Request(marketStrs[i].name, onFinished: (result, res) =>
                {
                    isFinished = true;
                });
                yield return new WaitUntil(() => isFinished);
                yield return new WaitForSeconds(INTERVAL);
            }
            Logger.Log($"총 {marketStrs.Count}개의 캔들 확인, 소요 시간: {(Time.NowTime - start).TotalSeconds}초");
        }


        /// <summary>
        /// 현재가 갱신
        /// </summary>
        /// <param name="marketInfo"></param>
        private void OnUpdateTicker(MarketInfo marketInfo)
        {
            metroListView1.BeginUpdate();
            ListViewItem item = metroListView1.Find(marketInfo.name);
            if (item != null)
            {
                // 현재가
                item.SubItems[(int)eTickerHeader.Price].Text = $"{marketInfo.trade_price:N0}원";

                // 전일대비
                var percentage = marketInfo.GetVariabilityNormalize() * 100f;
                string symbol = string.Empty;
                Color color = Color.Black;
                if (percentage > 0f)
                {
                    color = Color.Red;
                    symbol = "▲";
                }
                else if (percentage < 0f)
                {
                    color = Color.Blue;
                    symbol = "▽";
                }
                item.ForeColor= color;
                item.SubItems[(int)eTickerHeader.YesterDay].Text = $"{symbol} {marketInfo.trade_price - marketInfo.yesterday_trade_price:N0}원 {percentage:F2}%";
            }
            metroListView1.EndUpdate();
        }

        /// <summary>
        /// 로거
        /// </summary>
        /// <param name="text"></param>
        private void OnLogger(string text)
        {
            listView1.Items.Add(text);
            listView1.EnsureVisible(listView1.Items.Count - 1);
        }
    }
}
