using System.IO;
using UnityEngine;

namespace Utage
{
	public class SoundData
	{
		private AudioClip clip;

		private const int Version = 0;

		public AudioClip Clip
		{
			get
			{
				if (clip == null)
				{
					clip = File.Sound;
				}
				return clip;
			}
		}

		public AssetFile File { get; private set; }

		public string Name
		{
			get
			{
				if (File == null)
				{
					return Clip.name;
				}
				return File.FileName;
			}
		}

		public SoundPlayMode PlayMode { get; private set; }

		public bool IsLoop { get; set; }

		public float PlayVolume { get; set; }

		public float ResourceVolume { get; set; }

		public float IntroTime { get; set; }

		public string Tag { get; set; }

		public float Volume
		{
			get
			{
				return ResourceVolume * PlayVolume;
			}
		}

		public bool EnableIntroLoop
		{
			get
			{
				if (IsLoop)
				{
					return IntroTime > 0f;
				}
				return false;
			}
		}

		internal bool EnableSave
		{
			get
			{
				if (File != null)
				{
					return IsLoop;
				}
				return false;
			}
		}

		public SoundData(AudioClip clip, SoundPlayMode playmode, float playVolume, bool isLoop)
		{
			this.clip = clip;
			PlayMode = playmode;
			PlayVolume = playVolume;
			IsLoop = isLoop;
			ResourceVolume = 1f;
			Tag = "";
		}

		public SoundData(AssetFile file, SoundPlayMode playmode, float playVolume, bool isLoop)
		{
			File = file;
			PlayMode = playmode;
			PlayVolume = playVolume;
			IsLoop = isLoop;
			if (file.SettingData is IAssetFileSoundSettingData)
			{
				IAssetFileSoundSettingData assetFileSoundSettingData = file.SettingData as IAssetFileSoundSettingData;
				IntroTime = assetFileSoundSettingData.IntroTime;
				ResourceVolume = assetFileSoundSettingData.Volume;
			}
			else
			{
				IntroTime = 0f;
				ResourceVolume = 1f;
			}
			Tag = "";
		}

		public SoundData()
		{
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write((int)PlayMode);
			writer.Write(IsLoop);
			writer.Write(PlayVolume);
			writer.Write(ResourceVolume);
			writer.Write(IntroTime);
			writer.Write(Tag);
			writer.Write(File.FileName);
		}

		internal void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num <= 0)
			{
				PlayMode = (SoundPlayMode)reader.ReadInt32();
				IsLoop = reader.ReadBoolean();
				PlayVolume = reader.ReadSingle();
				ResourceVolume = reader.ReadSingle();
				IntroTime = reader.ReadSingle();
				Tag = reader.ReadString();
				File = AssetFileManager.GetFileCreateIfMissing(reader.ReadString());
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
