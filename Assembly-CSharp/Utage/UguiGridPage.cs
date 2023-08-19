using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/GridPages")]
	public class UguiGridPage : MonoBehaviour
	{
		public GridLayoutGroup grid;

		public GameObject itemPrefab;

		public UguiToggleGroupIndexed pageCarouselToggles;

		public UguiAlignGroup pageCarouselAlignGroup;

		public GameObject pageCarouselPrefab;

		private int maxItemPerPage = -1;

		private int maxItemNum;

		private int currentPage;

		private List<GameObject> items = new List<GameObject>();

		private Action<GameObject, int> CallbackCreateItem;

		public int MaxItemPerPage
		{
			get
			{
				if (maxItemPerPage < 0)
				{
					Rect rect = (grid.transform as RectTransform).rect;
					int num = GetCellCount(grid.cellSize.x, rect.size.x, grid.spacing.x);
					int num2 = GetCellCount(grid.cellSize.y, rect.size.y, grid.spacing.y);
					switch (grid.constraint)
					{
					case GridLayoutGroup.Constraint.FixedColumnCount:
						num = Mathf.Min(num, grid.constraintCount);
						break;
					case GridLayoutGroup.Constraint.FixedRowCount:
						num2 = Mathf.Min(num2, grid.constraintCount);
						break;
					}
					maxItemPerPage = num * num2;
				}
				return maxItemPerPage;
			}
		}

		public int CurrentPage
		{
			get
			{
				return currentPage;
			}
		}

		public int MaxPage
		{
			get
			{
				return (maxItemNum - 1) / MaxItemPerPage;
			}
		}

		public int NextPage
		{
			get
			{
				return Mathf.Min(CurrentPage + 1, MaxPage);
			}
		}

		public int PrevPage
		{
			get
			{
				return Mathf.Max(CurrentPage - 1, 0);
			}
		}

		public List<GameObject> Items
		{
			get
			{
				return items;
			}
		}

		private int GetCellCount(float cellSize, float rectSize, float space)
		{
			int num = 0;
			float num2 = 0f;
			while (true)
			{
				num2 += cellSize;
				if (num2 > rectSize)
				{
					break;
				}
				num++;
				num2 += space;
			}
			return num;
		}

		public void Init(int maxItemNum, Action<GameObject, int> callbackCreateItem)
		{
			this.maxItemNum = maxItemNum;
			CallbackCreateItem = callbackCreateItem;
			if (!pageCarouselToggles)
			{
				return;
			}
			pageCarouselToggles.ClearToggles();
			pageCarouselAlignGroup.DestroyAllChildren();
			if (MaxPage > 0)
			{
				foreach (GameObject item in pageCarouselAlignGroup.AddChildrenFromPrefab(MaxPage + 1, pageCarouselPrefab, null))
				{
					pageCarouselToggles.Add(item.GetComponent<Toggle>());
				}
				pageCarouselToggles.OnValueChanged.AddListener(CreateItems);
				pageCarouselToggles.CurrentIndex = 0;
				pageCarouselToggles.SetActiveLRButtons(true);
			}
			else
			{
				pageCarouselToggles.SetActiveLRButtons(false);
			}
		}

		public void CreateItems(int page)
		{
			currentPage = page;
			pageCarouselToggles.CurrentIndex = page;
			ClearItems();
			int num = MaxItemPerPage * CurrentPage;
			for (int i = 0; i < MaxItemPerPage; i++)
			{
				int num2 = num + i;
				if (num2 < maxItemNum)
				{
					GameObject gameObject = grid.transform.AddChildPrefab(itemPrefab);
					items.Add(gameObject);
					if (CallbackCreateItem != null)
					{
						CallbackCreateItem(gameObject, num2);
					}
					continue;
				}
				break;
			}
		}

		public void ClearItems()
		{
			grid.transform.DestroyChildren();
		}

		public void OnClickNextPage()
		{
			int nextPage = NextPage;
			if (nextPage != CurrentPage)
			{
				CreateItems(nextPage);
			}
		}

		public void OnClickPrevPage()
		{
			int prevPage = PrevPage;
			if (prevPage != CurrentPage)
			{
				CreateItems(prevPage);
			}
		}
	}
}
