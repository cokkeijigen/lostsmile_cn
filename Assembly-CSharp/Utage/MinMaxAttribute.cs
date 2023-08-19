using UnityEngine;

namespace Utage
{
	public class MinMaxAttribute : PropertyAttribute
	{
		public float Min { get; private set; }

		public float Max { get; private set; }

		public string MinPropertyName { get; private set; }

		public string MaxPropertyName { get; private set; }

		public MinMaxAttribute(float min, float max, string minPropertyName = "min", string maxPropertyName = "max")
		{
			Min = min;
			Max = max;
			MinPropertyName = minPropertyName;
			MaxPropertyName = maxPropertyName;
		}
	}
}
