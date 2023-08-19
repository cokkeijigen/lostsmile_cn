using UnityEngine;

namespace Utage
{
	public class NotEditableAttribute : PropertyAttribute
	{
		public string EnablePropertyPath { get; private set; }

		public bool IsEnableProperty { get; private set; }

		public NotEditableAttribute()
		{
		}

		public NotEditableAttribute(string enablePropertyPath, bool isEnableProperty = true)
		{
			EnablePropertyPath = enablePropertyPath;
			IsEnableProperty = isEnableProperty;
		}
	}
}
