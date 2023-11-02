using System;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public abstract class SerializableDictionaryKeyValue
	{
		[SerializeField]
		private string key;

		public string Key => key;

		public void InitKey(string key)
		{
			this.key = key;
		}
	}
}
