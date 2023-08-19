using System;
using UnityEngine;

namespace Utage
{
	public class InterfaceAttribute : PropertyAttribute
	{
		public Type Type { get; set; }

		public InterfaceAttribute(Type type)
		{
			Type = type;
		}
	}
}
