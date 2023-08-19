using UnityEngine;

namespace Utage
{
	public class PathDialogAttribute : PropertyAttribute
	{
		public enum DialogType
		{
			Directory,
			File
		}

		public DialogType Type { get; private set; }

		public string Extention { get; private set; }

		public PathDialogAttribute(DialogType type)
		{
			Type = type;
			Extention = "";
		}

		public PathDialogAttribute(DialogType type, string extention)
		{
			Type = type;
			Extention = extention;
		}
	}
}
