using UnityEngine;

namespace Utage
{
	public class OverridePropertyDrawAttribute : PropertyAttribute
	{
		public string Function { get; set; }

		public OverridePropertyDrawAttribute(string function)
		{
			Function = function;
		}
	}
}
