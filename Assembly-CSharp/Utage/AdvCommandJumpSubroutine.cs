using UnityEngine;

namespace Utage
{
	internal class AdvCommandJumpSubroutine : AdvCommand
	{
		private string scenarioLabel;

		private int subroutineCommandIndex;

		private string jumpLabel;

		private string returnLabel;

		private ExpressionParser exp;

		public AdvCommandJumpSubroutine(StringGridRow row, AdvSettingDataManager dataManager)
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
				SubRoutineInfo calledInfo = new SubRoutineInfo(engine, returnLabel, scenarioLabel, subroutineCommandIndex);
				base.CurrentTread.JumpManager.RegistoreSubroutine(jumpLabel, calledInfo);
			}
		}

		public override bool IsTypePage()
		{
			return true;
		}

		public override bool IsTypePageEnd()
		{
			return true;
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
