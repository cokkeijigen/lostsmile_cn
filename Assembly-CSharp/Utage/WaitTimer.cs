using UnityEngine;
using UnityEngine.Events;

namespace Utage
{
	public class WaitTimer : CustomYieldInstruction
	{
		private float duration;

		private float delay;

		private float initTime;

		private bool isStarted;

		private UnityAction<WaitTimer> onStart;

		private UnityAction<WaitTimer> onUpdate;

		private UnityAction<WaitTimer> onComplete;

		public float Time { get; protected set; }

		public float Time01 { get; protected set; }

		private float StartTimeDelyed => initTime + delay;

		private float EndTime => StartTimeDelyed + duration;

		public override bool keepWaiting => Waiting();

		public WaitTimer(float duration, UnityAction<WaitTimer> onStart = null, UnityAction<WaitTimer> onUpdate = null, UnityAction<WaitTimer> onComplete = null)
		{
			Init(duration, 0f, onStart, onUpdate, onComplete);
		}

		public WaitTimer(float duration, float delay, UnityAction<WaitTimer> onStart = null, UnityAction<WaitTimer> onUpdate = null, UnityAction<WaitTimer> onComplete = null)
		{
			Init(duration, delay, onStart, onUpdate, onComplete);
		}

		private void Init(float duration, float delay, UnityAction<WaitTimer> onStart, UnityAction<WaitTimer> onUpdate, UnityAction<WaitTimer> onComplete)
		{
			this.duration = duration;
			this.delay = delay;
			initTime = UnityEngine.Time.time;
			this.onStart = onStart;
			this.onUpdate = onUpdate;
			this.onComplete = onComplete;
		}

		private bool Waiting()
		{
			float time = UnityEngine.Time.time;
			if (time < StartTimeDelyed)
			{
				return true;
			}
			Time = time - StartTimeDelyed;
			if (duration == 0f)
			{
				Time01 = 1f;
			}
			else
			{
				Time01 = Mathf.Clamp01(Time / duration);
			}
			if (!isStarted)
			{
				if (onStart != null)
				{
					onStart(this);
				}
				isStarted = true;
			}
			if (onUpdate != null)
			{
				onUpdate(this);
			}
			if (time >= EndTime)
			{
				if (onComplete != null)
				{
					onComplete(this);
				}
				return false;
			}
			return true;
		}
	}
}
