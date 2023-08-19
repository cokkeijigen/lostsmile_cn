using System;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class MinMaxInt : MinMax<int>
	{
		public int RandomRange()
		{
			return UnityEngine.Random.Range(base.Min, base.Max + 1);
		}

		public int Clamp(int value)
		{
			return Mathf.Clamp(value, base.Min, base.Max);
		}

		public bool IsInnner(int v)
		{
			if (v >= base.Min)
			{
				return v <= base.Max;
			}
			return false;
		}

		public bool IsOver(int v)
		{
			if (v >= base.Min)
			{
				return v > base.Max;
			}
			return true;
		}
	}
}
