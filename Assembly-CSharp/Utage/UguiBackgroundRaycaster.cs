using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/BackgroundRaycaster ")]
	[RequireComponent(typeof(Camera))]
	public class UguiBackgroundRaycaster : BaseRaycaster
	{
		private Camera cachedCamera;

		[SerializeField]
		private LetterBoxCamera letterBoxCamera;

		[SerializeField]
		private int m_Priority = int.MaxValue;

		[NonSerialized]
		private List<GameObject> targetObjectList = new List<GameObject>();

		public override Camera eventCamera
		{
			get
			{
				return CachedCamera;
			}
		}

		private Camera CachedCamera
		{
			get
			{
				return cachedCamera ?? (cachedCamera = GetComponent<Camera>());
			}
		}

		public override int sortOrderPriority
		{
			get
			{
				return m_Priority;
			}
		}

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			Vector2 vector = ((!(letterBoxCamera == null)) ? ((Vector2)letterBoxCamera.CachedCamera.ScreenToViewportPoint(eventData.position)) : new Vector2(eventData.position.x / (float)Screen.width, eventData.position.y / (float)Screen.height));
			if (vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
			{
				return;
			}
			int num = 0;
			foreach (GameObject targetObject in targetObjectList)
			{
				RaycastResult item = default(RaycastResult);
				item.distance = float.MaxValue;
				item.gameObject = targetObject;
				item.index = num++;
				item.module = this;
				resultAppendList.Add(item);
			}
		}

		public void AddTarget(GameObject go)
		{
			if (!targetObjectList.Contains(go))
			{
				targetObjectList.Add(go);
			}
		}

		public void RemoveTarget(GameObject go)
		{
			if (targetObjectList.Contains(go))
			{
				targetObjectList.Remove(go);
			}
		}
	}
}
