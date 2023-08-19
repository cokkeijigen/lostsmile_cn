using System;
using System.Collections.Generic;
using System.Text;

namespace Utage
{
	public class TextParser
	{
		public const string TagSound = "sound";

		public const string TagSpeed = "speed";

		public const string TagUnderLine = "u";

		private List<CharData> charList = new List<CharData>();

		public static Func<string, object> CallbackCalcExpression;

		private string errorMsg;

		private string originalText;

		private string noneMetaString;

		private int currentTextIndex;

		private CharData.CustomCharaInfo customInfo = new CharData.CustomCharaInfo();

		private bool isParseParamOnly;

		public List<CharData> CharList
		{
			get
			{
				return charList;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return errorMsg;
			}
		}

		public int Length
		{
			get
			{
				return CharList.Count;
			}
		}

		public string OriginalText
		{
			get
			{
				return originalText;
			}
		}

		public string NoneMetaString
		{
			get
			{
				InitNoneMetaText();
				return noneMetaString;
			}
		}

		public static string AddTag(string text, string tag, string arg)
		{
			return string.Format("<{1}={2}>{0}</{1}>", text, tag, arg);
		}

		private void AddErrorMsg(string msg)
		{
			if (string.IsNullOrEmpty(errorMsg))
			{
				errorMsg = "";
			}
			else
			{
				errorMsg += "\n";
			}
			errorMsg += msg;
		}

		private void InitNoneMetaText()
		{
			if (string.IsNullOrEmpty(noneMetaString))
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < CharList.Count; i++)
				{
					stringBuilder.Append(CharList[i].Char);
				}
				noneMetaString = stringBuilder.ToString();
			}
		}

		public static string MakeLogText(string text)
		{
			return new TextParser(text, true).NoneMetaString;
		}

		public TextParser(string text, bool isParseParamOnly = false)
		{
			originalText = text;
			this.isParseParamOnly = isParseParamOnly;
			Parse();
		}

		private void Parse()
		{
			try
			{
				int length = OriginalText.Length;
				currentTextIndex = 0;
				while (currentTextIndex < length)
				{
					if (!ParseEscapeSequence())
					{
						int num = ParserUtil.ParseTag(callbackParseTag: (!isParseParamOnly) ? new Func<string, string, bool>(ParseTag) : new Func<string, string, bool>(ParseTagParamOnly), text: OriginalText, start: currentTextIndex);
						if (currentTextIndex == num)
						{
							AddChar(OriginalText[currentTextIndex]);
							currentTextIndex++;
						}
						else
						{
							currentTextIndex = num + 1;
						}
					}
				}
			}
			catch (Exception ex)
			{
				AddErrorMsg(ex.Message + ex.StackTrace);
			}
		}

		private bool ParseEscapeSequence()
		{
			if (currentTextIndex + 1 >= OriginalText.Length)
			{
				return false;
			}
			char c = OriginalText[currentTextIndex];
			char c2 = OriginalText[currentTextIndex + 1];
			if (c == '\\' && c2 == 'n')
			{
				AddDoubleLineBreak();
				currentTextIndex += 2;
				return true;
			}
			if (c == '\r' && c2 == '\n')
			{
				AddDoubleLineBreak();
				currentTextIndex += 2;
				return true;
			}
			return false;
		}

		private void AddStrng(string text)
		{
			foreach (char c in text)
			{
				AddChar(c);
			}
		}

		private void AddChar(char c)
		{
			CharData item = new CharData(c, customInfo);
			charList.Add(item);
			customInfo.ClearOnNextChar();
		}

		private void AddDoubleLineBreak()
		{
			CharData charData = new CharData('\n', customInfo);
			charData.CustomInfo.IsDoubleWord = true;
			charList.Add(charData);
		}

		private void AddDash(string arg)
		{
			int result;
			if (!int.TryParse(arg, out result))
			{
				result = 1;
			}
			CharData charData = new CharData('—', customInfo);
			charData.CustomInfo.IsDash = true;
			charData.CustomInfo.DashSize = result;
			charList.Add(charData);
		}

		private bool TryAddEmoji(string arg)
		{
			if (string.IsNullOrEmpty(arg))
			{
				return false;
			}
			CharData charData = new CharData('□', customInfo);
			charData.CustomInfo.IsEmoji = true;
			charData.CustomInfo.EmojiKey = arg;
			charList.Add(charData);
			return true;
		}

		private bool TryAddSpace(string arg)
		{
			int result;
			if (!int.TryParse(arg, out result))
			{
				return false;
			}
			CharData charData = new CharData(' ', customInfo);
			charData.CustomInfo.IsSpace = true;
			charData.CustomInfo.SpaceSize = result;
			charList.Add(charData);
			return true;
		}

		private bool TryAddInterval(string arg)
		{
			if (charList.Count <= 0)
			{
				return false;
			}
			return charList[charList.Count - 1].TryParseInterval(arg);
		}

		private bool ParseTag(string name, string arg)
		{
			switch (name)
			{
			case "b":
				return customInfo.TryParseBold(arg);
			case "/b":
				customInfo.ResetBold();
				return true;
			case "i":
				return customInfo.TryParseItalic(arg);
			case "/i":
				customInfo.ResetItalic();
				return true;
			case "color":
				return customInfo.TryParseColor(arg);
			case "/color":
				customInfo.ResetColor();
				return true;
			case "size":
				return customInfo.TryParseSize(arg);
			case "/size":
				customInfo.ResetSize();
				return true;
			case "ruby":
				return customInfo.TryParseRuby(arg);
			case "/ruby":
				customInfo.ResetRuby();
				return true;
			case "em":
				return customInfo.TryParseEmphasisMark(arg);
			case "/em":
				customInfo.ResetEmphasisMark();
				return true;
			case "sup":
				return customInfo.TryParseSuperScript(arg);
			case "/sup":
				customInfo.ResetSuperScript();
				return true;
			case "sub":
				return customInfo.TryParseSubScript(arg);
			case "/sub":
				customInfo.ResetSubScript();
				return true;
			case "u":
				return customInfo.TryParseUnderLine(arg);
			case "/u":
				customInfo.ResetUnderLine();
				return true;
			case "strike":
				return customInfo.TryParseStrike(arg);
			case "/strike":
				customInfo.ResetStrike();
				return true;
			case "group":
				return customInfo.TryParseGroup(arg);
			case "/group":
				customInfo.ResetGroup();
				return true;
			case "emoji":
				return TryAddEmoji(arg);
			case "dash":
				AddDash(arg);
				return true;
			case "space":
				return TryAddSpace(arg);
			case "param":
			{
				string text2 = ExpressionToString(arg);
				AddStrng(text2);
				return true;
			}
			case "link":
				return customInfo.TryParseLink(arg);
			case "/link":
				customInfo.ResetLink();
				return true;
			case "tips":
				return customInfo.TryParseTips(arg);
			case "/tips":
				customInfo.ResetTips();
				return true;
			case "sound":
				return customInfo.TryParseSound(arg);
			case "/sound":
				customInfo.ResetSound();
				return true;
			case "speed":
				return customInfo.TryParseSpeed(arg);
			case "/speed":
				customInfo.ResetSpeed();
				return true;
			case "interval":
				return TryAddInterval(arg);
			case "format":
			{
				char[] separator = new char[1] { ':' };
				string[] array = arg.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				int num = array.Length - 1;
				string[] array2 = new string[num];
				Array.Copy(array, 1, array2, 0, num);
				string text = FormatExpressionToString(array[0], array2);
				AddStrng(text);
				return true;
			}
			default:
				return false;
			}
		}

		private bool ParseTagParamOnly(string name, string arg)
		{
			if (!(name == "param"))
			{
				if (name == "format")
				{
					char[] separator = new char[1] { ':' };
					string[] array = arg.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					int num = array.Length - 1;
					string[] array2 = new string[num];
					Array.Copy(array, 1, array2, 0, num);
					string text = FormatExpressionToString(array[0], array2);
					AddStrng(text);
					return true;
				}
				return false;
			}
			string text2 = ExpressionToString(arg);
			AddStrng(text2);
			return true;
		}

		private string ExpressionToString(string exp)
		{
			if (CallbackCalcExpression == null)
			{
				AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextCallbackCalcExpression));
				return "";
			}
			object obj = CallbackCalcExpression(exp);
			if (obj == null)
			{
				AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextFailedCalcExpression));
				return "";
			}
			return obj.ToString();
		}

		private string FormatExpressionToString(string format, string[] exps)
		{
			if (CallbackCalcExpression == null)
			{
				AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextCallbackCalcExpression));
				return "";
			}
			List<object> list = new List<object>();
			foreach (string arg in exps)
			{
				list.Add(CallbackCalcExpression(arg));
			}
			return string.Format(format, list.ToArray());
		}
	}
}
