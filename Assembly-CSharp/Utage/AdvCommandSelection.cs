using UnityEngine;

namespace Utage
{
	public class AdvCommandSelection : AdvCommand
	{
		private string jumpLabel;

		private ExpressionParser exp;

		private ExpressionParser selectedExp;

		private string prefabName;

		private float? x;

		private float? y;

		public AdvCommandSelection(StringGridRow row, AdvSettingDataManager dataManager)
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
			prefabName = ParseCellOptional(AdvColumnName.Arg4, "");
			x = ParseCellOptionalNull<float>(AdvColumnName.Arg5);
			y = ParseCellOptionalNull<float>(AdvColumnName.Arg6);
			if (AdvCommand.IsEditorErrorCheck)
			{
				TextData textData = new TextData(ParseCellLocalizedText());
				if (!string.IsNullOrEmpty(textData.ErrorMsg))
				{
					Debug.LogError(ToErrorString(textData.ErrorMsg));
				}
			}
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (exp == null || engine.Param.CalcExpressionBoolean(exp))
			{
				engine.SelectionManager.AddSelection(jumpLabel, ParseCellLocalizedText(), selectedExp, prefabName, x, y, base.RowData);
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
