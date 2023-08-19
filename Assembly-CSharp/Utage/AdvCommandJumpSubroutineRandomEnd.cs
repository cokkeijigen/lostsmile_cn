namespace Utage
{
	internal class AdvCommandJumpSubroutineRandomEnd : AdvCommand
	{
		public AdvCommandJumpSubroutineRandomEnd(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
		}

		public override bool IsTypePage()
		{
			return true;
		}

		public override bool IsTypePageEnd()
		{
			return true;
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvCommandJumpSubroutineRandom advCommandJumpSubroutineRandom = base.CurrentTread.JumpManager.GetRandomJumpCommand() as AdvCommandJumpSubroutineRandom;
			if (advCommandJumpSubroutineRandom != null)
			{
				advCommandJumpSubroutineRandom.DoRandomEnd(base.CurrentTread, engine);
			}
		}
	}
}
