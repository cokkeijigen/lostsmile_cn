using System;
using System.IO;

namespace Utage
{
	[Serializable]
	public class DictionaryKeyValueString : SerializableDictionaryBinaryIOKeyValue
	{
		public string value;

		public DictionaryKeyValueString(string key, string value)
		{
			Init(key, value);
		}

		public DictionaryKeyValueString()
		{
		}

		private void Init(string key, string value)
		{
			InitKey(key);
			this.value = value;
		}

		public override void Read(BinaryReader reader)
		{
			InitKey(reader.ReadString());
			value = reader.ReadString();
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(base.Key);
			writer.Write(value);
		}
	}
}
