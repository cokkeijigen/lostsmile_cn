using UnityEngine;

namespace Utage
{
	internal class SampleCustomAdvCommandText : AdvCommand
	{
		private string log;

		public SampleCustomAdvCommandText(StringGridRow row)
			: base(row)
		{
			log = ParseCell<string>(AdvColumnName.Text);
		}

		public override void DoCommand(AdvEngine engine)
		{
			Debug.Log(log);
		}
	}
}
