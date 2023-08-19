using UnityEngine;

namespace Utage
{
	internal class AdvCommandGuiSize : AdvCommand
	{
		private string name;

		private float? x;

		private float? y;

		public AdvCommandGuiSize(StringGridRow row)
			: base(row)
		{
			name = ParseCellOptional(AdvColumnName.Arg1, "");
			x = ParseCellOptionalNull<float>(AdvColumnName.Arg2);
			y = ParseCellOptionalNull<float>(AdvColumnName.Arg3);
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvGuiBase gui;
			if (!engine.UiManager.GuiManager.TryGet(name, out gui))
			{
				Debug.LogError(ToErrorString(name + " is not found in GuiManager"));
			}
			else
			{
				gui.SetSize(x, y);
			}
		}
	}
}
