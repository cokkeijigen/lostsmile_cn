namespace Utage
{
	internal class AdvCommandStopSound : AdvCommand
	{
		private string[] groups;

		private float fadeTime = 0.15f;

		public AdvCommandStopSound(StringGridRow row)
			: base(row)
		{
			groups = ParseCellOptionalArray(AdvColumnName.Arg1, new string[2] { "Bgm", "Ambience" });
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, fadeTime);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (groups.Length == 1 && groups[0] == "All")
			{
				engine.SoundManager.StopAll(fadeTime);
			}
			else
			{
				engine.SoundManager.StopGroups(groups, fadeTime);
			}
		}
	}
}
