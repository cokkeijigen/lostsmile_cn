using UnityEngine;

namespace Utage
{
	internal class AdvCommandGuiReset : AdvCommand
	{
		private string name;

		public AdvCommandGuiReset(StringGridRow row)
			: base(row)
		{
			name = ParseCellOptional(AdvColumnName.Arg1, "");
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(name))
			{
				foreach (AdvGuiBase value in engine.UiManager.GuiManager.Objects.Values)
				{
					value.Reset();
				}
				return;
			}
			AdvGuiBase gui;
			if (!engine.UiManager.GuiManager.TryGet(name, out gui))
			{
				Debug.LogError(ToErrorString(name + " is not found in GuiManager"));
			}
			else
			{
				gui.Reset();
			}
		}
	}
}
