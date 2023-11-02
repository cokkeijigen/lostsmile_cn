using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[RequireComponent(typeof(DicingImage))]
	[AddComponentMenu("Utage/Lib/UI/DicingAnimation")]
	public class DicingAnimation : MonoBehaviour
	{
		[SerializeField]
		private bool playOnAwake;

		[SerializeField]
		[LimitEnum(new string[] { "Default", "Loop", "PingPong" })]
		private MotionPlayType wrapMode;

		[SerializeField]
		private bool reverse;

		[SerializeField]
		private float frameRate = 15f;

		private DicingImage dicing;

		private DicingImage Dicing => base.gameObject.GetComponentCache(ref dicing);

		private void Awake()
		{
			if (playOnAwake)
			{
				Play(null);
			}
		}

		public void Play(Action onComplete)
		{
			StartCoroutine(CoPlay(onComplete));
		}

		private IEnumerator CoPlay(Action onComplete)
		{
			List<string> list = Dicing.DicingData.GetPattenNameList();
			if (reverse)
			{
				list.Reverse();
			}
			if (list.Count > 0)
			{
				bool isEnd = false;
				while (!isEnd)
				{
					yield return CoPlayOnce(list);
					switch (wrapMode)
					{
					case MotionPlayType.Default:
						isEnd = true;
						break;
					case MotionPlayType.PingPong:
						list.Reverse();
						break;
					default:
						Debug.LogError("NotSupport");
						isEnd = true;
						break;
					case MotionPlayType.Loop:
						break;
					}
				}
			}
			onComplete?.Invoke();
		}

		private IEnumerator CoPlayOnce(List<string> patternList)
		{
			foreach (string pattern in patternList)
			{
				Dicing.ChangePattern(pattern);
				yield return new WaitForSeconds(1f / frameRate);
			}
		}
	}
}
