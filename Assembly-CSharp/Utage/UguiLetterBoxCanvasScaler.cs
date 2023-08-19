using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Canvas))]
	[AddComponentMenu("Utage/Lib/UI/LetterBoxCanvasScaler")]
	public class UguiLetterBoxCanvasScaler : UguiLayoutControllerBase, ILayoutSelfController, ILayoutController
	{
		private Canvas canvas;

		private LetterBoxCamera letterBoxCamera;

		public Canvas Canvas
		{
			get
			{
				if (canvas == null)
				{
					canvas = GetComponent<Canvas>();
				}
				return canvas;
			}
		}

		public LetterBoxCamera LetterBoxCamera
		{
			get
			{
				if (letterBoxCamera == null)
				{
					if (Canvas.worldCamera == null)
					{
						if (!IsPrefabAsset())
						{
							Debug.LogError("Canvas worldCamera is null");
						}
					}
					else
					{
						letterBoxCamera = Canvas.worldCamera.GetComponent<LetterBoxCamera>();
					}
				}
				return letterBoxCamera;
			}
		}

		protected override void Update()
		{
			Vector2 currentSize = LetterBoxCamera.CurrentSize;
			if (!Mathf.Approximately(currentSize.x, base.CachedRectTransform.sizeDelta.x) || !Mathf.Approximately(currentSize.y, base.CachedRectTransform.sizeDelta.y))
			{
				SetDirty();
				return;
			}
			float a = 1f / (float)LetterBoxCamera.PixelsToUnits;
			if (!Mathf.Approximately(a, base.CachedRectTransform.localScale.x) || !Mathf.Approximately(a, base.CachedRectTransform.localScale.y) || !Mathf.Approximately(a, base.CachedRectTransform.localScale.z))
			{
				SetDirty();
			}
		}

		public void SetLayoutHorizontal()
		{
			tracker.Clear();
			if (Canvas.renderMode != RenderMode.WorldSpace)
			{
				if (!IsPrefabAsset())
				{
					Debug.LogError("LetterBoxCanvas is not RenderMode.World");
				}
				return;
			}
			if (LetterBoxCamera == null)
			{
				if (!IsPrefabAsset())
				{
					Debug.LogError("LetterBoxCamera is null");
				}
				return;
			}
			tracker.Add(this, base.CachedRectTransform, DrivenTransformProperties.Anchors | DrivenTransformProperties.Scale | DrivenTransformProperties.SizeDelta);
			RectTransform rectTransform = base.CachedRectTransform;
			Vector2 anchorMin = (base.CachedRectTransform.anchorMax = new Vector2(0.5f, 0.5f));
			rectTransform.anchorMin = anchorMin;
			base.CachedRectTransform.sizeDelta = LetterBoxCamera.CurrentSize;
			float num = 1f / (float)LetterBoxCamera.PixelsToUnits;
			base.CachedRectTransform.localScale = Vector3.one * num;
		}

		public void SetLayoutVertical()
		{
		}

		private bool IsPrefabAsset()
		{
			return false;
		}
	}
}
