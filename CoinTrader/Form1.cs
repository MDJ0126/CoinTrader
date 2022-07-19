using Network;
using System;
using MetroFramework.Forms;
using System.Collections;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

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
            ProtocolManager.GetHandler<HandlerTicker>().Request("KRW-BTC, BTC-ETH");

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

            listView1.Items.Clear();
            listView1.DoubleBuffered(true);
        }

        private void SetStockList()
        {
            ProtocolManager.GetHandler<HandlerMarket>().Request((result, res) =>
            {
                metroListView1.BeginUpdate();
                for (int i = 0; i < res.Count; i++)
                {
                    metroListView1.Items.Add(res[i].korean_name);
                }
                metroListView1.EndUpdate();
            });
        }

        private void OnLogger(string text)
        {
            listView1.Items.Add(text);
            listView1.EnsureVisible(listView1.Items.Count - 1);
        }
    }
}
