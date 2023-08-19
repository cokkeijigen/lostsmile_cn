using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/CategoryGridPage")]
	public class UguiCategoryGridPage : MonoBehaviour
	{
		public UguiGridPage gridPage;

		public UguiToggleGroupIndexed categoryToggleGroup;

		public UguiAlignGroup categoryAlignGroup;

		public GameObject categoryPrefab;

		public List<Sprite> buttonSpriteList;

		private string[] categoryList;

		public string CurrentCategory
		{
			get
			{
				if (categoryList == null)
				{
					return "";
				}
				if (categoryToggleGroup.CurrentIndex >= categoryList.Length)
				{
					return "";
				}
				return categoryList[categoryToggleGroup.CurrentIndex];
			}
		}

		public void Clear()
		{
			categoryToggleGroup.ClearToggles();
			categoryAlignGroup.DestroyAllChildren();
			gridPage.ClearItems();
		}

		public void Init(string[] categoryList, Action<UguiCategoryGridPage> OpenCurrentCategory)
		{
			this.categoryList = categoryList;
			categoryToggleGroup.ClearToggles();
			categoryAlignGroup.DestroyAllChildren();
			if (categoryList.Length > 1)
			{
				foreach (GameObject item in categoryAlignGroup.AddChildrenFromPrefab(categoryList.Length, categoryPrefab, CreateTabButton))
				{
					categoryToggleGroup.Add(item.GetComponent<Toggle>());
				}
				categoryToggleGroup.CurrentIndex = 0;
			}
			categoryToggleGroup.OnValueChanged.AddListener(delegate
			{
				OpenCurrentCategory(this);
			});
			OpenCurrentCategory(this);
		}

		private void CreateTabButton(GameObject go, int index)
		{
			Text componentInChildren = go.GetComponentInChildren<Text>();
			if ((bool)componentInChildren && index < categoryList.Length)
			{
				componentInChildren.text = categoryList[index];
			}
			Image componentInChildren2 = go.GetComponentInChildren<Image>();
			if ((bool)componentInChildren2 && index < buttonSpriteList.Count)
			{
				componentInChildren2.sprite = buttonSpriteList[index];
			}
		}

		public void OpenCurrentCategory(int itemCount, Action<GameObject, int> CreateItem)
		{
			gridPage.Init(itemCount, CreateItem);
			gridPage.CreateItems(0);
		}
	}
}
