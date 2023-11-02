using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public class BinaryBufferGeneric<T> where T : IBinaryIO
	{
		private Dictionary<string, byte[]> buffers = new Dictionary<string, byte[]>();

		private Dictionary<string, byte[]> Buffers => buffers;

		public bool IsEmpty => Buffers.Count <= 0;

		public static void Write(BinaryWriter writer, List<T> ioList)
		{
			BinaryBufferGeneric<T> binaryBufferGeneric = new BinaryBufferGeneric<T>();
			binaryBufferGeneric.MakeBuffer(ioList);
			binaryBufferGeneric.Write(writer);
		}

		public static void Read(BinaryReader reader, List<T> ioList)
		{
			BinaryBufferGeneric<T> binaryBufferGeneric = new BinaryBufferGeneric<T>();
			binaryBufferGeneric.Read(reader);
			binaryBufferGeneric.Overrirde(ioList);
		}

		public void MakeBuffer(List<T> ioList)
		{
			Buffers.Clear();
			ioList.ForEach(delegate(T x)
			{
				if (Buffers.ContainsKey(x.SaveKey))
				{
					Debug.LogError($"Save data Key [{x.SaveKey}] is already exsits. Please use another key.");
				}
				else
				{
					byte[] value = BinaryUtil.BinaryWrite(((IBinaryIO)x).OnWrite);
					Buffers.Add(x.SaveKey, value);
				}
			});
		}

		public void Overrirde(List<T> ioList)
		{
			ioList.ForEach(delegate(T x)
			{
				Overrirde(x);
			});
		}

		public void Overrirde(T io)
		{
			if (Buffers.ContainsKey(io.SaveKey))
			{
				BinaryUtil.BinaryRead(Buffers[io.SaveKey], ((IBinaryIO)io).OnRead);
			}
			else
			{
				Debug.LogError($"Not found Save data Key [{io.SaveKey}] ");
			}
		}

		public TClone Clone<TClone>() where TClone : BinaryBufferGeneric<T>, new()
		{
			TClone val = new TClone();
			foreach (string key in Buffers.Keys)
			{
				val.Buffers.Add(key, Buffers[key]);
			}
			return val;
		}

		public void Read(BinaryReader reader)
		{
			Buffers.Clear();
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				Buffers.Add(reader.ReadString(), reader.ReadBuffer());
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(Buffers.Count);
			foreach (string key in Buffers.Keys)
			{
				writer.Write(key);
				writer.WriteBuffer(Buffers[key]);
			}
		}
	}
}
