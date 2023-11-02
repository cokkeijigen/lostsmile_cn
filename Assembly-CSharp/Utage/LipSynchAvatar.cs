using System.Collections;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/LipSynchAvatar")]
	public class LipSynchAvatar : LipSynch2d
	{
		private AvatarImage avator;

		private AvatarImage Avator => base.gameObject.GetComponentCache(ref avator);

		protected override IEnumerator CoUpdateLipSync()
		{
			while (base.IsPlaying)
			{
				string pattern = AvatarData.ToPatternName(Avator.AvatarPattern.GetPatternName(base.LipTag));
				if (string.IsNullOrEmpty(pattern))
				{
					break;
				}
				foreach (MiniAnimationData.Data data in base.AnimationData.DataList)
				{
					Avator.ChangePattern(base.LipTag, data.ComvertName(pattern));
					yield return new WaitForSeconds(data.Duration);
				}
				Avator.ChangePattern(base.LipTag, pattern);
				if (!base.IsPlaying)
				{
					break;
				}
				yield return new WaitForSeconds(base.Interval);
			}
			coLypSync = null;
		}

		protected override void OnStopLipSync()
		{
			base.OnStopLipSync();
			string text = AvatarData.ToPatternName(Avator.AvatarPattern.GetPatternName(base.LipTag));
			if (!string.IsNullOrEmpty(text))
			{
				Avator.ChangePattern(base.LipTag, text);
			}
		}
	}
}
