using Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ProtocolManager.GetHandler<HandlerMarket>().Request();
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
