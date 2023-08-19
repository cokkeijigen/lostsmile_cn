using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Sound/SoundManager")]
	public class SoundManager : MonoBehaviour, IBinaryIO
	{
		[Serializable]
		public class TaggedMasterVolume
		{
			[SerializeField]
			private string tag;

			[Range(0f, 1f)]
			[SerializeField]
			private float volume = 1f;

			public string Tag
			{
				get
				{
					return tag;
				}
				set
				{
					tag = value;
				}
			}

			public float Volume
			{
				get
				{
					return volume;
				}
				set
				{
					volume = value;
				}
			}
		}

		[Serializable]
		public class SoundManagerEvent : UnityEvent<SoundManager>
		{
		}

		public const string IdBgm = "Bgm";

		public const string IdAmbience = "Ambience";

		public const string IdVoice = "Voice";

		public const string IdSe = "Se";

		private static SoundManager instance;

		[SerializeField]
		[Range(0f, 1f)]
		private float masterVolume = 1f;

		[SerializeField]
		private List<TaggedMasterVolume> taggedMasterVolumes = new List<TaggedMasterVolume>();

		public const string TaggedMasterVolumeOthers = "Others";

		[SerializeField]
		[Range(0f, 1f)]
		private float duckVolume = 0.5f;

		[SerializeField]
		[Range(0f, 1f)]
		private float duckFadeTime = 0.1f;

		[SerializeField]
		private float defaultFadeTime = 0.2f;

		[SerializeField]
		private float defaultVoiceFadeTime = 0.05f;

		[SerializeField]
		[Range(0f, 1f)]
		private float defaultVolume = 1f;

		[SerializeField]
		private SoundPlayMode voicePlayMode = SoundPlayMode.Replay;

		[SerializeField]
		private SoundManagerEvent onCreateSoundSystem = new SoundManagerEvent();

		private SoundManagerSystemInterface system;

		public float MasterVolume
		{
			get
			{
				return masterVolume;
			}
			set
			{
				masterVolume = value;
			}
		}

		public float BgmVolume
		{
			get
			{
				return System.GetMasterVolume("Bgm");
			}
			set
			{
				System.SetMasterVolume("Bgm", value);
			}
		}

		public float AmbienceVolume
		{
			get
			{
				return System.GetMasterVolume("Ambience");
			}
			set
			{
				System.SetMasterVolume("Ambience", value);
			}
		}

		public float VoiceVolume
		{
			get
			{
				return System.GetMasterVolume("Voice");
			}
			set
			{
				System.SetMasterVolume("Voice", value);
			}
		}

		public float SeVolume
		{
			get
			{
				return System.GetMasterVolume("Se");
			}
			set
			{
				System.SetMasterVolume("Se", value);
			}
		}

		public List<TaggedMasterVolume> TaggedMasterVolumes
		{
			get
			{
				return taggedMasterVolumes;
			}
		}

		public bool MultiVoice
		{
			get
			{
				return System.IsMultiPlay("Voice");
			}
			set
			{
				System.SetMultiPlay("Voice", value);
			}
		}

		public float DuckVolume
		{
			get
			{
				return duckVolume;
			}
			set
			{
				duckVolume = value;
			}
		}

		public float DuckFadeTime
		{
			get
			{
				return duckFadeTime;
			}
			set
			{
				duckFadeTime = value;
			}
		}

		public float DefaultFadeTime
		{
			get
			{
				return defaultFadeTime;
			}
			set
			{
				TryChangeFloat(ref defaultFadeTime, value);
			}
		}

		public float DefaultVoiceFadeTime
		{
			get
			{
				return defaultVoiceFadeTime;
			}
			set
			{
				TryChangeFloat(ref defaultVoiceFadeTime, value);
			}
		}

		public float DefaultVolume
		{
			get
			{
				return defaultVolume;
			}
			set
			{
				TryChangeFloat(ref defaultVolume, value);
			}
		}

		public string CurrentVoiceCharacterLabel { get; set; }

		public SoundPlayMode VoicePlayMode
		{
			get
			{
				return voicePlayMode;
			}
			set
			{
				voicePlayMode = value;
			}
		}

		public SoundManagerEvent OnCreateSoundSystem
		{
			get
			{
				return onCreateSoundSystem;
			}
			set
			{
				onCreateSoundSystem = value;
			}
		}

		public SoundManagerSystemInterface System
		{
			get
			{
				if (system == null)
				{
					OnCreateSoundSystem.Invoke(this);
					if (system == null)
					{
						system = new SoundManagerSystem();
					}
					List<string> saveStreamNameList = new List<string>(new string[2] { "Bgm", "Ambience" });
					system.Init(this, saveStreamNameList);
				}
				return system;
			}
			set
			{
				system = value;
			}
		}

		public bool IsLoading
		{
			get
			{
				return System.IsLoading;
			}
		}

		public string SaveKey
		{
			get
			{
				return "SoundManager";
			}
		}

		public static SoundManager GetInstance()
		{
			if (null == instance)
			{
				instance = UnityEngine.Object.FindObjectOfType<SoundManager>();
			}
			return instance;
		}

		public void SetTaggedMasterVolume(string tag, float volmue)
		{
			TaggedMasterVolume taggedMasterVolume = TaggedMasterVolumes.Find((TaggedMasterVolume x) => x.Tag == tag);
			if (taggedMasterVolume == null)
			{
				TaggedMasterVolumes.Add(new TaggedMasterVolume
				{
					Tag = tag,
					Volume = volmue
				});
			}
			else
			{
				taggedMasterVolume.Volume = volmue;
			}
		}

		private bool TryChangeFloat(ref float volume, float value)
		{
			if (Mathf.Approximately(volume, value))
			{
				return false;
			}
			volume = value;
			return true;
		}

		public void PlayBgm(AudioClip clip, bool isLoop)
		{
			System.Play("Bgm", "Bgm", new SoundData(clip, SoundPlayMode.NotPlaySame, DefaultVolume, isLoop), 0f, DefaultFadeTime);
		}

		public void PlayBgm(AssetFile file)
		{
			PlayBgm(file, 0f, DefaultFadeTime);
		}

		public void PlayBgm(AssetFile file, float fadeInTime, float fadeOutTime)
		{
			PlayBgm(file, DefaultVolume, fadeInTime, fadeOutTime);
		}

		public void PlayBgm(AssetFile file, float volume, float fadeInTime, float fadeOutTime)
		{
			System.Play("Bgm", "Bgm", new SoundData(file, SoundPlayMode.NotPlaySame, volume, true), fadeInTime, fadeOutTime);
		}

		public void StopBgm()
		{
			StopBgm(DefaultFadeTime);
		}

		public void StopBgm(float fadeTime)
		{
			System.StopGroup("Bgm", fadeTime);
		}

		public void PlayAmbience(AssetFile file, bool isLoop)
		{
			PlayAmbience(file, isLoop, 0f, DefaultFadeTime);
		}

		public void PlayAmbience(AssetFile file, bool isLoop, float fadeInTime, float fadeOutTime)
		{
			PlayAmbience(file, DefaultVolume, isLoop, fadeInTime, fadeOutTime);
		}

		public void PlayAmbience(AssetFile file, float volume, bool isLoop, float fadeInTime, float fadeOutTime)
		{
			System.Play("Ambience", "Ambience", new SoundData(file, SoundPlayMode.NotPlaySame, volume, isLoop), fadeInTime, fadeOutTime);
		}

		public void PlayAmbience(AudioClip clip, bool isLoop)
		{
			PlayAmbience(clip, isLoop, 0f, DefaultFadeTime);
		}

		public void PlayAmbience(AudioClip clip, bool isLoop, float fadeInTime, float fadeOutTime)
		{
			System.Play("Ambience", "Ambience", new SoundData(clip, SoundPlayMode.NotPlaySame, DefaultVolume, isLoop), fadeInTime, fadeOutTime);
		}

		public void StopAmbience()
		{
			StopAmbience(DefaultFadeTime);
		}

		public void StopAmbience(float fadeTime)
		{
			System.StopGroup("Ambience", fadeTime);
		}

		public void PlayVoice(string characterLabel, AssetFile file)
		{
			PlayVoice(characterLabel, file, false);
		}

		public void PlayVoice(string characterLabel, AssetFile file, float fadeInTime, float fadeOutTime)
		{
			PlayVoice(characterLabel, file, DefaultVolume, false, fadeInTime, fadeOutTime);
		}

		public void PlayVoice(string characterLabel, AssetFile file, bool isLoop)
		{
			PlayVoice(characterLabel, file, DefaultVolume, isLoop, 0f, DefaultVoiceFadeTime);
		}

		public void PlayVoice(string characterLabel, AssetFile file, float volume, bool isLoop)
		{
			PlayVoice(characterLabel, file, volume, isLoop, 0f, DefaultVoiceFadeTime);
		}

		public void PlayVoice(string characterLabel, AssetFile file, float volume, bool isLoop, float fadeInTime, float fadeOutTime)
		{
			PlayVoice(characterLabel, new SoundData(file, VoicePlayMode, volume, isLoop), fadeInTime, fadeOutTime);
		}

		public void PlayVoice(string characterLabel, AudioClip clip, bool isLoop)
		{
			PlayVoice(characterLabel, clip, isLoop, 0f, DefaultVoiceFadeTime);
		}

		public void PlayVoice(string characterLabel, AudioClip clip, bool isLoop, float fadeInTime, float fadeOutTime)
		{
			PlayVoice(characterLabel, new SoundData(clip, VoicePlayMode, DefaultVolume, isLoop), fadeInTime, fadeOutTime);
		}

		public void PlayVoice(string characterLabel, SoundData data, float fadeInTime, float fadeOutTime)
		{
			CurrentVoiceCharacterLabel = characterLabel;
			data.Tag = (TaggedMasterVolumes.Exists((TaggedMasterVolume x) => x.Tag == characterLabel) ? characterLabel : "Others");
			System.Play("Voice", characterLabel, data, fadeInTime, fadeOutTime);
		}

		public void StopVoice()
		{
			StopVoice(DefaultVoiceFadeTime);
		}

		public void StopVoice(float fadeTime)
		{
			System.StopGroup("Voice", fadeTime);
		}

		public void StopVoiceIgnoreLoop()
		{
			StopVoiceIgnoreLoop(DefaultVoiceFadeTime);
		}

		public void StopVoiceIgnoreLoop(float fadeTime)
		{
			System.StopGroupIgnoreLoop("Voice", fadeTime);
		}

		public void StopVoice(string characterLabel)
		{
			StopVoice(characterLabel, DefaultVoiceFadeTime);
		}

		public void StopVoice(string characterLabel, float fadeTime)
		{
			System.Stop("Voice", characterLabel, fadeTime);
		}

		public bool IsPlayingVoice()
		{
			return IsPlayingVoice(CurrentVoiceCharacterLabel);
		}

		internal bool IsPlayingVoice(string characterLabel)
		{
			if (characterLabel == null)
			{
				return false;
			}
			return System.IsPlaying("Voice", characterLabel);
		}

		internal float GetCurrentCharacterVoiceSamplesVolume()
		{
			return GetVoiceSamplesVolume(CurrentVoiceCharacterLabel);
		}

		internal float GetVoiceSamplesVolume(string characterLabel)
		{
			return System.GetSamplesVolume("Voice", characterLabel);
		}

		public void PlaySe(AssetFile file, string label = "", SoundPlayMode playMode = SoundPlayMode.Add, bool isLoop = false)
		{
			PlaySe(file, DefaultVolume, label, playMode, isLoop);
		}

		public void PlaySe(AssetFile file, float volume, string label = "", SoundPlayMode playMode = SoundPlayMode.Add, bool isLoop = false)
		{
			if (string.IsNullOrEmpty(label))
			{
				label = file.Sound.name;
			}
			System.Play("Se", label, new SoundData(file, playMode, volume, isLoop), 0f, 0f);
		}

		public void PlaySe(AudioClip clip, string label = "", SoundPlayMode playMode = SoundPlayMode.Add, bool isLoop = false)
		{
			PlaySe(clip, DefaultVolume, label, playMode, isLoop);
		}

		public void PlaySe(AudioClip clip, float volume, string label = "", SoundPlayMode playMode = SoundPlayMode.Add, bool isLoop = false)
		{
			if (string.IsNullOrEmpty(label))
			{
				label = clip.name;
			}
			System.Play("Se", label, new SoundData(clip, playMode, volume, isLoop), 0f, 0.02f);
		}

		public void StopSe(string label, float fadeTime)
		{
			System.Stop("Se", label, fadeTime);
		}

		public void StopSeAll(float fadeTime)
		{
			System.StopGroup("Se", fadeTime);
		}

		public void SetGroupVolume(string groupName, float volume, float fadeTime = 0f)
		{
			System.SetGroupVolume(groupName, volume, fadeTime);
		}

		public float GetGroupVolume(string groupName)
		{
			return System.GetGroupVolume(groupName);
		}

		public void StopGroups(string[] groups)
		{
			StopGroups(groups, DefaultFadeTime);
		}

		public void StopGroups(string[] groups, float fadeTime)
		{
			foreach (string groupName in groups)
			{
				System.StopGroup(groupName, fadeTime);
			}
		}

		public void StopAll()
		{
			StopAll(DefaultFadeTime);
		}

		public void StopAll(float fadeTime)
		{
			System.StopAll(fadeTime);
		}

		public void StopAllLoop()
		{
			StopAllLoop(DefaultFadeTime);
		}

		public void StopAllLoop(float fadeTime)
		{
			System.StopAllLoop(fadeTime);
		}

		public void OnWrite(BinaryWriter writer)
		{
			System.WriteSaveData(writer);
		}

		public void OnRead(BinaryReader reader)
		{
			System.ReadSaveDataBuffer(reader);
		}
	}
}
