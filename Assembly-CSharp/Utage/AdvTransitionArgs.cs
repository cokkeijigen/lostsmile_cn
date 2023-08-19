namespace Utage
{
	public class AdvTransitionArgs
	{
		internal string TextureName { get; private set; }

		internal float Vague { get; private set; }

		private float Time { get; set; }

		internal AdvTransitionArgs(string textureName, float vague, float time)
		{
			TextureName = textureName;
			Vague = vague;
			Time = time;
		}

		internal float GetSkippedTime(AdvEngine engine)
		{
			return engine.Page.ToSkippedTime(Time);
		}
	}
}
