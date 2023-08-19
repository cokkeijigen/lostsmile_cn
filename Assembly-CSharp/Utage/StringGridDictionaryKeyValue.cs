using System;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class StringGridDictionaryKeyValue : SerializableDictionaryKeyValue
	{
		[SerializeField]
		private StringGrid grid;

		public string Name
		{
			get
			{
				return base.Key;
			}
		}

		public StringGrid Grid
		{
			get
			{
				return grid;
			}
		}

		public StringGridDictionaryKeyValue(string key, StringGrid grid)
		{
			InitKey(key);
			this.grid = grid;
		}
	}
}
