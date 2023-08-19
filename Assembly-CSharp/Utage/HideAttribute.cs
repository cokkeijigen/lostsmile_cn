using UnityEngine;

namespace Utage
{
	public class HideAttribute : PropertyAttribute
	{
		public string Function { get; set; }

		public HideAttribute(string function = "")
		{
			Function = function;
		}
	}
}
