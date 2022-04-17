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
            Logger.Log(1);
            Logger.Log(2);
            Logger.Log(3);
            Logger.Log(4);
            Logger.Log(5);
        }

        private void Initialize()
        {
            textBox1.Text = string.Empty;
        }

        private void OnLogger(string text)
        {
            textBox1.Text += $"{text}\r\n";
        }
    }
}
