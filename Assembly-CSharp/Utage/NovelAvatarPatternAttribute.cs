using UnityEngine;

namespace Utage
{
	public class NovelAvatarPatternAttribute : PropertyAttribute
	{
		public string Function { get; set; }

		public NovelAvatarPatternAttribute(string function)
		{
			Function = function;
		}
	}
}
