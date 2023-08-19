using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	public class UguiNovelTextCharacter
	{
		public bool isAutoLineBreak;

		public bool isKinsokuBurasage;

		public CharData charData;

		private CharacterInfo charInfo;

		private float width;

		private int fontSize;

		private int defaultFontSize;

		private FontStyle fontStyle;

		private UIVertex[] verts;

		private bool isError;

		private bool isSprite;

		private Color effectColor = Color.white;

		public CharData.CustomCharaInfo CustomInfo
		{
			get
			{
				return charData.CustomInfo;
			}
		}

		public float Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		public int FontSize
		{
			get
			{
				return fontSize;
			}
		}

		public int DefaultFontSize
		{
			get
			{
				return defaultFontSize;
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return fontStyle;
			}
		}

		public UIVertex[] Verts
		{
			get
			{
				return verts;
			}
		}

		public float PositionX
		{
			get
			{
				return X0 + OffsetX;
			}
		}

		public float PositionY
		{
			get
			{
				return Y0 + OffsetY;
			}
		}

		private float X0 { get; set; }

		private float Y0 { get; set; }

		private float OffsetX { get; set; }

		private float OffsetY { get; set; }

		public char Char
		{
			get
			{
				return charData.Char;
			}
		}

		public bool IsBr
		{
			get
			{
				return charData.IsBr;
			}
		}

		public bool IsBrOrAutoBr
		{
			get
			{
				if (!isAutoLineBreak)
				{
					return charData.IsBr;
				}
				return true;
			}
		}

		public bool IsBlank
		{
			get
			{
				if (!IsCustomBlank)
				{
					return char.IsWhiteSpace(charData.Char);
				}
				return true;
			}
		}

		private bool IsCustomBlank
		{
			get
			{
				if (!isError && !CustomSpace)
				{
					return charData.IsBr;
				}
				return true;
			}
		}

		private bool CustomSpace { get; set; }

		public bool IsSprite
		{
			get
			{
				return isSprite;
			}
			set
			{
				isSprite = value;
			}
		}

		public bool IsNoFontData
		{
			get
			{
				if (!IsCustomBlank)
				{
					return IsSprite;
				}
				return true;
			}
		}

		public float RubySpaceSize { get; set; }

		public bool IsDisableAutoLineBreak
		{
			get
			{
				if (CustomInfo.IsRuby && !CustomInfo.IsRubyTop)
				{
					return true;
				}
				if (CustomInfo.IsGroup && !CustomInfo.IsGroupTop)
				{
					return true;
				}
				return false;
			}
		}

		public float BeginPositionX
		{
			get
			{
				return PositionX - RubySpaceSize;
			}
		}

		public float EndPositionX
		{
			get
			{
				return PositionX + Width + RubySpaceSize;
			}
		}

		public bool IsVisible { get; set; }

		public float BmpFontScale { get; private set; }

		public UguiNovelTextCharacter(CharData charData, UguiNovelTextGenerator generator)
		{
			if (charData.CustomInfo.IsDash)
			{
				charData.Char = generator.DashChar;
			}
			int bmpFontSize = generator.BmpFontSize;
			Init(charData, generator.NovelText.font, generator.NovelText.fontSize, bmpFontSize, generator.NovelText.fontStyle, generator.Space);
			if (charData.CustomInfo.IsSuperOrSubScript)
			{
				fontSize = Mathf.FloorToInt(generator.SupOrSubSizeScale * (float)fontSize);
				if (!generator.NovelText.font.dynamic)
				{
					BmpFontScale = 1f * (float)fontSize / (float)bmpFontSize;
				}
			}
			if (charData.CustomInfo.IsSpace)
			{
				width = charData.CustomInfo.SpaceSize;
				CustomSpace = true;
			}
			if (!generator.EmojiData)
			{
				return;
			}
			if (CustomInfo.IsEmoji || generator.EmojiData.Contains(Char))
			{
				IsSprite = true;
			}
			if (IsSprite)
			{
				Sprite sprite = FindSprite(generator);
				if ((bool)sprite)
				{
					float num = sprite.rect.width / (float)generator.EmojiData.Size;
					width = num * (float)fontSize;
					return;
				}
				Debug.LogError("Not found Emoji[" + Char.ToString() + "]:" + (int)Char);
			}
		}

		public UguiNovelTextCharacter(CharData charData, Font font, int fontSize, int bmpFontSize, FontStyle fontStyle)
		{
			Init(charData, font, fontSize, bmpFontSize, fontStyle, -1f);
		}

		private void Init(CharData charData, Font font, int fontSize, int bmpFontSize, FontStyle fontStyle, float spacing)
		{
			this.charData = charData;
			this.fontSize = (defaultFontSize = charData.CustomInfo.GetCustomedSize(fontSize));
			this.fontStyle = charData.CustomInfo.GetCustomedStyle(fontStyle);
			if (charData.IsBr)
			{
				width = 0f;
			}
			else if (char.IsWhiteSpace(charData.Char) && spacing >= 0f)
			{
				CustomSpace = true;
				width = spacing;
			}
			if (font.dynamic)
			{
				BmpFontScale = 1f;
			}
			else
			{
				BmpFontScale = 1f * (float)this.fontSize / (float)bmpFontSize;
			}
		}

		public bool TrySetCharacterInfo(Font font)
		{
			if (IsNoFontData)
			{
				return true;
			}
			if (!font.dynamic)
			{
				if (!font.GetCharacterInfo(charData.Char, out charInfo))
				{
					return false;
				}
			}
			else if (!font.GetCharacterInfo(charData.Char, out charInfo, FontSize, FontStyle))
			{
				return false;
			}
			width = WrapperUnityVersion.GetCharacterInfoWidth(ref charInfo);
			width *= BmpFontScale;
			if (CustomInfo.IsDash)
			{
				width *= CustomInfo.DashSize;
			}
			return true;
		}

		public void SetCharacterInfo(Font font)
		{
			if (!TrySetCharacterInfo(font))
			{
				isError = true;
				width = fontSize;
			}
		}

		internal void InitPositionX(float x)
		{
			X0 = x;
			OffsetX = 0f;
		}

		internal void InitPositionY(float y)
		{
			Y0 = y;
			OffsetY = 0f;
		}

		public void ApplyOffsetX(float offsetX)
		{
			OffsetX += offsetX;
		}

		public void ApplyOffsetY(float offsetY)
		{
			OffsetY += offsetY;
		}

		public void MakeVerts(Color defaultColor, UguiNovelTextGenerator generator)
		{
			if (IsNoFontData)
			{
				return;
			}
			if (verts == null)
			{
				verts = new UIVertex[4];
				for (int i = 0; i < 4; i++)
				{
					verts[i] = UIVertex.simpleVert;
				}
			}
			Color customedColor = charData.CustomInfo.GetCustomedColor(defaultColor);
			customedColor *= effectColor;
			for (int j = 0; j < verts.Length; j++)
			{
				verts[j].color = customedColor;
			}
			WrapperUnityVersion.SetCharacterInfoToVertex(verts, this, ref charInfo, generator.NovelText.font);
			if (!generator.NovelText.font.dynamic && !generator.IsUnicodeFont)
			{
				float num = fontSize;
				for (int k = 0; k < 4; k++)
				{
					verts[k].position.y += num;
				}
			}
			if (CustomInfo.IsSuperScript)
			{
				float num2 = (1f - generator.SupOrSubSizeScale) * (float)DefaultFontSize;
				for (int l = 0; l < 4; l++)
				{
					verts[l].position.y += num2;
				}
			}
			if (CustomInfo.IsDash)
			{
				float num3 = Mathf.Abs(verts[2].position.y - verts[0].position.y);
				float num4 = PositionY + (float)(FontSize / 2);
				verts[0].position.y = (verts[1].position.y = num4 - num3 / 2f);
				verts[2].position.y = (verts[3].position.y = num4 + num3 / 2f);
				UIVertex[] array = new UIVertex[12];
				for (int m = 0; m < 12; m++)
				{
					array[m] = verts[m % 4];
				}
				float x = verts[0].position.x;
				float num5 = verts[1].position.x - verts[0].position.x;
				float num6 = x + Width;
				float num7 = x + num5 / 3f;
				float num8 = num6 - num5 / 3f;
				SetVertexX(array, 0, x, num7);
				SetVertexX(array, 4, num7, num8);
				SetVertexX(array, 8, num8, num6);
				Vector2 uv = verts[0].uv0;
				Vector2 uv2 = verts[1].uv0;
				Vector2 uv3 = verts[2].uv0;
				Vector2 uv4 = verts[3].uv0;
				Vector2 vector = (uv2 - uv) * 1f / 3f + uv;
				Vector2 vector2 = (uv2 - uv) * 2f / 3f + uv;
				Vector2 vector3 = (uv3 - uv4) * 2f / 3f + uv4;
				Vector2 vector4 = (uv3 - uv4) * 1f / 3f + uv4;
				SetVertexUV(array, 0, uv, vector, vector4, uv4);
				SetVertexUV(array, 4, vector, vector2, vector3, vector4);
				SetVertexUV(array, 8, vector2, uv2, uv3, vector3);
				verts = array;
			}
		}

		private void SetVertexX(UIVertex[] vertex, int index, float xMin, float xMax)
		{
			vertex[index].position.x = (vertex[index + 3].position.x = xMin);
			vertex[index + 1].position.x = (vertex[index + 2].position.x = xMax);
		}

		private void SetVertexUV(UIVertex[] vertex, int index, Vector2 uvBottomLeft, Vector2 uvBottomRight, Vector2 uvTopRight, Vector2 uvTopLeft)
		{
			vertex[index].uv0 = uvBottomLeft;
			vertex[index + 1].uv0 = uvBottomRight;
			vertex[index + 2].uv0 = uvTopRight;
			vertex[index + 3].uv0 = uvTopLeft;
		}

		internal RectTransform AddGraphicObject(RectTransform parent, UguiNovelTextGenerator generator)
		{
			if (!IsSprite)
			{
				return null;
			}
			Sprite sprite = FindSprite(generator);
			if ((bool)sprite)
			{
				RectTransform rectTransform = parent.AddChildGameObjectComponent<RectTransform>(sprite.name);
				rectTransform.gameObject.hideFlags = HideFlags.DontSave;
				rectTransform.gameObject.AddComponent<Image>().sprite = sprite;
				float num = 1f;
				float num2 = (num = 1f * (float)FontSize / (float)generator.EmojiData.Size);
				float num3 = sprite.rect.width * num2;
				float num4 = sprite.rect.height * num;
				rectTransform.sizeDelta = new Vector2(num3, num4);
				rectTransform.localPosition = new Vector3(PositionX + num3 / 2f, PositionY + num4 / 2f, 0f);
				return rectTransform;
			}
			return null;
		}

		private Sprite FindSprite(UguiNovelTextGenerator generator)
		{
			if (generator.EmojiData == null)
			{
				return null;
			}
			Sprite sprite = generator.EmojiData.GetSprite(Char);
			if (sprite == null && CustomInfo.IsEmoji)
			{
				sprite = generator.EmojiData.GetSprite(charData.CustomInfo.EmojiKey);
			}
			return sprite;
		}

		internal void ChangeEffectColor(Color effectColor)
		{
			this.effectColor = effectColor;
		}

		internal void ResetEffectColor()
		{
			effectColor = Color.white;
		}
	}
}
