using System;
using System.IO;

namespace Utage
{
	[Serializable]
	public abstract class SerializableDictionaryBinaryIOKeyValue : SerializableDictionaryKeyValue
	{
		public abstract void Read(BinaryReader reader);

		public abstract void Write(BinaryWriter writer);
	}
}
