namespace Utage
{
	internal class AdvCommandSpriteOff : AdvCommand
	{
		private string name;

		private float fadeTime = 0.2f;

		public AdvCommandSpriteOff(StringGridRow row)
			: base(row)
		{
			name = ParseCellOptional(AdvColumnName.Arg1, "");
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, fadeTime);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(name))
			{
				engine.GraphicManager.SpriteManager.FadeOutAll(engine.Page.ToSkippedTime(fadeTime));
				return;
			}
			AdvGraphicLayer advGraphicLayer = engine.GraphicManager.FindLayerByObjectName(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.FadeOut(name, engine.Page.ToSkippedTime(fadeTime));
			}
		}
	}
}
