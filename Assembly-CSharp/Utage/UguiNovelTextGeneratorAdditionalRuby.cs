using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class UguiNovelTextGeneratorAdditionalRuby
	{
		private List<UguiNovelTextCharacter> rubyList = new List<UguiNovelTextCharacter>();

		private List<UguiNovelTextCharacter> stringData = new List<UguiNovelTextCharacter>();

		private float rubyWidth;

		private float stringWidth;

		private UguiNovelTextLine textLine;

		public List<UguiNovelTextCharacter> RubyList
		{
			get
			{
				return rubyList;
			}
		}

		internal UguiNovelTextCharacter TopCharacter
		{
			get
			{
				return stringData[0];
			}
		}

		public float RubyWidth
		{
			get
			{
				return rubyWidth;
			}
		}

		public float StringWidth
		{
			get
			{
				return stringWidth;
			}
		}

		public bool IsWideType
		{
			get
			{
				return RubyWidth > StringWidth;
			}
		}

		internal UguiNovelTextGeneratorAdditionalRuby(List<UguiNovelTextCharacter> characters, int index, UguiNovelTextGenerator generator)
		{
			Font font = generator.NovelText.font;
			float rubySizeScale = generator.RubySizeScale;
			UguiNovelTextCharacter uguiNovelTextCharacter = characters[index];
			int fontSize = Mathf.CeilToInt(rubySizeScale * (float)uguiNovelTextCharacter.FontSize);
			stringData.Add(uguiNovelTextCharacter);
			for (int i = index + 1; i < characters.Count; i++)
			{
				UguiNovelTextCharacter uguiNovelTextCharacter2 = characters[i];
				if (!uguiNovelTextCharacter2.CustomInfo.IsRuby || uguiNovelTextCharacter2.CustomInfo.IsRubyTop)
				{
					break;
				}
				stringData.Add(uguiNovelTextCharacter2);
			}
			CharData.CustomCharaInfo customInfo = new CharData.CustomCharaInfo
			{
				IsColor = uguiNovelTextCharacter.charData.CustomInfo.IsColor,
				color = uguiNovelTextCharacter.charData.CustomInfo.color
			};
			if (uguiNovelTextCharacter.charData.CustomInfo.IsEmphasisMark)
			{
				for (int j = 0; j < stringData.Count; j++)
				{
					CharData charData = new CharData(uguiNovelTextCharacter.charData.CustomInfo.rubyStr[0], customInfo);
					rubyList.Add(new UguiNovelTextCharacter(charData, font, fontSize, generator.BmpFontSize, uguiNovelTextCharacter.FontStyle));
				}
				return;
			}
			string rubyStr = uguiNovelTextCharacter.charData.CustomInfo.rubyStr;
			for (int k = 0; k < rubyStr.Length; k++)
			{
				CharData charData2 = new CharData(rubyStr[k], customInfo);
				rubyList.Add(new UguiNovelTextCharacter(charData2, font, fontSize, generator.BmpFontSize, uguiNovelTextCharacter.FontStyle));
			}
		}

		internal void InitAfterCharacterInfo(UguiNovelTextGenerator generator)
		{
			float num = 0f;
			foreach (UguiNovelTextCharacter ruby in rubyList)
			{
				num += ruby.Width;
			}
			rubyWidth = num;
			num = 0f;
			foreach (UguiNovelTextCharacter stringDatum in stringData)
			{
				num += stringDatum.Width + generator.LetterSpaceSize;
			}
			stringWidth = num;
			if (!IsWideType)
			{
				return;
			}
			float rubySpaceSize = (RubyWidth - (StringWidth - (float)stringData.Count * generator.LetterSpaceSize)) / (float)stringData.Count / 2f;
			foreach (UguiNovelTextCharacter stringDatum2 in stringData)
			{
				stringDatum2.RubySpaceSize = rubySpaceSize;
			}
		}

		internal void InitPosition(UguiNovelTextGenerator generator)
		{
			foreach (UguiNovelTextLine lineData in generator.LineDataList)
			{
				if (lineData.Characters.IndexOf(TopCharacter) >= 0)
				{
					textLine = lineData;
				}
			}
			float letterSpaceSize = generator.LetterSpaceSize;
			float num = 0f;
			Vector2 vector = default(Vector2);
			if (IsWideType)
			{
				vector.x = 0f - TopCharacter.RubySpaceSize;
			}
			else
			{
				num = (StringWidth - RubyWidth) / (float)rubyList.Count;
				vector.x = (0f - letterSpaceSize) / 2f + num / 2f;
			}
			vector.y = textLine.MaxFontSize;
			float num2 = vector.x + TopCharacter.PositionX;
			float y = vector.y + TopCharacter.PositionY;
			foreach (UguiNovelTextCharacter ruby in rubyList)
			{
				ruby.InitPositionX(num2);
				ruby.InitPositionY(y);
				num2 += ruby.Width + num;
			}
		}

		internal void AddDrawVertex(List<UIVertex> verts, Vector2 endPosition)
		{
			if (!TopCharacter.IsVisible)
			{
				return;
			}
			try
			{
				foreach (UguiNovelTextCharacter ruby in rubyList)
				{
					if ((textLine.Y0 > endPosition.y || ruby.PositionX + ruby.Width / 2f < endPosition.x) && ruby.Verts != null)
					{
						verts.AddRange(ruby.Verts);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
			}
		}
	}
}
