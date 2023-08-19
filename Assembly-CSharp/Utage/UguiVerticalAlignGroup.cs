using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/VerticalAlignGroup")]
	public class UguiVerticalAlignGroup : UguiAlignGroup
	{
		public enum AlignDirection
		{
			TopToBottom,
			BottomToTop
		}

		public float paddingTop;

		public float paddingBottom;

		public AlignDirection direction;

		public override void Reposition()
		{
			if (base.CachedRectTransform.childCount <= 0)
			{
				return;
			}
			float offset = ((direction == AlignDirection.BottomToTop) ? paddingBottom : (0f - paddingTop));
			float num = 0f;
			foreach (RectTransform item in base.CachedRectTransform)
			{
				float num2 = AlignChild(item, ref offset);
				num += num2 + space;
			}
			num += paddingBottom + paddingTop - space;
			LayoutRectTransorm(num);
		}

		protected virtual float AlignChild(RectTransform child, ref float offset)
		{
			float num = ((direction == AlignDirection.BottomToTop) ? 1 : (-1));
			float y = ((direction != AlignDirection.BottomToTop) ? 1 : 0);
			DrivenTransformProperties drivenProperties = DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY;
			tracker.Add(this, child, drivenProperties);
			child.anchorMin = new Vector2(child.anchorMin.x, y);
			child.anchorMax = new Vector2(child.anchorMax.x, y);
			CustomChild(child, offset);
			float num2 = child.rect.height * Mathf.Abs(child.localScale.y);
			offset += num * (num2 * child.pivot.y);
			child.anchoredPosition = new Vector2(child.anchoredPosition.x, offset);
			offset += num * (num2 * (1f - child.pivot.y) + space);
			return num2;
		}

		protected virtual void LayoutRectTransorm(float totalSize)
		{
			if (isAutoResize)
			{
				tracker.Add(this, base.CachedRectTransform, DrivenTransformProperties.SizeDeltaY);
				base.CachedRectTransform.sizeDelta = new Vector2(base.CachedRectTransform.sizeDelta.x, totalSize);
			}
			CustomLayoutRectTransform();
		}

		protected virtual void CustomChild(RectTransform child, float offset)
		{
		}

		protected virtual void CustomLayoutRectTransform()
		{
		}
	}
}
