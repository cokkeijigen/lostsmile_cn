using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Sound/Group")]
	public class SoundGroup : MonoBehaviour
	{
		private Dictionary<string, SoundAudioPlayer> playerList = new Dictionary<string, SoundAudioPlayer>();

		[SerializeField]
		private bool multiPlay;

		[SerializeField]
		private bool autoDestoryPlayer;

		[Range(0f, 1f)]
		[SerializeField]
		private float masterVolume = 1f;

		[Range(0f, 1f)]
		[SerializeField]
		private float groupVolume = 1f;

		[SerializeField]
		private float groupVolumeFadeTime = 1f;

		private float groupVolumeVelocity = 1f;

		[SerializeField]
		private List<SoundGroup> duckGroups = new List<SoundGroup>();

		private float duckVelocity = 1f;

		private const int Version = 1;

		private const int Version0 = 0;

		internal SoundManager SoundManager => SoundManagerSystem.SoundManager;

		internal SoundManagerSystem SoundManagerSystem { get; private set; }

		internal Dictionary<string, SoundAudioPlayer> PlayerList => playerList;

		public string GroupName => base.gameObject.name;

		public bool MultiPlay
		{
			get
			{
				return multiPlay;
			}
			set
			{
				multiPlay = value;
			}
		}

		public bool AutoDestoryPlayer
		{
			get
			{
				return autoDestoryPlayer;
			}
			set
			{
				autoDestoryPlayer = value;
			}
		}

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

		public float GroupVolume
		{
			get
			{
				return groupVolume;
			}
			set
			{
				groupVolume = value;
			}
		}

		public float GroupVolumeFadeTime
		{
			get
			{
				return groupVolumeFadeTime;
			}
			set
			{
				groupVolumeFadeTime = value;
			}
		}

		public float CurrentGroupVolume { get; set; }

		public List<SoundGroup> DuckGroups => duckGroups;

		private float DuckVolume { get; set; }

		public bool IsLoading
		{
			get
			{
				foreach (KeyValuePair<string, SoundAudioPlayer> player in PlayerList)
				{
					if (player.Value.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal void Init(SoundManagerSystem soundManagerSystem)
		{
			SoundManagerSystem = soundManagerSystem;
			CurrentGroupVolume = GroupVolume;
			groupVolumeVelocity = 1f;
			DuckVolume = 1f;
			duckVelocity = 1f;
		}

		internal float GetVolume(string tag)
		{
			float num = CurrentGroupVolume * MasterVolume * SoundManager.MasterVolume;
			foreach (SoundManager.TaggedMasterVolume taggedMasterVolume in SoundManager.TaggedMasterVolumes)
			{
				if (taggedMasterVolume.Tag == tag)
				{
					num *= taggedMasterVolume.Volume;
				}
			}
			return num * DuckVolume;
		}

		private void Update()
		{
			UpdateDucking();
			CurrentGroupVolume = UpdateFade(CurrentGroupVolume, GroupVolume, GroupVolumeFadeTime, ref groupVolumeVelocity);
		}

		private void UpdateDucking()
		{
			if (Mathf.Approximately(1f, SoundManager.DuckVolume))
			{
				DuckVolume = 1f;
				return;
			}
			if (DuckGroups.Count <= 0)
			{
				DuckVolume = 1f;
				return;
			}
			float to = (DuckGroups.Exists((SoundGroup x) => x.IsPlaying()) ? SoundManager.DuckVolume : 1f);
			DuckVolume = UpdateFade(DuckVolume, to, SoundManager.DuckFadeTime, ref duckVelocity);
		}

		private float UpdateFade(float from, float to, float fadeTime, ref float velocity)
		{
			if (fadeTime <= 0f)
			{
				velocity = 0f;
				return to;
			}
			if (Mathf.Abs(to - from) < 0.001f)
			{
				velocity = 0f;
				return to;
			}
			return Mathf.SmoothDamp(from, to, ref velocity, fadeTime);
		}

		internal void Remove(string label)
		{
			PlayerList.Remove(label);
		}

		private SoundAudioPlayer GetPlayer(string label)
		{
			if (PlayerList.TryGetValue(label, out var value))
			{
				return value;
			}
			return null;
		}

		private SoundAudioPlayer GetPlayerOrCreateIfMissing(string label)
		{
			SoundAudioPlayer soundAudioPlayer = GetPlayer(label);
			if (soundAudioPlayer == null)
			{
				soundAudioPlayer = base.transform.AddChildGameObjectComponent<SoundAudioPlayer>(label);
				soundAudioPlayer.Init(label, this);
				PlayerList.Add(label, soundAudioPlayer);
			}
			return soundAudioPlayer;
		}

		private SoundAudioPlayer GetOnlyOnePlayer(string label, float fadeOutTime)
		{
			SoundAudioPlayer playerOrCreateIfMissing = GetPlayerOrCreateIfMissing(label);
			if (PlayerList.Count > 1)
			{
				foreach (KeyValuePair<string, SoundAudioPlayer> player in PlayerList)
				{
					if (player.Value != playerOrCreateIfMissing)
					{
						player.Value.Stop(fadeOutTime);
					}
				}
			}
			return playerOrCreateIfMissing;
		}

		internal bool IsPlaying()
		{
			foreach (KeyValuePair<string, SoundAudioPlayer> player in PlayerList)
			{
				if (player.Value.IsPlaying())
				{
					return true;
				}
			}
			return false;
		}

		internal bool IsPlaying(string label)
		{
			SoundAudioPlayer player = GetPlayer(label);
			if (player == null)
			{
				return false;
			}
			return player.IsPlaying();
		}

		internal bool IsPlayingLoop(string label)
		{
			SoundAudioPlayer player = GetPlayer(label);
			if (player == null)
			{
				return false;
			}
			return player.IsPlayingLoop();
		}

		internal void Play(string label, SoundData data, float fadeInTime, float fadeOutTime)
		{
			(MultiPlay ? GetPlayerOrCreateIfMissing(label) : GetOnlyOnePlayer(label, fadeOutTime)).Play(data, fadeInTime, fadeOutTime);
		}

		internal void Stop(string label, float fadeTime)
		{
			SoundAudioPlayer player = GetPlayer(label);
			if (!(player == null))
			{
				player.Stop(fadeTime);
			}
		}

		internal void StopAll(float fadeTime)
		{
			foreach (KeyValuePair<string, SoundAudioPlayer> player in PlayerList)
			{
				player.Value.Stop(fadeTime);
			}
		}

		internal void StopAllLoop(float fadeTime)
		{
			foreach (KeyValuePair<string, SoundAudioPlayer> player in PlayerList)
			{
				if (player.Value.IsPlayingLoop())
				{
					player.Value.Stop(fadeTime);
				}
			}
		}

		internal void StopAllIgnoreLoop(float fadeTime)
		{
			foreach (KeyValuePair<string, SoundAudioPlayer> player in PlayerList)
			{
				if (!player.Value.IsPlayingLoop())
				{
					player.Value.Stop(fadeTime);
				}
			}
		}

		internal AudioSource GetAudioSource(string label)
		{
			SoundAudioPlayer player = GetPlayer(label);
			if (player == null)
			{
				return null;
			}
			return player.Audio.AudioSource;
		}

		internal float GetSamplesVolume(string label)
		{
			SoundAudioPlayer player = GetPlayer(label);
			if (player == null)
			{
				return 0f;
			}
			return player.GetSamplesVolume();
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(1);
			writer.Write(GroupVolume);
			writer.Write(PlayerList.Count);
			foreach (KeyValuePair<string, SoundAudioPlayer> player in PlayerList)
			{
				writer.Write(player.Key);
				writer.WriteBuffer(player.Value.Write);
			}
		}

		internal void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num <= 1)
			{
				if (num > 0)
				{
					GroupVolume = reader.ReadSingle();
				}
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					string label = reader.ReadString();
					SoundAudioPlayer playerOrCreateIfMissing = GetPlayerOrCreateIfMissing(label);
					reader.ReadBuffer(playerOrCreateIfMissing.Read);
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
