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
            this.StartCoroutine(Test());
        }

        private IEnumerator Test()
        {
            int count = 0;
            while (true)
            {
                metroButton1.Text = (++count).ToString();
                yield return new WaitForSeconds(0.1f);
            }
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

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.Show();
        }
    }
}
