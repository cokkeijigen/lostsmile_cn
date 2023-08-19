using UnityEngine;

namespace Utage
{
	public class AdvCommandText : AdvCommand
	{
		public bool IsPageEnd { get; private set; }

		public bool IsNextBr { get; private set; }

		public AdvPageControllerType PageCtrlType { get; private set; }

		public AssetFile VoiceFile { get; private set; }

		private AdvScenarioPageData PageData { get; set; }

		private int IndexPageData { get; set; }

		public AdvCommandText(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			string text = ParseCellOptional(AdvColumnName.Voice, "");
			if (!string.IsNullOrEmpty(text) && !AdvCommand.IsEditorErrorCheck)
			{
				VoiceFile = AddLoadFile(dataManager.BootSetting.GetLocalizeVoiceFilePath(text), new AdvVoiceSetting(base.RowData));
			}
			PageCtrlType = ParseCellOptional(AdvColumnName.PageCtrl, AdvPageControllerType.InputBrPage);
			IsNextBr = AdvPageController.IsBrType(PageCtrlType);
			IsPageEnd = AdvPageController.IsPageEndType(PageCtrlType);
			if (AdvCommand.IsEditorErrorCheck)
			{
				TextData textData = new TextData(ParseCellLocalizedText());
				if (!string.IsNullOrEmpty(textData.ErrorMsg))
				{
					Debug.LogError(ToErrorString(textData.ErrorMsg));
				}
			}
		}

		public override void InitFromPageData(AdvScenarioPageData pageData)
		{
			PageData = pageData;
			IndexPageData = PageData.TextDataList.Count;
			PageData.AddTextData(this);
			PageData.InitMessageWindowName(this, ParseCellOptional(AdvColumnName.WindowType, ""));
		}

		internal void InitOnCreateEntity(AdvCommandText original)
		{
			PageData = original.PageData;
			PageData.ChangeTextDataOnCreateEntity(original.IndexPageData, this);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (IsEmptyCell(AdvColumnName.Arg1))
			{
				engine.Page.CharacterInfo = null;
			}
			if (VoiceFile != null && (!engine.Page.CheckSkip() || !engine.Config.SkipVoiceAndSe))
			{
				engine.SoundManager.PlayVoice(engine.Page.CharacterLabel, VoiceFile);
			}
			engine.Page.UpdatePageTextData(this);
		}

		public override bool Wait(AdvEngine engine)
		{
			return engine.Page.IsWaitTextCommand;
		}

		public override bool IsTypePage()
		{
			return true;
		}

		public override bool IsTypePageEnd()
		{
			return IsPageEnd;
		}
	}
}
