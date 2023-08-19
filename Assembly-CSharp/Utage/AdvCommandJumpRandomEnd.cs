namespace Utage
{
	internal class AdvCommandJumpRandomEnd : AdvCommand
	{
		public AdvCommandJumpRandomEnd(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvCommandJumpRandom advCommandJumpRandom = base.CurrentTread.JumpManager.GetRandomJumpCommand() as AdvCommandJumpRandom;
			if (advCommandJumpRandom != null)
			{
				advCommandJumpRandom.DoRandomEnd(engine, base.CurrentTread);
			}
		}
	}
}
