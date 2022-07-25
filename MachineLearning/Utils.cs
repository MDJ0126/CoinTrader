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
    /// 경로에 폴더가 없으면 생성
    /// </summary>
    /// <param name="path"></param>
    public static void CreatePathFolder(string path)
    {
        string[] folderNames = path.Split('\\');
        string fullPath = string.Empty;
        for (int i = 0; i < folderNames.Length - 1; i++)
        {
            fullPath += folderNames[i] + '\\';
            DirectoryInfo di = new DirectoryInfo(fullPath);
            if (!di.Exists) di.Create();
        }
    }

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
            CreatePathFolder(path);

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
                            if (sb.Length > 0)
                                sb.Append(',');
                            sb.Append(fieldInfos[i].Name);
                        }
                        writer.Write(sb);
                    }

                    // 2. 데이터 입력
                    do
                    {
                        sb.Length = 0;
                        sb.Append('\n');
                        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        for (int i = 0; i < fieldInfos.Length; i++)
                        {
                            if (sb.Length > 1)
                                sb.Append(',');
                            sb.Append(fieldInfos[i].GetValue(enumerator.Current));
                        }
                        writer.Write(sb);

                    } while (enumerator.MoveNext());

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
                        sb.Append('\n');
                        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        for (int i = 0; i < fieldInfos.Length; i++)
                        {
                            if (sb.Length > 0)
                                sb.Append(',');
                            sb.Append(fieldInfos[i].GetValue(enumerator.Current));
                        }
                    };

                    writer.Write(sb);
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
        try
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
        catch { }
    }
}