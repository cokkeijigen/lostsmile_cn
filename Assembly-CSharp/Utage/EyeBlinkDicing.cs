using System;
using System.Collections;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[RequireComponent(typeof(DicingImage))]
	[AddComponentMenu("Utage/Lib/UI/EyeBlinkDicing")]
	public class EyeBlinkDicing : EyeBlinkBase
	{
		private DicingImage dicing;

		private DicingImage Dicing => base.gameObject.GetComponentCache(ref dicing);

		protected override IEnumerator CoEyeBlink(Action onComplete)
		{
			foreach (MiniAnimationData.Data data in base.AnimationData.DataList)
			{
				Dicing.TryChangePatternWithOption(Dicing.MainPattern, base.EyeTag, data.ComvertNameSimple());
				yield return new WaitForSeconds(data.Duration);
			}
			Dicing.TryChangePatternWithOption(Dicing.MainPattern, base.EyeTag, "");
			onComplete?.Invoke();
		}
	}
}
