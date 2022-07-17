using Network;
using System;
using MetroFramework.Forms;
using System.Collections;
using System.Windows.Forms;
using System.Net.NetworkInformation;

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
            ProtocolManager.GetHandler<HandlerMarket>().Request((result, res) => 
            {
                // UI 쓰레드 접근 가능 (현재 여기서 호출했으므로 해당 쓰레드를 가져옴)
                Logger.Log("Ya!");
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Initialize();
            Logger.OnLogger += OnLogger;
        }

        private void Initialize()
        {
            listView1.Items.Clear();
            listView1.DoubleBuffered(true);
        }

        private void OnLogger(string text)
        {
            listView1.Items.Add(text);
            listView1.EnsureVisible(listView1.Items.Count - 1);
        }
    }
}
