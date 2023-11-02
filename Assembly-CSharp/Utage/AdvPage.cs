using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/MessageWindow")]
	public class AdvPage : MonoBehaviour
	{
		public enum PageStatus
		{
			None,
			SendChar,
			WaitEffectOnInputInPage,
			WaitInputInPage,
			OtherCommandInPage,
			WaitEffectOnEndPage,
			WaitInputBrPage
		}

		[SerializeField]
		private AdvPageEvent onBeginPage = new AdvPageEvent();

		[SerializeField]
		private AdvPageEvent onBeginText = new AdvPageEvent();

		[SerializeField]
		private AdvPageEvent onChangeText = new AdvPageEvent();

		[SerializeField]
		private AdvPageEvent onUpdateSendChar = new AdvPageEvent();

		[SerializeField]
		private AdvPageEvent onEndText = new AdvPageEvent();

		[SerializeField]
		private AdvPageEvent onEndPage = new AdvPageEvent();

		[SerializeField]
		private AdvPageEvent onChangeStatus = new AdvPageEvent();

		[SerializeField]
		private AdvPageEvent onTrigWaitInputInPage = new AdvPageEvent();

		[SerializeField]
		private AdvPageEvent onTrigWaitInputBrPage = new AdvPageEvent();

		[SerializeField]
		private AdvPageEvent onTrigInput = new AdvPageEvent();

		private List<AdvCommandText> textDataList = new List<AdvCommandText>();

		private PageStatus status;

		private AdvPageController contoller = new AdvPageController();

		private AdvEngine engine;

		private bool isInputSendMessage;

		private float deltaTimeSendMessage;

		private float waitingTimeInput;

		public AdvPageEvent OnBeginPage => onBeginPage;

		public AdvPageEvent OnBeginText => onBeginText;

		public AdvPageEvent OnChangeText => onChangeText;

		public AdvPageEvent OnUpdateSendChar => onUpdateSendChar;

		public AdvPageEvent OnEndText => onEndText;

		public AdvPageEvent OnEndPage => onEndPage;

		public AdvPageEvent OnChangeStatus => onChangeStatus;

		public AdvPageEvent OnTrigWaitInputInPage => onTrigWaitInputInPage;

		public AdvPageEvent OnTrigWaitInputBrPage => onTrigWaitInputBrPage;

		public AdvPageEvent OnTrigInput => onTrigInput;

		public AdvScenarioPageData CurrentData { get; private set; }

		public string ScenarioLabel
		{
			get
			{
				if (CurrentData != null)
				{
					return CurrentData.ScenarioLabelData.ScenarioLabel;
				}
				return "";
			}
		}

		public int PageNo
		{
			get
			{
				if (CurrentData != null)
				{
					return CurrentData.PageNo;
				}
				return 0;
			}
		}

		public bool IsSavePoint
		{
			get
			{
				if (CurrentData != null)
				{
					if (CurrentData.PageNo == 0)
					{
						return CurrentData.ScenarioLabelData.IsSavePoint;
					}
					return false;
				}
				return false;
			}
		}

		public TextData TextData { get; private set; }

		public AdvCommandText CurrentTextDataInPage { get; private set; }

		private List<AdvCommandText> TextDataList => textDataList;

		public AdvCharacterInfo CharacterInfo { get; set; }

		public string NameText
		{
			get
			{
				if (CharacterInfo != null)
				{
					return CharacterInfo.LocalizeNameText;
				}
				return "";
			}
		}

		public string CharacterLabel
		{
			get
			{
				if (CharacterInfo != null)
				{
					return CharacterInfo.Label;
				}
				return "";
			}
		}

		public string SaveDataTitle { get; private set; }

		public int CurrentTextLength { get; protected set; }

		public int CurrentTextLengthMax { get; private set; }

		public char CurrenLipiSyncWord => CurrentCharData.Char;

		public CharData CurrentCharData
		{
			get
			{
				int index = Mathf.Clamp(CurrentTextLength, 0, TextData.ParsedText.CharList.Count - 1);
				return TextData.ParsedText.CharList[index];
			}
		}

		public PageStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				if (status != value)
				{
					status = value;
					OnChangeStatus.Invoke(this);
					switch (Status)
					{
					case PageStatus.WaitInputInPage:
						OnTrigWaitInputInPage.Invoke(this);
						break;
					case PageStatus.WaitInputBrPage:
						OnTrigWaitInputBrPage.Invoke(this);
						break;
					}
				}
			}
		}

		public bool IsSendChar => Status == PageStatus.SendChar;

		public bool IsWaitTextCommand
		{
			get
			{
				if (Engine.SelectionManager.IsWaitInput)
				{
					return true;
				}
				PageStatus pageStatus = Status;
				if ((uint)(pageStatus - 1) <= 2u || (uint)(pageStatus - 5) <= 1u)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsWaitInputInPage
		{
			get
			{
				if (Status != PageStatus.WaitInputInPage)
				{
					return IsWaitingInputCommand;
				}
				return true;
			}
		}

		[Obsolete("Use IsWaitInputInPage instead")]
		public bool IsWaitIntputInPage => IsWaitInputInPage;

		public bool IsWaitingInputCommand { get; set; }

		[Obsolete("Use IsWaitingInputCommand instead")]
		public bool IsWaitingIntputCommand => IsWaitingInputCommand;

		public bool IsWaitBrPage => Status == PageStatus.WaitInputBrPage;

		[Obsolete]
		public bool IsShowingText => Engine.UiManager.IsShowingMessageWindow;

		[Obsolete]
		public bool IsWaitPage
		{
			get
			{
				if (!Engine.UiManager.IsShowingMessageWindow)
				{
					return Engine.SelectionManager.IsWaitInput;
				}
				return true;
			}
		}

		public AdvPageController Contoller => contoller;

		public AdvEngine Engine => engine ?? (engine = GetComponent<AdvEngine>());

		private AdvIfManager MainThreadIfManager => Engine.ScenarioPlayer.MainThread.IfManager;

		private bool LastInputSendMessage { get; set; }

		public float SkippedSpeed
		{
			get
			{
				if (!CheckSkip())
				{
					return 1f;
				}
				return Engine.Config.SkipSpped;
			}
		}

		private bool IsNoWaitAllText
		{
			get
			{
				if (TextData.IsNoWaitAll)
				{
					return true;
				}
				if (TextData.ContainsSpeedTag)
				{
					return false;
				}
				return Engine.Config.GetTimeSendChar(CheckReadPage()) <= 0f;
			}
		}

		public bool CheckReadPage()
		{
			return Engine.SystemSaveData.ReadData.CheckReadPage(ScenarioLabel, PageNo);
		}

		public void InputSendMessage()
		{
			isInputSendMessage = true;
		}

		private bool IsInputSendMessage()
		{
			if (!isInputSendMessage)
			{
				return CheckSkip();
			}
			return true;
		}

		public void Clear()
		{
			Status = PageStatus.None;
			CurrentData = null;
			CurrentTextLength = 0;
			CurrentTextLengthMax = 0;
			deltaTimeSendMessage = 0f;
			Contoller.Clear();
		}

		public void BeginPage(AdvScenarioPageData currentPageData)
		{
			LastInputSendMessage = false;
			CurrentData = currentPageData;
			CurrentTextLength = 0;
			CurrentTextLengthMax = 0;
			deltaTimeSendMessage = 0f;
			Contoller.Clear();
			TextData = new TextData("");
			TextDataList.Clear();
			UpdateText();
			RemakeTextData();
			SaveDataTitle = CurrentData.ScenarioLabelData.SaveTitle;
			if (string.IsNullOrEmpty(SaveDataTitle))
			{
				SaveDataTitle = TextData.OriginalText;
			}
			OnBeginPage.Invoke(this);
			Engine.UiManager.OnBeginPage();
			Engine.MessageWindowManager.ChangeCurrentWindow(currentPageData.MessageWindowName);
			if (!currentPageData.IsEmptyText)
			{
				Engine.BacklogManager.AddPage();
			}
		}

		public void EndPage()
		{
			Status = PageStatus.None;
			if (Engine.Config.VoiceStopType == VoiceStopType.OnClick && !CurrentData.IsEmptyText)
			{
				Engine.SoundManager.StopVoiceIgnoreLoop();
			}
			Engine.SystemSaveData.ReadData.AddReadPage(ScenarioLabel, PageNo);
			Engine.UiManager.OnEndPage();
			OnEndPage.Invoke(this);
			CurrentData = null;
			CurrentTextLength = 0;
			CurrentTextLengthMax = 0;
			deltaTimeSendMessage = 0f;
			Contoller.Clear();
		}

		public void UpdatePageTextData(AdvPageControllerType pageCtrlType)
		{
			bool isBr = Contoller.IsBr;
			Contoller.Update(pageCtrlType);
			if (isBr)
			{
				int currentTextLengthMax = CurrentTextLengthMax + 1;
				CurrentTextLengthMax = currentTextLengthMax;
			}
			if (!Engine.SelectionManager.TryStartWaitInputIfShowing())
			{
				Engine.UiManager.ShowMessageWindow();
			}
		}

		public void UpdatePageTextData(AdvCommandText text)
		{
			bool isBr = Contoller.IsBr;
			CurrentTextDataInPage = text;
			TextDataList.Add(text);
			Contoller.Update(CurrentTextDataInPage.PageCtrlType);
			if (isBr)
			{
				int currentTextLengthMax = CurrentTextLengthMax + 1;
				CurrentTextLengthMax = currentTextLengthMax;
			}
			RemakeText();
			Engine.UiManager.ShowMessageWindow();
			Engine.BacklogManager.AddCurrentPageLog(CurrentTextDataInPage, CharacterInfo);
		}

		public void RemakeText()
		{
			if (CurrentData != null)
			{
				RemakeTextData();
				Status = PageStatus.SendChar;
				if (CurrentTextLength == 0)
				{
					OnBeginText.Invoke(this);
				}
				if (IsNoWaitAllText || CheckSkip() || LastInputSendMessage)
				{
					EndSendChar();
				}
				OnChangeText.Invoke(this);
				Engine.MessageWindowManager.OnPageTextChange(this);
				Engine.OnPageTextChange.Invoke(Engine);
			}
		}

		private void RemakeTextData()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (AdvCommandText textData in TextDataList)
			{
				stringBuilder.Append(textData.ParseCellLocalizedText());
				if (textData.IsNextBr)
				{
					stringBuilder.Append("\n");
				}
			}
			CurrentTextLengthMax = new TextData(stringBuilder.ToString()).Length;
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = 0; i < CurrentData.TextDataList.Count; i++)
			{
				AdvCommandText advCommandText = CurrentData.TextDataList[i];
				stringBuilder2.Append(advCommandText.ParseCellLocalizedText());
				if (advCommandText.IsNextBr)
				{
					stringBuilder2.Append("\n");
				}
			}
			TextData = new TextData(stringBuilder2.ToString());
		}

		public void OnChangeLanguage()
		{
			if (Application.isPlaying)
			{
				RemakeText();
			}
		}

		public bool CheckSkip()
		{
			return Engine.Config.CheckSkip(Engine.SystemSaveData.ReadData.CheckReadPage(ScenarioLabel, PageNo));
		}

		public float ToSkippedTime(float time)
		{
			if (!CheckSkip())
			{
				return time;
			}
			return time / Engine.Config.SkipSpped;
		}

		public bool EnableSkip()
		{
			if (Engine.Config.IsSkipUnread)
			{
				return true;
			}
			return CheckReadPage();
		}

		public void UpdateText()
		{
			LastInputSendMessage = false;
			switch (Status)
			{
			case PageStatus.SendChar:
				UpdateSendChar();
				LastInputSendMessage = isInputSendMessage;
				break;
			case PageStatus.WaitInputInPage:
			case PageStatus.WaitInputBrPage:
				UpdateWaitInput();
				break;
			case PageStatus.WaitEffectOnInputInPage:
				UpdateWaitEffectOnInput();
				break;
			case PageStatus.WaitEffectOnEndPage:
				UpdateWaitEffectOnEndPage();
				break;
			}
			isInputSendMessage = false;
		}

		private void UpdateSendChar()
		{
			OnUpdateSendChar.Invoke(this);
			if (IsInputSendMessage() && !CurrentCharData.CustomInfo.IsSpeed)
			{
				EndSendChar();
				return;
			}
			float timeCharSend = Engine.Config.GetTimeSendChar(CheckReadPage());
			if (CurrentCharData.CustomInfo.IsSpeed && CurrentCharData.CustomInfo.speed >= 0f)
			{
				timeCharSend = CurrentCharData.CustomInfo.speed;
			}
			if (CurrentCharData.CustomInfo.IsInterval)
			{
				timeCharSend = CurrentCharData.CustomInfo.Interval;
			}
			SendChar(timeCharSend);
			if (CurrentTextLength >= CurrentTextLengthMax)
			{
				EndSendChar();
			}
		}

		private void UpdateWaitInput()
		{
			if (Engine.Config.IsAutoBrPage && !Engine.SoundManager.IsPlayingVoice() && waitingTimeInput >= Engine.Config.AutoPageWaitTime)
			{
				ToNextCommand();
			}
			else if (IsInputSendMessage())
			{
				if (isInputSendMessage)
				{
					OnTrigInput.Invoke(this);
				}
				if (Engine.Config.VoiceStopType == VoiceStopType.OnClick)
				{
					Engine.SoundManager.StopVoiceIgnoreLoop();
				}
				ToNextCommand();
			}
			else if (!Engine.Config.IsAutoBrPage || !Engine.SoundManager.IsPlayingVoice())
			{
				waitingTimeInput += Time.deltaTime;
			}
		}

		private void UpdateWaitEffectOnInput()
		{
			if (!Engine.ScenarioPlayer.MainThread.WaitManager.IsWaitingInputEffect)
			{
				Status = PageStatus.WaitInputInPage;
			}
		}

		private void UpdateWaitEffectOnEndPage()
		{
			if (!Engine.ScenarioPlayer.MainThread.WaitManager.IsWaitingPageEndEffect)
			{
				Status = PageStatus.WaitInputBrPage;
			}
		}

		private void EndSendChar()
		{
			OnEndText.Invoke(this);
			CurrentTextLength = CurrentTextLengthMax;
			if (CurrentTextDataInPage.IsPageEnd && Engine.SelectionManager.TryStartWaitInputIfShowing())
			{
				ToNextCommand();
			}
			else if (Contoller.IsWaitInput)
			{
				if (CurrentTextDataInPage.IsPageEnd)
				{
					if (Engine.ScenarioPlayer.MainThread.WaitManager.IsWaitingPageEndEffect)
					{
						Status = PageStatus.WaitEffectOnEndPage;
					}
					else
					{
						Status = PageStatus.WaitInputBrPage;
					}
				}
				else if (Engine.ScenarioPlayer.MainThread.WaitManager.IsWaitingInputEffect)
				{
					Status = PageStatus.WaitEffectOnInputInPage;
				}
				else
				{
					Status = PageStatus.WaitInputInPage;
				}
				waitingTimeInput = 0f;
			}
			else
			{
				ToNextCommand();
			}
		}

		private void ToNextCommand()
		{
			CurrentTextLength = CurrentTextLengthMax;
			if (CurrentTextDataInPage.IsPageEnd)
			{
				Status = PageStatus.None;
			}
			else
			{
				Status = PageStatus.OtherCommandInPage;
			}
		}

		private void SendChar(float timeCharSend)
		{
			if (timeCharSend <= 0f)
			{
				if (IsNoWaitAllText)
				{
					CurrentTextLength = CurrentTextLengthMax;
					return;
				}
				timeCharSend = 0f;
			}
			deltaTimeSendMessage += Time.deltaTime;
			while (deltaTimeSendMessage >= 0f)
			{
				int currentTextLength = CurrentTextLength + 1;
				CurrentTextLength = currentTextLength;
				deltaTimeSendMessage -= timeCharSend;
				if (CurrentTextLength > CurrentTextLengthMax)
				{
					CurrentTextLength = CurrentTextLengthMax;
					break;
				}
				if (CurrentCharData.CustomInfo.IsInterval || CurrentCharData.CustomInfo.IsSpeed)
				{
					break;
				}
			}
		}
	}
}
