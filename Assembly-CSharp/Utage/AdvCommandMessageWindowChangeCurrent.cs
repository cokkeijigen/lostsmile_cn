namespace Utage
{
	internal class AdvCommandMessageWindowChangeCurrent : AdvCommand
	{
		private string name;

		public AdvCommandMessageWindowChangeCurrent(StringGridRow row)
			: base(row)
		{
			name = ParseCell<string>(AdvColumnName.Arg1);
		}

		public override void InitFromPageData(AdvScenarioPageData pageData)
		{
			pageData.InitMessageWindowName(this, name);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.MessageWindowManager.ChangeCurrentWindow(name);
		}
	}
}
