using CoinTrader.ML;
using Network;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoinTrader.Forms
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
            StartProcess();
        }

        private async void StartProcess()
        {
            await Task.Delay(500);
            await DataManager.Updater(UpdateProgressBar);
            Completed();
        }

        private void Completed()
        {
            this.StopAllCoroutine();
            this.Hide();
            var mainForm = new MainForm();
            mainForm.Show();
        }

        private void UpdateProgressBar(float total, float current, string text)
        {
            loadingProgressBar.Value = (int)(current / total * 100f);
            loadingProgressBar.Update();
            loadingLabel.Text = $"{text} ({current}/{total})";
            loadingLabel.Update();
        }
    }
}
