using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utage
{
	[RequireComponent(typeof(CanvasGroup))]
	public abstract class UguiView : MonoBehaviour
	{
		public enum Status
		{
			None,
			Opening,
			Opened,
			Closing,
			Closed
		}

		[SerializeField]
		protected UguiView prevView;

		[SerializeField]
		protected AudioClip bgm;

		[SerializeField]
		private bool isStopBgmIfNoneBgm;

		public UnityEvent onOpen;

		public UnityEvent onClose;

		private CanvasGroup canvasGroup;

		protected Status status;

		protected bool storedCanvasGroupInteractable;

		protected bool storedCanvasGroupBlocksRaycasts;

		protected bool isStoredCanvasGroupInfo;

		public AudioClip Bgm
		{
			get
			{
				return bgm;
			}
			set
			{
				bgm = value;
			}
		}

		public bool IsStopBgmIfNoneBgm
		{
			get
			{
				return isStopBgmIfNoneBgm;
			}
			set
			{
				isStopBgmIfNoneBgm = value;
			}
		}

		public CanvasGroup CanvasGroup
		{
			get
			{
				return canvasGroup ?? (canvasGroup = GetComponent<CanvasGroup>());
			}
		}

		public virtual void Open()
		{
			Open(prevView);
		}

		public virtual void Open(UguiView prevView)
		{
			if (status == Status.Closing)
			{
				CancelClosing();
			}
			status = Status.Opening;
			this.prevView = prevView;
			base.gameObject.SetActive(true);
			ChangeBgm();
			base.gameObject.SendMessage("OnOpen", SendMessageOptions.DontRequireReceiver);
			onOpen.Invoke();
			StartCoroutine(CoOpening());
		}

		protected virtual IEnumerator CoOpening()
		{
			ITransition[] transitions = base.gameObject.GetComponents<ITransition>();
			ITransition[] array = transitions;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Open();
			}
			while (!Array.TrueForAll(transitions, (ITransition item) => !item.IsPlaying))
			{
				yield return null;
			}
			RestoreCanvasGroupInput();
			status = Status.Opened;
			base.gameObject.SendMessage("OnEndOpen", SendMessageOptions.DontRequireReceiver);
		}

		public virtual void Close()
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SendMessage("OnBeginClose", SendMessageOptions.DontRequireReceiver);
				StartCoroutine(CoClosing());
			}
		}

		protected virtual IEnumerator CoClosing()
		{
			status = Status.Closing;
			StoreAndChangeCanvasGroupInput(true);
			ITransition[] transitions = base.gameObject.GetComponents<ITransition>();
			ITransition[] array = transitions;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Close();
			}
			while (!Array.TrueForAll(transitions, (ITransition item) => !item.IsPlaying))
			{
				yield return null;
			}
			RestoreCanvasGroupInput();
			EndClose();
		}

		protected virtual void CancelClosing()
		{
			ITransition[] components = base.gameObject.GetComponents<ITransition>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].CancelClosing();
			}
			RestoreCanvasGroupInput();
			EndClose();
		}

		protected virtual void EndClose()
		{
			base.gameObject.SendMessage("OnClose", SendMessageOptions.DontRequireReceiver);
			onClose.Invoke();
			base.gameObject.SetActive(false);
			status = Status.Closed;
		}

		protected void StoreAndChangeCanvasGroupInput(bool isActive)
		{
			storedCanvasGroupInteractable = CanvasGroup.interactable;
			storedCanvasGroupBlocksRaycasts = CanvasGroup.blocksRaycasts;
			CanvasGroup.interactable = false;
			CanvasGroup.blocksRaycasts = false;
			isStoredCanvasGroupInfo = true;
		}

		protected void RestoreCanvasGroupInput()
		{
			if (isStoredCanvasGroupInfo)
			{
				CanvasGroup.interactable = storedCanvasGroupInteractable;
				CanvasGroup.blocksRaycasts = storedCanvasGroupBlocksRaycasts;
				isStoredCanvasGroupInfo = false;
			}
		}

		public virtual void ToggleOpen(bool isOpen)
		{
			if (isOpen)
			{
				Open();
			}
			else
			{
				Close();
			}
		}

		public virtual void Back()
		{
			Close();
			if (null != prevView)
			{
				prevView.Open(prevView.prevView);
			}
		}

		public virtual void OnTapBack()
		{
			Back();
		}

		protected virtual void ChangeBgm()
		{
			if ((bool)Bgm)
			{
				if ((bool)SoundManager.GetInstance())
				{
					SoundManager.GetInstance().PlayBgm(bgm, true);
				}
			}
			else if (IsStopBgmIfNoneBgm && (bool)SoundManager.GetInstance())
			{
				SoundManager.GetInstance().StopBgm();
			}
		}
	}
}
