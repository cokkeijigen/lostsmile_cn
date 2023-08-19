namespace Utage
{
	internal class AdvCommandParticleOff : AdvCommand
	{
		private string name;

		public AdvCommandParticleOff(StringGridRow row)
			: base(row)
		{
			name = ParseCellOptional(AdvColumnName.Arg1, "");
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(name))
			{
				engine.GraphicManager.FadeOutAllParticle();
			}
			else
			{
				engine.GraphicManager.FadeOutParticle(name);
			}
		}
	}
}
