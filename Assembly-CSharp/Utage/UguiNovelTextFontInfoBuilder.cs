using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	internal class UguiNovelTextFontInfoBuilder
	{
		internal class RequestCharactersInfo
		{
			public string characters;

			public readonly int size;

			public readonly FontStyle style;

			public RequestCharactersInfo(UguiNovelTextCharacter data)
			{
				characters = data.Char.ToString() ?? "";
				size = data.FontSize;
				style = data.FontStyle;
			}

			public bool TryAddData(UguiNovelTextCharacter data)
			{
				if (size == data.FontSize && style == data.FontStyle)
				{
					characters += data.Char;
					return true;
				}
				return false;
			}
		}

		private bool RequestingCharactersInTexture { get; set; }

		internal void InitFontCharactes(Font font, List<UguiNovelTextCharacter> characterDataList, UguiNovelTextGeneratorAdditional additional)
		{
			bool flag = false;
			int num = 5;
			for (int i = 0; i < num; i++)
			{
				if (TryeSetFontCharcters(font, characterDataList, additional))
				{
					flag = true;
					break;
				}
				RequestCharactersInTexture(font, characterDataList, additional);
				if (i == num - 1)
				{
					SetFontCharcters(font, characterDataList);
				}
			}
			if (!flag)
			{
				Debug.LogError("InitFontCharactes Error");
				TryeSetFontCharcters(font, characterDataList, additional);
			}
		}

		internal void RebuildFontTexture(Font font, List<UguiNovelTextCharacter> characterDataList, UguiNovelTextGeneratorAdditional additional)
		{
			throw new NotImplementedException();
		}

		private bool TryeSetFontCharcters(Font font, List<UguiNovelTextCharacter> characterDataList, UguiNovelTextGeneratorAdditional additional)
		{
			foreach (UguiNovelTextCharacter characterData in characterDataList)
			{
				if (!characterData.TrySetCharacterInfo(font))
				{
					return false;
				}
			}
			return additional.TrySetFontCharcters(font);
		}

		private void SetFontCharcters(Font font, List<UguiNovelTextCharacter> characterDataList)
		{
			foreach (UguiNovelTextCharacter characterData in characterDataList)
			{
				characterData.SetCharacterInfo(font);
			}
		}

		private void RequestCharactersInTexture(Font font, List<UguiNovelTextCharacter> characterDataList, UguiNovelTextGeneratorAdditional additional)
		{
			List<RequestCharactersInfo> list = MakeRequestCharactersInfoList(characterDataList, additional);
			RequestingCharactersInTexture = true;
			Font.textureRebuilt += FontTextureRebuildCallback;
			foreach (RequestCharactersInfo item in list)
			{
				font.RequestCharactersInTexture(item.characters, item.size, item.style);
			}
			Font.textureRebuilt -= FontTextureRebuildCallback;
			RequestingCharactersInTexture = false;
		}

		private void FontTextureRebuildCallback(Font font)
		{
		}

		private List<RequestCharactersInfo> MakeRequestCharactersInfoList(List<UguiNovelTextCharacter> characterDataList, UguiNovelTextGeneratorAdditional additional)
		{
			List<RequestCharactersInfo> list = new List<RequestCharactersInfo>();
			foreach (UguiNovelTextCharacter characterData in characterDataList)
			{
				AddToRequestCharactersInfoList(characterData, list);
			}
			foreach (UguiNovelTextCharacter item in additional.MakeCharacterList())
			{
				AddToRequestCharactersInfoList(item, list);
			}
			return list;
		}

		private void AddToRequestCharactersInfoList(UguiNovelTextCharacter characterData, List<RequestCharactersInfo> infoList)
		{
			if (characterData.IsNoFontData)
			{
				return;
			}
			foreach (RequestCharactersInfo info in infoList)
			{
				if (info.TryAddData(characterData))
				{
					return;
				}
			}
			infoList.Add(new RequestCharactersInfo(characterData));
		}
	}
}
