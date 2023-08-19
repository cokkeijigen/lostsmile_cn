using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	internal class AdvCommandMessageWindowInit : AdvCommand
	{
		private List<string> names = new List<string>();

		public AdvCommandMessageWindowInit(StringGridRow row)
			: base(row)
		{
			if (!IsEmptyCell(AdvColumnName.Arg1))
			{
				names.Add(ParseCell<string>(AdvColumnName.Arg1));
			}
			if (!IsEmptyCell(AdvColumnName.Arg2))
			{
				names.Add(ParseCell<string>(AdvColumnName.Arg2));
			}
			if (!IsEmptyCell(AdvColumnName.Arg3))
			{
				names.Add(ParseCell<string>(AdvColumnName.Arg3));
			}
			if (!IsEmptyCell(AdvColumnName.Arg4))
			{
				names.Add(ParseCell<string>(AdvColumnName.Arg4));
			}
			if (!IsEmptyCell(AdvColumnName.Arg5))
			{
				names.Add(ParseCell<string>(AdvColumnName.Arg5));
			}
			if (!IsEmptyCell(AdvColumnName.Arg6))
			{
				names.Add(ParseCell<string>(AdvColumnName.Arg6));
			}
			if (names.Count <= 0)
			{
				Debug.LogError(ToErrorString("Not set data in this command"));
			}
		}

		public override void InitFromPageData(AdvScenarioPageData pageData)
		{
			if (names.Count > 0)
			{
				pageData.InitMessageWindowName(this, names[0]);
			}
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.MessageWindowManager.ChangeActiveWindows(names);
		}
	}
}
