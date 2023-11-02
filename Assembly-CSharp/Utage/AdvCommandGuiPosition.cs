using UnityEngine;

namespace Utage
{
	internal class AdvCommandGuiPosition : AdvCommand
	{
		private string name;

		private float? x;

		private float? y;

		public AdvCommandGuiPosition(StringGridRow row)
			: base(row)
		{
			name = ParseCell<string>(AdvColumnName.Arg1);
			x = ParseCellOptionalNull<float>(AdvColumnName.Arg2);
			y = ParseCellOptionalNull<float>(AdvColumnName.Arg3);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (!engine.UiManager.GuiManager.TryGet(name, out var gui))
			{
				Debug.LogError(ToErrorString(name + " is not found in GuiManager"));
			}
			else
			{
				gui.SetPosition(x, y);
			}
		}
	}
}
