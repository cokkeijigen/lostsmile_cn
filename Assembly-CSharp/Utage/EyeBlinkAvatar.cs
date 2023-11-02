using System;
using System.Collections;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/EyeBlinkAvatar")]
	[RequireComponent(typeof(AvatarImage))]
	public class EyeBlinkAvatar : EyeBlinkBase
	{
		private AvatarImage avator;

		private AvatarImage Avator => base.gameObject.GetComponentCache(ref avator);

		protected override IEnumerator CoEyeBlink(Action onComplete)
		{
			string pattern = AvatarData.ToPatternName(Avator.AvatarPattern.GetPatternName(base.EyeTag));
			if (string.IsNullOrEmpty(pattern))
			{
				onComplete?.Invoke();
				yield break;
			}
			foreach (MiniAnimationData.Data data in base.AnimationData.DataList)
			{
				Avator.ChangePattern(base.EyeTag, data.ComvertName(pattern));
				yield return new WaitForSeconds(data.Duration);
			}
			Avator.ChangePattern(base.EyeTag, pattern);
			onComplete?.Invoke();
		}
	}
}
