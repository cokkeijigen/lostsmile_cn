using UnityEngine;

namespace Utage
{
	public class HelpBoxAttribute : PropertyAttribute
	{
		public enum Type
		{
			None,
			Info,
			Warning,
			Error
		}

		public string Message { get; set; }

		public Type MessageType { get; set; }

		public HelpBoxAttribute(string message, Type type = Type.None, int order = 0)
		{
			Message = message;
			MessageType = type;
			base.order = order;
		}
	}
}
