using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/UiBacklogManager")]
	public class AdvUguiBacklogManager : MonoBehaviour
	{
		public enum BacklogType
		{
			MessageWindow,
			FullScreenText
		}

		[SerializeField]
		private BacklogType type;

		[SerializeField]
		protected AdvEngine engine;

		[SerializeField]
		private UguiListView listView;

		[SerializeField]
		private UguiNovelText fullScreenLogText;

		public bool isCloseScrollWheelDown;

		private BacklogType Type => type;

		public UguiListView ListView => listView;

		public UguiNovelText FullScreenLogText => fullScreenLogText;

		protected AdvBacklogManager BacklogManager => engine.BacklogManager;

		public virtual bool IsOpen => base.gameObject.activeSelf;

		public virtual void Close()
		{
			if (ListView != null)
			{
				ListView.ClearItems();
			}
			if (FullScreenLogText != null)
			{
				FullScreenLogText.text = "";
			}
			base.gameObject.SetActive(false);
		}

		public virtual void Open()
		{
			base.gameObject.SetActive(true);
			BacklogType backlogType = Type;
			if (backlogType != 0 && backlogType == BacklogType.FullScreenText)
			{
				InitialzeAsFullScreenText();
			}
			else
			{
				InitialzeAsMessageWindow();
			}
		}

		protected virtual void InitialzeAsMessageWindow()
		{
			ListView.CreateItems(BacklogManager.Backlogs.Count, CallbackCreateItem);
		}

		protected virtual void InitialzeAsFullScreenText()
		{
			ListView.CreateItems(BacklogManager.Backlogs.Count, CallbackCreateItem);
		}

		protected virtual void CallbackCreateItem(GameObject go, int index)
		{
			AdvBacklog data = BacklogManager.Backlogs[BacklogManager.Backlogs.Count - index - 1];
			go.GetComponent<AdvUguiBacklog>().Init(data);
		}

		public void OnTapBack()
		{
			Back();
		}

		protected virtual void Update()
		{
			if (InputUtil.IsMouseRightButtonDown() || IsInputBottomEndScrollWheelDown())
			{
				Back();
			}
		}

		protected virtual void Back()
		{
			Close();
			engine.UiManager.Status = AdvUiManager.UiStatus.Default;
		}

		protected virtual bool IsInputBottomEndScrollWheelDown()
		{
			if (isCloseScrollWheelDown && InputUtil.IsInputScrollWheelDown())
			{
				Scrollbar verticalScrollbar = ListView.ScrollRect.verticalScrollbar;
				if ((bool)verticalScrollbar)
				{
					return verticalScrollbar.value <= 0f;
				}
			}
			return false;
		}
	}
}
