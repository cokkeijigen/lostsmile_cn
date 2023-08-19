using UnityEngine;

namespace Utage
{
	public static class ExtensionUtil
	{
		public const string Ogg = ".ogg";

		public const string Mp3 = ".mp3";

		public const string Wav = ".wav";

		public const string Txt = ".txt";

		public const string CSV = ".csv";

		public const string TSV = ".tsv";

		public const string AssetBundle = ".unity3d";

		public const string UtageFile = ".utage";

		public const string ConvertFileList = ".list.bytes";

		public const string ConvertFileListLog = ".list.log";

		public const string Log = ".log";

		public static AudioType GetAudioType(string path)
		{
			string text = FilePathUtil.GetExtension(path).ToLower();
			if (!(text == ".mp3"))
			{
				if (text == ".ogg")
				{
					return AudioType.OGGVORBIS;
				}
				return AudioType.WAV;
			}
			return AudioType.MPEG;
		}

		public static string ChangeSoundExt(string path)
		{
			string text = FilePathUtil.GetExtension(path).ToLower();
			if (!(text == ".ogg"))
			{
				if (text == ".mp3" && IsSupportOggPlatform())
				{
					return FilePathUtil.ChangeExtension(path, ".ogg");
				}
			}
			else if (!IsSupportOggPlatform())
			{
				return FilePathUtil.ChangeExtension(path, ".mp3");
			}
			return path;
		}

		public static bool IsSupportOggPlatform()
		{
			return true;
		}
	}
}
