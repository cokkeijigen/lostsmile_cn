using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/UiMessageWindow")]
	public class AdvUguiMessageWindow : MonoBehaviour, IAdvMessageWindow
	{
		protected enum ReadColorMode
		{
			None,
			Change
		}

		[SerializeField]
		protected AdvEngine engine;

		[SerializeField]
		protected ReadColorMode readColorMode;

		[SerializeField]
		protected Color readColor = new Color(0.8f, 0.8f, 0.8f);

		protected Color defaultTextColor = Color.white;

		protected Color defaultNameTextColor = Color.white;

		[SerializeField]
		private UguiNovelText text;

		[SerializeField]
		protected Text nameText;

		[SerializeField]
		protected GameObject rootChildren;

		[SerializeField]
		[FormerlySerializedAs("transrateMessageWindowRoot")]
		protected CanvasGroup translateMessageWindowRoot;

		[SerializeField]
		protected GameObject iconWaitInput;

		[SerializeField]
		protected GameObject iconBrPage;

		[SerializeField]
		protected bool isLinkPositionIconBrPage = true;

		public AdvEngine Engine => engine ?? (engine = GetComponent<AdvEngine>());

		public UguiNovelText Text => text;

		public bool IsCurrent { get; protected set; }

		public virtual void OnInit(AdvMessageWindowManager windowManager)
		{
			defaultTextColor = text.color;
			if ((bool)nameText)
			{
				defaultNameTextColor = nameText.color;
			}
			Clear();
		}

		protected virtual void Clear()
		{
			text.text = "";
			text.LengthOfView = 0;
			if ((bool)nameText)
			{
				nameText.text = "";
			}
			if ((bool)iconWaitInput)
			{
				iconWaitInput.SetActive(false);
			}
			if ((bool)iconBrPage)
			{
				iconBrPage.SetActive(false);
			}
			rootChildren.SetActive(false);
		}

		public virtual void OnReset()
		{
			Clear();
		}

		public virtual void OnChangeCurrent(bool isCurrent)
		{
			IsCurrent = isCurrent;
		}

		public virtual void OnChangeActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
			if (!isActive)
			{
				Clear();
			}
			else
			{
				rootChildren.SetActive(true);
			}
		}

		public virtual void OnTextChanged(AdvMessageWindow window)
		{
			if ((bool)text)
			{
				text.text = "";
				text.text = window.Text.OriginalText;
				text.LengthOfView = window.TextLength;
			}
			if ((bool)nameText)
			{
				nameText.text = "";
				nameText.text = window.NameText;
			}
			ReadColorMode readColorMode = this.readColorMode;
			if (readColorMode != 0 && readColorMode == ReadColorMode.Change)
			{
				text.color = (Engine.Page.CheckReadPage() ? readColor : defaultTextColor);
				if ((bool)nameText)
				{
					nameText.color = (Engine.Page.CheckReadPage() ? readColor : defaultNameTextColor);
				}
			}
			LinkIcon();
		}

		protected virtual void Awake()
		{
			if (!rootChildren.activeSelf)
			{
				rootChildren.SetActive(true);
				rootChildren.SetActive(false);
			}
		}

		protected virtual void LateUpdate()
		{
			if (Engine.UiManager.Status == AdvUiManager.UiStatus.Default)
			{
				rootChildren.SetActive(Engine.UiManager.IsShowingMessageWindow);
				if (Engine.UiManager.IsShowingMessageWindow)
				{
					translateMessageWindowRoot.alpha = Engine.Config.MessageWindowAlpha;
				}
			}
			UpdateCurrent();
		}

		protected virtual void UpdateCurrent()
		{
			if (IsCurrent && Engine.UiManager.Status == AdvUiManager.UiStatus.Default)
			{
				if (Engine.UiManager.IsShowingMessageWindow)
				{
					text.LengthOfView = Engine.Page.CurrentTextLength;
				}
				LinkIcon();
			}
		}

		protected virtual void LinkIcon()
		{
			if (iconWaitInput == null)
			{
				LinkIconSub(iconBrPage, Engine.Page.IsWaitInputInPage || Engine.Page.IsWaitBrPage);
				return;
			}
			LinkIconSub(iconWaitInput, Engine.Page.IsWaitInputInPage);
			LinkIconSub(iconBrPage, Engine.Page.IsWaitBrPage);
		}

		protected virtual void LinkIconSub(GameObject icon, bool isActive)
		{
			if (icon == null)
			{
				return;
			}
			if (!Engine.UiManager.IsShowingMessageWindow)
			{
				icon.SetActive(false);
				return;
			}
			icon.SetActive(isActive);
			if (isLinkPositionIconBrPage)
			{
				icon.transform.localPosition = text.CurrentEndPosition;
			}
		}

		public virtual void OnTapCloseWindow()
		{
			Engine.UiManager.Status = AdvUiManager.UiStatus.HideMessageWindow;
		}

		public virtual void OnTapBackLog()
		{
			Engine.UiManager.Status = AdvUiManager.UiStatus.Backlog;
		}

	}
}
