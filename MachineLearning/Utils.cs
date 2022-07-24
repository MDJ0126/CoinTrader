using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

internal static class Utils
{
    public static string CSV_DATA_PATH = Path.Combine(Environment.CurrentDirectory, "Data");

    private static StringBuilder sb = new StringBuilder();

    /// <summary>
    /// 컬렉션 CSV 파일 만들기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">컬렉션</param>
    /// <param name="path">파일 위치</param>
    /// <param name="overwrite">덮어쓰기 여부</param>
    /// <returns>작업 완료 여부</returns>
    public static bool CreateCSVFile<T>(ICollection<T> collection, string path, bool overwrite = true)
    {
        if (collection.Count > 0)
        {
            // 경로에 폴더가 없으면 생성
            string[] folderNames = path.Split('\\');
            string fullPath = string.Empty;
            for (int i = 0; i < folderNames.Length - 1; i++)
            {
                fullPath += folderNames[i] + '\\';
                DirectoryInfo di = new DirectoryInfo(fullPath);
                if (!di.Exists) di.Create();
            }

            sb.Length = 0;
            path += ".csv";
            if (!File.Exists(path) || overwrite)
            {
                // 텍스트 파일 생성
                using (var writer = File.CreateText(path))
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
                    do
                    {
                        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        for (int i = 0; i < fieldInfos.Length; i++)
                        {
                            sb.Append(fieldInfos[i].GetValue(enumerator.Current));
                            sb.Append(',');
                        }
                        sb.Append('\n');

                    } while (enumerator.MoveNext());

                    writer.WriteLine(sb);
                    writer.Close();
                }
                return true;
            }
            else
                Console.WriteLine("이미 파일이 존재합니다.");
        }
        return false;
    }

    /// <summary>
    /// 이어쓰기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">컬렉션</param>
    /// <param name="path">파일 위치</param>
    /// <returns>작업 완료 여부</returns>
    public static bool AppendCSVFile<T>(ICollection<T> collection, string path)
    {
        if (collection.Count > 0)
        {
            sb.Length = 0;
            path += ".csv";
            if (File.Exists(path))
            {
                var enumerator = collection.GetEnumerator();
                using (var writer = File.AppendText(path))
                {
                    // 데이터 입력
                    while (enumerator.MoveNext())
                    {
                        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        for (int i = 0; i < fieldInfos.Length; i++)
                        {
                            sb.Append(fieldInfos[i].GetValue(enumerator.Current));
                            sb.Append(',');
                        }
                        sb.Append('\n');
                    };

                    writer.WriteLine(sb);
                    writer.Close();
                }
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Depth-first recursive delete, with handling for descendant 
    /// directories open in Windows Explorer.
    /// </summary>
    public static void DeleteDirectory(string path)
    {
        foreach (string directory in Directory.GetDirectories(path))
        {
            DeleteDirectory(directory);
        }

        try
        {
            Directory.Delete(path, true);
        }
        catch (IOException)
        {
            Directory.Delete(path, true);
        }
        catch (UnauthorizedAccessException)
        {
            Directory.Delete(path, true);
        }
    }
}