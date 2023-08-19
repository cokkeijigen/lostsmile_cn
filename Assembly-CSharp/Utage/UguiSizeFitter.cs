using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/SizeFitter")]
	public class UguiSizeFitter : UguiLayoutControllerBase, ILayoutSelfController, ILayoutController
	{
		public RectTransform target;

		protected override void Update()
		{
			if (!(target == null) && target.rect.size != base.CachedRectTransform.rect.size)
			{
				SetDirty();
			}
		}

		public void SetLayoutHorizontal()
		{
			tracker.Clear();
			if (!(target == null))
			{
				tracker.Add(this, base.CachedRectTransform, DrivenTransformProperties.SizeDelta);
				base.CachedRectTransform.sizeDelta = target.sizeDelta;
			}
		}

		public void SetLayoutVertical()
		{
		}
	}
}
