using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utage
{
	[RequireComponent(typeof(UguiNovelTextGenerator))]
	[AddComponentMenu("Utage/Lib/UI/NovelText")]
	public class UguiNovelText : Text
	{
		private UguiNovelTextGenerator textGenerator;

		private readonly UIVertex[] m_TempVerts = new UIVertex[4];

		public int LengthOfView
		{
			get
			{
				return TextGenerator.LengthOfView;
			}
			set
			{
				TextGenerator.LengthOfView = value;
			}
		}

		public UguiNovelTextGenerator TextGenerator => textGenerator ?? (textGenerator = GetComponent<UguiNovelTextGenerator>());

		public Vector3 EndPosition => TextGenerator.EndPosition;

		public Vector3 CurrentEndPosition
		{
			get
			{
				TextGenerator.RefreshEndPosition();
				return TextGenerator.EndPosition;
			}
		}

		public override float preferredHeight => TextGenerator.PreferredHeight;

		public override float preferredWidth => TextGenerator.PreferredWidth;

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (base.font == null)
			{
				return;
			}
			List<UIVertex> list = ListPool<UIVertex>.Get();
			TextGenerator.CreateVertex(list);
			vh.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				int num = i & 3;
				m_TempVerts[num] = list[i];
				if (num == 3)
				{
					vh.AddUIVertexQuad(m_TempVerts);
				}
			}
			ListPool<UIVertex>.Release(list);
		}

		protected override void Awake()
		{
			base.Awake();
			m_OnDirtyVertsCallback = (UnityAction)Delegate.Combine(m_OnDirtyVertsCallback, new UnityAction(OnDirtyVerts));
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			Font.textureRebuilt += OnTextureRebuild;
		}

		protected override void OnDisable()
		{
			Font.textureRebuilt -= OnTextureRebuild;
			TextGenerator.ChangeAll();
			base.OnDisable();
		}

		private void OnTextureRebuild(Font font)
		{
			if (!(this == null) && IsActive())
			{
				TextGenerator.OnTextureRebuild(font);
				if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
				{
					UpdateGeometry();
				}
				else
				{
					SetVerticesDirty();
				}
			}
		}

		public override void SetAllDirty()
		{
			TextGenerator.ChangeAll();
			base.SetAllDirty();
		}

		internal void RemakeVerts()
		{
			TextGenerator.RemakeVerts();
			base.SetVerticesDirty();
		}

		internal void SetVerticesOnlyDirty()
		{
			TextGenerator.ChangeVertsOnly();
			base.SetVerticesDirty();
		}

		public override void SetVerticesDirty()
		{
			TextGenerator.ChangeAll();
			base.SetVerticesDirty();
		}

		private void OnDirtyVerts()
		{
			TextGenerator.OnDirtyVerts();
		}

		public int GetTotalLineHeight(int fontSize)
		{
			return Mathf.CeilToInt((float)fontSize * (base.lineSpacing + 0.2f));
		}
	}
}
