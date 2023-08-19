using UnityEngine;

namespace Utage
{
	internal class AdvCommandJumpSubroutineRandom : AdvCommand
	{
		private string scenarioLabel;

		private int subroutineCommandIndex;

		private string jumpLabel;

		private string returnLabel;

		private ExpressionParser exp;

		private ExpressionParser expRate;

		public AdvCommandJumpSubroutineRandom(StringGridRow row, AdvSettingDataManager dataManager)
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
			returnLabel = (IsEmptyCell(AdvColumnName.Arg3) ? "" : ParseScenarioLabel(AdvColumnName.Arg3));
			string value2 = ParseCellOptional(AdvColumnName.Arg4, "");
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

		public override void InitFromPageData(AdvScenarioPageData pageData)
		{
			scenarioLabel = pageData.ScenarioLabelData.ScenarioLabel;
			subroutineCommandIndex = pageData.ScenarioLabelData.CountSubroutineCommandIndex(this);
		}

		public override string[] GetJumpLabels()
		{
			if (!string.IsNullOrEmpty(returnLabel))
			{
				return new string[2] { jumpLabel, returnLabel };
			}
			return new string[1] { jumpLabel };
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (IsEnable(engine.Param))
			{
				base.CurrentTread.JumpManager.AddRandom(this, CalcRate(engine.Param));
			}
		}

		internal void DoRandomEnd(AdvScenarioThread thread, AdvEngine engine)
		{
			SubRoutineInfo calledInfo = new SubRoutineInfo(engine, returnLabel, scenarioLabel, subroutineCommandIndex);
			thread.JumpManager.RegistoreSubroutine(jumpLabel, calledInfo);
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

		public override string[] GetExtraCommandIdArray(AdvCommand next)
		{
			if (next == null || !(next is AdvCommandJumpSubroutineRandom))
			{
				return new string[1] { "JumpSubroutineRandomEnd" };
			}
			return null;
		}
	}
}
