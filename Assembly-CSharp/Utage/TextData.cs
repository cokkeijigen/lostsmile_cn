using System.Collections.Generic;

namespace Utage
{
	public class TextData
	{
		private string unityRitchText;

		private const string BoldEndTag = "</b>";

		private const string ItalicEndTag = "</i>";

		private const string ColorEndTag = "</color>";

		private const string SizeEndTag = "</size>";

		public TextParser ParsedText { get; private set; }

		public string OriginalText => ParsedText.OriginalText;

		public string NoneMetaString => ParsedText.NoneMetaString;

		public List<CharData> CharList => ParsedText.CharList;

		public int Length => CharList.Count;

		public string ErrorMsg => ParsedText.ErrorMsg;

		public bool ContainsSpeedTag { get; protected set; }

		public bool IsNoWaitAll { get; protected set; }

		public string UnityRitchText
		{
			get
			{
				InitUnityRitchText();
				return unityRitchText;
			}
		}

		public TextData(string text)
		{
			ParsedText = new TextParser(text);
			IsNoWaitAll = true;
			foreach (CharData @char in ParsedText.CharList)
			{
				if (@char.CustomInfo.IsSpeed)
				{
					ContainsSpeedTag = true;
				}
				if (!@char.CustomInfo.IsSpeed || @char.CustomInfo.speed != 0f)
				{
					IsNoWaitAll = false;
				}
			}
		}

		public void InitUnityRitchText()
		{
			if (!string.IsNullOrEmpty(unityRitchText))
			{
				return;
			}
			unityRitchText = "";
			CharData.CustomCharaInfo customCharaInfo = new CharData.CustomCharaInfo();
			Stack<string> stack = new Stack<string>();
			for (int i = 0; i < CharList.Count; i++)
			{
				CharData charData = CharList[i];
				if (charData.CustomInfo.IsEndBold(customCharaInfo))
				{
					unityRitchText += stack.Pop();
				}
				if (charData.CustomInfo.IsEndItalic(customCharaInfo))
				{
					unityRitchText += stack.Pop();
				}
				if (charData.CustomInfo.IsEndSize(customCharaInfo))
				{
					unityRitchText += stack.Pop();
				}
				if (charData.CustomInfo.IsEndColor(customCharaInfo))
				{
					unityRitchText += stack.Pop();
				}
				if (charData.CustomInfo.IsBeginColor(customCharaInfo))
				{
					unityRitchText = unityRitchText + "<color=" + charData.CustomInfo.colorStr + ">";
					stack.Push("</color>");
				}
				if (charData.CustomInfo.IsBeginSize(customCharaInfo))
				{
					unityRitchText = unityRitchText + "<size=" + charData.CustomInfo.size + ">";
					stack.Push("</size>");
				}
				if (charData.CustomInfo.IsBeginItalic(customCharaInfo))
				{
					unityRitchText += "<i>";
					stack.Push("</i>");
				}
				if (charData.CustomInfo.IsBeginBold(customCharaInfo))
				{
					unityRitchText += "<b>";
					stack.Push("</b>");
				}
				charData.UnityRitchTextIndex = unityRitchText.Length;
				unityRitchText += charData.Char;
				if (charData.CustomInfo.IsDoubleWord)
				{
					unityRitchText += " ";
				}
				customCharaInfo = charData.CustomInfo;
			}
			if (customCharaInfo.IsBold)
			{
				unityRitchText += stack.Pop();
			}
			if (customCharaInfo.IsItalic)
			{
				unityRitchText += stack.Pop();
			}
			if (customCharaInfo.IsSize)
			{
				unityRitchText += stack.Pop();
			}
			if (customCharaInfo.IsColor)
			{
				unityRitchText += stack.Pop();
			}
		}
	}
}
