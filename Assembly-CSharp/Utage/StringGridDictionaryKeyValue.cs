using System;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class StringGridDictionaryKeyValue : SerializableDictionaryKeyValue
	{
		[SerializeField]
		private StringGrid grid;

		public string Name => base.Key;

		public StringGrid Grid => grid;

		public StringGridDictionaryKeyValue(string key, StringGrid grid)
		{
			InitKey(key);
			this.grid = grid;
		}
	}
}
