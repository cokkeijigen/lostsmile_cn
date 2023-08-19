namespace Utage
{
	internal class AdvCommandStopVoice : AdvCommand
	{
		private float fadeTime;

		public AdvCommandStopVoice(StringGridRow row)
			: base(row)
		{
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.StopVoice(fadeTime);
		}
	}
}
