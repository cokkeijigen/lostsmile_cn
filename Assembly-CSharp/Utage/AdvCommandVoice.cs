namespace Utage
{
	internal class AdvCommandVoice : AdvCommand
	{
		protected string characterLabel;

		protected AssetFile voiceFile;

		private float volume;

		private bool isLoop;

		public AdvCommandVoice(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			characterLabel = ParseCell<string>(AdvColumnName.Arg1);
			string file = ParseCell<string>(AdvColumnName.Voice);
			if (!AdvCommand.IsEditorErrorCheck)
			{
				voiceFile = AddLoadFile(dataManager.BootSetting.GetLocalizeVoiceFilePath(file), null);
			}
			isLoop = ParseCellOptional(AdvColumnName.Arg2, false);
			volume = ParseCellOptional(AdvColumnName.Arg3, 1f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (!engine.Page.CheckSkip() || !engine.Config.SkipVoiceAndSe)
			{
				engine.SoundManager.PlayVoice(characterLabel, voiceFile, volume, isLoop);
			}
		}
	}
}
