using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/UiSelectionManager")]
	public class AdvUguiSelectionManager : MonoBehaviour
	{
		protected enum SelectedColorMode
		{
			None,
			Change
		}

		[SerializeField]
		protected AdvEngine engine;

		[SerializeField]
		protected SelectedColorMode selectedColorMode;

		[SerializeField]
		protected Color selectedColor = new Color(0.8f, 0.8f, 0.8f);

		[SerializeField]
		protected List<GameObject> prefabList;

		private UguiListView listView;

		private CanvasGroup canvasGroup;

		private List<GameObject> items = new List<GameObject>();

		public AdvEngine Engine
		{
			get
			{
				return engine ?? (engine = GetComponent<AdvEngine>());
			}
		}

		protected List<GameObject> PrefabList
		{
			get
			{
				return prefabList;
			}
		}

		protected AdvSelectionManager SelectionManager
		{
			get
			{
				return engine.SelectionManager;
			}
		}

		public UguiListView ListView
		{
			get
			{
				return listView ?? (listView = GetComponent<UguiListView>());
			}
		}

		private CanvasGroup CanvasGroup
		{
			get
			{
				return base.gameObject.GetComponentCacheCreateIfMissing(ref canvasGroup);
			}
		}

		public List<GameObject> Items
		{
			get
			{
				return items;
			}
		}

		public virtual void Open()
		{
			base.gameObject.SetActive(true);
		}

		public virtual void Close()
		{
			base.gameObject.SetActive(false);
		}

		protected virtual void Awake()
		{
			SelectionManager.OnClear.AddListener(OnClear);
			SelectionManager.OnBeginShow.AddListener(OnBeginShow);
			SelectionManager.OnBeginWaitInput.AddListener(OnBeginWaitInput);
			ClearAll();
		}

		protected virtual void ClearAll()
		{
			ListView.ClearItems();
			foreach (GameObject item in Items)
			{
				Object.Destroy(item);
			}
			Items.Clear();
		}

		protected virtual void CreateItems()
		{
			ClearAll();
			List<GameObject> list = new List<GameObject>();
			foreach (AdvSelection selection in SelectionManager.Selections)
			{
				GameObject gameObject = Object.Instantiate(GetPrefab(selection));
				AdvUguiSelection componentInChildren = gameObject.GetComponentInChildren<AdvUguiSelection>();
				if ((bool)componentInChildren)
				{
					componentInChildren.Init(selection, OnTap);
				}
				SelectedColorMode selectedColorMode = this.selectedColorMode;
				if (selectedColorMode != 0 && selectedColorMode == SelectedColorMode.Change && Engine.SystemSaveData.SelectionData.Check(selection))
				{
					gameObject.SendMessage("OnInitSelected", selectedColor);
				}
				Items.Add(gameObject);
				if (!selection.X.HasValue || !selection.Y.HasValue)
				{
					list.Add(gameObject);
				}
				else
				{
					base.transform.AddChild(gameObject, new Vector3(selection.X.Value, selection.Y.Value, 0f));
				}
			}
			ListView.AddItems(list);
			ListView.Reposition();
		}

		protected virtual GameObject GetPrefab(AdvSelection selectionData)
		{
			GameObject gameObject = null;
			if (!string.IsNullOrEmpty(selectionData.PrefabName))
			{
				gameObject = PrefabList.Find((GameObject x) => x.name == selectionData.PrefabName);
				if (gameObject != null)
				{
					return gameObject;
				}
				Debug.LogError("Not found Selection Prefab : " + selectionData.PrefabName);
			}
			return (PrefabList.Count > 0) ? PrefabList[0] : ListView.ItemPrefab;
		}

		protected virtual void CallbackCreateItem(GameObject go, int index)
		{
			AdvSelection data = SelectionManager.Selections[index];
			go.GetComponentInChildren<AdvUguiSelection>().Init(data, OnTap);
		}

		protected virtual void OnTap(AdvUguiSelection item)
		{
			SelectionManager.Select(item.Data);
			ClearAll();
		}

		public virtual void OnClear(AdvSelectionManager manager)
		{
			ClearAll();
		}

		public virtual void OnBeginShow(AdvSelectionManager manager)
		{
			CreateItems();
			CanvasGroup.interactable = false;
		}

		public virtual void OnBeginWaitInput(AdvSelectionManager manager)
		{
			CanvasGroup.interactable = true;
		}
	}
}
