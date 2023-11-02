using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class StringPopupAttribute : PropertyAttribute
	{
		private List<string> stringList = new List<string>();

		public List<string> StringList => stringList;

		public StringPopupAttribute(params string[] args)
		{
			stringList.AddRange(args);
		}
	}
}
