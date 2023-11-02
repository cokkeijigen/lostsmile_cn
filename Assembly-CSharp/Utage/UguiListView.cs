using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[RequireComponent(typeof(ScrollRect))]
	[AddComponentMenu("Utage/Lib/UI/ListView")]
	public class UguiListView : MonoBehaviour
	{
		public enum Type
		{
			Horizontal,
			Vertical
		}

		[SerializeField]
		private Type scrollType;

		[SerializeField]
		private GameObject itemPrefab;

		private RectTransform content;

		[SerializeField]
		private bool isStopScroolWithAllInnner = true;

		[SerializeField]
		private bool isAutoCenteringOnRepostion;

		private UguiAlignGroup positionGroup;

		private ScrollRect scrollRect;

		private RectTransform scrollRectTransform;

		[SerializeField]
		private GameObject minArrow;

		[SerializeField]
		private GameObject maxArrow;

		private List<GameObject> items = new List<GameObject>();

		public Type ScrollType => scrollType;

		public GameObject ItemPrefab
		{
			get
			{
				return itemPrefab;
			}
			set
			{
				itemPrefab = value;
			}
		}

		public RectTransform Content => content ?? (content = ScrollRect.content);

		public bool IsStopScroolWithAllInnner => isStopScroolWithAllInnner;

		public bool IsAutoCenteringOnRepostion => isAutoCenteringOnRepostion;

		public UguiAlignGroup PositionGroup
		{
			get
			{
				if (positionGroup == null)
				{
					positionGroup = Content.GetComponent<UguiAlignGroup>();
					if (positionGroup == null)
					{
						Debug.LogError("AlignGroup Component is not attached on ScrollRect Content");
					}
				}
				return positionGroup;
			}
		}

		public ScrollRect ScrollRect => scrollRect ?? (scrollRect = GetComponent<ScrollRect>());

		public RectTransform ScrollRectTransform => scrollRectTransform ?? (scrollRectTransform = ScrollRect.GetComponent<RectTransform>());

		public GameObject MinArrow
		{
			get
			{
				return minArrow;
			}
			set
			{
				minArrow = value;
			}
		}

		public GameObject MaxArrow
		{
			get
			{
				return maxArrow;
			}
			set
			{
				maxArrow = value;
			}
		}

		public List<GameObject> Items => items;

		public void CreateItems(int itemNum, Action<GameObject, int> callbackCreateItem)
		{
			ClearItems();
			for (int i = 0; i < itemNum; i++)
			{
				GameObject gameObject = Content.AddChildPrefab(ItemPrefab.gameObject);
				items.Add(gameObject);
				callbackCreateItem?.Invoke(gameObject, i);
			}
			Reposition();
		}

		public void AddItems(List<GameObject> items)
		{
			foreach (GameObject item in items)
			{
				Content.AddChild(item);
			}
		}

		public void Reposition()
		{
			Content.anchoredPosition = Vector2.zero;
			ScrollRect.velocity = Vector2.zero;
			PositionGroup.Reposition();
			bool flag = IsContentInnerScrollRect() && IsStopScroolWithAllInnner;
			switch (ScrollType)
			{
			case Type.Horizontal:
				ScrollRect.horizontal = !flag;
				ScrollRect.vertical = false;
				if (isAutoCenteringOnRepostion)
				{
					if (flag)
					{
						float x = (ScrollRectTransform.sizeDelta.x - Content.sizeDelta.x) / 2f;
						Content.anchoredPosition = new Vector3(x, 0f, 0f);
					}
					else
					{
						ScrollRect.horizontalNormalizedPosition = 0.5f;
					}
				}
				break;
			case Type.Vertical:
				ScrollRect.horizontal = false;
				ScrollRect.vertical = !flag;
				if (isAutoCenteringOnRepostion)
				{
					if (flag)
					{
						float y = (0f - (ScrollRectTransform.sizeDelta.y - Content.sizeDelta.y)) / 2f;
						Content.anchoredPosition = new Vector3(0f, y, 0f);
					}
					else
					{
						ScrollRect.verticalNormalizedPosition = 0.5f;
					}
				}
				break;
			}
			ScrollRect.enabled = !flag;
		}

		public void ClearItems()
		{
			items.Clear();
			Content.DestroyChildren();
			ScrollRect.velocity = Vector2.zero;
		}

		private void Update()
		{
			RefreshArrow();
		}

		private void RefreshArrow()
		{
			if (IsContentInnerScrollRect())
			{
				if (null != MinArrow)
				{
					MinArrow.SetActive(false);
				}
				if (null != MaxArrow)
				{
					MaxArrow.SetActive(false);
				}
				return;
			}
			switch (ScrollType)
			{
			case Type.Horizontal:
			{
				float verticalNormalizedPosition = ScrollRect.horizontalNormalizedPosition;
				if (null != MinArrow)
				{
					MinArrow.SetActive(verticalNormalizedPosition > 0f);
				}
				if (null != MaxArrow)
				{
					MaxArrow.SetActive(verticalNormalizedPosition < 1f);
				}
				break;
			}
			case Type.Vertical:
			{
				float verticalNormalizedPosition = ScrollRect.verticalNormalizedPosition;
				if (null != MinArrow)
				{
					MinArrow.SetActive(verticalNormalizedPosition < 1f);
				}
				if (null != MaxArrow)
				{
					MaxArrow.SetActive(verticalNormalizedPosition > 0f);
				}
				break;
			}
			}
		}

		private bool IsContentInnerScrollRect()
		{
			switch (ScrollType)
			{
			case Type.Horizontal:
				return Content.rect.width <= ScrollRectTransform.rect.width;
			case Type.Vertical:
				return Content.rect.height <= ScrollRectTransform.rect.height;
			default:
				return false;
			}
		}
	}
}
