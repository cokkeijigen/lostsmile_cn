using UnityEngine;

namespace Utage
{
	internal class AdvCommandJumpRandom : AdvCommand
	{
		private string jumpLabel;

		private ExpressionParser exp;

		private ExpressionParser expRate;

		public AdvCommandJumpRandom(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			jumpLabel = ParseScenarioLabel(AdvColumnName.Arg1);
			string value = ParseCellOptional(AdvColumnName.Arg2, "");
			if (string.IsNullOrEmpty(value))
			{
				exp = null;
			}
			else
			{
				exp = dataManager.DefaultParam.CreateExpressionBoolean(value);
				if (exp.ErrorMsg != null)
				{
					Debug.LogError(ToErrorString(exp.ErrorMsg));
				}
			}
			string value2 = ParseCellOptional(AdvColumnName.Arg3, "");
			if (string.IsNullOrEmpty(value2))
			{
				expRate = null;
				return;
			}
			expRate = dataManager.DefaultParam.CreateExpression(value2);
			if (expRate.ErrorMsg != null)
			{
				Debug.LogError(ToErrorString(expRate.ErrorMsg));
			}
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (IsEnable(engine.Param))
			{
				base.CurrentTread.JumpManager.AddRandom(this, CalcRate(engine.Param));
			}
		}

		internal void DoRandomEnd(AdvEngine engine, AdvScenarioThread thread)
		{
			if (!string.IsNullOrEmpty(jumpLabel))
			{
				thread.JumpManager.ClearOnJump();
				thread.JumpManager.RegistoreLabel(jumpLabel);
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

		private float CalcRate(AdvParamManager param)
		{
			if (expRate == null)
			{
				return 1f;
			}
			return param.CalcExpressionFloat(expRate);
		}

		public override string[] GetJumpLabels()
		{
			return new string[1] { jumpLabel };
		}

		public override string[] GetExtraCommandIdArray(AdvCommand next)
		{
			if (next == null || !(next is AdvCommandJumpRandom))
			{
				return new string[1] { "JumpRandomEnd" };
			}
			return null;
		}
	}
}
