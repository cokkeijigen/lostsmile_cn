using System;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Examples/CustomCommand")]
	public class SampleCustomCommand : AdvCustomCommandManager
	{
		public override void OnBootInit()
		{
			AdvCommandParser.OnCreateCustomCommandFromID = (AdvCommandParser.CreateCustomCommandFromID)Delegate.Combine(AdvCommandParser.OnCreateCustomCommandFromID, new AdvCommandParser.CreateCustomCommandFromID(CreateCustomCommand));
		}

		public override void OnClear()
		{
		}

		public void CreateCustomCommand(string id, StringGridRow row, AdvSettingDataManager dataManager, ref AdvCommand command)
		{
			if (id == "DebugLog")
			{
				command = new SampleAdvCommandDebugLog(row);
			}
		}
	}
}
