using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class ReorderableList<T>
	{
		[SerializeField]
		private List<T> list = new List<T>();

		public List<T> List
		{
			get
			{
				return list;
			}
		}
	}
}
