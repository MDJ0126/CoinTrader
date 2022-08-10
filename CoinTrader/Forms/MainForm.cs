using Network;
using System;
using MetroFramework.Forms;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

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
            //this.StyleManager = metroStyleManager;
            //this.StyleManager.Theme = MetroFramework.MetroThemeStyle.Dark;
            metroListView1.DoubleBuffered(true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("투자에 주의 바랍니다.");
            Initialize();
        }

        private void OnAddLog(string text)
        {
            toolStripStatusLabel1.Text = text;
        }

        /// <summary>
        /// 초기화
        /// </summary>
        private void Initialize()
        {
            // 로거 등록
            Logger.OnLogger += OnAddLog;

            // 타이머 작동
            Timer();

            // 마켓 리스트 리스트뷰 세팅
            SetMarketList();

            // 개인 잔고 업데이터
            AccountUpdater.Start();
            AccountUpdater.updateAccount = OnUpdateAccount;

            // 딥러닝 프로세스
            DeeplearningProcess.Start();
            DeeplearningProcess.onUpdateMarketInfo += OnUpdateMarketInfo;

            // 자동 거래 프로세스
            AutoTradingProcess.Start();

            // 마켓 정보 갱신 이벤트
            ModelCenter.Market.OnUpdateMarketInfo += OnUpdateMarketInfo;

            // 자정 업데이터
            PassthedayUpdater();
        }

        private async void PassthedayUpdater()
        {
            while (true)
            {
                // 하루에 한 번씩 업데이트 하는 프로세스
                DateTime nowTime = Time.NowTime;
                DateTime nextDate = nowTime.Date.AddDays(1f);
                TimeSpan remain = nextDate - nowTime;
                await Task.Delay(remain);

                AutoTradingProcess.Stop();
                DeeplearningProcess.Stop();
                AccountUpdater.Stop();

                await DataManager.Updater();

                AutoTradingProcess.Start();
                DeeplearningProcess.Start();
                AccountUpdater.Start();
            }
        }

        /// <summary>
        /// 현재 자산 총평가
        /// </summary>
        /// <param name="price"></param>
        private void OnUpdateAccount(double price)
        {
            currentPrice.Text = price.ToString("N0");
        }

        /// <summary>
        /// 타이머
        /// </summary>
        /// <returns></returns>
        private async void Timer()
        {
            while (true)
            {
                timeLabel.Text = Time.NowTime.ToString("yyyy년 M월 d일 tt hh:mm:ss");
                await Task.Delay(100);
            }
        }

        /// <summary>
        /// 마켓 리스트뷰 세팅
        /// </summary>
        private void SetMarketList()
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
            
            RequestTicker();
        }

        /// <summary>
        /// 주기적으로 현재가 갱신 요청
        /// </summary>
        /// <returns></returns>
        private async void RequestTicker()
        {
            while (true)
            {
                var res = await ProtocolManager.GetHandler<HandlerTicker>().Request(eMarketType.KRW);
                await Task.Delay(200);
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
                percentage = (marketInfo.movingAverage_15 - marketInfo.prev_closing_price) / marketInfo.prev_closing_price * 100f;
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
                percentage = (marketInfo.movingAverage_30 - marketInfo.prev_closing_price) / marketInfo.prev_closing_price * 100f;
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
                percentage = (marketInfo.buy_target_price - marketInfo.prev_closing_price) / marketInfo.prev_closing_price * 100f;
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

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
