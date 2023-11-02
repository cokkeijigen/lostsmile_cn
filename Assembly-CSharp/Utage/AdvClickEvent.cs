using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/ClickEvent")]
	internal class AdvClickEvent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IAdvClickEvent
	{
		private AdvGraphicBase advGraphic;

		private AdvGraphicBase AdvGraphic => this.GetComponentCache(ref advGraphic);

		private StringGridRow Row { get; set; }

		private UnityAction<BaseEventData> action { get; set; }

		private void Awake()
		{
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (action != null)
			{
				action(eventData);
			}
		}

		public virtual void AddClickEvent(bool isPolygon, StringGridRow row, UnityAction<BaseEventData> action)
		{
			Row = row;
			this.action = action;
			SetEnableCanvasRaycaster(true);
		}

		public virtual void RemoveClickEvent()
		{
			Row = null;
			action = null;
			SetEnableCanvasRaycaster(false);
		}

		private void SetEnableCanvasRaycaster(bool enable)
		{
			Canvas componentInParent = GetComponentInParent<Canvas>();
			if (!(componentInParent == null))
			{
				componentInParent.GetComponentCreateIfMissing<GraphicRaycaster>().enabled = enable;
			}
		}

	}
}
