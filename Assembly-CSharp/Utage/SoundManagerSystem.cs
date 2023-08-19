using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public class SoundManagerSystem : SoundManagerSystemInterface
	{
		private const string GameObjectNameSe = "One shot audio";

		private Dictionary<string, SoundGroup> groups = new Dictionary<string, SoundGroup>();

		private const int Version = 0;

		private Dictionary<string, SoundGroup> Groups
		{
			get
			{
				return groups;
			}
		}

		internal SoundManager SoundManager { get; private set; }

		public bool IsLoading
		{
			get
			{
				foreach (KeyValuePair<string, SoundGroup> group in Groups)
				{
					if (group.Value.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		public void Init(SoundManager soundManager, List<string> saveStreamNameList)
		{
			SoundManager = soundManager;
		}

		private SoundGroup GetGroupAndCreateIfMissing(string name)
		{
			SoundGroup soundGroup = GetGroup(name);
			if (soundGroup == null)
			{
				soundGroup = SoundManager.transform.Find<SoundGroup>(name);
				if (soundGroup == null)
				{
					soundGroup = SoundManager.transform.AddChildGameObjectComponent<SoundGroup>(name);
					switch (name)
					{
					case "Bgm":
						soundGroup.DuckGroups.Add(GetGroupAndCreateIfMissing("Voice"));
						break;
					case "Ambience":
						soundGroup.DuckGroups.Add(GetGroupAndCreateIfMissing("Voice"));
						break;
					case "Voice":
						soundGroup.AutoDestoryPlayer = true;
						break;
					case "Se":
						soundGroup.AutoDestoryPlayer = true;
						soundGroup.MultiPlay = true;
						break;
					}
				}
				soundGroup.Init(this);
				Groups.Add(name, soundGroup);
			}
			return soundGroup;
		}

		public SoundGroup GetGroup(string name)
		{
			SoundGroup value;
			if (!Groups.TryGetValue(name, out value))
			{
				return null;
			}
			return value;
		}

		public void Play(string groupName, string label, SoundData data, float fadeInTime, float fadeOutTime)
		{
			GetGroupAndCreateIfMissing(groupName).Play(label, data, fadeInTime, fadeOutTime);
		}

		public void Stop(string groupName, string label, float fadeTime)
		{
			SoundGroup group = GetGroup(groupName);
			if (!(group == null))
			{
				group.Stop(label, fadeTime);
			}
		}

		public bool IsPlaying(string groupName, string label)
		{
			SoundGroup group = GetGroup(groupName);
			if (group == null)
			{
				return false;
			}
			return group.IsPlaying(label);
		}

		public AudioSource GetAudioSource(string groupName, string label)
		{
			SoundGroup group = GetGroup(groupName);
			if (group == null)
			{
				return null;
			}
			return group.GetAudioSource(label);
		}

		public float GetSamplesVolume(string groupName, string label)
		{
			SoundGroup group = GetGroup(groupName);
			if (group == null)
			{
				return 0f;
			}
			return group.GetSamplesVolume(label);
		}

		public void StopGroup(string groupName, float fadeTime)
		{
			SoundGroup group = GetGroup(groupName);
			if (!(group == null))
			{
				group.StopAll(fadeTime);
			}
		}

		public void StopGroupIgnoreLoop(string groupName, float fadeTime)
		{
			SoundGroup group = GetGroup(groupName);
			if (!(group == null))
			{
				group.StopAllIgnoreLoop(fadeTime);
			}
		}

		public void StopAll(float fadeTime)
		{
			foreach (KeyValuePair<string, SoundGroup> group in Groups)
			{
				group.Value.StopAll(fadeTime);
			}
		}

		public void StopAllLoop(float fadeTime)
		{
			foreach (KeyValuePair<string, SoundGroup> group in Groups)
			{
				group.Value.StopAllLoop(fadeTime);
			}
		}

		public float GetMasterVolume(string groupName)
		{
			SoundGroup group = GetGroup(groupName);
			if (group == null)
			{
				Debug.LogError(groupName + " is not created");
				return 1f;
			}
			return group.MasterVolume;
		}

		public void SetMasterVolume(string groupName, float volume)
		{
			GetGroupAndCreateIfMissing(groupName).MasterVolume = volume;
		}

		public float GetGroupVolume(string groupName)
		{
			SoundGroup group = GetGroup(groupName);
			if (group == null)
			{
				Debug.LogError(groupName + " is not created");
				return 1f;
			}
			return group.GroupVolume;
		}

		public void SetGroupVolume(string groupName, float volume, float fadeTime)
		{
			SoundGroup groupAndCreateIfMissing = GetGroupAndCreateIfMissing(groupName);
			groupAndCreateIfMissing.GroupVolume = volume;
			groupAndCreateIfMissing.GroupVolumeFadeTime = fadeTime;
		}

		public bool IsMultiPlay(string groupName)
		{
			SoundGroup group = GetGroup(groupName);
			if (group == null)
			{
				Debug.LogError(groupName + " is not created");
				return false;
			}
			return group.MultiPlay;
		}

		public void SetMultiPlay(string groupName, bool multiPlay)
		{
			GetGroupAndCreateIfMissing(groupName).MultiPlay = multiPlay;
		}

		public void WriteSaveData(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(Groups.Count);
			foreach (KeyValuePair<string, SoundGroup> group in Groups)
			{
				writer.Write(group.Key);
			}
			foreach (KeyValuePair<string, SoundGroup> group2 in Groups)
			{
				writer.WriteBuffer(group2.Value.Write);
			}
		}

		public void ReadSaveDataBuffer(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num <= 0)
			{
				int num2 = reader.ReadInt32();
				List<SoundGroup> list = new List<SoundGroup>();
				for (int i = 0; i < num2; i++)
				{
					string name = reader.ReadString();
					list.Add(GetGroupAndCreateIfMissing(name));
				}
				for (int j = 0; j < num2; j++)
				{
					reader.ReadBuffer(list[j].Read);
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
