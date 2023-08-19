using System;
using System.IO;

namespace Utage
{
	[Serializable]
	public class DictionaryKeyValueBool : SerializableDictionaryBinaryIOKeyValue
	{
		public bool value;

		public DictionaryKeyValueBool(string key, bool value)
		{
			Init(key, value);
		}

		public DictionaryKeyValueBool()
		{
		}

		private void Init(string key, bool value)
		{
			InitKey(key);
			this.value = value;
		}

		public override void Read(BinaryReader reader)
		{
			InitKey(reader.ReadString());
			value = reader.ReadBoolean();
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(base.Key);
			writer.Write(value);
		}
	}
}
