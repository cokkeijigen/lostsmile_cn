namespace Utage
{
	public class AdvScenarioJumpData
	{
		public string ToLabel { get; private set; }

		public StringGridRow FromRow { get; private set; }

		public AdvScenarioJumpData(string toLabel, StringGridRow fromRow)
		{
			ToLabel = toLabel;
			FromRow = fromRow;
		}
	}
}
