using UnityEngine;

namespace Utage
{
	public class AdvCommandParamTblKeyCount : AdvCommand
	{
		private string paramName;

		private string tblName;

		public AdvCommandParamTblKeyCount(StringGridRow row)
			: base(row)
		{
			paramName = ParseCell<string>(AdvColumnName.Arg1);
			tblName = ParseCell<string>(AdvColumnName.Arg2);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (engine.Param.StructTbl.TryGetValue(tblName, out var value))
			{
				int count = value.Tbl.Count;
				if (!engine.Param.TrySetParameter(paramName, count))
				{
					Debug.LogError(paramName + " is not parameter name");
				}
			}
			else
			{
				Debug.LogError(tblName + " is not ParamTbl name");
			}
		}
	}
}
