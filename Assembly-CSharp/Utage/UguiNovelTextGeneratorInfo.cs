using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	internal class UguiNovelTextGeneratorInfo
	{
		private class GraphicObject
		{
			public UguiNovelTextCharacter character;

			public RectTransform graphic;

			public GraphicObject(UguiNovelTextCharacter character, RectTransform graphic)
			{
				this.character = character;
				this.graphic = graphic;
			}
		}

		private bool isDebugLog;

		private List<GraphicObject> graphicObjectList;

		private bool isInitGraphicObjectList;

		private UguiNovelTextGenerator Generator { get; set; }

		private UguiNovelText NovelText => Generator.NovelText;

		internal TextData TextData { get; set; }

		private List<UguiNovelTextCharacter> CharacterDataList { get; set; }

		internal List<UguiNovelTextLine> LineDataList { get; private set; }

		internal Vector3 EndPosition { get; private set; }

		internal UguiNovelTextGeneratorAdditional Additional { get; private set; }

		internal float PreferredHeight { get; private set; }

		internal float PreferredWidth { get; private set; }

		public float MaxWidth { get; private set; }

		public float MaxHeight { get; private set; }

		internal float Height { get; private set; }

		internal float Width { get; private set; }

		internal List<UguiNovelTextHitArea> HitGroupLists { get; private set; }

		private UguiNovelTextFontInfoBuilder FontInfoBuilder { get; set; }

		internal UguiNovelTextGeneratorInfo(UguiNovelTextGenerator generator)
		{
			Generator = generator;
			CharacterDataList = new List<UguiNovelTextCharacter>();
			LineDataList = new List<UguiNovelTextLine>();
			HitGroupLists = new List<UguiNovelTextHitArea>();
			FontInfoBuilder = new UguiNovelTextFontInfoBuilder();
		}

		internal void BuildCharcteres()
		{
			TextData = new TextData(NovelText.text);
			if (isDebugLog)
			{
				Debug.Log(TextData.ParsedText.OriginalText);
			}
			CharacterDataList = CreateCharacterDataList(TextData);
			Additional = new UguiNovelTextGeneratorAdditional(CharacterDataList, Generator);
			FontInfoBuilder.InitFontCharactes(NovelText.font, CharacterDataList, Additional);
			Additional.InitAfterCharacterInfo(Generator);
			PreferredWidth = CalcPreferredWidth(CharacterDataList);
			ClearGraphicObjectList();
		}

		internal void BuildTextArea(RectTransform rectTransform)
		{
			Rect rect = rectTransform.rect;
			float maxWidth = Mathf.Abs(rect.width);
			float maxHeight = Mathf.Abs(rect.height);
			ApplyXPosition(CharacterDataList, maxWidth);
			LineDataList = CreateLineList(CharacterDataList, maxHeight);
			MaxWidth = maxWidth;
			MaxHeight = maxHeight;
			ApplyTextAnchor(LineDataList, NovelText.alignment, MaxWidth, MaxHeight);
			ApplyOffset(LineDataList, MaxWidth, MaxHeight, rectTransform.pivot);
			Additional.InitPosition(Generator);
			MakeHitGroups(CharacterDataList);
			MakeVerts(LineDataList);
		}

		internal void RebuildFontTexture(Font font)
		{
			if (TextData != null)
			{
				FontInfoBuilder.InitFontCharactes(NovelText.font, CharacterDataList, Additional);
				MakeVerts(LineDataList);
			}
		}

		internal void RemakeVerts()
		{
			if (TextData != null)
			{
				MakeVerts(LineDataList);
			}
		}

		internal void CreateVertex(List<UIVertex> verts)
		{
			if (TextData != null)
			{
				CreateVertexList(verts, LineDataList, Generator.CurrentLengthOfView);
				RefreshHitArea();
			}
		}

		private List<UguiNovelTextCharacter> CreateCharacterDataList(TextData data)
		{
			List<UguiNovelTextCharacter> list = new List<UguiNovelTextCharacter>();
			if (data == null)
			{
				return list;
			}
			for (int i = 0; i < data.Length; i++)
			{
				UguiNovelTextCharacter item = new UguiNovelTextCharacter(data.CharList[i], Generator);
				list.Add(item);
			}
			return list;
		}

		private void ApplyXPosition(List<UguiNovelTextCharacter> characterDataList, float maxWidth)
		{
			ClacXPosition(characterDataList, true, true, maxWidth);
		}

		private float CalcPreferredWidth(List<UguiNovelTextCharacter> characterDataList)
		{
			return ClacXPosition(characterDataList, false, false, float.MaxValue);
		}

		private float ClacXPosition(List<UguiNovelTextCharacter> characterDataList, bool autoLineBreak, bool applyX, float maxWidth)
		{
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			while (num3 < characterDataList.Count)
			{
				int beginIndex = num3;
				float num4 = 0f;
				float num5 = 0f;
				while (num3 < characterDataList.Count)
				{
					UguiNovelTextCharacter uguiNovelTextCharacter = characterDataList[num3];
					if (num5 == 0f)
					{
						num4 = Additional.GetTopLetterSpace(uguiNovelTextCharacter, Generator);
						num5 += num2;
						if (num3 == 0 && IsAutoIndentation(uguiNovelTextCharacter.Char))
						{
							num2 = uguiNovelTextCharacter.Width + Generator.LetterSpaceSize;
						}
					}
					if (uguiNovelTextCharacter.CustomInfo.IsRuby)
					{
						num4 += uguiNovelTextCharacter.RubySpaceSize;
					}
					num5 += num4;
					if (!uguiNovelTextCharacter.IsBlank && autoLineBreak)
					{
						uguiNovelTextCharacter.isAutoLineBreak = false;
						if (IsOverMaxWidth(num5, Additional.GetMaxWidth(uguiNovelTextCharacter), maxWidth))
						{
							num3 = GetAutoLineBreakIndex(characterDataList, beginIndex, num3);
							uguiNovelTextCharacter = characterDataList[num3];
							uguiNovelTextCharacter.isAutoLineBreak = true;
						}
					}
					num3++;
					if ((autoLineBreak && uguiNovelTextCharacter.IsBrOrAutoBr) || uguiNovelTextCharacter.IsBr)
					{
						break;
					}
					if (applyX)
					{
						uguiNovelTextCharacter.InitPositionX(num5);
					}
					num5 += uguiNovelTextCharacter.Width;
					if (uguiNovelTextCharacter.RubySpaceSize != 0f)
					{
						num4 = uguiNovelTextCharacter.RubySpaceSize;
						continue;
					}
					num4 = Generator.LetterSpaceSize;
					if ((bool)Generator.TextSettings && num3 < characterDataList.Count && Generator.TextSettings.IsIgonreLetterSpace(uguiNovelTextCharacter, characterDataList[num3]))
					{
						num4 = 0f;
					}
				}
				num = Mathf.Max(num5, num);
			}
			return num;
		}

		private List<UguiNovelTextLine> CreateLineList(List<UguiNovelTextCharacter> characterDataList, float maxHeight)
		{
			List<UguiNovelTextLine> list = new List<UguiNovelTextLine>();
			UguiNovelTextLine uguiNovelTextLine = new UguiNovelTextLine();
			foreach (UguiNovelTextCharacter characterData in characterDataList)
			{
				uguiNovelTextLine.AddCharaData(characterData);
				if (characterData.IsBrOrAutoBr)
				{
					uguiNovelTextLine.EndCharaData(Generator);
					list.Add(uguiNovelTextLine);
					uguiNovelTextLine = new UguiNovelTextLine();
				}
			}
			if (uguiNovelTextLine.Characters.Count > 0)
			{
				uguiNovelTextLine.EndCharaData(Generator);
				list.Add(uguiNovelTextLine);
			}
			if (list.Count <= 0)
			{
				return list;
			}
			float num = 0f;
			for (int i = 0; i < list.Count; i++)
			{
				UguiNovelTextLine uguiNovelTextLine2 = list[i];
				float num2 = num;
				num -= (float)uguiNovelTextLine2.MaxFontSize;
				uguiNovelTextLine2.IsOver = IsOverMaxHeight(0f - num, maxHeight);
				if (!uguiNovelTextLine2.IsOver)
				{
					Height = 0f - num;
				}
				PreferredHeight = 0f - num;
				uguiNovelTextLine2.SetLineY(num, Generator);
				num = num2 - uguiNovelTextLine2.TotalHeight;
			}
			return list;
		}

		private void ApplyTextAnchor(List<UguiNovelTextLine> lineList, TextAnchor anchor, float maxWidth, float maxHeight)
		{
			Vector2 textAnchorPivot = Text.GetTextAnchorPivot(anchor);
			foreach (UguiNovelTextLine line in lineList)
			{
				line.ApplyTextAnchorX(textAnchorPivot.x, maxWidth);
			}
			if (textAnchorPivot.y == 1f)
			{
				return;
			}
			float offsetY = (maxHeight - Height) * (textAnchorPivot.y - 1f);
			foreach (UguiNovelTextLine line2 in lineList)
			{
				line2.ApplyTextAnchorY(offsetY);
			}
		}

		private void ApplyOffset(List<UguiNovelTextLine> lineList, float maxWidth, float maxHeight, Vector2 pivot)
		{
			Vector2 offset = new Vector2((0f - pivot.x) * maxWidth, (1f - pivot.y) * maxHeight);
			foreach (UguiNovelTextLine line in lineList)
			{
				line.ApplyOffset(offset);
			}
			if (isDebugLog)
			{
				Debug.LogFormat("PosX={0}", LineDataList[0].Characters[0].PositionX);
			}
		}

		private void MakeHitGroups(List<UguiNovelTextCharacter> characterDataList)
		{
			HitGroupLists = new List<UguiNovelTextHitArea>();
			int i = 0;
			while (i < characterDataList.Count)
			{
				UguiNovelTextCharacter uguiNovelTextCharacter = characterDataList[i];
				if (uguiNovelTextCharacter.charData.CustomInfo.IsHitEventTop)
				{
					CharData.HitEventType hitEventType = uguiNovelTextCharacter.CustomInfo.HitEventType;
					string hitEventArg = uguiNovelTextCharacter.CustomInfo.HitEventArg;
					List<UguiNovelTextCharacter> list = new List<UguiNovelTextCharacter>();
					list.Add(uguiNovelTextCharacter);
					for (i++; i < characterDataList.Count; i++)
					{
						UguiNovelTextCharacter uguiNovelTextCharacter2 = characterDataList[i];
						if (!uguiNovelTextCharacter2.CustomInfo.IsHitEvent || uguiNovelTextCharacter2.CustomInfo.IsHitEventTop)
						{
							break;
						}
						list.Add(uguiNovelTextCharacter2);
					}
					HitGroupLists.Add(new UguiNovelTextHitArea(NovelText, hitEventType, hitEventArg, list));
				}
				else
				{
					i++;
				}
			}
		}

		private void RefreshHitArea()
		{
			foreach (UguiNovelTextHitArea hitGroupList in HitGroupLists)
			{
				hitGroupList.RefreshHitAreaList();
			}
		}

		private void MakeVerts(List<UguiNovelTextLine> lineList)
		{
			Color color = NovelText.color;
			foreach (UguiNovelTextLine line in lineList)
			{
				foreach (UguiNovelTextCharacter character in line.Characters)
				{
					character.MakeVerts(color, Generator);
				}
			}
			Additional.MakeVerts(color, Generator);
		}

		private void CreateVertexList(List<UIVertex> verts, List<UguiNovelTextLine> lineList, int max)
		{
			if (lineList == null || (max <= 0 && lineList.Count <= 0))
			{
				return;
			}
			int num = 0;
			UguiNovelTextCharacter uguiNovelTextCharacter = null;
			foreach (UguiNovelTextLine line in lineList)
			{
				if (line.IsOver)
				{
					break;
				}
				for (int i = 0; i < line.Characters.Count; i++)
				{
					UguiNovelTextCharacter uguiNovelTextCharacter2 = line.Characters[i];
					uguiNovelTextCharacter2.IsVisible = num < max;
					num++;
					if (!uguiNovelTextCharacter2.IsBr && uguiNovelTextCharacter2.IsVisible)
					{
						uguiNovelTextCharacter = uguiNovelTextCharacter2;
						EndPosition = new Vector3(uguiNovelTextCharacter.EndPositionX, line.Y0, 0f);
						if (!uguiNovelTextCharacter2.IsNoFontData)
						{
							verts.AddRange(uguiNovelTextCharacter2.Verts);
						}
					}
				}
			}
			Additional.AddVerts(verts, EndPosition, Generator);
		}

		internal void RefreshEndPosition()
		{
			int currentLengthOfView = Generator.CurrentLengthOfView;
			if (LineDataList == null || (currentLengthOfView <= 0 && LineDataList.Count <= 0))
			{
				return;
			}
			int num = 0;
			UguiNovelTextCharacter uguiNovelTextCharacter = null;
			foreach (UguiNovelTextLine lineData in LineDataList)
			{
				if (lineData.IsOver)
				{
					break;
				}
				for (int i = 0; i < lineData.Characters.Count; i++)
				{
					UguiNovelTextCharacter uguiNovelTextCharacter2 = lineData.Characters[i];
					uguiNovelTextCharacter2.IsVisible = num < currentLengthOfView;
					num++;
					if (!uguiNovelTextCharacter2.IsBr && uguiNovelTextCharacter2.IsVisible)
					{
						uguiNovelTextCharacter = uguiNovelTextCharacter2;
						EndPosition = new Vector3(uguiNovelTextCharacter.EndPositionX, lineData.Y0, 0f);
					}
				}
			}
		}

		private void ClearGraphicObjectList()
		{
			if (graphicObjectList == null)
			{
				return;
			}
			foreach (GraphicObject graphicObject in graphicObjectList)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(graphicObject.graphic.gameObject);
				}
				else
				{
					Object.DestroyImmediate(graphicObject.graphic.gameObject);
				}
			}
			graphicObjectList.Clear();
			graphicObjectList = null;
			isInitGraphicObjectList = false;
		}

		internal void UpdateGraphicObjectList(RectTransform parent)
		{
			if (!isInitGraphicObjectList)
			{
				ClearGraphicObjectList();
				graphicObjectList = new List<GraphicObject>();
				foreach (UguiNovelTextLine lineData in LineDataList)
				{
					foreach (UguiNovelTextCharacter character in lineData.Characters)
					{
						RectTransform rectTransform = character.AddGraphicObject(parent, Generator);
						if ((bool)rectTransform)
						{
							graphicObjectList.Add(new GraphicObject(character, rectTransform));
						}
					}
				}
				isInitGraphicObjectList = true;
			}
			foreach (GraphicObject graphicObject in graphicObjectList)
			{
				graphicObject.graphic.gameObject.SetActive(graphicObject.character.IsVisible);
			}
		}

		private int GetAutoLineBreakIndex(List<UguiNovelTextCharacter> characterList, int beginIndex, int index)
		{
			if (index <= beginIndex)
			{
				return index;
			}
			UguiNovelTextCharacter current = characterList[index];
			UguiNovelTextCharacter uguiNovelTextCharacter = characterList[index - 1];
			if (uguiNovelTextCharacter.IsBrOrAutoBr)
			{
				return index;
			}
			if (CheckWordWrap(current, uguiNovelTextCharacter))
			{
				int num = ParseWordWrap(characterList, beginIndex, index - 1);
				if (num != beginIndex)
				{
					return num;
				}
				return --index;
			}
			return --index;
		}

		private int ParseWordWrap(List<UguiNovelTextCharacter> infoList, int beginIndex, int index)
		{
			if (index <= beginIndex)
			{
				return beginIndex;
			}
			UguiNovelTextCharacter current = infoList[index];
			UguiNovelTextCharacter prev = infoList[index - 1];
			if (CheckWordWrap(current, prev))
			{
				return ParseWordWrap(infoList, beginIndex, index - 1);
			}
			return index - 1;
		}

		private bool CheckWordWrap(UguiNovelTextCharacter current, UguiNovelTextCharacter prev)
		{
			if (current.IsDisableAutoLineBreak)
			{
				return true;
			}
			if (Generator.TextSettings != null)
			{
				return Generator.TextSettings.CheckWordWrap(Generator, current, prev);
			}
			return false;
		}

		private bool IsOverMaxWidth(float x, float width, float maxWidth)
		{
			if (x > 0f)
			{
				return x + width > maxWidth;
			}
			return false;
		}

		private bool IsOverMaxHeight(float height, float maxHeight)
		{
			return height > maxHeight;
		}

		private bool IsAutoIndentation(char character)
		{
			if (Generator.TextSettings != null)
			{
				return Generator.TextSettings.IsAutoIndentation(character);
			}
			return false;
		}
	}
}
