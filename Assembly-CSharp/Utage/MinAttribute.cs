using UnityEngine;

namespace Utage
{
	public class MinAttribute : PropertyAttribute
	{
		public float Min { get; private set; }

		public MinAttribute(float min)
		{
			Min = min;
		}
	}
}
