using UnityEngine;

namespace Utage
{
	public class StringPopupIndexedAttribute : PropertyAttribute
	{
		public string Function { get; set; }

		public StringPopupIndexedAttribute(string function)
		{
			Function = function;
		}
	}
}
