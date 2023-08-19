using System;
using System.IO;

namespace Utage
{
	[Serializable]
	public class DictionaryKeyValueInt : SerializableDictionaryBinaryIOKeyValue
	{
		public int value;

		public DictionaryKeyValueInt(string key, int value)
		{
			Init(key, value);
		}

		public DictionaryKeyValueInt()
		{
		}

		private void Init(string key, int value)
		{
			InitKey(key);
			this.value = value;
		}

		public override void Read(BinaryReader reader)
		{
			InitKey(reader.ReadString());
			value = reader.ReadInt32();
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(base.Key);
			writer.Write(value);
		}
	}
}
