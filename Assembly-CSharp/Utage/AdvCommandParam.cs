using UnityEngine;

namespace Utage
{
	internal class AdvCommandParam : AdvCommand
	{
		private ExpressionParser exp;

		public AdvCommandParam(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			exp = dataManager.DefaultParam.CreateExpression(ParseCell<string>(AdvColumnName.Arg1));
			if (exp.ErrorMsg != null)
			{
				Debug.LogError(ToErrorString(exp.ErrorMsg));
			}
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.Param.CalcExpression(exp);
		}
	}
}
