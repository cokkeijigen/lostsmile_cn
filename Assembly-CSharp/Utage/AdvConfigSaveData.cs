using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AdvConfigSaveData
	{
		[Serializable]
		public class TaggedMasterVolume
		{
			public string tag;

			public float volume;
		}

		public bool isFullScreen;

		public bool isMouseWheelSendMessage = true;

		public bool isEffect = true;

		public bool isSkipUnread;

		public bool isStopSkipInSelection = true;

		public float messageSpeed = 0.5f;

		public float autoBrPageSpeed = 0.5f;

		public float messageWindowTransparency = 0.3f;

		public float soundMasterVolume = 1f;

		public float bgmVolume = 0.5f;

		public float seVolume = 0.5f;

		public float ambienceVolume = 0.5f;

		public float voiceVolume = 0.75f;

		public VoiceStopType voiceStopType;

		public bool isAutoBrPage;

		public float messageSpeedRead = 0.5f;

		public bool hideMessageWindowOnPlayingVoice;

		public List<TaggedMasterVolume> taggedMasterVolumeList = new List<TaggedMasterVolume>
		{
			new TaggedMasterVolume
			{
				tag = "Others",
				volume = 1f
			}
		};

		private const int VERSION0 = 0;

		private const int VERSION = 1;

		public void SetTaggedMasterVolume(string tag, float volume)
		{
			TaggedMasterVolume taggedMasterVolume = taggedMasterVolumeList.Find((TaggedMasterVolume x) => x.tag == tag);
			if (taggedMasterVolume == null)
			{
				taggedMasterVolume = new TaggedMasterVolume();
				taggedMasterVolume.tag = tag;
				taggedMasterVolumeList.Add(taggedMasterVolume);
			}
			taggedMasterVolume.volume = volume;
		}

		public bool TryGetTaggedMasterVolume(string tag, out float volume)
		{
			TaggedMasterVolume taggedMasterVolume = taggedMasterVolumeList.Find((TaggedMasterVolume x) => x.tag == tag);
			if (taggedMasterVolume == null)
			{
				volume = 0f;
				return false;
			}
			volume = taggedMasterVolume.volume;
			return true;
		}

		public virtual void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num <= 1)
			{
				isFullScreen = reader.ReadBoolean();
				isMouseWheelSendMessage = reader.ReadBoolean();
				isEffect = reader.ReadBoolean();
				isSkipUnread = reader.ReadBoolean();
				isStopSkipInSelection = reader.ReadBoolean();
				messageSpeed = reader.ReadSingle();
				autoBrPageSpeed = reader.ReadSingle();
				messageWindowTransparency = reader.ReadSingle();
				soundMasterVolume = reader.ReadSingle();
				bgmVolume = reader.ReadSingle();
				seVolume = reader.ReadSingle();
				ambienceVolume = reader.ReadSingle();
				voiceVolume = reader.ReadSingle();
				voiceStopType = (VoiceStopType)reader.ReadInt32();
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					reader.ReadBoolean();
				}
				isAutoBrPage = reader.ReadBoolean();
				if (num > 0)
				{
					messageSpeedRead = reader.ReadSingle();
					hideMessageWindowOnPlayingVoice = reader.ReadBoolean();
					int num3 = reader.ReadInt32();
					taggedMasterVolumeList.Clear();
					for (int j = 0; j < num3; j++)
					{
						TaggedMasterVolume taggedMasterVolume = new TaggedMasterVolume();
						taggedMasterVolume.tag = reader.ReadString();
						taggedMasterVolume.volume = reader.ReadSingle();
						taggedMasterVolumeList.Add(taggedMasterVolume);
					}
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}

		public virtual void Write(BinaryWriter writer)
		{
			writer.Write(1);
			writer.Write(isFullScreen);
			writer.Write(isMouseWheelSendMessage);
			writer.Write(isEffect);
			writer.Write(isSkipUnread);
			writer.Write(isStopSkipInSelection);
			writer.Write(messageSpeed);
			writer.Write(autoBrPageSpeed);
			writer.Write(messageWindowTransparency);
			writer.Write(soundMasterVolume);
			writer.Write(bgmVolume);
			writer.Write(seVolume);
			writer.Write(ambienceVolume);
			writer.Write(voiceVolume);
			writer.Write((int)voiceStopType);
			writer.Write(0);
			writer.Write(isAutoBrPage);
			writer.Write(messageSpeedRead);
			writer.Write(hideMessageWindowOnPlayingVoice);
			writer.Write(taggedMasterVolumeList.Count);
			foreach (TaggedMasterVolume taggedMasterVolume in taggedMasterVolumeList)
			{
				writer.Write(taggedMasterVolume.tag);
				writer.Write(taggedMasterVolume.volume);
			}
		}
	}
}
