using UnityEngine;

namespace Utage
{
	public class UguiNovelTextSettings : ScriptableObject
	{
		[SerializeField]
		private string wordWrapSeparator = "!#%&(),-.:<=>?@[]{}";

		[SerializeField]
		private string kinsokuTop = ",)]}、〕〉》」』】〙〗〟’”｠»ゝゞーァィゥェォッャュョヮヵヶぁぃぅぇぉっゃゅょゎゕゖㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇷ\u309aㇺㇻㇼㇽㇾㇿ々〻‐゠–〜～?!‼⁇⁈⁉・:;/。.，）］｝＝？！：；／";

		[SerializeField]
		private string kinsokuEnd = "([{〔〈《「『【〘〖〝‘“｟«（［｛";

		[SerializeField]
		private string ignoreLetterSpace = "…‒–—―⁓〜〰";

		[SerializeField]
		private string autoIndentation = "";

		[SerializeField]
		private bool forceIgonreJapaneseKinsoku;

		internal string WordWrapSeparator => wordWrapSeparator;

		internal string KinsokuTop => kinsokuTop;

		internal string KinsokuEnd => kinsokuEnd;

		internal string IgnoreLetterSpace => ignoreLetterSpace;

		internal string AutoIndentation => autoIndentation;

		internal bool IsIgonreLetterSpace(UguiNovelTextCharacter current, UguiNovelTextCharacter next)
		{
			if (current == null || next == null)
			{
				return false;
			}
			if (current.Char == next.Char && IgnoreLetterSpace.IndexOf(current.Char) >= 0)
			{
				return true;
			}
			return false;
		}

		internal bool CheckWordWrap(UguiNovelTextGenerator generator, UguiNovelTextCharacter current, UguiNovelTextCharacter prev)
		{
			if (IsIgonreLetterSpace(prev, current))
			{
				return true;
			}
			bool num = (generator.WordWrapType & UguiNovelTextGenerator.WordWrap.Default) == UguiNovelTextGenerator.WordWrap.Default;
			bool flag = !forceIgonreJapaneseKinsoku && (generator.WordWrapType & UguiNovelTextGenerator.WordWrap.JapaneseKinsoku) == UguiNovelTextGenerator.WordWrap.JapaneseKinsoku;
			if (num)
			{
				if (flag)
				{
					if (CheckWordWrapDefaultEnd(prev) && CheckWordWrapDefaultTop(current))
					{
						return true;
					}
				}
				else if (!char.IsSeparator(prev.Char) && !char.IsSeparator(current.Char))
				{
					return true;
				}
			}
			if (flag && (CheckKinsokuEnd(prev) || CheckKinsokuTop(current)))
			{
				return true;
			}
			return false;
		}

		internal bool IsAutoIndentation(char character)
		{
			return autoIndentation.IndexOf(character) >= 0;
		}

		private bool CheckWordWrapDefaultEnd(UguiNovelTextCharacter character)
		{
			char @char = character.Char;
			if (UtageToolKit.IsHankaku(@char) && !char.IsSeparator(@char))
			{
				return wordWrapSeparator.IndexOf(@char) < 0;
			}
			return false;
		}

		private bool CheckWordWrapDefaultTop(UguiNovelTextCharacter character)
		{
			char @char = character.Char;
			if (UtageToolKit.IsHankaku(@char))
			{
				return !char.IsSeparator(@char);
			}
			return false;
		}

		private bool CheckKinsokuBurasage(UguiNovelTextCharacter c)
		{
			return false;
		}

		private bool CheckKinsokuTop(UguiNovelTextCharacter character)
		{
			return kinsokuTop.IndexOf(character.Char) >= 0;
		}

		private bool CheckKinsokuEnd(UguiNovelTextCharacter character)
		{
			return kinsokuEnd.IndexOf(character.Char) >= 0;
		}
	}
}
