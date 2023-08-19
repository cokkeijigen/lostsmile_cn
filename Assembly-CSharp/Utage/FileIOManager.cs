using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/File/FileIOManager")]
	public class FileIOManager : FileIOManagerBase
	{
		private byte[] cryptKeyBytes;

		[SerializeField]
		private string cryptKey = "InputOriginalKey";

		public byte[] CryptKeyBytes
		{
			get
			{
				if (cryptKeyBytes == null || cryptKeyBytes.Length == 0)
				{
					cryptKeyBytes = Encoding.UTF8.GetBytes(cryptKey);
				}
				return cryptKeyBytes;
			}
		}

		private void OnValidate()
		{
			cryptKeyBytes = Encoding.UTF8.GetBytes(cryptKey);
		}

		public override byte[] Decode(byte[] bytes)
		{
			return FileIOManagerBase.CustomDecode(CryptKeyBytes, bytes);
		}

		public override void DecodeNoCompress(byte[] bytes)
		{
			FileIOManagerBase.CustomDecodeNoCompress(CryptKeyBytes, bytes, 0, bytes.Length);
		}

		public override byte[] Encode(byte[] bytes)
		{
			return FileIOManagerBase.CustomEncode(CryptKeyBytes, bytes);
		}

		public override byte[] EncodeNoCompress(byte[] bytes)
		{
			FileIOManagerBase.CustomEncodeNoCompress(CryptKeyBytes, bytes, 0, bytes.Length);
			return bytes;
		}

		public override bool Write(string path, byte[] bytes)
		{
			try
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					int num = 0;
					do
					{
						int num2 = Math.Min(262144, bytes.Length - num);
						fileStream.Write(bytes, num, num2);
						num += num2;
					}
					while (num < bytes.Length);
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return false;
			}
		}

		public override bool ReadBinaryDecode(string path, Action<BinaryReader> callbackRead)
		{
			try
			{
				if (!Exists(path))
				{
					return false;
				}
				using (MemoryStream input = new MemoryStream(FileIOManagerBase.CustomDecode(CryptKeyBytes, FileReadAllBytes(path))))
				{
					using (BinaryReader obj = new BinaryReader(input))
					{
						callbackRead(obj);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.FileRead, path, ex.ToString()));
				return false;
			}
		}

		public override bool WriteBinaryEncode(string path, Action<BinaryWriter> callbackWrite)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter obj = new BinaryWriter(memoryStream))
					{
						callbackWrite(obj);
					}
					FileWriteAllBytes(path, FileIOManagerBase.CustomEncode(CryptKeyBytes, memoryStream.ToArray()));
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.FileWrite, path, ex.ToString()));
				return false;
			}
		}

		public override bool WriteEncode(string path, byte[] bytes)
		{
			try
			{
				FileWriteAllBytes(path, FileIOManagerBase.CustomEncode(CryptKeyBytes, bytes));
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.FileWrite, path, ex.ToString()));
				return false;
			}
		}

		public override bool WriteEncodeNoCompress(string path, byte[] bytes)
		{
			try
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					int num = 0;
					do
					{
						int num2 = Math.Min(262144, bytes.Length - num);
						Buffer.BlockCopy(bytes, num, FileIOManagerBase.workBufferArray, 0, num2);
						FileIOManagerBase.CustomEncodeNoCompress(CryptKeyBytes, FileIOManagerBase.workBufferArray, 0, num2);
						fileStream.Write(FileIOManagerBase.workBufferArray, 0, num2);
						num += num2;
					}
					while (num < bytes.Length);
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return false;
			}
		}

		public override bool WriteSound(string path, AudioClip audioClip)
		{
			try
			{
				FileIOManagerBase.audioHeader[0] = audioClip.samples;
				FileIOManagerBase.audioHeader[2] = audioClip.frequency;
				FileIOManagerBase.audioHeader[1] = audioClip.channels;
				int num = audioClip.samples * audioClip.channels;
				using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					Buffer.BlockCopy(FileIOManagerBase.audioHeader, 0, FileIOManagerBase.workBufferArray, 0, 12);
					FileIOManagerBase.CustomEncodeNoCompress(CryptKeyBytes, FileIOManagerBase.workBufferArray, 0, 12);
					fileStream.Write(FileIOManagerBase.workBufferArray, 0, 12);
					int num2 = 0;
					do
					{
						int num3 = Math.Min(FileIOManagerBase.audioSamplesWorkArray.Length, num - num2);
						audioClip.GetData(FileIOManagerBase.audioSamplesWorkArray, num2 / audioClip.channels);
						for (int i = 0; i < num3; i++)
						{
							FileIOManagerBase.audioShortWorkArray[i] = (short)(32767f * FileIOManagerBase.audioSamplesWorkArray[i]);
						}
						int num4 = num3 * 2;
						Buffer.BlockCopy(FileIOManagerBase.audioShortWorkArray, 0, FileIOManagerBase.workBufferArray, 0, num4);
						FileIOManagerBase.CustomEncodeNoCompress(CryptKeyBytes, FileIOManagerBase.workBufferArray, 0, num4);
						fileStream.Write(FileIOManagerBase.workBufferArray, 0, num4);
						num2 += num3;
					}
					while (num2 < num);
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.ToString());
				return false;
			}
		}

		public override void CreateDirectory(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
		}

		public override void DeleteDirectory(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			if (Directory.Exists(directoryName))
			{
				Directory.Delete(directoryName, true);
			}
		}

		public override bool Exists(string path)
		{
			return File.Exists(path);
		}

		protected virtual byte[] FileReadAllBytes(string path)
		{
			return File.ReadAllBytes(path);
		}

		protected virtual void FileWriteAllBytes(string path, byte[] bytes)
		{
			File.WriteAllBytes(path, bytes);
		}

		public override void Delete(string path)
		{
			File.Delete(path);
		}
	}
}
