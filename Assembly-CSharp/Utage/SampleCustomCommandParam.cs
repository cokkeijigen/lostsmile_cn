using System;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Examples/CustomCommandParam")]
	public class SampleCustomCommandParam : AdvCustomCommandManager
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
			if (!(id == "SetParamTblCount"))
			{
				if (id == "SetParamTblCount2")
				{
					command = new AdvCommandParamTblKeyCount2(row);
				}
			}
			else
			{
				command = new AdvCommandParamTblKeyCount(row);
			}
		}
	}
}
