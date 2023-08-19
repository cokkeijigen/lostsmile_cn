namespace Utage
{
	public class Crypt
	{
		public static void EncryptXor(byte[] key, byte[] buffer)
		{
			EncryptXor(key, buffer, 0, buffer.Length);
		}

		public static void EncryptXor(byte[] key, byte[] buffer, int offset, int count)
		{
			if (key == null || key.Length == 0)
			{
				return;
			}
			int num = key.Length;
			for (int i = offset; i < offset + count; i++)
			{
				if (buffer[i] != 0)
				{
					byte b = key[i % num];
					buffer[i] ^= b;
					if (buffer[i] == 0)
					{
						buffer[i] = b;
					}
				}
			}
		}

		public static void DecryptXor(byte[] key, byte[] buffer)
		{
			DecryptXor(key, buffer, 0, buffer.Length);
		}

		public static void DecryptXor(byte[] key, byte[] buffer, int offset, int count)
		{
			if (key == null || key.Length == 0)
			{
				return;
			}
			int num = key.Length;
			for (int i = offset; i < offset + count; i++)
			{
				byte b = key[i % num];
				if (buffer[i] != 0 && buffer[i] != b)
				{
					buffer[i] ^= b;
				}
			}
		}
	}
}
