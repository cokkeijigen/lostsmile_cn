using System;

namespace Utage
{
	[Serializable]
	public class DictionaryDouble : SerializableDictionaryBinaryIO<DictionaryKeyValueDouble>
	{
		public void Add(string key, double val)
		{
			Add(new DictionaryKeyValueDouble(key, val));
		}

		public double Get(string key)
		{
			return GetValue(key).value;
		}

		public bool TryGetValue(string key, out double val)
		{
			DictionaryKeyValueDouble val2;
			if (TryGetValue(key, out val2))
			{
				val = val2.value;
				return true;
			}
			val = 0.0;
			return false;
		}
	}
}
