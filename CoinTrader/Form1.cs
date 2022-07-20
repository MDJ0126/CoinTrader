using Network;
using System;
using MetroFramework.Forms;
using System.Collections;
using System.Windows.Forms;

namespace CoinTrader
{
    public partial class Form1 : MetroForm
    {
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
            Initialize();
            Logger.OnLogger += OnLogger;
        }

        private void Initialize()
        {
            SetStockList();
            ModelCenter.Market.OnUpdateTicker += UpdateStockListTicker;

            listView1.Items.Clear();
            listView1.DoubleBuffered(true);
            metroListView1.DoubleBuffered(true);
        }

        /// <summary>
        /// 리스트 세팅
        /// </summary>
        private void SetStockList()
        {
            ProtocolManager.GetHandler<HandlerMarket>().Request((result, res) =>
            {
                metroListView1.BeginUpdate();
                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].GetWarning() == MarketRes.eWarning.NONE)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = res[i].market;
                        item.SubItems.Add(res[i].korean_name);
                        item.SubItems.Add("");
                        metroListView1.Items.Add(item);
                    }
                }
                metroListView1.EndUpdate();
                if (tickerCoroutine != null)
                    this.StopCoroutine(tickerCoroutine);
                tickerCoroutine = this.StartCoroutine(RequestTicker());
            });
        }

        private Coroutine tickerCoroutine = null;
        private IEnumerator RequestTicker()
        {
            while (true)
            {
                bool isFinished = false;
                ProtocolManager.GetHandler<HandlerTicker>().Request(ModelCenter.Market.allMarketNames, (result, res) =>
                {
                    isFinished = true;
                });
                yield return new WaitUntil(() => isFinished);
                yield return new WaitForSeconds(0.2f);
            }
        }

        private void UpdateStockListTicker(MarketInfo marketInfo)
        {
            metroListView1.BeginUpdate();
            ListViewItem item = metroListView1.Find(marketInfo.name);
            if (item != null)
                item.SubItems[2].Text = marketInfo.trade_price.ToString();
            metroListView1.EndUpdate();
        }

        private void OnLogger(string text)
        {
            listView1.Items.Add(text);
            listView1.EnsureVisible(listView1.Items.Count - 1);
        }
    }
}
