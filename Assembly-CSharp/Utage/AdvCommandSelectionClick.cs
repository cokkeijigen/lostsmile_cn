using UnityEngine;

namespace Utage
{
	internal class AdvCommandSelectionClick : AdvCommand
	{
		private string name;

		private bool isPolygon;

		private string jumpLabel;

		private ExpressionParser exp;

		private ExpressionParser selectedExp;

		public AdvCommandSelectionClick(StringGridRow row, AdvSettingDataManager dataManager)
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
				selectedExp = null;
			}
			else
			{
				selectedExp = dataManager.DefaultParam.CreateExpression(value2);
				if (selectedExp.ErrorMsg != null)
				{
					Debug.LogError(ToErrorString(selectedExp.ErrorMsg));
				}
			}
			name = ParseCell<string>(AdvColumnName.Arg4);
			isPolygon = ParseCellOptional(AdvColumnName.Arg5, true);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (exp == null || engine.Param.CalcExpressionBoolean(exp))
			{
				engine.SelectionManager.AddSelectionClick(jumpLabel, name, isPolygon, selectedExp, base.RowData);
			}
		}

		public override string[] GetJumpLabels()
		{
			return new string[1] { jumpLabel };
		}

		public override string[] GetExtraCommandIdArray(AdvCommand next)
		{
			if (next != null && (next is AdvCommandSelection || next is AdvCommandSelectionClick))
			{
				return null;
			}
			if (!AdvPageController.IsPageEndType(ParseCellOptional(AdvColumnName.PageCtrl, AdvPageControllerType.InputBrPage)))
			{
				return new string[1] { "SelectionEnd" };
			}
			return new string[2] { "SelectionEnd", "PageControl" };
		}

		public override bool IsTypePage()
		{
			return true;
		}
	}
}
