using UnityEngine;

namespace Utage
{
	internal class AdvCommandSprite : AdvCommand
	{
		protected AdvGraphicInfoList graphic;

		protected string layerName;

		protected string spriteName;

		protected float fadeTime;

		public AdvCommandSprite(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			spriteName = ParseCell<string>(AdvColumnName.Arg1);
			string text = ParseCellOptional(AdvColumnName.Arg2, spriteName);
			if (!dataManager.TextureSetting.ContainsLabel(text))
			{
				Debug.LogError(ToErrorString(text + " is not contained in file setting"));
			}
			graphic = dataManager.TextureSetting.LabelToGraphic(text);
			AddLoadGraphic(graphic);
			layerName = ParseCellOptional(AdvColumnName.Arg3, "");
			if (string.IsNullOrEmpty(layerName))
			{
				layerName = dataManager.LayerSetting.FindDefaultLayer(AdvLayerSettingData.LayerType.Sprite).Name;
			}
			else if (!dataManager.LayerSetting.Contains(layerName))
			{
				Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
			}
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvGraphicOperaitonArg graphicOperaitonArg = new AdvGraphicOperaitonArg(this, graphic.Main, fadeTime);
			engine.GraphicManager.DrawObject(layerName, spriteName, graphicOperaitonArg);
			AdvGraphicObject advGraphicObject = engine.GraphicManager.FindObject(spriteName);
			if (advGraphicObject != null)
			{
				advGraphicObject.SetCommandPostion(this);
				advGraphicObject.TargetObject.SetCommandArg(this);
			}
		}
	}
}
