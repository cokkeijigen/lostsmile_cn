using UnityEngine;

namespace Utage
{
	public class ButtonAttribute : PropertyAttribute
	{
		public string Function { get; set; }

		public string Text { get; set; }

		public ButtonAttribute(string function, string text = "", int order = 0)
		{
			Function = function;
			Text = text;
			base.order = order;
		}
	}
}
