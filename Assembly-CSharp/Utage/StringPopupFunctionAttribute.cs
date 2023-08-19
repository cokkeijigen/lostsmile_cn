using UnityEngine;

namespace Utage
{
	public class StringPopupFunctionAttribute : PropertyAttribute
	{
		public string Function { get; set; }

		public StringPopupFunctionAttribute(string function)
		{
			Function = function;
		}
	}
}
