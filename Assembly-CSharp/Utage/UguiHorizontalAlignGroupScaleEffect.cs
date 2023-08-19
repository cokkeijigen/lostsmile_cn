using System;
using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/HolizontalAlignGroupScaleEffect")]
	public class UguiHorizontalAlignGroupScaleEffect : UguiHorizontalAlignGroup
	{
		public float scaleRangeLeft = -100f;

		public float scaleRangeWidth = 200f;

		public bool ignoreLocalPositionToScaleEffectRage = true;

		public float minScale = 0.5f;

		public float maxScale = 1f;

		private Vector3 ScaleEffectWorldPointLeft
		{
			get
			{
				Vector3 position = new Vector3(scaleRangeLeft, 0f, 0f);
				if (ignoreLocalPositionToScaleEffectRage)
				{
					position -= base.CachedRectTransform.localPosition;
				}
				return base.CachedRectTransform.TransformPoint(position);
			}
		}

		private Vector3 ScaleEffectWorldPointRight
		{
			get
			{
				Vector3 position = new Vector3(scaleRangeLeft + scaleRangeWidth, 0f, 0f);
				if (ignoreLocalPositionToScaleEffectRage)
				{
					position -= base.CachedRectTransform.localPosition;
				}
				return base.CachedRectTransform.TransformPoint(position);
			}
		}

		private float ScaleEffectChildLocalPointLeft
		{
			get
			{
				Vector3 scaleEffectWorldPointLeft = ScaleEffectWorldPointLeft;
				return base.CachedRectTransform.InverseTransformPoint(scaleEffectWorldPointLeft).x;
			}
		}

		private float ScaleEffectChildLocalPointRight
		{
			get
			{
				Vector3 scaleEffectWorldPointRight = ScaleEffectWorldPointRight;
				return base.CachedRectTransform.InverseTransformPoint(scaleEffectWorldPointRight).x;
			}
		}

		protected override void CustomChild(RectTransform child, float offset)
		{
			tracker.Add(this, child, DrivenTransformProperties.Scale);
			float num = minScale;
			float num2 = child.rect.width * num;
			float scaleEffectChildLocalPointLeft = ScaleEffectChildLocalPointLeft;
			float scaleEffectChildLocalPointRight = ScaleEffectChildLocalPointRight;
			if (direction == AlignDirection.LeftToRight)
			{
				scaleEffectChildLocalPointLeft -= num2;
				if (scaleEffectChildLocalPointLeft < offset && offset < scaleEffectChildLocalPointRight)
				{
					float num3 = (offset - scaleEffectChildLocalPointLeft) / (scaleEffectChildLocalPointRight - scaleEffectChildLocalPointLeft);
					if (num3 > 0.5f)
					{
						num3 = 1f - num3;
					}
					num = Mathf.Lerp(minScale, maxScale, num3);
				}
			}
			else
			{
				scaleEffectChildLocalPointRight += num2;
				if (scaleEffectChildLocalPointLeft < offset && offset < scaleEffectChildLocalPointRight)
				{
					float t = Mathf.Sin((float)Math.PI * (offset - scaleEffectChildLocalPointLeft) / (scaleEffectChildLocalPointRight - scaleEffectChildLocalPointLeft));
					num = Mathf.Lerp(minScale, maxScale, t);
				}
			}
			child.localScale = Vector3.one * num;
		}

		protected override void CustomLayoutRectTransform()
		{
			DrivenTransformProperties drivenTransformProperties = DrivenTransformProperties.None;
			drivenTransformProperties |= DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.PivotX;
			tracker.Add(this, base.CachedRectTransform, drivenTransformProperties);
			if (direction == AlignDirection.LeftToRight)
			{
				base.CachedRectTransform.anchorMin = new Vector2(0f, base.CachedRectTransform.anchorMin.y);
				base.CachedRectTransform.anchorMax = new Vector2(0f, base.CachedRectTransform.anchorMax.y);
				base.CachedRectTransform.pivot = new Vector2(0f, base.CachedRectTransform.pivot.y);
			}
			else
			{
				base.CachedRectTransform.anchorMin = new Vector2(1f, base.CachedRectTransform.anchorMin.y);
				base.CachedRectTransform.anchorMax = new Vector2(1f, base.CachedRectTransform.anchorMax.y);
				base.CachedRectTransform.pivot = new Vector2(1f, base.CachedRectTransform.pivot.y);
			}
		}

		private void OnDrawGizmos()
		{
			Vector3 scaleEffectWorldPointLeft = ScaleEffectWorldPointLeft;
			Vector3 scaleEffectWorldPointRight = ScaleEffectWorldPointRight;
			Gizmos.DrawLine(scaleEffectWorldPointLeft, scaleEffectWorldPointRight);
		}
	}
}
