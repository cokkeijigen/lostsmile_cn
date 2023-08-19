using System;
using System.Collections;
using UnityEngine;

namespace Utage
{
	public abstract class EyeBlinkBase : MonoBehaviour
	{
		[SerializeField]
		[MinMax(0f, 10f, "min", "max")]
		private MinMaxFloat intervalTime = new MinMaxFloat
		{
			Min = 2f,
			Max = 6f
		};

		[SerializeField]
		[Range(0f, 1f)]
		private float randomDoubleEyeBlink = 0.2f;

		[SerializeField]
		[Range(0f, 1f)]
		private float intervalDoubleEyeBlink = 0.01f;

		[SerializeField]
		private string eyeTag = "eye";

		[SerializeField]
		private MiniAnimationData animationData = new MiniAnimationData();

		public MinMaxFloat IntervalTime
		{
			get
			{
				return intervalTime;
			}
			set
			{
				intervalTime = value;
			}
		}

		public float RandomDoubleEyeBlink
		{
			get
			{
				return randomDoubleEyeBlink;
			}
			set
			{
				randomDoubleEyeBlink = value;
			}
		}

		public string EyeTag
		{
			get
			{
				return eyeTag;
			}
			set
			{
				eyeTag = value;
			}
		}

		public MiniAnimationData AnimationData
		{
			get
			{
				return animationData;
			}
			set
			{
				animationData = value;
			}
		}

		private void Start()
		{
			StartWaiting();
		}

		private void StartWaiting()
		{
			float waitTime = intervalTime.RandomRange();
			StartCoroutine(CoUpateWaiting(waitTime));
		}

		private IEnumerator CoUpateWaiting(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			StartCoroutine(CoEyeBlink(OnEndBlink));
		}

		protected abstract IEnumerator CoEyeBlink(Action onComplete);

		private void OnEndBlink()
		{
			if (randomDoubleEyeBlink > UnityEngine.Random.value)
			{
				StartCoroutine(CoDoubleEyeBlink());
			}
			else
			{
				StartWaiting();
			}
		}

		private IEnumerator CoDoubleEyeBlink()
		{
			yield return new WaitForSeconds(intervalDoubleEyeBlink);
			StartCoroutine(CoEyeBlink(StartWaiting));
		}
	}
}
