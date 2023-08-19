using UnityEngine;

namespace Utage
{
	internal class AdvCommandBgEvent : AdvCommand
	{
		private string label;

		private AdvGraphicInfoList graphic;

		private float fadeTime;

		public AdvCommandBgEvent(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			label = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.TextureSetting.ContainsLabel(label))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}
			graphic = dataManager.TextureSetting.LabelToGraphic(label);
			AddLoadGraphic(graphic);
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvGraphicOperaitonArg advGraphicOperaitonArg = new AdvGraphicOperaitonArg(this, graphic.Main, fadeTime);
			engine.SystemSaveData.GalleryData.AddCgLabel(label);
			engine.GraphicManager.IsEventMode = true;
			engine.GraphicManager.CharacterManager.FadeOutAll(advGraphicOperaitonArg.GetSkippedFadeTime(engine));
			engine.GraphicManager.BgManager.DrawToDefault(engine.GraphicManager.BgSpriteName, advGraphicOperaitonArg);
			AdvGraphicObject advGraphicObject = engine.GraphicManager.BgManager.FindObject(engine.GraphicManager.BgSpriteName);
			if (advGraphicObject != null)
			{
				advGraphicObject.SetCommandPostion(this);
				advGraphicObject.TargetObject.SetCommandArg(this);
			}
		}
	}
}
