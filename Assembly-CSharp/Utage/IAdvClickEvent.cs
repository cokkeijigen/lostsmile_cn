using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utage
{
	internal interface IAdvClickEvent
	{
		GameObject gameObject { get; }

		void AddClickEvent(bool isPolygon, StringGridRow row, UnityAction<BaseEventData> action);

		void RemoveClickEvent();
	}
}
