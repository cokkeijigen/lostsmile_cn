using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class UguiNovelTextLine
	{
		private List<UguiNovelTextCharacter> characters = new List<UguiNovelTextCharacter>();

		private List<UguiNovelTextCharacter> rubyCharacters = new List<UguiNovelTextCharacter>();

		private bool isOver;

		private int maxFontSize;

		private float width;

		private float totalHeight;

		public List<UguiNovelTextCharacter> Characters
		{
			get
			{
				return characters;
			}
		}

		public List<UguiNovelTextCharacter> RubyCharacters
		{
			get
			{
				return rubyCharacters;
			}
		}

		public bool IsOver
		{
			get
			{
				return isOver;
			}
			set
			{
				isOver = value;
			}
		}

		public int MaxFontSize
		{
			get
			{
				return maxFontSize;
			}
		}

		public float Width
		{
			get
			{
				return width;
			}
		}

		public float TotalHeight
		{
			get
			{
				return totalHeight;
			}
		}

		public float Y0 { get; set; }

		public void AddCharaData(UguiNovelTextCharacter charaData)
		{
			characters.Add(charaData);
		}

		public void EndCharaData(UguiNovelTextGenerator generator)
		{
			maxFontSize = 0;
			float num = 0f;
			for (int i = 0; i < characters.Count; i++)
			{
				UguiNovelTextCharacter uguiNovelTextCharacter = characters[i];
				maxFontSize = Mathf.Max(maxFontSize, uguiNovelTextCharacter.DefaultFontSize);
				if (i == 0)
				{
					num = uguiNovelTextCharacter.PositionX - uguiNovelTextCharacter.RubySpaceSize;
				}
			}
			float num2 = 0f;
			for (int num3 = characters.Count - 1; num3 >= 0; num3--)
			{
				UguiNovelTextCharacter uguiNovelTextCharacter2 = characters[num3];
				if (!uguiNovelTextCharacter2.IsBr)
				{
					num2 = uguiNovelTextCharacter2.PositionX + uguiNovelTextCharacter2.Width + uguiNovelTextCharacter2.RubySpaceSize;
					break;
				}
			}
			width = Mathf.Abs(num2 - num);
			totalHeight = generator.NovelText.GetTotalLineHeight(MaxFontSize);
		}

		public void SetLineY(float y, UguiNovelTextGenerator generator)
		{
			Y0 = y;
			foreach (UguiNovelTextCharacter character in characters)
			{
				character.InitPositionY(Y0);
			}
		}

		public void ApplyTextAnchorX(float pivotX, float maxWidth)
		{
			if (pivotX == 0f)
			{
				return;
			}
			float offsetX = (maxWidth - Width) * pivotX;
			foreach (UguiNovelTextCharacter character in characters)
			{
				character.ApplyOffsetX(offsetX);
			}
		}

		public void ApplyTextAnchorY(float offsetY)
		{
			Y0 += offsetY;
			foreach (UguiNovelTextCharacter character in characters)
			{
				character.ApplyOffsetY(offsetY);
			}
		}

		public void ApplyOffset(Vector2 offset)
		{
			Y0 += offset.y;
			foreach (UguiNovelTextCharacter character in characters)
			{
				character.ApplyOffsetX(offset.x);
				character.ApplyOffsetY(offset.y);
			}
		}
	}
}
