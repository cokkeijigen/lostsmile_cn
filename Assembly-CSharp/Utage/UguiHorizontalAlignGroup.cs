using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/HorizontalAlignGroup")]
	public class UguiHorizontalAlignGroup : UguiAlignGroup
	{
		public enum AlignDirection
		{
			LeftToRight,
			RightToLeft
		}

		public float paddingLeft;

		public float paddingRight;

		public AlignDirection direction;

		public override void Reposition()
		{
			if (base.CachedRectTransform.childCount <= 0)
			{
				return;
			}
			float offset = ((direction == AlignDirection.LeftToRight) ? paddingLeft : (0f - paddingRight));
			float num = 0f;
			foreach (RectTransform item in base.CachedRectTransform)
			{
				float num2 = AlignChild(item, ref offset);
				num += num2 + space;
			}
			num += paddingLeft + paddingRight - space;
			LayoutRectTransorm(num);
		}

		protected virtual float AlignChild(RectTransform child, ref float offset)
		{
			float num = ((direction == AlignDirection.LeftToRight) ? 1 : (-1));
			float x = ((direction != 0) ? 1 : 0);
			DrivenTransformProperties drivenProperties = DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX;
			tracker.Add(this, child, drivenProperties);
			child.anchorMin = new Vector2(x, child.anchorMin.y);
			child.anchorMax = new Vector2(x, child.anchorMax.y);
			CustomChild(child, offset);
			float num2 = child.rect.width * Mathf.Abs(child.localScale.x);
			offset += num * (num2 * child.pivot.x);
			child.anchoredPosition = new Vector2(offset, child.anchoredPosition.y);
			offset += num * (num2 * (1f - child.pivot.x) + space);
			return num2;
		}

		protected virtual void LayoutRectTransorm(float totalSize)
		{
			if (isAutoResize)
			{
				tracker.Add(this, base.CachedRectTransform, DrivenTransformProperties.SizeDeltaX);
				base.CachedRectTransform.sizeDelta = new Vector2(totalSize, base.CachedRectTransform.sizeDelta.y);
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
