using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[ExecuteInEditMode]
	public abstract class UguiLayoutControllerBase : MonoBehaviour
	{
		private RectTransform cachedRectTransform;

		protected DrivenRectTransformTracker tracker;

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

		protected virtual void OnEnable()
		{
			SetDirty();
		}

		protected virtual void OnDisable()
		{
			tracker.Clear();
		}

		protected void SetDirty()
		{
			if (base.gameObject.activeInHierarchy)
			{
				LayoutRebuilder.MarkLayoutForRebuild(CachedRectTransform);
			}
		}

		protected virtual void Update()
		{
			bool flag = CachedRectTransform.hasChanged;
			if (!flag)
			{
				foreach (RectTransform item in CachedRectTransform)
				{
					if (item.hasChanged)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				SetDirty();
			}
		}
	}
}
