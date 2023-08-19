using UnityEngine;

namespace Utage
{
	internal class AdvCommandBgm : AdvCommand
	{
		private AssetFile file;

		private float volume;

		private float fadeInTime;

		private float fadeOutTime;

		public AdvCommandBgm(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			string text = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.SoundSetting.Contains(text, SoundType.Bgm))
			{
				Debug.LogError(ToErrorString(text + " is not contained in file setting"));
			}
			file = AddLoadFile(dataManager.SoundSetting.LabelToFilePath(text, SoundType.Bgm), dataManager.SoundSetting.FindData(text));
			volume = ParseCellOptional(AdvColumnName.Arg3, 1f);
			fadeOutTime = ParseCellOptional(AdvColumnName.Arg5, 0.2f);
			fadeInTime = ParseCellOptional(AdvColumnName.Arg6, 0f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.PlayBgm(file, volume, fadeInTime, fadeOutTime);
		}
	}
}
