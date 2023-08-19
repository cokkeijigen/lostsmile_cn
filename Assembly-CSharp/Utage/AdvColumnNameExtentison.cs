using System;

namespace Utage
{
	public static class AdvColumnNameExtentison
	{
		private static string[] names;

		public static string QuickToString(this AdvColumnName value)
		{
			if (names == null)
			{
				names = Enum.GetNames(typeof(AdvColumnName));
			}
			return names[(int)value];
		}
	}
}
