using UnityEngine;

namespace Utage
{
	public class AddButtonAttribute : PropertyAttribute
	{
		public string Function { get; set; }

		public string Text { get; set; }

		public AddButtonAttribute(string function, string text = "Button", int order = 0)
		{
			Function = function;
			Text = text;
			base.order = order;
		}
	}
}
