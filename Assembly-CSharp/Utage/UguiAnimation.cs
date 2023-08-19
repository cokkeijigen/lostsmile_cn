using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utage
{
	public abstract class UguiAnimation : CurveAnimation, IBeginDragHandler, IEventSystemHandler, ICancelHandler, IDeselectHandler, IDragHandler, IDropHandler, IEndDragHandler, IInitializePotentialDragHandler, IMoveHandler, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IScrollHandler, ISelectHandler, ISubmitHandler, IUpdateSelectedHandler
	{
		public enum AnimationType
		{
			To,
			From,
			FromTo,
			By
		}

		[SerializeField]
		[EnumFlags]
		private UiEventMask eventMask = UiEventMask.PointerClick;

		[SerializeField]
		private AnimationType animationType;

		[SerializeField]
		private Graphic targetGraphic;

		public UiEventMask EventMask
		{
			get
			{
				return eventMask;
			}
			set
			{
				eventMask = value;
			}
		}

		public AnimationType Type
		{
			get
			{
				return animationType;
			}
			set
			{
				animationType = value;
			}
		}

		public Graphic TargetGraphic
		{
			get
			{
				return targetGraphic;
			}
			set
			{
				targetGraphic = value;
			}
		}

		protected void Reset()
		{
			targetGraphic = GetComponent<Graphic>();
		}

		public void Play()
		{
			Play(null);
		}

		public void Play(Action onComplete)
		{
			StartAnimation();
			PlayAnimation(UpdateAnimation, onComplete);
		}

		protected abstract void StartAnimation();

		protected abstract void UpdateAnimation(float value);

		protected virtual bool CheckEventMask(UiEventMask mask)
		{
			return (EventMask & mask) == mask;
		}

		protected virtual void PlayOnEvent(UiEventMask mask)
		{
			if (CheckEventMask(mask))
			{
				Play();
			}
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.BeginDrag);
		}

		public virtual void OnCancel(BaseEventData eventData)
		{
			PlayOnEvent(UiEventMask.Cancel);
		}

		public virtual void OnDeselect(BaseEventData eventData)
		{
			PlayOnEvent(UiEventMask.Deselect);
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.Drag);
		}

		public virtual void OnDrop(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.Drop);
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.EndDrag);
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.InitializePotentialDrag);
		}

		public virtual void OnMove(AxisEventData eventData)
		{
			PlayOnEvent(UiEventMask.Move);
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.PointerClick);
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.PointerDown);
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.PointerEnter);
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.PointerExit);
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.PointerUp);
		}

		public virtual void OnScroll(PointerEventData eventData)
		{
			PlayOnEvent(UiEventMask.Scroll);
		}

		public virtual void OnSelect(BaseEventData eventData)
		{
			PlayOnEvent(UiEventMask.Select);
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			PlayOnEvent(UiEventMask.Submit);
		}

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			PlayOnEvent(UiEventMask.UpdateSelected);
		}
	}
}
