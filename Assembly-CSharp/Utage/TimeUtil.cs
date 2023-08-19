using UnityEngine;

namespace Utage
{
	public static class TimeUtil
	{
		public static float GetTime(bool unscaled)
		{
			if (!unscaled)
			{
				return Time.time;
			}
			return Time.unscaledTime;
		}

		public static float GetDeltaTime(bool unscaled)
		{
			if (!unscaled)
			{
				return Time.deltaTime;
			}
			return Time.unscaledDeltaTime;
		}
	}
}
