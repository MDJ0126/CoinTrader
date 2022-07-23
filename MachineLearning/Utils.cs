using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

public static class Utils
{
    /// <summary>
    /// CSV 파일 만들기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="path"></param>
    public static void CreateCSVFile<T>(ICollection<T> collection, string path)
    {
        if (collection.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            if (!File.Exists(path))
            {
                // 텍스트 파일 생성
                using (var writer = File.CreateText(path + ".csv"))
                {
                    // CSV 컬럼 데이터 입력
                    var enumerator = collection.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        for (int i = 0; i < fieldInfos.Length; i++)
                        {
                            sb.Append(fieldInfos[i].Name);
                            sb.Append(',');
                        }
                        sb.Append('\n');
                    }

                    do
                    {
                        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        for (int i = 0; i < fieldInfos.Length; i++)
                        {
                            sb.Append(fieldInfos[i].GetValue(null));
                            sb.Append(',');
                        }
                        sb.Append('\n');

                    } while (enumerator.MoveNext());

                    writer.WriteLine(sb);
                    writer.Close();
                }

                //writer = File.AppendText(path);
            }
            else
            {
                //MessageBox.Show("이미 파일이 존재 합니다.");
            }
        }
    }
}