using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[ExecuteInEditMode]
	public abstract class UguiAlignGroup : UguiLayoutControllerBase, ILayoutController
	{
		public bool isAutoResize;

		public float space;

		public void SetLayoutHorizontal()
		{
			tracker.Clear();
			Reposition();
		}

		public void SetLayoutVertical()
		{
			tracker.Clear();
			Reposition();
		}

		public List<GameObject> AddChildrenFromPrefab(int count, GameObject prefab, Action<GameObject, int> callcackCreateItem)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = base.CachedRectTransform.AddChildPrefab(prefab);
				list.Add(gameObject);
				callcackCreateItem?.Invoke(gameObject, i);
			}
			return list;
		}

		public void DestroyAllChildren()
		{
			base.CachedRectTransform.DestroyChildren();
		}

		public abstract void Reposition();
	}
}
