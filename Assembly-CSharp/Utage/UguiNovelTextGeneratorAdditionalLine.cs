using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class UguiNovelTextGeneratorAdditionalLine
	{
		public enum Type
		{
			UnderLine,
			Strike
		}

		private Type type;

		private UguiNovelTextCharacter characteData;

		private List<UguiNovelTextCharacter> stringData = new List<UguiNovelTextCharacter>();

		private UguiNovelTextLine textLine;

		public Type LineType
		{
			get
			{
				return type;
			}
		}

		public UguiNovelTextCharacter CharacteData
		{
			get
			{
				return characteData;
			}
		}

		internal UguiNovelTextCharacter TopCharacter
		{
			get
			{
				return stringData[0];
			}
		}

		internal UguiNovelTextGeneratorAdditionalLine(Type type, List<UguiNovelTextCharacter> characters, int index, UguiNovelTextGenerator generator)
		{
			InitSub(type, generator);
			stringData.Add(characters[index]);
			for (int i = index + 1; i < characters.Count; i++)
			{
				UguiNovelTextCharacter uguiNovelTextCharacter = characters[i];
				if (!IsLineEnd(uguiNovelTextCharacter))
				{
					stringData.Add(uguiNovelTextCharacter);
					continue;
				}
				break;
			}
		}

		private UguiNovelTextGeneratorAdditionalLine(UguiNovelTextGeneratorAdditionalLine srcLine, int index, int count, UguiNovelTextGenerator generator)
		{
			InitSub(srcLine.type, generator);
			for (int i = 0; i < count; i++)
			{
				stringData.Add(srcLine.stringData[index + i]);
			}
		}

		private void InitSub(Type type, UguiNovelTextGenerator generator)
		{
			this.type = type;
			CharData charData = new CharData(generator.DashChar);
			charData.CustomInfo.IsDash = true;
			charData.CustomInfo.DashSize = 1;
			characteData = new UguiNovelTextCharacter(charData, generator);
		}

		private bool IsLineEnd(UguiNovelTextCharacter c)
		{
			switch (LineType)
			{
			case Type.Strike:
				if (c.CustomInfo.IsStrike)
				{
					return c.CustomInfo.IsStrikeTop;
				}
				return true;
			case Type.UnderLine:
				if (c.CustomInfo.IsUnderLine)
				{
					return c.CustomInfo.IsUnderLineTop;
				}
				return true;
			default:
				return false;
			}
		}

		internal List<UguiNovelTextGeneratorAdditionalLine> MakeOtherLineInTextLine(UguiNovelTextGenerator generator)
		{
			InitTextLine(generator);
			return MakeOtherLineInTextLineSub(generator);
		}

		private void InitTextLine(UguiNovelTextGenerator generator)
		{
			foreach (UguiNovelTextLine lineData in generator.LineDataList)
			{
				if (lineData.Characters.IndexOf(TopCharacter) >= 0)
				{
					textLine = lineData;
				}
			}
		}

		internal List<UguiNovelTextGeneratorAdditionalLine> MakeOtherLineInTextLineSub(UguiNovelTextGenerator generator)
		{
			List<UguiNovelTextGeneratorAdditionalLine> list = new List<UguiNovelTextGeneratorAdditionalLine>();
			int num = stringData.Count - 1;
			foreach (UguiNovelTextLine lineData in generator.LineDataList)
			{
				if (textLine == lineData)
				{
					continue;
				}
				bool flag = false;
				int index = 0;
				int num2 = 0;
				for (int i = 0; i < stringData.Count; i++)
				{
					if (lineData.Characters.IndexOf(stringData[i]) >= 0)
					{
						if (!flag)
						{
							index = i;
							num = Mathf.Min(i, num);
							flag = true;
						}
						num2++;
					}
				}
				if (flag)
				{
					UguiNovelTextGeneratorAdditionalLine uguiNovelTextGeneratorAdditionalLine = new UguiNovelTextGeneratorAdditionalLine(this, index, num2, generator);
					list.Add(uguiNovelTextGeneratorAdditionalLine);
					uguiNovelTextGeneratorAdditionalLine.InitTextLine(generator);
					if (!uguiNovelTextGeneratorAdditionalLine.characteData.TrySetCharacterInfo(generator.NovelText.font))
					{
						Debug.LogError("Line Font Missing");
					}
				}
			}
			if (num < stringData.Count - 1)
			{
				stringData.RemoveRange(num, stringData.Count - num);
			}
			return list;
		}

		internal void InitPosition(UguiNovelTextGenerator generator)
		{
			Vector2 zero = Vector2.zero;
			float num = textLine.MaxFontSize;
			switch (LineType)
			{
			case Type.UnderLine:
				zero.y -= num / 2f;
				break;
			}
			CharacteData.InitPositionX(TopCharacter.PositionX + zero.x);
			CharacteData.InitPositionY(TopCharacter.PositionY + zero.y);
		}

		internal void AddDrawVertex(List<UIVertex> verts, Vector2 endPosition, UguiNovelTextGenerator generator)
		{
			if (!TopCharacter.IsVisible)
			{
				return;
			}
			float positionX = TopCharacter.PositionX;
			float endPositionX = TopCharacter.EndPositionX;
			foreach (UguiNovelTextCharacter stringDatum in stringData)
			{
				if (!stringDatum.IsVisible)
				{
					break;
				}
				endPositionX = stringDatum.EndPositionX;
			}
			Color defaultColor = Color.white;
			foreach (UguiNovelTextCharacter stringDatum2 in stringData)
			{
				if (!stringDatum2.IsVisible)
				{
					break;
				}
				if (stringDatum2.Verts != null)
				{
					defaultColor = stringDatum2.Verts[0].color;
				}
			}
			CharacteData.Width = endPositionX - positionX;
			CharacteData.MakeVerts(defaultColor, generator);
			verts.AddRange(CharacteData.Verts);
		}
	}
}
