namespace Utage
{
	internal class AdvCommandStopBgm : AdvCommand
	{
		private float fadeTime;

		public AdvCommandStopBgm(StringGridRow row)
			: base(row)
		{
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.StopBgm(fadeTime);
		}
	}
}
