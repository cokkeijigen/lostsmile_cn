using System;

namespace Utage
{
	[Serializable]
	public class DictionaryInt : SerializableDictionaryBinaryIO<DictionaryKeyValueInt>
	{
		public void Add(string key, int val)
		{
			Add(new DictionaryKeyValueInt(key, val));
		}

		public int Get(string key)
		{
			return GetValue(key).value;
		}

		public bool TryGetValue(string key, out int val)
		{
			if (TryGetValue(key, out DictionaryKeyValueInt val2))
			{
				val = val2.value;
				return true;
			}
			val = 0;
			return false;
		}
	}
}
