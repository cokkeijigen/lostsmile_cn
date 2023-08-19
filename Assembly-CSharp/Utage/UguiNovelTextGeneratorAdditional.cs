using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class UguiNovelTextGeneratorAdditional
	{
		private List<UguiNovelTextGeneratorAdditionalRuby> rubyList = new List<UguiNovelTextGeneratorAdditionalRuby>();

		private List<UguiNovelTextGeneratorAdditionalLine> lineList = new List<UguiNovelTextGeneratorAdditionalLine>();

		public List<UguiNovelTextGeneratorAdditionalRuby> RubyList
		{
			get
			{
				return rubyList;
			}
		}

		public List<UguiNovelTextGeneratorAdditionalLine> LineList
		{
			get
			{
				return lineList;
			}
		}

		internal UguiNovelTextGeneratorAdditional(List<UguiNovelTextCharacter> characters, UguiNovelTextGenerator generataor)
		{
			for (int i = 0; i < characters.Count; i++)
			{
				UguiNovelTextCharacter uguiNovelTextCharacter = characters[i];
				if (uguiNovelTextCharacter.CustomInfo.IsStrikeTop)
				{
					lineList.Add(new UguiNovelTextGeneratorAdditionalLine(UguiNovelTextGeneratorAdditionalLine.Type.Strike, characters, i, generataor));
				}
				if (uguiNovelTextCharacter.CustomInfo.IsUnderLineTop)
				{
					lineList.Add(new UguiNovelTextGeneratorAdditionalLine(UguiNovelTextGeneratorAdditionalLine.Type.UnderLine, characters, i, generataor));
				}
				if (uguiNovelTextCharacter.CustomInfo.IsRubyTop)
				{
					rubyList.Add(new UguiNovelTextGeneratorAdditionalRuby(characters, i, generataor));
				}
			}
		}

		internal bool TrySetFontCharcters(Font font)
		{
			foreach (UguiNovelTextGeneratorAdditionalLine line in lineList)
			{
				if (!line.CharacteData.TrySetCharacterInfo(font))
				{
					return false;
				}
			}
			foreach (UguiNovelTextGeneratorAdditionalRuby ruby in rubyList)
			{
				foreach (UguiNovelTextCharacter ruby2 in ruby.RubyList)
				{
					if (!ruby2.TrySetCharacterInfo(font))
					{
						return false;
					}
				}
			}
			return true;
		}

		internal List<UguiNovelTextCharacter> MakeCharacterList()
		{
			List<UguiNovelTextCharacter> list = new List<UguiNovelTextCharacter>();
			foreach (UguiNovelTextGeneratorAdditionalLine line in lineList)
			{
				list.Add(line.CharacteData);
			}
			foreach (UguiNovelTextGeneratorAdditionalRuby ruby in rubyList)
			{
				foreach (UguiNovelTextCharacter ruby2 in ruby.RubyList)
				{
					list.Add(ruby2);
				}
			}
			return list;
		}

		internal void InitAfterCharacterInfo(UguiNovelTextGenerator generator)
		{
			foreach (UguiNovelTextGeneratorAdditionalRuby ruby in RubyList)
			{
				ruby.InitAfterCharacterInfo(generator);
			}
		}

		internal float GetTopLetterSpace(UguiNovelTextCharacter lineTopCharacter, UguiNovelTextGenerator generator)
		{
			float result = 0f;
			foreach (UguiNovelTextGeneratorAdditionalRuby ruby in RubyList)
			{
				if (!ruby.IsWideType && ruby.TopCharacter == lineTopCharacter)
				{
					result = generator.LetterSpaceSize / 2f;
					break;
				}
			}
			return result;
		}

		internal float GetMaxWidth(UguiNovelTextCharacter currentData)
		{
			if (currentData.CustomInfo.IsRubyTop)
			{
				foreach (UguiNovelTextGeneratorAdditionalRuby ruby in RubyList)
				{
					if (ruby.TopCharacter == currentData)
					{
						return Mathf.Max(ruby.StringWidth, ruby.RubyWidth);
					}
				}
			}
			return currentData.Width;
		}

		internal void InitPosition(UguiNovelTextGenerator generator)
		{
			List<UguiNovelTextGeneratorAdditionalLine> list = new List<UguiNovelTextGeneratorAdditionalLine>();
			foreach (UguiNovelTextGeneratorAdditionalLine line in lineList)
			{
				list.AddRange(line.MakeOtherLineInTextLine(generator));
			}
			lineList.AddRange(list);
			foreach (UguiNovelTextGeneratorAdditionalLine line2 in lineList)
			{
				line2.InitPosition(generator);
			}
			foreach (UguiNovelTextGeneratorAdditionalRuby ruby in RubyList)
			{
				ruby.InitPosition(generator);
			}
		}

		internal void MakeVerts(Color color, UguiNovelTextGenerator generator)
		{
			foreach (UguiNovelTextGeneratorAdditionalLine line in lineList)
			{
				line.CharacteData.MakeVerts(color, generator);
			}
			foreach (UguiNovelTextGeneratorAdditionalRuby ruby in RubyList)
			{
				foreach (UguiNovelTextCharacter ruby2 in ruby.RubyList)
				{
					ruby2.MakeVerts(color, generator);
				}
			}
		}

		internal void AddVerts(List<UIVertex> verts, Vector2 endPosition, UguiNovelTextGenerator generator)
		{
			foreach (UguiNovelTextGeneratorAdditionalLine line in lineList)
			{
				line.AddDrawVertex(verts, endPosition, generator);
			}
			foreach (UguiNovelTextGeneratorAdditionalRuby ruby in RubyList)
			{
				ruby.AddDrawVertex(verts, endPosition);
			}
		}
	}
}
