using System;

namespace Utage
{
	[Serializable]
	public class DictionaryFloat : SerializableDictionaryBinaryIO<DictionaryKeyValueFloat>
	{
		public void Add(string key, float val)
		{
			Add(new DictionaryKeyValueFloat(key, val));
		}

		public float Get(string key)
		{
			return GetValue(key).value;
		}

		public bool TryGetValue(string key, out float val)
		{
			DictionaryKeyValueFloat val2;
			if (TryGetValue(key, out val2))
			{
				val = val2.value;
				return true;
			}
			val = 0f;
			return false;
		}
	}
}
