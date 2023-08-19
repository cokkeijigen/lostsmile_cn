using System;
using System.IO;
using UnityEngine;

namespace Utage
{
	public abstract class FileIOManagerBase : MonoBehaviour
	{
		protected enum SoundHeader
		{
			Samples,
			Channels,
			Frequency,
			Max
		}

		private const string sdkDirectoryName = "Utage/";

		private static Func<byte[], byte[], byte[]> customEncode = DefaultEncode;

		private static Func<byte[], byte[], byte[]> customDecode = DefaultDecode;

		private static Action<byte[], byte[], int, int> customEncodeNoCompress = DefaultEncodeNoCompress;

		private static Action<byte[], byte[], int, int> customDecodeNoCompress = DefaultDecodeNoCompress;

		protected static int[] audioHeader = new int[3];

		protected const int audioHeaderSize = 12;

		protected const int maxWorkBufferSize = 262144;

		protected const int maxAudioWorkSize = 131072;

		protected static byte[] workBufferArray = new byte[262144];

		protected static short[] audioShortWorkArray = new short[131072];

		protected static float[] audioSamplesWorkArray = new float[131072];

		public static string SdkPersistentDataPath
		{
			get
			{
				return FilePathUtil.Combine(Application.persistentDataPath, "Utage/");
			}
		}

		public static string SdkTemporaryCachePath
		{
			get
			{
				return FilePathUtil.Combine(Application.temporaryCachePath, "Utage/");
			}
		}

		public static Func<byte[], byte[], byte[]> CustomEncode
		{
			get
			{
				return customEncode;
			}
			set
			{
				customEncode = value;
			}
		}

		public static Func<byte[], byte[], byte[]> CustomDecode
		{
			get
			{
				return customDecode;
			}
			set
			{
				customDecode = value;
			}
		}

		public static Action<byte[], byte[], int, int> CustomEncodeNoCompress
		{
			get
			{
				return customEncodeNoCompress;
			}
			set
			{
				customEncodeNoCompress = value;
			}
		}

		public static Action<byte[], byte[], int, int> CustomDecodeNoCompress
		{
			get
			{
				return customDecodeNoCompress;
			}
			set
			{
				customDecodeNoCompress = value;
			}
		}

		public static int ToMagicID(char id0, char id1, char id2, char id3)
		{
			return (int)(((uint)id3 << 24) + ((uint)id2 << 16) + ((uint)id1 << 8) + id0);
		}

		private static byte[] DefaultEncode(byte[] keyBytes, byte[] bytes)
		{
			byte[] array = Compression.Compress(bytes);
			Crypt.EncryptXor(keyBytes, array);
			return array;
		}

		private static byte[] DefaultDecode(byte[] keyBytes, byte[] bytes)
		{
			Crypt.DecryptXor(keyBytes, bytes);
			return Compression.Decompress(bytes);
		}

		private static void DefaultEncodeNoCompress(byte[] keyBytes, byte[] bytes, int offset, int count)
		{
			Crypt.EncryptXor(keyBytes, bytes, offset, count);
		}

		private static void DefaultDecodeNoCompress(byte[] keyBytes, byte[] bytes, int offset, int count)
		{
			Crypt.DecryptXor(keyBytes, bytes, offset, count);
		}

		public abstract byte[] Decode(byte[] bytes);

		public abstract void DecodeNoCompress(byte[] bytes);

		public abstract byte[] Encode(byte[] bytes);

		public abstract byte[] EncodeNoCompress(byte[] bytes);

		public abstract bool Write(string path, byte[] bytes);

		public abstract bool ReadBinaryDecode(string path, Action<BinaryReader> callbackRead);

		public abstract bool WriteBinaryEncode(string path, Action<BinaryWriter> callbackWrite);

		public abstract bool WriteEncode(string path, byte[] bytes);

		public abstract bool WriteEncodeNoCompress(string path, byte[] bytes);

		public abstract bool WriteSound(string path, AudioClip audioClip);

		public static AudioClip ReadAudioFromMem(string name, byte[] bytes)
		{
			return ReadAudioFromMem(name, bytes, false);
		}

		public static AudioClip ReadAudioFromMem(string name, byte[] bytes, bool is3D)
		{
			Buffer.BlockCopy(bytes, 0, audioHeader, 0, 12);
			AudioClip audioClip = WrapperUnityVersion.CreateAudioClip(name, audioHeader[0], audioHeader[1], audioHeader[2], is3D, false);
			int num = audioHeader[0] * audioHeader[1];
			int num2 = 0;
			int num3 = 12;
			do
			{
				int num4 = Math.Min(audioSamplesWorkArray.Length, num - num2);
				Buffer.BlockCopy(bytes, num3, audioShortWorkArray, 0, num4 * 2);
				num3 += num4 * 2;
				float[] array = ((num4 == audioSamplesWorkArray.Length) ? audioSamplesWorkArray : new float[num4]);
				for (int i = 0; i < num4; i++)
				{
					array[i] = 1f * (float)audioShortWorkArray[i] / 32767f;
				}
				audioClip.SetData(array, num2 / audioClip.channels);
				num2 += num4;
			}
			while (num2 < num);
			return audioClip;
		}

		public abstract void CreateDirectory(string path);

		public abstract void DeleteDirectory(string path);

		public abstract bool Exists(string path);

		public abstract void Delete(string path);
	}
}
