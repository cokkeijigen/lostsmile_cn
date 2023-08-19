using UnityEngine;

namespace Utage
{
	public class LimitEnumAttribute : PropertyAttribute
	{
		public string[] Args { get; private set; }

		public LimitEnumAttribute(params string[] args)
		{
			Args = args;
		}
	}
}
