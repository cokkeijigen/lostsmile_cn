using System;
using System.Text;
using UnityEngine;

namespace Utage
{
	public static class ParserUtil
	{
		public static bool TryParaseEnum<T>(string str, out T val)
		{
			try
			{
				val = (T)Enum.Parse(typeof(T), str);
				return true;
			}
			catch (Exception)
			{
				val = default(T);
				return false;
			}
		}

		public static string ParseTagTextToString(string text, Func<string, string, bool> callbackTagParse)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			while (num < text.Length)
			{
				int num2 = ParseTag(text, num, callbackTagParse);
				if (num2 == num)
				{
					stringBuilder.Append(text[num]);
					num++;
				}
				else
				{
					num = num2 + 1;
				}
			}
			return stringBuilder.ToString();
		}

		public static int ParseTag(string text, int start, Func<string, string, bool> callbackParseTag)
		{
			if (text[start] != '<')
			{
				return start;
			}
			int num = start + 1;
			int num2 = text.IndexOf('>', num);
			if (num2 < 0)
			{
				return start;
			}
			char[] separator = new char[1] { '=' };
			string[] array = text.Substring(num, num2 - num).Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 1 || array.Length > 2)
			{
				return start;
			}
			string arg = array[0];
			string arg2 = ((array.Length > 1) ? array[1] : "");
			if (callbackParseTag(arg, arg2))
			{
				return num2;
			}
			return start;
		}

		public static Vector2 ParsePivotOptional(string text, Vector2 defaultValue)
		{
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			Vector2 vec = Vector2.one * 0.5f;
			if (TryParaseEnum<Pivot>(text, out var val))
			{
				return PivotUtil.PivotEnumToVector2(val);
			}
			if (TryParseVector2Optional(text, vec, out vec))
			{
				return vec;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.PivotParse, text));
		}

		public static Vector2 ParseScale2DOptional(string text, Vector2 defaultValue)
		{
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			Vector2 vec = defaultValue;
			if (WrapperUnityVersion.TryParseFloatGlobal(text, out var val))
			{
				return Vector2.one * val;
			}
			if (TryParseVector2Optional(text, vec, out vec))
			{
				return vec;
			}
			throw new Exception("Parse Scale2D Error " + text);
		}

		public static bool TryParseVector2Optional(string text, Vector2 defaultValue, out Vector2 vec2)
		{
			vec2 = defaultValue;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			bool result = false;
			char[] separator = new char[1] { ' ' };
			string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text2 in array)
			{
				char[] separator2 = new char[1] { '=' };
				string[] array2 = text2.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length == 2)
				{
					string text3 = array2[0];
					if (!(text3 == "x"))
					{
						if (!(text3 == "y"))
						{
							return false;
						}
						if (!WrapperUnityVersion.TryParseFloatGlobal(array2[1], out vec2.y))
						{
							return false;
						}
						result = true;
					}
					else
					{
						if (!WrapperUnityVersion.TryParseFloatGlobal(array2[1], out vec2.x))
						{
							return false;
						}
						result = true;
					}
					continue;
				}
				return false;
			}
			return result;
		}

		public static Vector3 ParseScale3DOptional(string text, Vector3 defaultValue)
		{
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			Vector3 vec = defaultValue;
			if (WrapperUnityVersion.TryParseFloatGlobal(text, out var val))
			{
				return Vector3.one * val;
			}
			if (TryParseVector3Optional(text, vec, out vec))
			{
				return vec;
			}
			throw new Exception("Parse Scale3D Error " + text);
		}

		public static bool TryParseVector3Optional(string text, Vector3 defaultValue, out Vector3 vec3)
		{
			vec3 = defaultValue;
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			bool result = false;
			char[] separator = new char[1] { ' ' };
			string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text2 in array)
			{
				char[] separator2 = new char[1] { '=' };
				string[] array2 = text2.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length == 2)
				{
					switch (array2[0])
					{
					case "x":
						if (!WrapperUnityVersion.TryParseFloatGlobal(array2[1], out vec3.x))
						{
							return false;
						}
						result = true;
						break;
					case "y":
						if (!WrapperUnityVersion.TryParseFloatGlobal(array2[1], out vec3.y))
						{
							return false;
						}
						result = true;
						break;
					case "z":
						if (!WrapperUnityVersion.TryParseFloatGlobal(array2[1], out vec3.z))
						{
							return false;
						}
						result = true;
						break;
					default:
						return false;
					}
					continue;
				}
				return false;
			}
			return result;
		}

		public static int ToMagicID(char id0, char id1, char id2, char id3)
		{
			return (int)(((uint)id3 << 24) + ((uint)id2 << 16) + ((uint)id1 << 8) + id0);
		}
	}
}
