using System;
using System.ComponentModel;
using System.Reflection;

public static class Utils
{
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
}