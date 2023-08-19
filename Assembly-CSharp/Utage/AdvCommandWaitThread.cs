namespace Utage
{
	internal class AdvCommandWaitThread : AdvCommand
	{
		private string label;

		private bool cancelInput;

		public AdvCommandWaitThread(StringGridRow row)
			: base(row)
		{
			label = ParseScenarioLabel(AdvColumnName.Arg1);
			cancelInput = ParseCellOptional(AdvColumnName.Arg2, false);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (cancelInput)
			{
				engine.Page.IsWaitingInputCommand = true;
			}
		}

		public override bool Wait(AdvEngine engine)
		{
			if (IsWaiting(engine))
			{
				return true;
			}
			if (cancelInput)
			{
				engine.Page.IsWaitingInputCommand = false;
			}
			return false;
		}

		private bool IsWaiting(AdvEngine engine)
		{
			if ((cancelInput && engine.UiManager.IsInputTrig) || engine.Page.CheckSkip())
			{
				base.CurrentTread.CancelSubThread(label);
				return false;
			}
			return base.CurrentTread.IsPlayingSubThread(label);
		}
	}
}
