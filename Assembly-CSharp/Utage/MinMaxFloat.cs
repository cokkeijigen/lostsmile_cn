using System;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class MinMaxFloat : MinMax<float>
	{
		public float RandomRange()
		{
			return UnityEngine.Random.Range(base.Min, base.Max);
		}

		public float Clamp(float value)
		{
			return Mathf.Clamp(value, base.Min, base.Max);
		}

		public bool IsInnner(float v)
		{
			if (v >= base.Min)
			{
				return v <= base.Max;
			}
			return false;
		}

		public bool IsOver(float v)
		{
			if (!(v < base.Min))
			{
				return v > base.Max;
			}
			return true;
		}
	}
}
