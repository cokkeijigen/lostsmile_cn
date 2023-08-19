using System;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/AnimationPlayer")]
	public class AdvAnimationPlayer : MonoBehaviour
	{
		public const WrapMode NoneOverrideWrapMode = (WrapMode)(-1);

		private Action onComplete;

		private Animation lecayAnimation;

		private Animator animator;

		private const int Version = 0;

		public bool AutoDestory { get; set; }

		public bool EnableSave { get; set; }

		private AnimationClip Clip { get; set; }

		private float Speed { get; set; }

		internal void Play(AnimationClip clip, float speed, Action onComplete = null)
		{
			Clip = clip;
			Speed = speed;
			this.onComplete = onComplete;
			if (clip.legacy)
			{
				PlayAnimatinLegacy(clip, speed);
			}
			else
			{
				Debug.LogError("Not Support");
			}
		}

		internal void Cancel()
		{
			if (lecayAnimation != null)
			{
				lecayAnimation.Stop();
			}
			OnComplete();
		}

		private void PlayAnimatinLegacy(AnimationClip clip, float speed)
		{
			if (lecayAnimation == null)
			{
				lecayAnimation = base.gameObject.GetComponentCreateIfMissing<Animation>();
			}
			lecayAnimation.AddClip(clip, clip.name);
			lecayAnimation[clip.name].speed = speed;
			lecayAnimation.Play(clip.name);
		}

		private float GetTime()
		{
			if (lecayAnimation != null)
			{
				return lecayAnimation[Clip.name].time;
			}
			if ((bool)animator)
			{
				Debug.Log("Not Support");
				return 0f;
			}
			return 0f;
		}

		private void SetTime(float time)
		{
			if (lecayAnimation != null)
			{
				lecayAnimation[Clip.name].time = time;
			}
			else if ((bool)animator)
			{
				Debug.Log("Not Support");
			}
		}

		private void Update()
		{
			if (lecayAnimation != null)
			{
				if (!lecayAnimation.isPlaying)
				{
					OnComplete();
				}
			}
			else if ((bool)animator)
			{
				Debug.LogError("Not Support");
			}
		}

		private void OnComplete()
		{
			if (onComplete != null)
			{
				onComplete();
			}
			if (AutoDestory)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void OnDestroy()
		{
			if (lecayAnimation != null)
			{
				UnityEngine.Object.Destroy(lecayAnimation);
			}
			if ((bool)animator)
			{
				UnityEngine.Object.Destroy(animator);
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(Clip.name);
			writer.Write(Speed);
			writer.Write(GetTime());
		}

		public void Read(BinaryReader reader, AdvEngine engine)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			string text = reader.ReadString();
			float speed = reader.ReadSingle();
			float time = reader.ReadSingle();
			AdvAnimationData advAnimationData = engine.DataManager.SettingDataManager.AnimationSetting.Find(text);
			if (advAnimationData == null)
			{
				Debug.LogError(text + " is not found in Animation sheet");
				UnityEngine.Object.Destroy(this);
				return;
			}
			EnableSave = true;
			AutoDestory = true;
			Play(advAnimationData.Clip, speed);
			SetTime(time);
		}

		internal static void WriteSaveData(BinaryWriter writer, GameObject go)
		{
			AdvAnimationPlayer[] components = go.GetComponents<AdvAnimationPlayer>();
			int num = 0;
			AdvAnimationPlayer[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].EnableSave)
				{
					num++;
				}
			}
			writer.Write(num);
			array = components;
			foreach (AdvAnimationPlayer advAnimationPlayer in array)
			{
				if (advAnimationPlayer.EnableSave)
				{
					advAnimationPlayer.Write(writer);
				}
			}
		}

		internal static void ReadSaveData(BinaryReader reader, GameObject go, AdvEngine engine)
		{
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				go.AddComponent<AdvAnimationPlayer>().Read(reader, engine);
			}
		}
	}
}
