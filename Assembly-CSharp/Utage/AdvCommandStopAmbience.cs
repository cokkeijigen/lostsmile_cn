namespace Utage
{
	internal class AdvCommandStopAmbience : AdvCommand
	{
		private float fadeTime;

		public AdvCommandStopAmbience(StringGridRow row)
			: base(row)
		{
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.StopAmbience(fadeTime);
		}
	}
}
