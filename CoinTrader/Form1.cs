using Network;
using System;
using System.Windows.Forms;

namespace CoinTrader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ProtocolManager.GetHandler<HandlerAccount>().Request();
            ProtocolManager.GetHandler<HandlerApiKey>().Request();
            ProtocolManager.GetHandler<HandlerTicker>().Request("KRW-BTC, BTC-ETH");
            ProtocolManager.GetHandler<HandlerMarket>().Request((result, res) => 
            {
                Logger.Error(MultiThread.IsCurrentMainThread());
                listView1.Items.Clear();    // UI 쓰레드 접근 가능
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
            listView1.BeginUpdate();
            listView1.Items.Add(text);
            listView1.EndUpdate();
            listView1.Items[listView1.Items.Count - 1].EnsureVisible();
        }
    }
}
