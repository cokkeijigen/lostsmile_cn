using System;
using System.Collections;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Effect/CurveAnimation")]
	public class CurveAnimation : MonoBehaviour
	{
		[SerializeField]
		private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[SerializeField]
		private float delay;

		[SerializeField]
		private float duration = 1f;

		[SerializeField]
		private bool unscaledTime = true;

		[SerializeField]
		private CurveAnimationEvent onStart = new CurveAnimationEvent();

		[SerializeField]
		private CurveAnimationEvent onUpdate = new CurveAnimationEvent();

		[SerializeField]
		private CurveAnimationEvent onComplete = new CurveAnimationEvent();

		private Coroutine currentCoroutine;

		public AnimationCurve Curve
		{
			get
			{
				return curve;
			}
		}

		public float Delay
		{
			get
			{
				return delay;
			}
			set
			{
				delay = value;
			}
		}

		public float Duration
		{
			get
			{
				return duration;
			}
			set
			{
				duration = value;
			}
		}

		public bool UnscaledTime
		{
			get
			{
				return unscaledTime;
			}
			set
			{
				unscaledTime = value;
			}
		}

		public float Value { get; set; }

		public bool IsPlaying { get; protected set; }

		public CurveAnimationEvent OnStart
		{
			get
			{
				return onStart;
			}
		}

		public CurveAnimationEvent OnUpdate
		{
			get
			{
				return onUpdate;
			}
		}

		public CurveAnimationEvent OnComplete
		{
			get
			{
				return onComplete;
			}
		}

		protected float Time
		{
			get
			{
				return TimeUtil.GetTime(UnscaledTime);
			}
		}

		protected float DeltaTime
		{
			get
			{
				return TimeUtil.GetDeltaTime(UnscaledTime);
			}
		}

		protected float CurrentAnimationTime { get; set; }

		public float LerpValue(float from, float to)
		{
			return Mathf.Lerp(from, to, Value);
		}

		public void PlayAnimation()
		{
			PlayAnimation(null, null);
		}

		public void PlayAnimation(Action<float> onUpdate = null, Action onComplete = null)
		{
			if (IsPlaying)
			{
				StopCoroutine(currentCoroutine);
			}
			currentCoroutine = StartCoroutine(CoAnimation(onUpdate, onComplete));
		}

		private IEnumerator CoAnimation(Action<float> onUpdate, Action onComplete)
		{
			IsPlaying = true;
			if (Delay >= 0f)
			{
				float delayStartTime = Time;
				while (Time - delayStartTime < Delay)
				{
					yield return null;
				}
			}
			float endTime = Curve.keys[Curve.length - 1].time;
			Value = Curve.Evaluate(0f);
			OnStart.Invoke(this);
			float startTime = Time;
			for (CurrentAnimationTime = 0f; CurrentAnimationTime < Duration; CurrentAnimationTime = Time - startTime)
			{
				Value = Curve.Evaluate(endTime * CurrentAnimationTime / Duration);
				if (onUpdate != null)
				{
					onUpdate(Value);
				}
				OnUpdate.Invoke(this);
				yield return null;
			}
			Value = Curve.Evaluate(endTime);
			if (onUpdate != null)
			{
				onUpdate(Value);
			}
			OnUpdate.Invoke(this);
			if (onComplete != null)
			{
				onComplete();
			}
			OnComplete.Invoke(this);
			IsPlaying = false;
			currentCoroutine = null;
		}
	}
}
