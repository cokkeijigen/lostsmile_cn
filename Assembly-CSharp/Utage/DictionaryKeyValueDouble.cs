using System;
using System.IO;

namespace Utage
{
	[Serializable]
	public class DictionaryKeyValueDouble : SerializableDictionaryBinaryIOKeyValue
	{
		public double value;

		public DictionaryKeyValueDouble(string key, double value)
		{
			Init(key, value);
		}

		public DictionaryKeyValueDouble()
		{
		}

		private void Init(string key, double value)
		{
			InitKey(key);
			this.value = value;
		}

		public override void Read(BinaryReader reader)
		{
			InitKey(reader.ReadString());
			value = reader.ReadDouble();
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(base.Key);
			writer.Write(value);
		}
	}
}
