using UnityEngine;

namespace Utage
{
	public class EaseValue
	{
		public float Duration { get; private set; }

		public EaseType EaseType { get; private set; }

		private float Goal { get; set; }

		private float Start { get; set; }

		public float Current { get; private set; }

		private float CurrentTime { get; set; }

		public EaseValue(float duration, EaseType easeType = EaseType.Linear)
		{
			Duration = duration;
			EaseType = easeType;
		}

		public void Reset(float current)
		{
			Current = current;
			CurrentTime = 0f;
		}

		public float Update(float value)
		{
			return Update(value, Duration, EaseType, Time.deltaTime);
		}

		public float Update(float value, float duration, EaseType easeType, float deltaTime)
		{
			if (Current == value)
			{
				return value;
			}
			if (Goal != value)
			{
				CurrentTime = 0f;
				Start = Current;
				Goal = value;
			}
			CurrentTime += deltaTime;
			if (CurrentTime >= duration || Current == Goal)
			{
				Current = Goal;
			}
			else
			{
				Current = Easing.GetCurve(Start, Goal, CurrentTime / duration, easeType);
			}
			return Current;
		}
	}
}
