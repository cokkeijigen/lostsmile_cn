namespace Utage
{
	internal class AdvCommandChangeSoundVolume : AdvCommand
	{
		private string[] groups;

		private float volume;

		private float fadeTime;

		public AdvCommandChangeSoundVolume(StringGridRow row)
			: base(row)
		{
			groups = ParseCellArray<string>(AdvColumnName.Arg1);
			volume = ParseCell<float>(AdvColumnName.Arg2);
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (groups.Length == 1 && groups[0] == "All")
			{
				engine.SoundManager.SetGroupVolume("Bgm", volume, fadeTime);
				engine.SoundManager.SetGroupVolume("Ambience", volume, fadeTime);
				engine.SoundManager.SetGroupVolume("Se", volume, fadeTime);
				engine.SoundManager.SetGroupVolume("Voice", volume, fadeTime);
				return;
			}
			string[] array = groups;
			foreach (string groupName in array)
			{
				engine.SoundManager.SetGroupVolume(groupName, volume, fadeTime);
			}
		}
	}
}
