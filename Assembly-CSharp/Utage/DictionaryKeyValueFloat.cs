using System;
using System.IO;

namespace Utage
{
	[Serializable]
	public class DictionaryKeyValueFloat : SerializableDictionaryBinaryIOKeyValue
	{
		public float value;

		public DictionaryKeyValueFloat(string key, float value)
		{
			Init(key, value);
		}

		public DictionaryKeyValueFloat()
		{
		}

		private void Init(string key, float value)
		{
			InitKey(key);
			this.value = value;
		}

		public override void Read(BinaryReader reader)
		{
			InitKey(reader.ReadString());
			value = reader.ReadSingle();
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(base.Key);
			writer.Write(value);
		}
	}
}
