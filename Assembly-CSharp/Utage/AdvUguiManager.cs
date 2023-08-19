using UnityEngine;
using UnityEngine.EventSystems;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/UiManager")]
	public class AdvUguiManager : AdvUiManager
	{
		[SerializeField]
		protected AdvUguiMessageWindowManager messageWindow;

		[SerializeField]
		protected AdvUguiSelectionManager selection;

		[SerializeField]
		protected AdvUguiBacklogManager backLog;

		[SerializeField]
		protected bool disableMouseWheelBackLog;

		public AdvUguiMessageWindowManager MessageWindow
		{
			get
			{
				return messageWindow ?? (messageWindow = GetMessageWindowManagerCreateIfMissing());
			}
		}

		public override void Open()
		{
			base.gameObject.SetActive(true);
			ChangeStatus(UiStatus.Default);
		}

		public override void Close()
		{
			base.gameObject.SetActive(false);
			MessageWindow.Close();
			if (selection != null)
			{
				selection.Close();
			}
			if (backLog != null)
			{
				backLog.Close();
			}
		}

		protected override void ChangeStatus(UiStatus newStatus)
		{
			switch (newStatus)
			{
			case UiStatus.Backlog:
				if (backLog == null)
				{
					return;
				}
				MessageWindow.Close();
				if (selection != null)
				{
					selection.Close();
				}
				if (backLog != null)
				{
					backLog.Open();
				}
				base.Engine.Config.IsSkip = false;
				break;
			case UiStatus.HideMessageWindow:
				MessageWindow.Close();
				if (selection != null)
				{
					selection.Close();
				}
				if (backLog != null)
				{
					backLog.Close();
				}
				base.Engine.Config.IsSkip = false;
				break;
			case UiStatus.Default:
				MessageWindow.Open();
				if (selection != null)
				{
					selection.Open();
				}
				if (backLog != null)
				{
					backLog.Close();
				}
				break;
			}
			status = newStatus;
		}

		protected virtual void OnTapCloseWindow()
		{
			base.Status = UiStatus.HideMessageWindow;
		}

		protected virtual void Update()
		{
			bool flag = (base.Engine.Config.IsMouseWheelSendMessage && InputUtil.IsInputScrollWheelDown()) || InputUtil.IsInputKeyboadReturnDown();
			switch (base.Status)
			{
			case UiStatus.HideMessageWindow:
				if (InputUtil.IsMouseRightButtonDown())
				{
					base.Status = UiStatus.Default;
				}
				else if (!disableMouseWheelBackLog && InputUtil.IsInputScrollWheelUp())
				{
					base.Status = UiStatus.Backlog;
				}
				break;
			case UiStatus.Default:
				if (base.IsShowingMessageWindow)
				{
					base.Engine.Page.UpdateText();
				}
				if (base.IsShowingMessageWindow || base.Engine.SelectionManager.IsWaitInput)
				{
					if (InputUtil.IsMouseRightButtonDown())
					{
						base.Status = UiStatus.HideMessageWindow;
					}
					else if (!disableMouseWheelBackLog && InputUtil.IsInputScrollWheelUp())
					{
						base.Status = UiStatus.Backlog;
					}
					else if (flag)
					{
						base.Engine.Page.InputSendMessage();
						base.IsInputTrig = true;
					}
				}
				else if (flag)
				{
					base.IsInputTrig = false;
				}
				break;
			case UiStatus.Backlog:
				break;
			}
		}

		public virtual void OnPointerDown(BaseEventData data)
		{
			if (data == null || !(data is PointerEventData) || (data as PointerEventData).button == PointerEventData.InputButton.Left)
			{
				OnInput(data);
			}
		}

		public virtual void OnInput(BaseEventData data = null)
		{
			switch (base.Status)
			{
			case UiStatus.HideMessageWindow:
				base.Status = UiStatus.Default;
				break;
			case UiStatus.Default:
				if (base.Engine.Config.IsSkip)
				{
					base.Engine.Config.ToggleSkip();
					break;
				}
				if (base.IsShowingMessageWindow && !base.Engine.Config.IsSkip)
				{
					base.Engine.Page.InputSendMessage();
				}
				if (data != null && data is PointerEventData)
				{
					base.OnPointerDown(data as PointerEventData);
				}
				break;
			case UiStatus.Backlog:
				break;
			}
		}

		public AdvUguiMessageWindowManager GetMessageWindowManagerCreateIfMissing()
		{
			AdvUguiMessageWindowManager[] componentsInChildren = GetComponentsInChildren<AdvUguiMessageWindowManager>(true);
			if (componentsInChildren.Length != 0)
			{
				return componentsInChildren[0];
			}
			AdvUguiMessageWindowManager advUguiMessageWindowManager = base.transform.AddChildGameObjectComponent<AdvUguiMessageWindowManager>("MessageWindowManager");
			RectTransform rectTransform = advUguiMessageWindowManager.gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.SetAsFirstSibling();
			AdvUguiMessageWindow[] componentsInChildren2 = GetComponentsInChildren<AdvUguiMessageWindow>(true);
			foreach (AdvUguiMessageWindow advUguiMessageWindow in componentsInChildren2)
			{
				advUguiMessageWindow.transform.SetParent(rectTransform, true);
				if (advUguiMessageWindow.transform.localScale == Vector3.zero)
				{
					advUguiMessageWindow.transform.localScale = Vector3.one;
				}
			}
			return advUguiMessageWindowManager;
		}
	}
}
