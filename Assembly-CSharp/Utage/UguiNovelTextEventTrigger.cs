using UnityEngine;
using UnityEngine.EventSystems;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/NovelTextEventTrigger")]
	[RequireComponent(typeof(UguiNovelText))]
	public class UguiNovelTextEventTrigger : MonoBehaviour, ICanvasRaycastFilter, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
	{
		private UguiNovelTextGenerator generator;

		private UguiNovelText novelText;

		private RectTransform cachedRectTransform;

		public OnClickLinkEvent OnClick = new OnClickLinkEvent();

		public Color hoverColor = ColorUtil.Red;

		private UguiNovelTextHitArea currentTarget;

		private bool isEntered;

		public UguiNovelTextGenerator Generator
		{
			get
			{
				return generator ?? (generator = GetComponent<UguiNovelTextGenerator>());
			}
		}

		public UguiNovelText NovelText
		{
			get
			{
				return novelText ?? (novelText = GetComponent<UguiNovelText>());
			}
		}

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

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			UguiNovelTextHitArea uguiNovelTextHitArea = HitTest(sp, eventCamera);
			if (isEntered)
			{
				ChangeCurrentTarget(uguiNovelTextHitArea);
			}
			return uguiNovelTextHitArea != null;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			UguiNovelTextHitArea uguiNovelTextHitArea = HitTest(eventData);
			if (uguiNovelTextHitArea != null)
			{
				OnClick.Invoke(uguiNovelTextHitArea);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			isEntered = true;
			ChangeCurrentTarget(HitTest(eventData));
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			isEntered = false;
			ChangeCurrentTarget(null);
		}

		private UguiNovelTextHitArea HitTest(PointerEventData eventData)
		{
			return HitTest(eventData.position, eventData.pressEventCamera);
		}

		private UguiNovelTextHitArea HitTest(Vector2 screenPoint, Camera cam)
		{
			Vector2 localPoint;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(CachedRectTransform, screenPoint, cam, out localPoint);
			foreach (UguiNovelTextHitArea hitGroupList in Generator.HitGroupLists)
			{
				if (hitGroupList.HitTest(localPoint))
				{
					return hitGroupList;
				}
			}
			return null;
		}

		private void ChangeCurrentTarget(UguiNovelTextHitArea target)
		{
			if (currentTarget != target)
			{
				if (currentTarget != null)
				{
					currentTarget.ResetEffectColor();
				}
				currentTarget = target;
				if (currentTarget != null)
				{
					currentTarget.ChangeEffectColor(hoverColor);
				}
			}
		}

		private void OnDrawGizmos()
		{
			foreach (UguiNovelTextHitArea hitGroupList in Generator.HitGroupLists)
			{
				foreach (Rect hitArea in hitGroupList.HitAreaList)
				{
					Gizmos.color = Color.yellow;
					Vector3 position = hitArea.center;
					Vector3 vector = hitArea.size;
					position = CachedRectTransform.TransformPoint(position);
					vector = CachedRectTransform.TransformVector(vector);
					Gizmos.DrawWireCube(position, vector);
				}
			}
		}
	}
}
