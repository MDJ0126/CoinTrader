using MetroFramework.Forms;
using System;
using System.Windows.Forms;

namespace CoinTrader.Forms
{
    public partial class LoginForm : MetroForm
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            TryLogin();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                TryLogin();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                TryLogin();
            }
        }

        /// <summary>
        /// 로그인 시도
        /// </summary>
        private void TryLogin()
        {
            Config.ACCESS_KEY = accessKeyText.Text;
            Config.SECRET_KEY = secretKeyText.Text;

            this.Hide();
            var lodingForm = new LoadingForm();
            lodingForm.Show();
        }
    }
}
