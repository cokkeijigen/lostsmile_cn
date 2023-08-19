using System;
using System.IO;

namespace Utage
{
	[Serializable]
	public abstract class SerializableDictionaryBinaryIO<T> : SerializableDictionary<T> where T : SerializableDictionaryBinaryIOKeyValue, new()
	{
		public void Read(BinaryReader reader)
		{
			Clear();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				T val = new T();
				val.Read(reader);
				Add(val);
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(base.Count);
			foreach (T item in base.List)
			{
				item.Write(writer);
			}
		}
	}
}
