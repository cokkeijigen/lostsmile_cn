using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/System UI/SystemUi")]
	public class SystemUi : MonoBehaviour
	{
		private static SystemUi instance;

		[SerializeField]
		private SystemUiDialog2Button dialogGameExit;

		[SerializeField]
		private SystemUiDialog1Button dialog1Button;

		[SerializeField]
		private SystemUiDialog2Button dialog2Button;

		[SerializeField]
		private SystemUiDialog3Button dialog3Button;

		[SerializeField]
		private IndicatorIcon indicator;

		[SerializeField]
		private bool isEnableInputEscape = true;

		public bool IsEnableInputEscape
		{
			get
			{
				return isEnableInputEscape;
			}
			set
			{
				isEnableInputEscape = value;
			}
		}

		public static SystemUi GetInstance()
		{
			return instance;
		}

		private void Awake()
		{
			if (null == instance)
			{
				instance = this;
				return;
			}
			Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.SingletonError));
			Object.Destroy(this);
		}

		public void OpenDialog(string text, List<ButtonEventInfo> buttons)
		{
			switch (buttons.Count)
			{
			case 1:
				OpenDialog1Button(text, buttons[0]);
				break;
			case 2:
				OpenDialog2Button(text, buttons[0], buttons[1]);
				break;
			case 3:
				OpenDialog3Button(text, buttons[0], buttons[1], buttons[2]);
				break;
			default:
				Debug.LogError(" Dilog Button Count over = " + buttons.Count);
				break;
			}
		}

		public void OpenDialog1Button(string text, ButtonEventInfo button1)
		{
			OpenDialog1Button(text, button1.text, button1.callBackClicked);
		}

		public void OpenDialog2Button(string text, ButtonEventInfo button1, ButtonEventInfo button2)
		{
			OpenDialog2Button(text, button1.text, button2.text, button1.callBackClicked, button2.callBackClicked);
		}

		public void OpenDialog3Button(string text, ButtonEventInfo button1, ButtonEventInfo button2, ButtonEventInfo button3)
		{
			OpenDialog3Button(text, button1.text, button2.text, button3.text, button1.callBackClicked, button2.callBackClicked, button3.callBackClicked);
		}

		public void OpenDialog1Button(string text, string buttonText1, UnityAction callbackOnClickButton1)
		{
			dialog1Button.Open(text, buttonText1, callbackOnClickButton1);
		}

		public void OpenDialog2Button(string text, string buttonText1, string buttonText2, UnityAction callbackOnClickButton1, UnityAction callbackOnClickButton2)
		{
			dialog2Button.Open(text, buttonText1, buttonText2, callbackOnClickButton1, callbackOnClickButton2);
		}

		public void OpenDialog3Button(string text, string buttonText1, string buttonText2, string buttonText3, UnityAction callbackOnClickButton1, UnityAction callbackOnClickButton2, UnityAction callbackOnClickButton3)
		{
			dialog3Button.Open(text, buttonText1, buttonText2, buttonText3, callbackOnClickButton1, callbackOnClickButton2, callbackOnClickButton3);
		}

		public void OpenDialogYesNo(string text, UnityAction callbackOnClickYes, UnityAction callbackOnClickNo)
		{
			OpenDialog2Button(text, LanguageSystemText.LocalizeText(SystemText.Yes), LanguageSystemText.LocalizeText(SystemText.No), callbackOnClickYes, callbackOnClickNo);
		}

		public void StartIndicator(Object obj)
		{
			if ((bool)indicator)
			{
				indicator.StartIndicator(obj);
			}
		}

		public void StopIndicator(Object obj)
		{
			if ((bool)indicator)
			{
				indicator.StopIndicator(obj);
			}
		}

		private void Update()
		{
			if (IsEnableInputEscape && (!(WrapperMoviePlayer.GetInstance() != null) || !WrapperMoviePlayer.IsPlaying()) && Input.GetKeyDown(KeyCode.Escape))
			{
				OnOpenDialogExitGame();
			}
		}

		public void OnOpenDialogExitGame()
		{
			IsEnableInputEscape = false;
			dialogGameExit.Open(LanguageSystemText.LocalizeText(SystemText.QuitGame), LanguageSystemText.LocalizeText(SystemText.Yes), LanguageSystemText.LocalizeText(SystemText.No), OnDialogExitGameYes, OnDialogExitGameNo);
		}

		private void OnDialogExitGameYes()
		{
			IsEnableInputEscape = true;
			StartCoroutine(CoGameExit());
		}

		private void OnDialogExitGameNo()
		{
			IsEnableInputEscape = true;
		}

		protected IEnumerator CoGameExit()
		{
			Application.Quit();
			yield break;
		}
	}
}
