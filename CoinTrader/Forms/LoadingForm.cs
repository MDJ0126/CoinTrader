using CoinTrader.ML;
using System;
using System.Collections;
using System.Windows.Forms;

namespace CoinTrader.Forms
{
    public partial class LoadingForm : Form
    {
        private float total = 0f;
        private float current = 0f;
        private string text = string.Empty;
        private bool isCompleted = false;

        public LoadingForm()
        {
            InitializeComponent();
            this.StartCoroutine(Updater());
            this.StartCoroutine(Start());
        }

        private IEnumerator Updater()
        {
            while (true)
            {
                if (current > 0f && total > 0f)
                {
                    loadingProgressBar.Value = (int)(current / total * 100f);
                    loadingProgressBar.Update();
                    loadingLabel.Text = $"{text}({current}/{total})";
                    loadingLabel.Update();
                }
                yield return null;

                if (isCompleted)
                    Completed();
            }
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            WaterfallProcess wfp = new WaterfallProcess();
            wfp.Add(MachineLearningLoad);
            wfp.Start(result =>
            {
                isCompleted = true;
            });
        }

        private void MachineLearningLoad(Action<bool> onFinished)
        {
            MachineLearning.Initialize((total, current, text) =>
            {
                UpdateProgressBar(total, current, text);
                if (total == current)
                    onFinished?.Invoke(true);
            });
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
            this.total = total;
            this.current = current;
            this.text = text;
        }
    }
}
