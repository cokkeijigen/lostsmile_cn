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
			if (base.CurrentTread.JumpManager.GetRandomJumpCommand() is AdvCommandJumpSubroutineRandom advCommandJumpSubroutineRandom)
			{
				advCommandJumpSubroutineRandom.DoRandomEnd(base.CurrentTread, engine);
			}
		}
	}
}
