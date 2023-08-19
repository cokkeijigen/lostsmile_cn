namespace Utage
{
	internal class AdvCommandCharacterOff : AdvCommand
	{
		private string name;

		private float time;

		public AdvCommandCharacterOff(StringGridRow row)
			: base(row)
		{
			name = ParseCellOptional(AdvColumnName.Arg1, "");
			time = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			float fadeTime = engine.Page.ToSkippedTime(time);
			AdvGraphicGroup characterManager = engine.GraphicManager.CharacterManager;
			if (string.IsNullOrEmpty(name))
			{
				characterManager.FadeOutAll(fadeTime);
				return;
			}
			AdvGraphicLayer advGraphicLayer = characterManager.FindLayerFromObjectName(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.FadeOut(name, fadeTime);
				return;
			}
			advGraphicLayer = characterManager.FindLayer(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.FadeOutAll(fadeTime);
			}
		}
	}
}
