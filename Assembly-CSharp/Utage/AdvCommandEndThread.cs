namespace Utage
{
	internal class AdvCommandEndThread : AdvCommand
	{
		public AdvCommandEndThread(StringGridRow row)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			base.CurrentTread.IsPlaying = false;
		}
	}
}
