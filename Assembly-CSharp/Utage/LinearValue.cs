using System;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class LinearValue
	{
		private float time;

		private float timeCurrent;

		private float valueBegin;

		private float valueEnd;

		private float valueCurrent;

		public void Init(float time, float valueBegin, float valueEnd)
		{
			this.time = time;
			timeCurrent = 0f;
			this.valueBegin = valueBegin;
			this.valueEnd = valueEnd;
			valueCurrent = valueBegin;
		}

		public void Clear()
		{
			time = 0f;
			timeCurrent = 0f;
			valueBegin = 0f;
			valueEnd = 0f;
			valueCurrent = 0f;
		}

		public void IncTime()
		{
			if (!IsEnd())
			{
				timeCurrent = Mathf.Min(timeCurrent + Time.deltaTime, time);
				valueCurrent = Mathf.Lerp(valueBegin, valueEnd, timeCurrent / time);
			}
		}

		public bool IsEnd()
		{
			return timeCurrent >= time;
		}

		public float GetValue()
		{
			return valueCurrent;
		}
	}
}
