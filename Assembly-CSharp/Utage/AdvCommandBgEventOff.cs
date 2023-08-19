namespace Utage
{
	internal class AdvCommandBgEventOff : AdvCommand
	{
		private float fadeTime;

		public AdvCommandBgEventOff(StringGridRow row)
			: base(row)
		{
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.GraphicManager.BgManager.FadeOutAll(engine.Page.ToSkippedTime(fadeTime));
		}
	}
}
