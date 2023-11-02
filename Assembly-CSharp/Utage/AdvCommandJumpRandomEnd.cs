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
			if (base.CurrentTread.JumpManager.GetRandomJumpCommand() is AdvCommandJumpRandom advCommandJumpRandom)
			{
				advCommandJumpRandom.DoRandomEnd(engine, base.CurrentTread);
			}
		}
	}
}
