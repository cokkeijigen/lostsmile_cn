using UnityEngine;

namespace Utage
{
	public class MinMax<T>
	{
		[SerializeField]
		private T min;

		[SerializeField]
		private T max;

		public T Min
		{
			get
			{
				return min;
			}
			set
			{
				min = value;
			}
		}

		public T Max
		{
			get
			{
				return max;
			}
			set
			{
				max = value;
			}
		}
	}
}
