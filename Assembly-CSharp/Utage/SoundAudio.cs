using System.Collections;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Sound/Audio")]
	internal class SoundAudio : MonoBehaviour
	{
		private enum SoundStreamStatus
		{
			None,
			FadeIn,
			Play,
			FadeOut
		}

		private SoundStreamStatus status;

		private LinearValue fadeValue = new LinearValue();

		private const int samples = 256;

		private const int channel = 0;

		private static float[] waveData = new float[256];

		private SoundStreamStatus Status => status;

		public AudioSource AudioSource { get; private set; }

		private AudioSource AudioSourceForIntroLoop { get; set; }

		private AudioSource Audio0 { get; set; }

		private AudioSource Audio1 { get; set; }

		internal SoundData Data { get; private set; }

		private SoundAudioPlayer Player { get; set; }

		internal bool IsLoading { get; private set; }

		internal bool EnableSave
		{
			get
			{
				SoundStreamStatus soundStreamStatus = Status;
				if ((uint)(soundStreamStatus - 1) <= 1u)
				{
					return Data.EnableSave;
				}
				return false;
			}
		}

		public void Init(SoundAudioPlayer player, SoundData soundData)
		{
			Player = player;
			Data = soundData;
			Audio0 = base.gameObject.AddComponent<AudioSource>();
			Audio0.playOnAwake = false;
			if (Data.EnableIntroLoop)
			{
				Audio1 = base.gameObject.AddComponent<AudioSource>();
				Audio1.playOnAwake = false;
				Audio1.clip = Data.Clip;
				Audio1.loop = false;
			}
			AudioSource = Audio0;
			AudioSource.clip = Data.Clip;
			AudioSource.loop = Data.IsLoop && !Data.EnableIntroLoop;
			if (Data.File != null)
			{
				Data.File.AddReferenceComponent(base.gameObject);
			}
		}

		private void OnDestroy()
		{
			Player.Remove(this);
		}

		internal void Play(float fadeInTime, float delay = 0f)
		{
			StartCoroutine(CoWaitDelay(fadeInTime, delay));
		}

		private IEnumerator CoWaitDelay(float fadeInTime, float delay)
		{
			IsLoading = Data.File != null && !Data.File.IsLoadEnd;
			if (IsLoading)
			{
				AssetFileManager.Load(Data.File, this);
			}
			if (delay > 0f)
			{
				yield return new WaitForSeconds(delay);
			}
			if (IsLoading)
			{
				while (!Data.File.IsLoadEnd)
				{
					yield return null;
				}
				Data.File.Unuse(this);
			}
			IsLoading = false;
			PlaySub(fadeInTime);
		}

		private void PlaySub(float fadeInTime)
		{
			float volume = GetVolume();
			AudioSource.clip = Data.Clip;
			if (Data.EnableIntroLoop)
			{
				Audio1.clip = Data.Clip;
				Audio1.volume = volume;
			}
			if (fadeInTime > 0f)
			{
				status = SoundStreamStatus.FadeIn;
				fadeValue.Init(fadeInTime, fadeValue.GetValue(), 1f);
			}
			else
			{
				status = SoundStreamStatus.Play;
				fadeValue.Init(0f, 1f, 1f);
			}
			AudioSource.volume = volume;
			if (Data.EnableIntroLoop)
			{
				AudioSource.PlayScheduled(AudioSettings.dspTime + 0.10000000149011612);
			}
			else
			{
				AudioSource.Play();
			}
		}

		public void End()
		{
			if (Audio0 != null)
			{
				Audio0.Stop();
			}
			if (Audio1 != null)
			{
				Audio1.Stop();
			}
			Object.Destroy(base.gameObject);
		}

		private void EndFadeOut()
		{
			End();
		}

		public bool IsEqualClip(AudioClip clip)
		{
			if (AudioSource != null)
			{
				return AudioSource.clip == clip;
			}
			return false;
		}

		public bool IsPlaying(AudioClip clip)
		{
			if (IsEqualClip(clip) && IsPlaying())
			{
				return status == SoundStreamStatus.Play;
			}
			return false;
		}

		public bool IsPlaying()
		{
			SoundStreamStatus soundStreamStatus = status;
			if ((uint)(soundStreamStatus - 1) <= 1u)
			{
				return true;
			}
			return false;
		}

		public bool IsPlayingLoop()
		{
			if (IsPlaying())
			{
				return Data.IsLoop;
			}
			return false;
		}

		public void FadeOut(float fadeTime)
		{
			if (fadeTime > 0f && IsPlaying())
			{
				status = SoundStreamStatus.FadeOut;
				fadeValue.Init(fadeTime, fadeValue.GetValue(), 0f);
			}
			else
			{
				EndFadeOut();
			}
		}

		private void Update()
		{
			switch (status)
			{
			case SoundStreamStatus.Play:
				IntroUpdate();
				UpdatePlay();
				break;
			case SoundStreamStatus.FadeIn:
				IntroUpdate();
				UpdateFadeIn();
				break;
			case SoundStreamStatus.FadeOut:
				IntroUpdate();
				UpdateFadeOut();
				break;
			}
		}

		private void IntroUpdate()
		{
			if (!Data.EnableIntroLoop)
			{
				return;
			}
			if (AudioSourceForIntroLoop == null && AudioSource.time > 0f)
			{
				SetNextIntroLoop();
			}
			if (IsEndCurrentAudio())
			{
				AudioSource = AudioSourceForIntroLoop;
				if (AudioSource != null && !AudioSource.isPlaying)
				{
					AudioSource.Play();
				}
				SetNextIntroLoop();
			}
		}

		private void SetNextIntroLoop()
		{
			AudioSourceForIntroLoop = ((AudioSource == Audio0) ? Audio1 : Audio0);
			AudioSourceForIntroLoop.Stop();
			AudioSourceForIntroLoop.time = Data.IntroTime;
			if (AudioSource != null && AudioSource.clip != null)
			{
				float num = Mathf.Max(0f, AudioSource.clip.length - AudioSource.time);
				AudioSourceForIntroLoop.PlayScheduled(AudioSettings.dspTime + (double)num);
			}
		}

		private bool IsEndCurrentAudio()
		{
			if (AudioSource == null)
			{
				return false;
			}
			if (AudioSource.isPlaying)
			{
				return false;
			}
			if (AudioSource.clip.length - AudioSource.time < 0.001f)
			{
				return true;
			}
			if (Mathf.Approximately(AudioSource.time, 0f) || Mathf.Approximately(AudioSource.time, Data.IntroTime))
			{
				return true;
			}
			return false;
		}

		private void UpdatePlay()
		{
			if (!Data.IsLoop && IsEndCurrentAudio())
			{
				EndFadeOut();
			}
		}

		private void UpdateFadeIn()
		{
			fadeValue.IncTime();
			if (fadeValue.IsEnd())
			{
				status = SoundStreamStatus.Play;
			}
		}

		private void UpdateFadeOut()
		{
			fadeValue.IncTime();
			if (fadeValue.IsEnd())
			{
				EndFadeOut();
			}
		}

		private void LateUpdate()
		{
			if (AudioSource == null)
			{
				return;
			}
			float volume = GetVolume();
			if (!Mathf.Approximately(volume, AudioSource.volume))
			{
				if (Audio0 != null)
				{
					Audio0.volume = volume;
				}
				if (Audio1 != null)
				{
					Audio1.volume = volume;
				}
			}
		}

		private float GetVolume()
		{
			return fadeValue.GetValue() * Data.Volume * Player.Group.GetVolume(Data.Tag);
		}

		public float GetSamplesVolume()
		{
			if (AudioSource.isPlaying)
			{
				return GetSamplesVolume(AudioSource);
			}
			return 0f;
		}

		private float GetSamplesVolume(AudioSource audio)
		{
			audio.GetOutputData(waveData, 0);
			float num = 0f;
			float[] array = waveData;
			foreach (float f in array)
			{
				num += Mathf.Abs(f);
			}
			return num / 256f;
		}
	}
}
