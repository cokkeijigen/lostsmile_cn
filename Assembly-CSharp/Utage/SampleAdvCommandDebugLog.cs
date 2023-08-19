using UnityEngine;

namespace Utage
{
	public class SampleAdvCommandDebugLog : AdvCommand
	{
		private string log;

		public SampleAdvCommandDebugLog(StringGridRow row)
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
