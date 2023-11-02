using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class IntPopupAttribute : PropertyAttribute
	{
		private List<int> popupList = new List<int>();

		public List<int> PopupList => popupList;

		public IntPopupAttribute(params int[] args)
		{
			popupList.AddRange(args);
		}
	}
}
