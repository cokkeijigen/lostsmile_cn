namespace Utage
{
	public class AdvGraphicOperaitonArg
	{
		private float FadeTime { get; set; }

		private AdvCommand Command { get; set; }

		public AdvGraphicInfo Graphic { get; private set; }

		public float GetSkippedFadeTime(AdvEngine engine)
		{
			return engine.Page.ToSkippedTime(FadeTime);
		}

		internal AdvGraphicOperaitonArg(AdvCommand command, AdvGraphicInfo graphic, float fadeTime)
		{
			Command = command;
			Graphic = graphic;
			FadeTime = fadeTime;
		}
	}
}
