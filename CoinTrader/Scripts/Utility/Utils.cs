using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

public static class Utils
{
    private static string SOLUTION_NAME = Assembly.GetEntryAssembly().GetName().Name;
    private static string APPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"/{SOLUTION_NAME}";

    /// <summary>
    /// 파일 저장
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="instance"></param>
    public static void FileSave<T>(string name, T instance)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            try
            {
                // 부모 폴더가 없으면 생성
                DirectoryInfo di = new DirectoryInfo(APPDATA_PATH);
                if (!di.Exists) di.Create();

                // 바이너리 직렬화 후 파일로 저장
                var bf = new BinaryFormatter();
                bf.Serialize(ms, instance);
                using (StreamWriter writer = new StreamWriter(APPDATA_PATH + $"/{name}.dat"))
                {
                    writer.Write(Convert.ToBase64String(ms.ToArray()));
                    writer.Close();
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// 파일 불러오기
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FileLoad<T>(string name)
    {
        try
        {
            // 파일 읽기
            byte[] bytes;
            using (StreamReader reader = new StreamReader(APPDATA_PATH + $"/{name}.dat"))
            {
                bytes = Convert.FromBase64String(reader.ReadToEnd());
                reader.Close();
            }

            // 바이너리 역직렬화 후 반환
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                object obj = new BinaryFormatter().Deserialize(ms);
                return (T)obj;
            }
        }
        catch
        {
            return default(T);
        }
    }

    /// <summary>
    /// 폼 캡쳐
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static string Capture(this System.Windows.Forms.Control control)
    {
        // 경로 참고: https://pcsak3.com/502
        string strFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        string strOutput = $"{strFolder}/{SOLUTION_NAME}.png";
        Bitmap bmp = new Bitmap(control.Size.Width, control.Size.Height);
        Graphics grp = Graphics.FromImage(bmp);
        grp.CopyFromScreen(new Point(control.Bounds.X, control.Bounds.Y), new Point(0, 0), control.Size);
        bmp.Save(strOutput, System.Drawing.Imaging.ImageFormat.Png);
        return strOutput;
    }

    /// <summary>
    /// Enum 확장메소드, Description 읽어오기
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ToDescription(this Enum source)
	{
		FieldInfo fi = source.GetType().GetField(source.ToString());
		var att = (DescriptionAttribute)fi.GetCustomAttribute(typeof(DescriptionAttribute));
		if (att != null)
		{
			return att.Description;
		}
		else
		{
			return source.ToString();
		}
    }

    /// <summary>
    /// index로 Enum 값 찾기
    /// </summary>
    /// <typeparam name="T">Enum Type</typeparam>
    /// <param name="index">Enum item index</param>
    /// <returns></returns>

    public static T FindEnumValue<T>(int index) where T : Enum
    {
        return (T)Enum.ToObject(typeof(T), index);
    }

    /// <summary>
    /// string으로 Enum 값 찾기
    /// </summary>
    /// <typeparam name="T">Enum Type</typeparam>
    /// <param name="str">Enum item string</param>
    /// <returns></returns>
    public static T FindEnumValue<T>(string str) where T : Enum
    {
        string[] enums = Enum.GetNames(typeof(T));

        T result = (T)Enum.ToObject(typeof(T), 0);

        for (int i = 0; i < enums.Length; i++)
        {
            if (str == enums[i])
                result = (T)Enum.ToObject(typeof(T), i);
        }

        return result;
    }

    /// <summary>
    /// 헤더 변수 읽어오기
    /// </summary>
    /// <param name="headerValue"></param>
    /// <param name="variableName"></param>
    /// <returns></returns>
    public static string GetHeaderValue(string headerValue, string variableName)
	{
		int startIndex = headerValue.IndexOf(variableName, 0, System.StringComparison.OrdinalIgnoreCase);
		int endIndex = startIndex + variableName.Length;
		string variable = headerValue.Substring(startIndex, variableName.Length);
		string value = string.Empty;
		for (int i = endIndex + 1; i < headerValue.Length; i++)
		{
			if (headerValue[i] == ';') break;
			value += headerValue[i];
		}
		return value;
	}

	/// <summary>
	/// 더블 버퍼링 옵션
	/// </summary>
	/// <param name="control"></param>
	/// <param name="enabled"></param>
	public static void DoubleBuffered(this Control control, bool enabled)
	{
		var prop = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
		prop.SetValue(control, enabled, null);
	}

    /// <summary>
    /// 리스트 아이템 찾기
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static ListViewItem Find(this ListView listView, string text)
    {
        for (int i = 0; i < listView.Items.Count; i++)
        {
            if (listView.Items[i].Text == text)
                return listView.Items[i];
        }
        return null;
    }
}