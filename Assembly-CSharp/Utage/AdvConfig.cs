using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/Config")]
	public class AdvConfig : MonoBehaviour, IBinaryIO
	{
		[SerializeField]
		private bool dontUseSystemSaveData;

		[SerializeField]
		private bool ignoreSoundVolume;

		[SerializeField]
		[FormerlySerializedAs("dontUseFullScreen")]
		private bool dontSaveFullScreen = true;

		[SerializeField]
		private float sendCharWaitSecMax = 0.1f;

		[SerializeField]
		private float autoPageWaitSecMax = 2.5f;

		[SerializeField]
		private float autoPageWaitSecMin;

		[SerializeField]
		private bool forceSkipInputCtl = true;

		[SerializeField]
		private bool useMessageSpeedRead;

		[FormerlySerializedAs("skipSpped")]
		[SerializeField]
		private float skipSpeed = 20f;

		[SerializeField]
		private bool skipVoiceAndSe;

		[SerializeField]
		protected AdvConfigSaveData defaultData;

		protected AdvConfigSaveData current = new AdvConfigSaveData();

		private bool isSkip;

		public float SkipSpped
		{
			get
			{
				return skipSpeed;
			}
		}

		public bool SkipVoiceAndSe
		{
			get
			{
				return skipVoiceAndSe;
			}
		}

		public virtual string SaveKey
		{
			get
			{
				return "AdvConfig";
			}
		}

		public bool IsFullScreen
		{
			get
			{
				return current.isFullScreen;
			}
			set
			{
				if (UtageToolKit.IsPlatformStandAloneOrEditor())
				{
					current.isFullScreen = value;
					Screen.fullScreen = value;
				}
			}
		}

		public bool IsMouseWheelSendMessage
		{
			get
			{
				return current.isMouseWheelSendMessage;
			}
			set
			{
				current.isMouseWheelSendMessage = value;
			}
		}

		public bool IsEffect
		{
			get
			{
				return current.isEffect;
			}
			set
			{
				current.isEffect = value;
			}
		}

		public bool IsSkipUnread
		{
			get
			{
				return current.isSkipUnread;
			}
			set
			{
				current.isSkipUnread = value;
			}
		}

		public bool IsStopSkipInSelection
		{
			get
			{
				return current.isStopSkipInSelection;
			}
			set
			{
				current.isStopSkipInSelection = value;
			}
		}

		public float MessageSpeed
		{
			get
			{
				return current.messageSpeed;
			}
			set
			{
				current.messageSpeed = value;
			}
		}

		public float MessageSpeedRead
		{
			get
			{
				return current.messageSpeedRead;
			}
			set
			{
				current.messageSpeedRead = value;
			}
		}

		public bool HideMessageWindowOnPlayingVoice
		{
			get
			{
				return current.hideMessageWindowOnPlayingVoice;
			}
			set
			{
				current.hideMessageWindowOnPlayingVoice = value;
			}
		}

		public float AutoBrPageSpeed
		{
			get
			{
				return current.autoBrPageSpeed;
			}
			set
			{
				current.autoBrPageSpeed = value;
			}
		}

		public float AutoPageWaitTime
		{
			get
			{
				return (1f - AutoBrPageSpeed) * (autoPageWaitSecMax - autoPageWaitSecMin) + autoPageWaitSecMin;
			}
		}

		public float MessageWindowTransparency
		{
			get
			{
				return current.messageWindowTransparency;
			}
			set
			{
				current.messageWindowTransparency = value;
			}
		}

		public float MessageWindowAlpha
		{
			get
			{
				return 1f - MessageWindowTransparency;
			}
		}

		public float SoundMasterVolume
		{
			get
			{
				return current.soundMasterVolume;
			}
			set
			{
				current.soundMasterVolume = value;
				SoundManager instance = SoundManager.GetInstance();
				if ((bool)instance)
				{
					instance.MasterVolume = value;
				}
			}
		}

		public float BgmVolume
		{
			get
			{
				return current.bgmVolume;
			}
			set
			{
				current.bgmVolume = value;
				SoundManager instance = SoundManager.GetInstance();
				if ((bool)instance)
				{
					instance.BgmVolume = value;
				}
			}
		}

		public float SeVolume
		{
			get
			{
				return current.seVolume;
			}
			set
			{
				current.seVolume = value;
				SoundManager instance = SoundManager.GetInstance();
				if ((bool)instance)
				{
					instance.SeVolume = value;
				}
			}
		}

		public float AmbienceVolume
		{
			get
			{
				return current.ambienceVolume;
			}
			set
			{
				current.ambienceVolume = value;
				SoundManager instance = SoundManager.GetInstance();
				if ((bool)instance)
				{
					instance.AmbienceVolume = value;
				}
			}
		}

		public float VoiceVolume
		{
			get
			{
				return current.voiceVolume;
			}
			set
			{
				current.voiceVolume = value;
				SoundManager instance = SoundManager.GetInstance();
				if ((bool)instance)
				{
					instance.VoiceVolume = value;
				}
			}
		}

		public VoiceStopType VoiceStopType
		{
			get
			{
				return current.voiceStopType;
			}
			set
			{
				current.voiceStopType = value;
			}
		}

		public bool NoSkip { get; set; }

		public bool IsSkip
		{
			get
			{
				return isSkip;
			}
			set
			{
				isSkip = value;
			}
		}

		public bool IsAutoBrPage
		{
			get
			{
				return current.isAutoBrPage;
			}
			set
			{
				current.isAutoBrPage = value;
			}
		}

		public void InitDefault()
		{
			SetData(defaultData, false);
		}

		public virtual void OnRead(BinaryReader reader)
		{
			AdvConfigSaveData advConfigSaveData = new AdvConfigSaveData();
			advConfigSaveData.Read(reader);
			if (!dontUseSystemSaveData)
			{
				SetData(advConfigSaveData, false);
			}
			else
			{
				InitDefault();
			}
		}

		public virtual void OnWrite(BinaryWriter writer)
		{
			current.Write(writer);
		}

		public void InitDefaultAll()
		{
			SetData(defaultData, true);
		}

		protected virtual void SetData(AdvConfigSaveData data, bool isSetDefault)
		{
			if (UtageToolKit.IsPlatformStandAloneOrEditor())
			{
				if (dontSaveFullScreen)
				{
					IsFullScreen = Screen.fullScreen;
				}
				else
				{
					IsFullScreen = data.isFullScreen;
				}
			}
			IsMouseWheelSendMessage = data.isMouseWheelSendMessage;
			IsEffect = data.isEffect;
			IsSkipUnread = data.isSkipUnread;
			IsStopSkipInSelection = data.isStopSkipInSelection;
			MessageSpeed = data.messageSpeed;
			AutoBrPageSpeed = data.autoBrPageSpeed;
			MessageWindowTransparency = data.messageWindowTransparency;
			if (!ignoreSoundVolume)
			{
				SoundMasterVolume = data.soundMasterVolume;
				BgmVolume = data.bgmVolume;
				SeVolume = data.seVolume;
				AmbienceVolume = data.ambienceVolume;
				VoiceVolume = data.voiceVolume;
			}
			VoiceStopType = data.voiceStopType;
			if (!isSetDefault)
			{
				IsAutoBrPage = data.isAutoBrPage;
			}
			MessageSpeedRead = data.messageSpeedRead;
			HideMessageWindowOnPlayingVoice = data.hideMessageWindowOnPlayingVoice;
			current.taggedMasterVolumeList.Clear();
			int count = data.taggedMasterVolumeList.Count;
			for (int i = 0; i < count; i++)
			{
				SetTaggedMasterVolume(data.taggedMasterVolumeList[i].tag, data.taggedMasterVolumeList[i].volume);
			}
		}

		public void ToggleFullScreen()
		{
			IsFullScreen = !IsFullScreen;
		}

		public void ToggleMouseWheelSendMessage()
		{
			IsMouseWheelSendMessage = !IsMouseWheelSendMessage;
		}

		public void ToggleEffect()
		{
			IsEffect = !IsEffect;
		}

		public void ToggleSkipUnread()
		{
			IsSkipUnread = !IsSkipUnread;
		}

		public void ToggleStopSkipInSelection()
		{
			IsStopSkipInSelection = !IsStopSkipInSelection;
		}

		public float GetTimeSendChar(bool read)
		{
			if (read && useMessageSpeedRead)
			{
				return (1f - MessageSpeedRead) * sendCharWaitSecMax;
			}
			return (1f - MessageSpeed) * sendCharWaitSecMax;
		}

		public void SetTaggedMasterVolume(string tag, float volume)
		{
			current.SetTaggedMasterVolume(tag, volume);
			SoundManager instance = SoundManager.GetInstance();
			if ((bool)instance)
			{
				instance.SetTaggedMasterVolume(tag, volume);
			}
		}

		public bool TryGetTaggedMasterVolume(string tag, out float volume)
		{
			return current.TryGetTaggedMasterVolume(tag, out volume);
		}

		public virtual bool CheckSkip(bool isReadPage)
		{
			if (NoSkip)
			{
				return false;
			}
			if (forceSkipInputCtl && InputUtil.IsInputControl())
			{
				return true;
			}
			if (isSkip)
			{
				if (IsSkipUnread)
				{
					return true;
				}
				return isReadPage;
			}
			return false;
		}

		public void ToggleSkip()
		{
			isSkip = !isSkip;
		}

		public void StopSkipInSelection()
		{
			if (IsStopSkipInSelection && isSkip)
			{
				isSkip = false;
			}
		}

		public void ToggleAuto()
		{
			IsAutoBrPage = !IsAutoBrPage;
		}
	}
}
