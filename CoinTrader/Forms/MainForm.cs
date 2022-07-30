using Network;
using System;
using MetroFramework.Forms;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace CoinTrader.Forms
{
    public partial class MainForm : MetroForm
    {
        private enum eTickerHeader
        {
            Market,
            Name,
            Price,
            YesterDay,
            MovingAverage15,
            MovingAverage30,
            TodayPredicteClosePrice,
            BuyTargetPrice,
            GoldenCross,
        }

        public MainForm()
        {
            InitializeComponent();
            this.StyleManager = metroStyleManager;
            //this.StyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            ProtocolManager.GetHandler<HandlerAccount>().Request();
            ProtocolManager.GetHandler<HandlerApiKey>().Request();
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
            // 타이머 작동
            this.StartCoroutine(Timer());

            // 워터풀 프로세스 시작
            WaterfallProcess wfp = new WaterfallProcess();
            wfp.Add(SetMarketList);
            wfp.Start(result =>
            {
                DeeplearningProcess.Start();
                DeeplearningProcess.onUpdateMarketInfo += OnUpdateMarketInfo;
                ModelCenter.Market.OnUpdateMarketInfo += OnUpdateMarketInfo;

                listView1.Items.Clear();
                listView1.DoubleBuffered(true);
                metroListView1.DoubleBuffered(true);
            });
        }

        /// <summary>
        /// 타이머 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator Timer()
        {
            while (true)
            {
                timeLabel.Text = Time.NowTime.ToString("yyyy년 M월 d일 tt hh:mm:ss");
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// 코인 리스트 세팅
        /// </summary>
        private void SetMarketList(Action<bool> onFinished)
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
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("");
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
        /// 현재가 갱신
        /// </summary>
        /// <param name="marketInfo"></param>
        private void OnUpdateMarketInfo(MarketInfo marketInfo)
        {
            metroListView1.BeginUpdate();
            ListViewItem item = metroListView1.Find(marketInfo.name);
            if (item != null)
            {
                item.UseItemStyleForSubItems = false;

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
                item.SubItems[(int)eTickerHeader.YesterDay].ForeColor = color;
                item.SubItems[(int)eTickerHeader.YesterDay].Text = $"{symbol} {marketInfo.trade_price - marketInfo.prev_closing_price:N0}원 ({percentage:F2}%)";

                // 이동평균 15일
                percentage = (marketInfo.movingAverage_15 - marketInfo.prev_closing_price) / marketInfo.prev_closing_price;
                symbol = string.Empty;
                color = Color.Black;
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
                item.SubItems[(int)eTickerHeader.MovingAverage15].ForeColor = color;
                item.SubItems[(int)eTickerHeader.MovingAverage15].Text = $"{symbol} {marketInfo.movingAverage_15:N0}원 ({percentage:F2}%)";

                // 이동평균 30일
                percentage = (marketInfo.movingAverage_30 - marketInfo.prev_closing_price) / marketInfo.prev_closing_price;
                symbol = string.Empty;
                color = Color.Black;
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
                item.SubItems[(int)eTickerHeader.MovingAverage30].ForeColor = color;
                item.SubItems[(int)eTickerHeader.MovingAverage30].Text = $"{symbol} {marketInfo.movingAverage_30:N0}원 ({percentage:F2}%)";

                // 금일 예상 종가
                if (marketInfo.predictPrices != null && marketInfo.predictPrices.Count > 0)
                {
                    percentage = (marketInfo.predictPrices[0].forecasted - marketInfo.prev_closing_price) / marketInfo.prev_closing_price * 100f;
                    symbol = string.Empty;
                    color = Color.Black;
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
                    item.SubItems[(int)eTickerHeader.TodayPredicteClosePrice].ForeColor = color;
                    item.SubItems[(int)eTickerHeader.TodayPredicteClosePrice].Text = $"{symbol} {marketInfo.predictPrices[0].forecasted:N0}원 ({percentage:F2}%)";
                }

                // 매수 예상 가격
                percentage = (marketInfo.buy_target_price - marketInfo.prev_closing_price) / marketInfo.prev_closing_price;
                symbol = string.Empty;
                color = Color.Black;
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
                item.SubItems[(int)eTickerHeader.BuyTargetPrice].ForeColor = color;
                item.SubItems[(int)eTickerHeader.BuyTargetPrice].Text = $"{symbol} {marketInfo.buy_target_price:N0}원 ({percentage:F2}%)";

                // 골든 크로스
                bool isGoldenCross = marketInfo.IsGoldenCross;
                string text = string.Empty;
                color = Color.Black;
                if (isGoldenCross)
                {
                    color = Color.Red;
                    text = "Golden Cross";
                }
                else
                {
                    color = Color.Black;
                    text = "Dead Cross";
                }
                item.SubItems[(int)eTickerHeader.GoldenCross].ForeColor = color;
                item.SubItems[(int)eTickerHeader.GoldenCross].Text = text;
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

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
