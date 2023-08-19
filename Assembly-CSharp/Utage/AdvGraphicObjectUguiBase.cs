using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public abstract class AdvGraphicObjectUguiBase : AdvGraphicBase
	{
		protected abstract Material Material { get; set; }

		public override void Init(AdvGraphicObject parentObject)
		{
			base.Init(parentObject);
			AddGraphicComponentOnInit();
			if (GetComponent<IAdvClickEvent>() == null)
			{
				base.gameObject.AddComponent<AdvClickEvent>();
			}
		}

		protected abstract void AddGraphicComponentOnInit();

		internal override void Scale(AdvGraphicInfo graphic)
		{
			(base.transform as RectTransform).localScale = graphic.Scale;
		}

		internal override void Alignment(Alignment alignment, AdvGraphicInfo graphic)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			rectTransform.pivot = graphic.Pivot;
			if (alignment == Utage.Alignment.None)
			{
				rectTransform.anchoredPosition = graphic.Position;
				Vector3 localPosition = rectTransform.localPosition;
				localPosition.z = graphic.Position.z;
				rectTransform.localPosition = localPosition;
				return;
			}
			Vector2 alignmentValue = AlignmentUtil.GetAlignmentValue(alignment);
			Vector2 anchorMin = (rectTransform.anchorMax = alignmentValue);
			rectTransform.anchorMin = anchorMin;
			Vector3 vector2 = rectTransform.pivot - alignmentValue;
			vector2.Scale(rectTransform.GetSizeScaled());
			Vector3 vector3 = graphic.Position + vector2;
			rectTransform.anchoredPosition = vector3;
			Vector3 localPosition2 = rectTransform.localPosition;
			localPosition2.z = vector3.z;
			rectTransform.localPosition = localPosition2;
		}
	}
}
