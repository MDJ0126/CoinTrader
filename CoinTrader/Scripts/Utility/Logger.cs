using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class Logger
{
    public delegate void OnAddLog(string text);
    private static event OnAddLog onLog;
    public static event OnAddLog OnLogger
    {
        add
        {
            onLog -= value;
            onLog += value;
        }
        remove
        {
            onLog -= value;
        }
    }

    public static Queue<string> Logs { get; private set; } = new Queue<string>();

    private static StringBuilder sb = new StringBuilder();

    private static bool isStop = false;

    static Logger()
    {

    }

    private static string CurrentTime()
    {
        return Time.NowTime.ToString("[HH:mm:ss]");
    }

    public static void Log(object obj)
    {
        sb.Length = 0;
        Add(sb.Append(CurrentTime()).Append("[Log] ").Append(obj).ToString());
    }

    public static void Warning(object obj)
    {
        sb.Length = 0;
        Add(sb.Append(CurrentTime()).Append("[LogWarning] ").Append(obj).ToString());
    }

    public static void Error(object obj)
    {
        sb.Length = 0;
        Add(sb.Append(CurrentTime()).Append("[LogError] ").Append(obj).ToString());
    }

    public static string GetLogs()
    {
        sb.Length = 0;
        var enumerator = Logs.GetEnumerator();
        while (enumerator.MoveNext())
        {
            sb.Append(enumerator.Current).Append('\n');
        }
        return sb.ToString();
    }

    public static void Start()
    {
        Update();
    }

    public static void Stop()
    {
        isStop = true;
    }

    public static async void Update()
    {
        while (!isStop)
        {
            while (Logs.Count > 0)
            {
                string text = Logs.Dequeue();
                if (!string.IsNullOrEmpty(text))
                {
                    // Console 출력
                    Console.WriteLine(text);

                    // 로그 추가 이벤트 발생
                    onLog?.Invoke(text);
                }
            }

            await Task.Delay(10);
        }
    }

    private static void Add(string text)
    {
        // 추가
        Logs.Enqueue(text);
    }
}