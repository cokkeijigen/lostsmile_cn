using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/TemplateUI/Config")]
public class UtageUguiConfig : UguiView
{
	[Serializable]
	protected class TagedMasterVolumSliders
	{
		public string tag = "";

		public Slider volumeSlider;
	}

	[SerializeField]
	protected AdvEngine engine;

	[SerializeField]
	protected UtageUguiTitle title;

	[SerializeField]
	protected Toggle checkFullscreen;

	[SerializeField]
	protected Toggle checkMouseWheel;

	[SerializeField]
	protected Toggle checkSkipUnread;

	[SerializeField]
	protected Toggle checkStopSkipInSelection;

	[SerializeField]
	protected Toggle checkHideMessageWindowOnPlyaingVoice;

	[SerializeField]
	protected Slider sliderMessageSpeed;

	[SerializeField]
	protected Slider sliderMessageSpeedRead;

	[SerializeField]
	protected Slider sliderAutoBrPageSpeed;

	[SerializeField]
	protected Slider sliderMessageWindowTransparency;

	[SerializeField]
	protected Slider sliderSoundMasterVolume;

	[SerializeField]
	protected Slider sliderBgmVolume;

	[SerializeField]
	protected Slider sliderSeVolume;

	[SerializeField]
	protected Slider sliderAmbienceVolume;

	[SerializeField]
	protected Slider sliderVoiceVolume;

	[SerializeField]
	protected UguiToggleGroupIndexed radioButtonsVoiceStopType;

	[SerializeField]
	protected List<TagedMasterVolumSliders> tagedMasterVolumSliders;

	protected bool isInit;

	public AdvEngine Engine => engine ?? (engine = UnityEngine.Object.FindObjectOfType<AdvEngine>());

	protected virtual AdvConfig Config => Engine.Config;

	public virtual float MessageSpeed
	{
		set
		{
			if (IsInit)
			{
				Config.MessageSpeed = value;
			}
		}
	}

	public virtual float MessageSpeedRead
	{
		set
		{
			if (IsInit)
			{
				Config.MessageSpeedRead = value;
			}
		}
	}

	public virtual float AutoBrPageSpeed
	{
		set
		{
			if (IsInit)
			{
				Config.AutoBrPageSpeed = value;
			}
		}
	}

	public virtual float MessageWindowTransparency
	{
		set
		{
			if (IsInit)
			{
				Config.MessageWindowTransparency = value;
			}
		}
	}

	public virtual float SoundMasterVolume
	{
		set
		{
			if (IsInit)
			{
				Config.SoundMasterVolume = value;
			}
		}
	}

	public virtual float BgmVolume
	{
		set
		{
			if (IsInit)
			{
				Config.BgmVolume = value;
			}
		}
	}

	public virtual float SeVolume
	{
		set
		{
			if (IsInit)
			{
				Config.SeVolume = value;
			}
		}
	}

	public virtual float AmbienceVolume
	{
		set
		{
			if (IsInit)
			{
				Config.AmbienceVolume = value;
			}
		}
	}

	public virtual float VoiceVolume
	{
		set
		{
			if (IsInit)
			{
				Config.VoiceVolume = value;
			}
		}
	}

	public virtual bool IsFullScreen
	{
		set
		{
			if (IsInit)
			{
				Config.IsFullScreen = value;
			}
		}
	}

	public virtual bool IsMouseWheel
	{
		set
		{
			if (IsInit)
			{
				Config.IsMouseWheelSendMessage = value;
			}
		}
	}

	public virtual bool IsEffect
	{
		set
		{
			if (IsInit)
			{
				Config.IsEffect = value;
			}
		}
	}

	public virtual bool IsSkipUnread
	{
		set
		{
			if (IsInit)
			{
				Config.IsSkipUnread = value;
			}
		}
	}

	public virtual bool IsStopSkipInSelection
	{
		set
		{
			if (IsInit)
			{
				Config.IsStopSkipInSelection = value;
			}
		}
	}

	public virtual bool HideMessageWindowOnPlyaingVoice
	{
		set
		{
			if (IsInit)
			{
				Config.HideMessageWindowOnPlayingVoice = value;
			}
		}
	}

	public virtual bool IsInit
	{
		get
		{
			return isInit;
		}
		set
		{
			isInit = value;
		}
	}

	protected virtual void OnOpen()
	{
		isInit = false;
		if (Engine.SaveManager.Type != AdvSaveManager.SaveType.SavePoint)
		{
			Engine.SaveManager.ClearCaptureTexture();
		}
		StartCoroutine(CoWaitOpen());
	}

	protected virtual IEnumerator CoWaitOpen()
	{
		if (!Engine.IsWaitBootLoading)
		{
			LoadValues();
		}
		yield break;
	}

	public override void Close()
	{
		Engine.WriteSystemData();
		base.Close();
	}

	protected virtual void Update()
	{
		if (isInit && InputUtil.IsMouseRightButtonDown())
		{
			Back();
		}
	}

	protected virtual void LoadValues()
	{
		isInit = false;
		if ((bool)checkFullscreen)
		{
			checkFullscreen.isOn = Config.IsFullScreen;
		}
		if ((bool)checkMouseWheel)
		{
			checkMouseWheel.isOn = Config.IsMouseWheelSendMessage;
		}
		if ((bool)checkSkipUnread)
		{
			checkSkipUnread.isOn = Config.IsSkipUnread;
		}
		if ((bool)checkStopSkipInSelection)
		{
			checkStopSkipInSelection.isOn = Config.IsStopSkipInSelection;
		}
		if ((bool)checkHideMessageWindowOnPlyaingVoice)
		{
			checkHideMessageWindowOnPlyaingVoice.isOn = Config.HideMessageWindowOnPlayingVoice;
		}
		if ((bool)sliderMessageSpeed)
		{
			sliderMessageSpeed.value = Config.MessageSpeed;
		}
		if ((bool)sliderMessageSpeedRead)
		{
			sliderMessageSpeedRead.value = Config.MessageSpeedRead;
		}
		if ((bool)sliderAutoBrPageSpeed)
		{
			sliderAutoBrPageSpeed.value = Config.AutoBrPageSpeed;
		}
		if ((bool)sliderMessageWindowTransparency)
		{
			sliderMessageWindowTransparency.value = Config.MessageWindowTransparency;
		}
		if ((bool)sliderSoundMasterVolume)
		{
			sliderSoundMasterVolume.value = Config.SoundMasterVolume;
		}
		if ((bool)sliderBgmVolume)
		{
			sliderBgmVolume.value = Config.BgmVolume;
		}
		if ((bool)sliderSeVolume)
		{
			sliderSeVolume.value = Config.SeVolume;
		}
		if ((bool)sliderAmbienceVolume)
		{
			sliderAmbienceVolume.value = Config.AmbienceVolume;
		}
		if ((bool)sliderVoiceVolume)
		{
			sliderVoiceVolume.value = Config.VoiceVolume;
		}
		if ((bool)radioButtonsVoiceStopType)
		{
			radioButtonsVoiceStopType.CurrentIndex = (int)Config.VoiceStopType;
		}
		foreach (TagedMasterVolumSliders tagedMasterVolumSlider in tagedMasterVolumSliders)
		{
			if (!string.IsNullOrEmpty(tagedMasterVolumSlider.tag) && !(tagedMasterVolumSlider.volumeSlider == null) && Config.TryGetTaggedMasterVolume(tagedMasterVolumSlider.tag, out var volume))
			{
				tagedMasterVolumSlider.volumeSlider.value = volume;
			}
		}
		if (!UtageToolKit.IsPlatformStandAloneOrEditor())
		{
			if ((bool)checkFullscreen)
			{
				checkFullscreen.gameObject.SetActive(false);
			}
			if (Application.platform != RuntimePlatform.WebGLPlayer && (bool)checkMouseWheel)
			{
				checkMouseWheel.gameObject.SetActive(false);
			}
		}
		isInit = true;
	}

	public virtual void OnTapBackTitle()
	{
		Engine.EndScenario();
		Close();
		title.Open();
	}

	public virtual void OnTapInitDefaultAll()
	{
		if (IsInit)
		{
			Config.InitDefaultAll();
			LoadValues();
		}
	}

	public virtual void OnTapRadioButtonVoiceStopType(int index)
	{
		if (IsInit)
		{
			Config.VoiceStopType = (VoiceStopType)index;
		}
	}

	public virtual void OnValugeChangedTaggedMasterVolume(string tag, float value)
	{
		if (IsInit)
		{
			Config.SetTaggedMasterVolume(tag, value);
		}
	}
}
