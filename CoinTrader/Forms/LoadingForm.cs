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
            MachineLearning.Initialize(value => UpdateProgressBar(value));
            yield return new WaitForSeconds(0.5f);
            Completed();
        }

        private void Completed()
        {
            this.Hide();
            var mainForm = new MainForm();
            mainForm.Show();
        }

        private void UpdateProgressBar(float value)
        {
            loadingProgressBar.Value = (int)(value * 100f);
        }
    }
}
