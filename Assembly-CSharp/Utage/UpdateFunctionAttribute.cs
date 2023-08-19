using UnityEngine;

namespace Utage
{
	public class UpdateFunctionAttribute : PropertyAttribute
	{
		public string Function { get; set; }

		public UpdateFunctionAttribute(string function, int order = 0)
		{
			Function = function;
			base.order = order;
		}
	}
}
