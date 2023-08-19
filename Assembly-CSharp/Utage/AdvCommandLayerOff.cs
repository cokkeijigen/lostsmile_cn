using UnityEngine;

namespace Utage
{
	internal class AdvCommandLayerOff : AdvCommand
	{
		private string name;

		private float fadeTime;

		public AdvCommandLayerOff(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			name = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.LayerSetting.Contains(name))
			{
				Debug.LogError(row.ToErrorString("Not found " + name + " Please input Layer name"));
			}
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvGraphicLayer advGraphicLayer = engine.GraphicManager.FindLayer(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.FadeOutAll(engine.Page.ToSkippedTime(fadeTime));
			}
			else
			{
				Debug.LogError("Not found " + name + " Please input Layer name");
			}
		}
	}
}
