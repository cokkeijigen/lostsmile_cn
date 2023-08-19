using UnityEngine;

namespace Utage
{
	internal class AdvCommandBg : AdvCommand
	{
		protected AdvGraphicInfoList graphic;

		protected string layerName;

		protected float fadeTime;

		public AdvCommandBg(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			string text = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.TextureSetting.ContainsLabel(text))
			{
				Debug.LogError(ToErrorString(text + " is not contained in file setting"));
			}
			graphic = dataManager.TextureSetting.LabelToGraphic(text);
			AddLoadGraphic(graphic);
			layerName = ParseCellOptional(AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName, AdvLayerSettingData.LayerType.Bg))
			{
				Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
			}
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvGraphicOperaitonArg arg = new AdvGraphicOperaitonArg(this, graphic.Main, fadeTime);
			engine.GraphicManager.IsEventMode = false;
			if (string.IsNullOrEmpty(layerName))
			{
				engine.GraphicManager.BgManager.DrawToDefault(engine.GraphicManager.BgSpriteName, arg);
			}
			else
			{
				engine.GraphicManager.BgManager.Draw(layerName, engine.GraphicManager.BgSpriteName, arg);
			}
			AdvGraphicObject advGraphicObject = engine.GraphicManager.BgManager.FindObject(engine.GraphicManager.BgSpriteName);
			if (advGraphicObject != null)
			{
				advGraphicObject.SetCommandPostion(this);
				advGraphicObject.TargetObject.SetCommandArg(this);
			}
		}
	}
}
