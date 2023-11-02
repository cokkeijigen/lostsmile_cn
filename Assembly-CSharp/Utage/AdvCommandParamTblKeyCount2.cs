using UnityEngine;

namespace Utage
{
	public class AdvCommandParamTblKeyCount2 : AdvCommand
	{
		private string paramName;

		private string tblName;

		private string valueName;

		private string countValue;

		public AdvCommandParamTblKeyCount2(StringGridRow row)
			: base(row)
		{
			paramName = ParseCell<string>(AdvColumnName.Arg1);
			tblName = ParseCell<string>(AdvColumnName.Arg2);
			valueName = ParseCell<string>(AdvColumnName.Arg3);
			countValue = ParseCell<string>(AdvColumnName.Arg4);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (engine.Param.StructTbl.TryGetValue(tblName, out var value))
			{
				int num = 0;
				foreach (AdvParamStruct value3 in value.Tbl.Values)
				{
					if (!value3.Tbl.TryGetValue(valueName, out var value2))
					{
						Debug.LogError(valueName + " is not parameter name");
						return;
					}
					if ((string)value2.Parameter == countValue)
					{
						num++;
					}
				}
				if (!engine.Param.TrySetParameter(paramName, num))
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
