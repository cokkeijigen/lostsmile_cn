using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Sound/AudioPlayer")]
	internal class SoundAudioPlayer : MonoBehaviour
	{
		private const int Version = 0;

		internal string Label { get; private set; }

		internal SoundGroup Group { get; set; }

		public SoundAudio Audio { get; private set; }

		private SoundAudio FadeOutAudio { get; set; }

		private List<SoundAudio> AudioList { get; set; }

		private List<SoundAudio> CurrentFrameAudioList { get; set; }

		public bool IsLoading => AudioList.Exists((SoundAudio x) => x.IsLoading);

		internal void Init(string label, SoundGroup group)
		{
			Group = group;
			Label = label;
			AudioList = new List<SoundAudio>();
			CurrentFrameAudioList = new List<SoundAudio>();
		}

		private void OnDestroy()
		{
			Group.Remove(Label);
		}

		internal void Remove(SoundAudio audio)
		{
			AudioList.Remove(audio);
			if (Group.AutoDestoryPlayer && AudioList.Count == 0)
			{
				Object.Destroy(base.gameObject);
			}
		}

		public bool IsStop()
		{
			foreach (SoundAudio audio in AudioList)
			{
				if (audio != null)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsPlaying()
		{
			foreach (SoundAudio audio in AudioList)
			{
				if (audio != null && audio.IsPlaying())
				{
					return true;
				}
			}
			return false;
		}

		public bool IsPlayingLoop()
		{
			foreach (SoundAudio audio in AudioList)
			{
				if (audio != null && audio.IsPlayingLoop())
				{
					return true;
				}
			}
			return false;
		}

		private void LateUpdate()
		{
			CurrentFrameAudioList.Clear();
		}

		internal void Play(SoundData data, float fadeInTime, float fadeOutTime)
		{
			switch (data.PlayMode)
			{
			case SoundPlayMode.Add:
				PlayAdd(data, fadeInTime, fadeOutTime);
				break;
			case SoundPlayMode.Replay:
				PlayFade(data, fadeInTime, fadeOutTime, true);
				break;
			case SoundPlayMode.NotPlaySame:
				if (!(Audio != null) || !Audio.IsPlaying(data.Clip))
				{
					PlayFade(data, fadeInTime, fadeOutTime, false);
				}
				break;
			}
		}

		private void PlayAdd(SoundData data, float fadeInTime, float fadeOutTime)
		{
			foreach (SoundAudio currentFrameAudio in CurrentFrameAudioList)
			{
				if (currentFrameAudio != null && currentFrameAudio.IsEqualClip(data.Clip))
				{
					return;
				}
			}
			SoundAudio soundAudio = CreateNewAudio(data);
			soundAudio.Play(fadeInTime);
			CurrentFrameAudioList.Add(soundAudio);
		}

		private void PlayFade(SoundData data, float fadeInTime, float fadeOutTime, bool corssFade)
		{
			if (FadeOutAudio != null)
			{
				Object.Destroy(FadeOutAudio.gameObject);
			}
			if (Audio == null)
			{
				Audio = CreateNewAudio(data);
				Audio.Play(fadeInTime);
				return;
			}
			FadeOutAudio = Audio;
			Audio = CreateNewAudio(data);
			FadeOutAudio.FadeOut(fadeOutTime);
			if (corssFade)
			{
				Audio.Play(fadeInTime);
			}
			else if (Audio != null)
			{
				Audio.Play(fadeInTime, fadeOutTime);
			}
		}

		private SoundAudio CreateNewAudio(SoundData soundData)
		{
			SoundAudio soundAudio = base.transform.AddChildGameObjectComponent<SoundAudio>(soundData.Name);
			soundAudio.Init(this, soundData);
			AudioList.Add(soundAudio);
			return soundAudio;
		}

		public void Stop(float fadeTime)
		{
			foreach (SoundAudio audio in AudioList)
			{
				if (!(audio == null))
				{
					audio.FadeOut(fadeTime);
				}
			}
		}

		internal float GetSamplesVolume()
		{
			if (!IsPlaying())
			{
				return 0f;
			}
			return Audio.GetSamplesVolume();
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(AudioList.Count);
			foreach (SoundAudio audio in AudioList)
			{
				bool enableSave = audio.EnableSave;
				writer.Write(enableSave);
				if (enableSave)
				{
					writer.WriteBuffer(audio.Data.Write);
				}
			}
			writer.Write((Audio == null) ? "" : Audio.gameObject.name);
		}

		internal void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num <= 0)
			{
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					if (reader.ReadBoolean())
					{
						SoundData soundData = new SoundData();
						reader.ReadBuffer(soundData.Read);
						Play(soundData, 0.1f, 0f);
					}
				}
				string audioName = reader.ReadString();
				if (!string.IsNullOrEmpty(audioName))
				{
					Audio = AudioList.Find((SoundAudio x) => x != FadeOutAudio && x.gameObject.name == audioName);
				}
				if (Group.AutoDestoryPlayer && AudioList.Count == 0)
				{
					Object.Destroy(base.gameObject);
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
