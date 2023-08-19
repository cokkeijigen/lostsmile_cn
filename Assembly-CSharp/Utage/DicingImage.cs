using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/DicingImage")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class DicingImage : MaskableGraphic, ICanvasRaycastFilter
	{
		[SerializeField]
		private DicingTextures dicingData;

		[SerializeField]
		[StringPopupFunction("GetPattenNameList")]
		private string pattern;

		private Dictionary<string, string> patternOption = new Dictionary<string, string>();

		private DicingTextureData patternData;

		[SerializeField]
		private bool skipTransParentCell = true;

		[SerializeField]
		private Rect uvRect = new Rect(0f, 0f, 1f, 1f);

		private Texture m_Texture;

		public DicingTextures DicingData
		{
			get
			{
				return dicingData;
			}
			set
			{
				dicingData = value;
				pattern = "";
				OnChangePattern();
				SetAllDirty();
			}
		}

		private string Pattern
		{
			get
			{
				return pattern;
			}
			set
			{
				if (!DicingData.Exists(value))
				{
					Debug.LogError(value + " is not find in " + DicingData.name);
					return;
				}
				pattern = value;
				OnChangePattern();
				SetAllDirty();
			}
		}

		public string MainPattern { get; private set; }

		public DicingTextureData PatternData
		{
			get
			{
				return patternData;
			}
		}

		public bool SkipTransParentCell
		{
			get
			{
				return skipTransParentCell;
			}
			set
			{
				skipTransParentCell = value;
			}
		}

		public Rect UvRect
		{
			get
			{
				return uvRect;
			}
			set
			{
				uvRect = value;
				SetAllDirty();
			}
		}

		public override Texture mainTexture
		{
			get
			{
				if (m_Texture == null)
				{
					if (material != null && material.mainTexture != null)
					{
						return material.mainTexture;
					}
					return Graphic.s_WhiteTexture;
				}
				return m_Texture;
			}
		}

		public void ChangePattern(string pattern)
		{
			MainPattern = pattern;
			patternOption.Clear();
			Pattern = pattern;
		}

		public bool TryChangePatternWithOption(string mainPattern, string optionTag, string option)
		{
			MainPattern = mainPattern;
			patternOption[optionTag] = option;
			string text = MakePatternWithOption();
			if (DicingData.Exists(text))
			{
				Pattern = text;
				return true;
			}
			if (DicingData.Exists(option))
			{
				Pattern = option;
				return true;
			}
			Pattern = MainPattern;
			return false;
		}

		public string MakePatternWithOption()
		{
			string text = MainPattern;
			foreach (KeyValuePair<string, string> item in new SortedDictionary<string, string>(patternOption))
			{
				text += item.Value;
			}
			return text;
		}

		private List<string> GetPattenNameList()
		{
			if (dicingData == null)
			{
				return null;
			}
			return dicingData.GetPattenNameList();
		}

		private void OnChangePattern()
		{
			if (DicingData == null || string.IsNullOrEmpty(pattern))
			{
				m_Texture = Graphic.s_WhiteTexture;
				return;
			}
			patternData = DicingData.GetTextureData(Pattern);
			if (patternData == null)
			{
				Debug.LogError(Pattern + " is not find in " + DicingData.name);
			}
			else
			{
				m_Texture = DicingData.GetTexture(patternData.AtlasName);
			}
		}

		public override void SetNativeSize()
		{
			if (PatternData != null)
			{
				base.rectTransform.anchorMax = base.rectTransform.anchorMin;
				base.rectTransform.sizeDelta = GetNaitiveSize();
			}
		}

		internal List<DicingTextureData.QuadVerts> GetVerts(DicingTextureData patternData)
		{
			return DicingData.GetVerts(patternData);
		}

		private Vector2 GetNaitiveSize()
		{
			return new Vector2(uvRect.width * (float)PatternData.Width, uvRect.height * (float)PatternData.Height);
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			OnChangePattern();
			if (PatternData != null)
			{
				Color color32 = color;
				vh.Clear();
				int index = 0;
				ForeachVertexList(delegate(Rect r, Rect uv)
				{
					vh.AddVert(new Vector3(r.xMin, r.yMin), color32, new Vector2(uv.xMin, uv.yMin));
					vh.AddVert(new Vector3(r.xMin, r.yMax), color32, new Vector2(uv.xMin, uv.yMax));
					vh.AddVert(new Vector3(r.xMax, r.yMax), color32, new Vector2(uv.xMax, uv.yMax));
					vh.AddVert(new Vector3(r.xMax, r.yMin), color32, new Vector2(uv.xMax, uv.yMin));
					vh.AddTriangle(index, index + 1, index + 2);
					vh.AddTriangle(index + 2, index + 3, index);
					index += 4;
				});
			}
		}

		protected void ForeachVertexList(Action<Rect, Rect> function)
		{
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			PatternData.ForeachVertexList(pixelAdjustedRect, uvRect, skipTransParentCell, DicingData, function);
		}

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, sp, eventCamera, out localPoint);
			return HitTest(localPoint);
		}

		public bool HitTest(Vector2 localPosition)
		{
			if (!GetPixelAdjustedRect().Contains(localPosition))
			{
				return false;
			}
			if (PatternData == null)
			{
				return false;
			}
			bool isHit = false;
			ForeachVertexList(delegate(Rect r, Rect uv)
			{
				isHit |= r.Contains(localPosition);
			});
			return isHit;
		}
	}
}
