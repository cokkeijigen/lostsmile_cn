using System;
using System.IO;

namespace Utage
{
	public class BinaryUtil
	{
		public static void BinaryReadFromString(string str, Action<BinaryReader> onRead)
		{
			BinaryRead(Convert.FromBase64String(str), onRead);
		}

		public static void BinaryRead(byte[] bytes, Action<BinaryReader> onRead)
		{
			using (MemoryStream input = new MemoryStream(bytes))
			{
				using (BinaryReader obj = new BinaryReader(input))
				{
					onRead(obj);
				}
			}
		}

		public static string BinaryWriteToString(Action<BinaryWriter> onWrite)
		{
			return Convert.ToBase64String(BinaryWrite(onWrite));
		}

		public static byte[] BinaryWrite(Action<BinaryWriter> onWrite)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter obj = new BinaryWriter(memoryStream))
				{
					onWrite(obj);
				}
				return memoryStream.ToArray();
			}
		}
	}
}
