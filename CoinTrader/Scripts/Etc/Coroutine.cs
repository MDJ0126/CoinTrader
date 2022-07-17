using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.Windows.Forms
{
    public class Coroutine
    {
        public Coroutine parent;
        public Control control;
        public IEnumerator enumerator;
        public bool isWait = false;
    }

    public static class CoroutineSchedule
    {
        private const int UPDATE_INTERVAL = 100;    // 0.1초

        private static List<Coroutine> coroutines = new List<Coroutine>();
        private static List<Coroutine> waitAdds = new List<Coroutine>();
        private static List<Coroutine> waitRemoves = new List<Coroutine>();

        private static bool isStarted = false;

        /// <summary>
        /// 코루틴 시작
        /// </summary>
        /// <param name="control"></param>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public static Coroutine StartCoroutine(this Control control, IEnumerator enumerator)
        {
            control.HandleDestroyed += HandleDestroyed;
            Coroutine coroutine = new Coroutine { control = control, enumerator = enumerator };
            coroutines.Add(coroutine);
            if (!isStarted) Updater();
            return coroutine;
        }

        /// <summary>
        /// 코루틴 중지
        /// </summary>
        /// <param name="control"></param>
        /// <param name="coroutine"></param>
        public static void StopCoroutine(this Control control, Coroutine coroutine)
        {
            if (coroutine != null)
                waitRemoves.Add(coroutine);
        }

        /// <summary>
        /// 핸들러 소멸 이벤트 (코루틴에서 삭제 처리)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HandleDestroyed(object sender, EventArgs e)
        {
            waitRemoves.Add(coroutines.Find(co => co.control.Equals(sender)));
        }

        /// <summary>
        /// 코루틴 연장 처리
        /// </summary>
        /// <param name="coroutineInfo"></param>
        private static void YieldCoroutine(Coroutine coroutineInfo)
        {
            coroutines.Add(new Coroutine { parent = coroutineInfo, control = coroutineInfo.control, enumerator = coroutineInfo.enumerator.Current as IEnumerator });
        }

        /// <summary>
        /// UPdater
        /// </summary>
        public static async void Updater()
        {
            isStarted = true;
            while (coroutines.Count > 0)
            {
                // 반복기 체크
                for (int i = 0; i < coroutines.Count; i++)
                {
                    IEnumerator enumerator = coroutines[i].enumerator;
                    if (enumerator != null)
                    {
                        if (enumerator.Current == null)
                            enumerator.MoveNext();

                        if (enumerator != null)
                        {
                            iKeepWait keepWait = enumerator.Current as iKeepWait;
                            if (keepWait != null && keepWait.IsMoveNext())
                            {
                                var isEnd = !enumerator.MoveNext();
                                if (isEnd)
                                {
                                    waitRemoves.Add(coroutines[i]);
                                    if (coroutines[i].parent != null)
                                    {
                                        coroutines[i].parent.isWait = false;
                                        var isEndParent = !coroutines[i].parent.enumerator.MoveNext();
                                        if (isEndParent)
                                        {
                                            waitRemoves.Add(coroutines[i].parent);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!coroutines[i].isWait)
                                {
                                    IEnumerator newEnumerator = enumerator.Current as IEnumerator;
                                    if (newEnumerator != null)
                                    {
                                        coroutines[i].isWait = true;
                                        waitAdds.Add(coroutines[i]);
                                    }
                                }
                            }
                        }
                    }
                }

                // 반복기 종료 처리
                for (int i = 0; i < waitRemoves.Count; i++)
                {
                    coroutines.Remove(waitRemoves[i]);
                }
                waitRemoves.Clear();

                // 반복기에서 반복기로 연장되는 경우
                for (int i = 0; i < waitAdds.Count; i++)
                {
                    YieldCoroutine(waitAdds[i]);
                }
                waitAdds.Clear();

                // 반복 주기
                await Task.Delay(UPDATE_INTERVAL);
            }
            isStarted = false;
        }
    }
}