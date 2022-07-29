using CoinTrader.ML;
using System.Collections;
using System.Windows.Forms;

namespace CoinTrader.Forms
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
            this.StartCoroutine(Start());
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            MachineLearning.Initialize((float total, float current, string text) => UpdateProgressBar(total, current, text));
            yield return new WaitForSeconds(0.5f);
            Completed();
        }

        private void Completed()
        {
            this.Hide();
            var mainForm = new MainForm();
            mainForm.Show();
        }

        private void UpdateProgressBar(float total, float current, string text)
        {
            loadingProgressBar.Value = (int)(current / total * 100f);
            loadingProgressBar.Update();
            loadingLabel.Text = $"{text}...({current}/{total})";
            loadingLabel.Update();
        }
    }
}
