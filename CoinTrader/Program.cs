using CoinTrader.Forms;
using Network;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoinTrader
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutex = new Mutex(true, "F2D98EF4-4736-4DE4-BD7B-F8267D914387", out bool createNew);
            if (createNew)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // 초기화
                MultiThread.SetMainThread();
                Time.Start();
                Logger.Start();

                // 폼 실행
                Application.Run(new LoadingForm());

                // 종료
                MultiThread.Release();
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("이미 실행 중입니다.", "CoinTrader", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }



        private static string ConvertJson(Dictionary<string, string> parameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            var enumerator = parameters.GetEnumerator();
            bool isFirst = true;
            while (enumerator.MoveNext())
            {
                if (!isFirst) sb.Append(',');
                sb.Append($"\"{enumerator.Current.Key}\"").Append(',').Append($"\"{enumerator.Current.Value}\"");
                isFirst = false;
            }
            sb.Append('}');
            return sb.ToString();
        }
    }
}
