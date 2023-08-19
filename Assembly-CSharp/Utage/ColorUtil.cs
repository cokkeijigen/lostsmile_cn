using System;
using UnityEngine;

namespace Utage
{
	public static class ColorUtil
	{
		public static readonly Color Aqua = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public static readonly Color Black = new Color32(0, 0, 0, byte.MaxValue);

		public static readonly Color Blue = new Color32(0, 0, byte.MaxValue, byte.MaxValue);

		public static readonly Color Brown = new Color32(165, 42, 42, byte.MaxValue);

		public static readonly Color Cyan = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public static readonly Color Darkblue = new Color32(0, 0, 160, byte.MaxValue);

		public static readonly Color Fuchsia = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);

		public static readonly Color Green = new Color32(0, 128, 0, byte.MaxValue);

		public static readonly Color Grey = new Color32(128, 128, 128, byte.MaxValue);

		public static readonly Color Lightblue = new Color32(173, 216, 230, byte.MaxValue);

		public static readonly Color Lime = new Color32(0, byte.MaxValue, 0, byte.MaxValue);

		public static readonly Color Magenta = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);

		public static readonly Color Maroon = new Color32(128, 0, 0, byte.MaxValue);

		public static readonly Color Navy = new Color32(0, 0, 128, byte.MaxValue);

		public static readonly Color Olive = new Color32(128, 128, 0, byte.MaxValue);

		public static readonly Color Orange = new Color32(byte.MaxValue, 165, 0, byte.MaxValue);

		public static readonly Color Purple = new Color32(128, 0, 128, byte.MaxValue);

		public static readonly Color Red = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

		public static readonly Color Silver = new Color32(192, 192, 192, byte.MaxValue);

		public static readonly Color Teal = new Color32(0, 128, 128, byte.MaxValue);

		public static readonly Color White = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		public static readonly Color Yellow = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);

		public static Color ParseColor(string text)
		{
			Color color = Color.white;
			if (TryParseColor(text, ref color))
			{
				return color;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.ColorParseError, text));
		}

		public static bool TryParseColor(string text, ref Color color)
		{
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (text[0] == '#')
			{
				return TryParseColorDetail(text.Substring(1), ref color);
			}
			switch (text)
			{
			case "aqua":
				color = Cyan;
				break;
			case "black":
				color = Black;
				break;
			case "blue":
				color = Blue;
				break;
			case "brown":
				color = Brown;
				break;
			case "cyan":
				color = Cyan;
				break;
			case "darkblue":
				color = Darkblue;
				break;
			case "fuchsia":
				color = Magenta;
				break;
			case "green":
				color = Green;
				break;
			case "grey":
				color = Grey;
				break;
			case "lightblue":
				color = Lightblue;
				break;
			case "lime":
				color = Lime;
				break;
			case "magenta":
				color = Magenta;
				break;
			case "maroon":
				color = Maroon;
				break;
			case "navy":
				color = Navy;
				break;
			case "olive":
				color = Olive;
				break;
			case "orange":
				color = Orange;
				break;
			case "purple":
				color = Purple;
				break;
			case "red":
				color = Red;
				break;
			case "silver":
				color = Silver;
				break;
			case "teal":
				color = Teal;
				break;
			case "white":
				color = White;
				break;
			case "yellow":
				color = Yellow;
				break;
			default:
				return false;
			}
			return true;
		}

		private static bool TryParseColorDetail(string text, ref Color color)
		{
			try
			{
				if (text.Length == 6)
				{
					int num = Convert.ToInt32(text, 16);
					float r = (float)((num & 0xFF0000) >> 16) / 255f;
					float g = (float)((num & 0xFF00) >> 8) / 255f;
					float b = (float)(num & 0xFF) / 255f;
					color = new Color(r, g, b);
					return true;
				}
				if (text.Length == 8)
				{
					int num2 = Convert.ToInt32(text, 16);
					float r2 = (float)((num2 & 0xFF000000u) >> 24) / 255f;
					float g2 = (float)((num2 & 0xFF0000) >> 16) / 255f;
					float b2 = (float)((num2 & 0xFF00) >> 8) / 255f;
					float a = (float)(num2 & 0xFF) / 255f;
					color = new Color(r2, g2, b2, a);
					return true;
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static string ToColorString(Color color)
		{
			int num = (int)(255f * color.r);
			int num2 = (int)(255f * color.g);
			int num3 = (int)(255f * color.b);
			int num4 = (int)(255f * color.a);
			return ((num << 24) + (num2 << 16) + (num3 << 8) + num4).ToString("X8").ToLower();
		}

		public static string ToNguiColorString(Color color)
		{
			int num = (int)(255f * color.r);
			int num2 = (int)(255f * color.g);
			int num3 = (int)(255f * color.b);
			return ((num << 16) + (num2 << 8) + num3).ToString("X6").ToLower();
		}

		public static string AddColorTag(string str, string colorKey)
		{
			return string.Format("<color={1}>{0}</color>", str, colorKey);
		}

		public static string AddColorTag(string str, Color color)
		{
			return AddColorTag(str, "#" + ToColorString(color));
		}
	}
}
