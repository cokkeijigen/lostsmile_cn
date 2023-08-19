using UnityEngine;

namespace Utage
{
	internal class AdvCommandLayerReset : AdvCommand
	{
		private string name;

		public AdvCommandLayerReset(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			name = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.LayerSetting.Contains(name))
			{
				Debug.LogError(row.ToErrorString("Not found " + name + " Please input Layer name"));
			}
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvGraphicLayer advGraphicLayer = engine.GraphicManager.FindLayer(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.ResetCanvasRectTransform();
			}
			else
			{
				Debug.LogError("Not found " + name + " Please input Layer name");
			}
		}
	}
}
