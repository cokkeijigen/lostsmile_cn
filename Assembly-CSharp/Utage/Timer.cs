using System;
using System.Collections;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Sound/Timer")]
	public class Timer : MonoBehaviour
	{
		[SerializeField]
		private float duration;

		[SerializeField]
		private float delay;

		[SerializeField]
		[NotEditable]
		private float time;

		[SerializeField]
		[NotEditable]
		private float time01;

		public TimerEvent onStart = new TimerEvent();

		public TimerEvent onUpdate = new TimerEvent();

		public TimerEvent onComplete = new TimerEvent();

		[SerializeField]
		private bool autoDestroy;

		[SerializeField]
		private bool autoStart;

		private Action<Timer> callbackUpdate;

		private Action<Timer> callbackComplete;

		public float Duration
		{
			get
			{
				return duration;
			}
			protected set
			{
				duration = value;
			}
		}

		public float Delay
		{
			get
			{
				return delay;
			}
			protected set
			{
				delay = value;
			}
		}

		public float Time
		{
			get
			{
				return time;
			}
			protected set
			{
				time = value;
			}
		}

		public float Time01
		{
			get
			{
				return time01;
			}
			protected set
			{
				time01 = value;
			}
		}

		public float Time01Inverse
		{
			get
			{
				return 1f - Time01;
			}
		}

		public bool AutoDestroy
		{
			get
			{
				return autoDestroy;
			}
			set
			{
				autoDestroy = value;
			}
		}

		public float GetCurve01(EaseType easeType = EaseType.Linear)
		{
			return Easing.GetCurve01(Time01, easeType);
		}

		public float GetCurve01Inverse(EaseType easeType = EaseType.Linear)
		{
			return Easing.GetCurve01(Time01Inverse, easeType);
		}

		public float GetCurve(float start, float end)
		{
			return GetCurve(start, end, EaseType.Linear);
		}

		public float GetCurve(float start, float end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01, easeType);
		}

		public Vector2 GetCurve(Vector2 start, Vector2 end)
		{
			return GetCurve(start, end, EaseType.Linear);
		}

		public Vector2 GetCurve(Vector2 start, Vector2 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01, easeType);
		}

		public Vector3 GetCurve(Vector3 start, Vector3 end)
		{
			return GetCurve(start, end, EaseType.Linear);
		}

		public Vector3 GetCurve(Vector3 start, Vector3 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01, easeType);
		}

		public Vector4 GetCurve(Vector4 start, Vector4 end)
		{
			return GetCurve(start, end, EaseType.Linear);
		}

		public Vector4 GetCurve(Vector4 start, Vector4 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01, easeType);
		}

		public float GetCurveInverse(float start, float end)
		{
			return GetCurveInverse(start, end, EaseType.Linear);
		}

		public float GetCurveInverse(float start, float end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01Inverse, easeType);
		}

		public Vector2 GetCurveInverse(Vector2 start, Vector2 end)
		{
			return GetCurveInverse(start, end, EaseType.Linear);
		}

		public Vector2 GetCurveInverse(Vector2 start, Vector2 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01Inverse, easeType);
		}

		public Vector3 GetCurveInverse(Vector3 start, Vector3 end)
		{
			return GetCurveInverse(start, end, EaseType.Linear);
		}

		public Vector3 GetCurveInverse(Vector3 start, Vector3 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01Inverse, easeType);
		}

		public Vector4 GetCurveInverse(Vector4 start, Vector4 end)
		{
			return GetCurveInverse(start, end, EaseType.Linear);
		}

		public Vector4 GetCurveInverse(Vector4 start, Vector4 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01Inverse, easeType);
		}

		private void Start()
		{
			if (autoStart)
			{
				StartCoroutine(CoTimer(duration, delay));
			}
		}

		public void Cancel()
		{
			OnCompleteCallback();
			Stop();
		}

		public void StartTimer(float duration, Action<Timer> onUpdate = null, Action<Timer> onComplete = null, float delay = 0f)
		{
			callbackUpdate = onUpdate;
			callbackComplete = onComplete;
			StartTimer(duration, delay);
		}

		public void StartTimer(float duration, float delay = 0f)
		{
			autoStart = false;
			Stop();
			StartCoroutine(CoTimer(duration, delay));
		}

		private IEnumerator CoTimer(float duration, float delay)
		{
			this.duration = duration;
			this.delay = delay;
			yield return new WaitTimer(duration, delay, OnStart, OnUpdate, OnComplete);
		}

		private void Stop()
		{
			StopAllCoroutines();
		}

		private void OnStart(WaitTimer timer)
		{
			onStart.Invoke(this);
		}

		private void OnUpdate(WaitTimer timer)
		{
			Time = timer.Time;
			Time01 = timer.Time01;
			onUpdate.Invoke(this);
			if (callbackUpdate != null)
			{
				callbackUpdate(this);
			}
		}

		private void OnComplete(WaitTimer timer)
		{
			OnCompleteCallback();
			if (AutoDestroy)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void OnCompleteCallback()
		{
			onComplete.Invoke(this);
			if (callbackComplete != null)
			{
				callbackComplete(this);
			}
			callbackComplete = null;
		}
	}
}
