using UnityEngine;

namespace Utage
{
	internal class AdvCommandStopSe : AdvCommand
	{
		private string label;

		private float fadeTime;

		public AdvCommandStopSe(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			label = ParseCellOptional(AdvColumnName.Arg1, "");
			if (!string.IsNullOrEmpty(label) && !dataManager.SoundSetting.Contains(label, SoundType.Se))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(label))
			{
				engine.SoundManager.StopSeAll(fadeTime);
			}
			else
			{
				engine.SoundManager.StopSe(label, fadeTime);
			}
		}
	}
}
