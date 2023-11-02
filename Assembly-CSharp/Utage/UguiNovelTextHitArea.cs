using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class UguiNovelTextHitArea
	{
		public enum Type
		{
			Link,
			Sound
		}

		private UguiNovelText novelText;

		private List<UguiNovelTextCharacter> characters = new List<UguiNovelTextCharacter>();

		private List<Rect> hitAreaList = new List<Rect>();

		public CharData.HitEventType HitEventType { get; private set; }

		public string Arg { get; private set; }

		public List<Rect> HitAreaList => hitAreaList;

		public UguiNovelTextHitArea(UguiNovelText novelText, CharData.HitEventType type, string arg, List<UguiNovelTextCharacter> characters)
		{
			this.novelText = novelText;
			HitEventType = type;
			Arg = arg;
			this.characters = characters;
		}

		public void RefreshHitAreaList()
		{
			hitAreaList.Clear();
			List<UguiNovelTextCharacter> list = new List<UguiNovelTextCharacter>();
			foreach (UguiNovelTextCharacter character in characters)
			{
				if (!character.IsBr && character.IsVisible)
				{
					list.Add(character);
				}
				if (character.IsBrOrAutoBr)
				{
					if (list.Count > 0)
					{
						hitAreaList.Add(MakeHitArea(list));
					}
					list.Clear();
				}
			}
			if (list.Count > 0)
			{
				hitAreaList.Add(MakeHitArea(list));
			}
		}

		private Rect MakeHitArea(List<UguiNovelTextCharacter> lineCharacters)
		{
			UguiNovelTextCharacter uguiNovelTextCharacter = lineCharacters[0];
			float beginPositionX = uguiNovelTextCharacter.BeginPositionX;
			float num = uguiNovelTextCharacter.EndPositionX;
			int num2 = 0;
			foreach (UguiNovelTextCharacter lineCharacter in lineCharacters)
			{
				num = Mathf.Max(num, lineCharacter.EndPositionX);
				num2 = Mathf.Max(num2, lineCharacter.FontSize);
			}
			int totalLineHeight = novelText.GetTotalLineHeight(num2);
			float y = uguiNovelTextCharacter.PositionY - (float)(totalLineHeight - num2) / 2f;
			return new Rect(beginPositionX, y, num - beginPositionX, totalLineHeight);
		}

		internal bool HitTest(Vector2 localPosition)
		{
			foreach (Rect hitArea in hitAreaList)
			{
				if (hitArea.Contains(localPosition))
				{
					return true;
				}
			}
			return false;
		}

		internal void ChangeEffectColor(Color effectColor)
		{
			foreach (UguiNovelTextCharacter character in characters)
			{
				character.ChangeEffectColor(effectColor);
			}
			novelText.RemakeVerts();
		}

		internal void ResetEffectColor()
		{
			foreach (UguiNovelTextCharacter character in characters)
			{
				character.ResetEffectColor();
			}
			novelText.RemakeVerts();
		}
	}
}
