using UnityEngine;

namespace Utage
{
	internal class AdvCommandAmbience : AdvCommand
	{
		private AssetFile file;

		private float volume;

		private bool isLoop;

		private float fadeInTime;

		private float fadeOutTime;

		public AdvCommandAmbience(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			string text = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.SoundSetting.Contains(text, SoundType.Ambience))
			{
				Debug.LogError(ToErrorString(text + " is not contained in file setting"));
			}
			file = AddLoadFile(dataManager.SoundSetting.LabelToFilePath(text, SoundType.Ambience), dataManager.SoundSetting.FindData(text));
			isLoop = ParseCellOptional(AdvColumnName.Arg2, false);
			volume = ParseCellOptional(AdvColumnName.Arg3, 1f);
			fadeOutTime = ParseCellOptional(AdvColumnName.Arg5, 0.2f);
			fadeInTime = ParseCellOptional(AdvColumnName.Arg6, 0f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.PlayAmbience(file, volume, isLoop, fadeInTime, fadeOutTime);
		}
	}
}
