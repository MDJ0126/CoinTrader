using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

internal static class CVSUtils
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
    /// <param name="writeHeader">헤더 작성 여부</param>
    /// <returns>작업 완료 여부</returns>
    public static bool CreateCSVFile<T>(ICollection<T> collection, string path, bool overwrite = true, bool writeHeader = true)
    {
        try
        {
            if (collection.Count > 0)
            {
                CreatePathFolder(path);

                sb.Length = 0;
                path += ".csv";
                if (!File.Exists(path) || overwrite)
                {
                    // 텍스트 파일 생성
                    using (var writer = File.CreateText(path + ".temp"))
                    {
                        var enumerator = collection.GetEnumerator();
                        if (writeHeader)
                        {
                            // 1. 헤더 입력
                            if (enumerator.MoveNext())
                            {
                                FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                                for (int i = 0; i < fieldInfos.Length; i++)
                                {
                                    if (sb.Length > 0)
                                        sb.Append(',');
                                    sb.Append(fieldInfos[i].Name);
                                }
                                writer.WriteLine(sb);
                            }
                        }

                        // 2. 데이터 입력
                        enumerator = collection.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                            for (int i = 0; i < fieldInfos.Length; i++)
                            {
                                if (i > 0) sb.Append(',');
                                sb.Append(fieldInfos[i].GetValue(enumerator.Current));
                            }
                            sb.Append('\n');
                        }
                        sb.Length -= 1;

                        writer.Write(sb);
                        writer.Close();
                    }
                    try
                    {
                        File.Delete(path);
                    }
                    catch { }
                    File.Move(path + ".temp", path);
                    return true;
                }
                else
                    Console.WriteLine("이미 파일이 존재합니다.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return false;
    }

    /// <summary>
    /// 이어쓰기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">컬렉션</param>
    /// <param name="path">파일 위치</param>
    /// <param name="pasteInFront">앞에 붙여넣을지 여부</param>
    /// <returns>작업 완료 여부</returns>
    public static bool AppendCSVFile<T>(ICollection<T> collection, string path, bool pasteInFront = false)
    {
        if (collection.Count > 0)
        {
            sb.Length = 0;
            path += ".csv";
            if (File.Exists(path))
            {
                string originalText = File.ReadAllText(path);
                var enumerator = collection.GetEnumerator();
                if (pasteInFront)
                {
                    while (enumerator.MoveNext())
                    {
                        FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        for (int i = 0; i < fieldInfos.Length; i++)
                        {
                            if (i > 0) sb.Append(',');
                            sb.Append(fieldInfos[i].GetValue(enumerator.Current));
                        }
                        sb.Append('\n');
                    };
                    using (var writer = File.CreateText(path + ".temp"))
                    {
                        writer.Write(sb + originalText);
                        writer.Close();
                    }
                }
                else
                {
                    using (var writer = File.CreateText(path + ".temp"))
                    {
                        // 데이터 입력
                        while (enumerator.MoveNext())
                        {
                            sb.Append('\n');
                            FieldInfo[] fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                            for (int i = 0; i < fieldInfos.Length; i++)
                            {
                                if (i > 0) sb.Append(',');
                                sb.Append(fieldInfos[i].GetValue(enumerator.Current));
                            }
                        };
                        writer.Write(originalText + sb);
                        writer.Close();
                    }
                }
                try
                {
                    File.Delete(path);
                }
                catch { }
                File.Move(path + ".temp", path);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 특정한 로우 길이만 남기고 이전 기록은 모두 지운다.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="remainRowCount"></param>
    public static void CleanUpRowCVS(string path, int remainRowCount)
    {
        path += ".csv";
        if (File.Exists(path))
        {
            string[] strs = File.ReadAllLines(path);
            if (strs.Length > remainRowCount)
            {
                sb.Length = 0;
                for (int i = strs.Length - remainRowCount; i < strs.Length; i++)
                {
                    sb.Append(strs[i]);
                    if (i < strs.Length - 1)
                        sb.Append('\n');
                }

                using (var writer = File.CreateText(path + ".temp"))
                {
                    writer.Write(sb);
                    writer.Close();
                }

                try
                {
                    File.Delete(path);
                }
                catch { }
                File.Move(path + ".temp", path);
            }
        }
    }

    /// <summary>
    /// 로우 길이 가져오기
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static int GetRowCount(string path)
    {
        path += ".csv";
        if (File.Exists(path))
        {
            string[] strs = File.ReadAllLines(path);
            return strs.Length;
        }
        return 0;
    }

    /// <summary>
    /// 텍스트 파일 열기
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[,] OpenCSVFile(string path)
    {
        try
        {
            path += ".csv";
            if (File.Exists(path))
            {
                string[] strs = File.ReadAllLines(path);
                if (strs != null && strs.Length > 0)
                {
                    int col = strs[0].Split(',').Length;
                    int row = strs.Length;

                    string[,] result = new string[row, col];
                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        string[] split = strs[i].Split(',');
                        for (int j = 0; j < result.GetLength(1); j++)
                        {
                            result[i, j] = split[j];
                        }
                    }
                    return result;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }

    /// <summary>
    /// 경로상의 모든 폴더 및 하위 파일 삭제 시도
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

    public static float Clamp(float value, float min, float max)
    {
        if (value < min)
        {
            value = min;
        }
        else if (value > max)
        {
            value = max;
        }
        return value;
    }
}