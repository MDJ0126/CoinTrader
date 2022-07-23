using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

internal static class Utils
{
    public static string CSV_DATA_PATH = Path.Combine(Environment.CurrentDirectory, "Data");

    /// <summary>
    /// 컬렉션 CSV 파일 만들기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">컬렉션</param>
    /// <param name="path">생성 위치</param>
    /// <param name="overwrite">덮어쓰기 여부</param>
    public static void CreateCSVFile<T>(ICollection<T> collection, string path, bool overwrite = true)
    {
        if (collection.Count > 0)
        {
            // 부모 폴더가 없으면 생성
            DirectoryInfo di = new DirectoryInfo(CSV_DATA_PATH);
            if (!di.Exists) di.Create();

            StringBuilder sb = new StringBuilder();
            if (!File.Exists(path) || overwrite)
            {
                // 텍스트 파일 생성
                using (var writer = File.CreateText(path + ".csv"))
                {
                    // 1. 컬럼 입력
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

                    // 2. 데이터 입력
                    var obj = enumerator.Current;
                    do
                    {
                        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        for (int i = 0; i < fieldInfos.Length; i++)
                        {
                            sb.Append(fieldInfos[i].GetValue(obj));
                            sb.Append(',');
                        }
                        sb.Append('\n');

                    } while (enumerator.MoveNext());

                    writer.WriteLine(sb);
                    writer.Close();
                }
            }
            else
            {
                Console.WriteLine("이미 파일이 존재합니다.");
            }
        }
    }
}