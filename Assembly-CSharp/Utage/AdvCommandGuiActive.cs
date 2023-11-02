using UnityEngine;

namespace Utage
{
	internal class AdvCommandGuiActive : AdvCommand
	{
		private string name;

		private bool isActive;

		public AdvCommandGuiActive(StringGridRow row)
			: base(row)
		{
			name = ParseCellOptional(AdvColumnName.Arg1, "");
			isActive = ParseCell<bool>(AdvColumnName.Arg2);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(name))
			{
				foreach (AdvGuiBase value in engine.UiManager.GuiManager.Objects.Values)
				{
					value.SetActive(isActive);
				}
				return;
			}
			if (!engine.UiManager.GuiManager.TryGet(name, out var gui))
			{
				Debug.LogError(ToErrorString(name + " is not found in GuiManager"));
			}
			else
			{
				gui.SetActive(isActive);
			}
		}
	}
}
