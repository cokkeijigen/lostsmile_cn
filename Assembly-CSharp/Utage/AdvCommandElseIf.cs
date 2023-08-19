using UnityEngine;

namespace Utage
{
	internal class AdvCommandElseIf : AdvCommand
	{
		private ExpressionParser exp;

		public override bool IsIfCommand
		{
			get
			{
				return true;
			}
		}

		public AdvCommandElseIf(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			exp = dataManager.DefaultParam.CreateExpressionBoolean(ParseCell<string>(AdvColumnName.Arg1));
			if (exp.ErrorMsg != null)
			{
				Debug.LogError(ToErrorString(exp.ErrorMsg));
			}
		}

		public override void DoCommand(AdvEngine engine)
		{
			base.CurrentTread.IfManager.ElseIf(engine.Param, exp);
		}
	}
}
