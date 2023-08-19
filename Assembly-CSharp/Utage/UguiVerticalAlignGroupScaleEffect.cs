using System;
using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/VerticalAlignGroupScaleEffect")]
	public class UguiVerticalAlignGroupScaleEffect : UguiVerticalAlignGroup
	{
		public float scaleRangeTop = -100f;

		public float scaleRangeHeight = 200f;

		public bool ignoreLocalPositionToScaleEffectRage = true;

		public float minScale = 0.5f;

		public float maxScale = 1f;

		private Vector3 ScaleEffectWorldPointTop
		{
			get
			{
				Vector3 position = new Vector3(0f, scaleRangeTop, 0f);
				if (ignoreLocalPositionToScaleEffectRage)
				{
					position -= base.CachedRectTransform.localPosition;
				}
				return base.CachedRectTransform.TransformPoint(position);
			}
		}

		private Vector3 ScaleEffectWorldPointBottom
		{
			get
			{
				Vector3 position = new Vector3(0f, scaleRangeTop - scaleRangeHeight, 0f);
				if (ignoreLocalPositionToScaleEffectRage)
				{
					position -= base.CachedRectTransform.localPosition;
				}
				return base.CachedRectTransform.TransformPoint(position);
			}
		}

		private float ScaleEffectChildLocalPointTop
		{
			get
			{
				Vector3 scaleEffectWorldPointTop = ScaleEffectWorldPointTop;
				return base.CachedRectTransform.InverseTransformPoint(scaleEffectWorldPointTop).y;
			}
		}

		private float ScaleEffectChildLocalPointBottom
		{
			get
			{
				Vector3 scaleEffectWorldPointBottom = ScaleEffectWorldPointBottom;
				return base.CachedRectTransform.InverseTransformPoint(scaleEffectWorldPointBottom).y;
			}
		}

		protected override void CustomChild(RectTransform child, float offset)
		{
			tracker.Add(this, child, DrivenTransformProperties.Scale);
			float num = minScale;
			float num2 = child.rect.height * num;
			float scaleEffectChildLocalPointTop = ScaleEffectChildLocalPointTop;
			float scaleEffectChildLocalPointBottom = ScaleEffectChildLocalPointBottom;
			if (direction == AlignDirection.BottomToTop)
			{
				scaleEffectChildLocalPointBottom -= num2;
				if (scaleEffectChildLocalPointBottom < offset && offset < scaleEffectChildLocalPointTop)
				{
					float num3 = (offset - scaleEffectChildLocalPointBottom) / (scaleEffectChildLocalPointTop - scaleEffectChildLocalPointBottom);
					if (num3 > 0.5f)
					{
						num3 = 1f - num3;
					}
					num = Mathf.Lerp(minScale, maxScale, num3);
				}
			}
			else
			{
				scaleEffectChildLocalPointTop += num2;
				if (scaleEffectChildLocalPointBottom < offset && offset < scaleEffectChildLocalPointTop)
				{
					float t = Mathf.Sin((float)Math.PI * (offset - scaleEffectChildLocalPointBottom) / (scaleEffectChildLocalPointTop - scaleEffectChildLocalPointBottom));
					num = Mathf.Lerp(minScale, maxScale, t);
				}
			}
			child.localScale = Vector3.one * num;
		}

		protected override void CustomLayoutRectTransform()
		{
			DrivenTransformProperties drivenTransformProperties = DrivenTransformProperties.None;
			drivenTransformProperties |= DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.PivotY;
			tracker.Add(this, base.CachedRectTransform, drivenTransformProperties);
			if (direction == AlignDirection.BottomToTop)
			{
				base.CachedRectTransform.anchorMin = new Vector2(base.CachedRectTransform.anchorMin.x, 0f);
				base.CachedRectTransform.anchorMax = new Vector2(base.CachedRectTransform.anchorMax.x, 0f);
				base.CachedRectTransform.pivot = new Vector2(base.CachedRectTransform.pivot.x, 0f);
			}
			else
			{
				base.CachedRectTransform.anchorMin = new Vector2(base.CachedRectTransform.anchorMin.x, 1f);
				base.CachedRectTransform.anchorMax = new Vector2(base.CachedRectTransform.anchorMax.x, 1f);
				base.CachedRectTransform.pivot = new Vector2(base.CachedRectTransform.pivot.x, 1f);
			}
		}

		private void OnDrawGizmos()
		{
			Vector3 scaleEffectWorldPointTop = ScaleEffectWorldPointTop;
			Vector3 scaleEffectWorldPointBottom = ScaleEffectWorldPointBottom;
			Gizmos.DrawLine(scaleEffectWorldPointTop, scaleEffectWorldPointBottom);
		}
	}
}
