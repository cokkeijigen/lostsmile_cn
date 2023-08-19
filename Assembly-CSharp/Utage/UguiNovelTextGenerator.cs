using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/Internal/NovelTextGenerator")]
	public class UguiNovelTextGenerator : MonoBehaviour
	{
		[Flags]
		public enum WordWrap
		{
			Default = 1,
			JapaneseKinsoku = 2
		}

		private enum ChagneType
		{
			None,
			VertexOnly,
			All
		}

		private UguiNovelText novelText;

		private UguiNovelTextGeneratorInfo info;

		[SerializeField]
		private float space = -1f;

		[SerializeField]
		private float letterSpaceSize = 1f;

		[SerializeField]
		[EnumFlags]
		private WordWrap wordWrap = WordWrap.Default | WordWrap.JapaneseKinsoku;

		[SerializeField]
		private int lengthOfView = -1;

		[SerializeField]
		private UguiNovelTextSettings textSettings;

		[SerializeField]
		private float rubySizeScale = 0.5f;

		[SerializeField]
		private float supOrSubSizeScale = 0.5f;

		[SerializeField]
		private UguiNovelTextEmojiData emojiData;

		[SerializeField]
		private char dashChar = '—';

		[SerializeField]
		private int bmpFontSize = 1;

		[SerializeField]
		private bool isUnicodeFont;

		private RectTransform cachedRectTransform;

		private bool isDebugLog;

		public UguiNovelText NovelText
		{
			get
			{
				return novelText ?? (novelText = GetComponent<UguiNovelText>());
			}
		}

		private UguiNovelTextGeneratorInfo Info
		{
			get
			{
				if (info == null)
				{
					info = new UguiNovelTextGeneratorInfo(this);
				}
				return info;
			}
		}

		private TextData TextData
		{
			get
			{
				return Info.TextData;
			}
		}

		internal List<UguiNovelTextLine> LineDataList
		{
			get
			{
				return Info.LineDataList;
			}
		}

		public List<UguiNovelTextHitArea> HitGroupLists
		{
			get
			{
				return Info.HitGroupLists;
			}
		}

		public float Space
		{
			get
			{
				return space;
			}
			set
			{
				space = value;
				SetAllDirty();
			}
		}

		public float LetterSpaceSize
		{
			get
			{
				return letterSpaceSize;
			}
			set
			{
				letterSpaceSize = value;
				SetAllDirty();
			}
		}

		public WordWrap WordWrapType
		{
			get
			{
				return wordWrap;
			}
			set
			{
				wordWrap = value;
				SetAllDirty();
			}
		}

		public int LengthOfView
		{
			get
			{
				return lengthOfView;
			}
			set
			{
				if (lengthOfView != value)
				{
					lengthOfView = value;
					NovelText.SetVerticesOnlyDirty();
				}
			}
		}

		internal int CurrentLengthOfView
		{
			get
			{
				if (LengthOfView >= 0)
				{
					return LengthOfView;
				}
				return TextData.Length;
			}
		}

		public UguiNovelTextSettings TextSettings
		{
			get
			{
				return textSettings;
			}
			set
			{
				textSettings = value;
				SetAllDirty();
			}
		}

		public float RubySizeScale
		{
			get
			{
				return rubySizeScale;
			}
			set
			{
				rubySizeScale = value;
				SetAllDirty();
			}
		}

		public float SupOrSubSizeScale
		{
			get
			{
				return supOrSubSizeScale;
			}
			set
			{
				supOrSubSizeScale = value;
				SetAllDirty();
			}
		}

		public UguiNovelTextEmojiData EmojiData
		{
			get
			{
				return emojiData;
			}
			set
			{
				emojiData = value;
				SetAllDirty();
			}
		}

		public char DashChar
		{
			get
			{
				if (dashChar != 0)
				{
					return dashChar;
				}
				return '—';
			}
		}

		public int BmpFontSize
		{
			get
			{
				if (NovelText.font != null && !NovelText.font.dynamic && bmpFontSize <= 0)
				{
					Debug.LogError("bmpFontSize is zero", this);
					return 1;
				}
				return bmpFontSize;
			}
		}

		public bool IsUnicodeFont
		{
			get
			{
				return isUnicodeFont;
			}
		}

		public RectTransform CachedRectTransform
		{
			get
			{
				if (cachedRectTransform == null)
				{
					cachedRectTransform = GetComponent<RectTransform>();
				}
				return cachedRectTransform;
			}
		}

		private ChagneType CurrentChangeType { get; set; }

		public float PreferredHeight
		{
			get
			{
				Refresh();
				return Info.PreferredHeight;
			}
		}

		public float PreferredWidth
		{
			get
			{
				Refresh();
				return Info.PreferredWidth;
			}
		}

		public Vector3 EndPosition
		{
			get
			{
				return Info.EndPosition;
			}
		}

		internal void RefreshEndPosition()
		{
			Refresh();
			Info.RefreshEndPosition();
		}

		private void SetAllDirty()
		{
			NovelText.SetAllDirty();
		}

		private void OnEnable()
		{
			RefreshAll();
		}

		private void LateUpdate()
		{
			Refresh();
			Info.UpdateGraphicObjectList(CachedRectTransform);
		}

		private void RefreshAll()
		{
			ChangeAll();
			Refresh();
		}

		private void Refresh()
		{
			switch (CurrentChangeType)
			{
			case ChagneType.All:
				if (isDebugLog)
				{
					Debug.Log("RefreshAll " + NovelText.text);
				}
				Info.BuildCharcteres();
				Info.BuildTextArea(CachedRectTransform);
				break;
			}
			CurrentChangeType = ChagneType.None;
		}

		internal void ChangeAll()
		{
			CurrentChangeType = ChagneType.All;
			if (isDebugLog)
			{
				Debug.Log("CurrentChangeType = ChagneType.All" + NovelText.text);
			}
		}

		internal void ChangeVertsOnly()
		{
			if (CurrentChangeType != ChagneType.All)
			{
				CurrentChangeType = ChagneType.VertexOnly;
				if (isDebugLog)
				{
					Debug.Log("CurrentChangeType = ChagneType.VertexOnly" + NovelText.text);
				}
			}
		}

		internal void RemakeVerts()
		{
			if (CurrentChangeType != ChagneType.All)
			{
				Info.RemakeVerts();
			}
		}

		internal void OnDirtyVerts()
		{
			if (isDebugLog)
			{
				Debug.Log("OnDirtyVerts" + CurrentChangeType.ToString() + NovelText.text);
			}
			Refresh();
		}

		internal void OnTextureRebuild(Font font)
		{
			if (font == NovelText.font)
			{
				if (isDebugLog)
				{
					Debug.Log("OnTextureRebuild " + NovelText.text);
				}
				Info.RebuildFontTexture(font);
			}
		}

		public void CreateVertex(List<UIVertex> verts)
		{
			if (CurrentChangeType != 0)
			{
				if (Application.isPlaying)
				{
					Debug.LogError("NotTextUpdated OnCreateVertex " + NovelText.text);
				}
				return;
			}
			if (isDebugLog)
			{
				Debug.Log("CreateVertex" + NovelText.text);
			}
			Info.CreateVertex(verts);
		}
	}
}
