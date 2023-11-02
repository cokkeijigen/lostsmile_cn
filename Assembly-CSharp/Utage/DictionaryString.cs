using System;

namespace Utage
{
	[Serializable]
	public class DictionaryString : SerializableDictionaryBinaryIO<DictionaryKeyValueString>
	{
		public void Add(string key, string val)
		{
			Add(new DictionaryKeyValueString(key, val));
		}

		public string Get(string key)
		{
			return GetValue(key).value;
		}

		public bool TryGetValue(string key, out string val)
		{
			if (TryGetValue(key, out DictionaryKeyValueString val2))
			{
				val = val2.value;
				return true;
			}
			val = "";
			return false;
		}
	}
}
