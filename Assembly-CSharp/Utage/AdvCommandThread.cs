namespace Utage
{
	internal class AdvCommandThread : AdvCommand
	{
		private string label;

		private string name;

		public AdvCommandThread(StringGridRow row)
			: base(row)
		{
			label = ParseScenarioLabel(AdvColumnName.Arg1);
			name = ParseCellOptional(AdvColumnName.Arg2, label);
		}

		public override void DoCommand(AdvEngine engine)
		{
			base.CurrentTread.StartSubThread(label, name);
		}
	}
}
