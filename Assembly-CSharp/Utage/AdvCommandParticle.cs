using UnityEngine;

namespace Utage
{
	internal class AdvCommandParticle : AdvCommand
	{
		protected string label;

		protected string layerName;

		protected AdvGraphicInfo graphic;

		protected AdvGraphicOperaitonArg graphicOperaitonArg;

		public AdvCommandParticle(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			label = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.ParticleSetting.Dictionary.ContainsKey(label))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}
			graphic = dataManager.ParticleSetting.LabelToGraphic(label);
			AddLoadGraphic(graphic);
			layerName = ParseCellOptional(AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName))
			{
				Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
			}
			graphicOperaitonArg = new AdvGraphicOperaitonArg(this, graphic, 0f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			string name = layerName;
			if (string.IsNullOrEmpty(name))
			{
				name = engine.GraphicManager.SpriteManager.DefaultLayer.name;
			}
			engine.GraphicManager.DrawObject(name, label, graphicOperaitonArg);
			AdvGraphicObject advGraphicObject = engine.GraphicManager.FindObject(label);
			if (advGraphicObject != null)
			{
				advGraphicObject.SetCommandPostion(this);
				advGraphicObject.TargetObject.SetCommandArg(this);
			}
		}
	}
}
