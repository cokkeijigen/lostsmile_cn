using UnityEngine;

namespace Utage
{
	public class AdvCommandJump : AdvCommand
	{
		private string jumpLabel;

		private ExpressionParser exp;

		public AdvCommandJump(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			jumpLabel = ParseScenarioLabel(AdvColumnName.Arg1);
			string value = ParseCellOptional(AdvColumnName.Arg2, "");
			if (string.IsNullOrEmpty(value))
			{
				exp = null;
				return;
			}
			exp = dataManager.DefaultParam.CreateExpressionBoolean(value);
			if (exp.ErrorMsg != null)
			{
				Debug.LogError(ToErrorString(exp.ErrorMsg));
			}
		}

		public override string[] GetJumpLabels()
		{
			return new string[1] { jumpLabel };
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (IsEnable(engine.Param))
			{
				base.CurrentTread.JumpManager.RegistoreLabel(jumpLabel);
			}
		}

		private bool IsEnable(AdvParamManager param)
		{
			if (exp != null)
			{
				return param.CalcExpressionBoolean(exp);
			}
			return true;
		}
	}
}
