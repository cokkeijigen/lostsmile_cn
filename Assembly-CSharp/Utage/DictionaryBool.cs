using System;

namespace Utage
{
	[Serializable]
	public class DictionaryBool : SerializableDictionaryBinaryIO<DictionaryKeyValueBool>
	{
		public void Add(string key, bool val)
		{
			Add(new DictionaryKeyValueBool(key, val));
		}

		public bool Get(string key)
		{
			return GetValue(key).value;
		}

		public bool TryGetValue(string key, out bool val)
		{
			if (TryGetValue(key, out DictionaryKeyValueBool val2))
			{
				val = val2.value;
				return true;
			}
			val = false;
			return false;
		}
	}
}
