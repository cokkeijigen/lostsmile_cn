using System.Collections.Generic;

namespace Utage
{
	public class AdvSettingDataManager
	{
		private AdvBootSetting bootSetting = new AdvBootSetting();

		private AdvCharacterSetting characterSetting = new AdvCharacterSetting();

		private AdvTextureSetting textureSetting = new AdvTextureSetting();

		private AdvSoundSetting soundSetting = new AdvSoundSetting();

		private AdvLayerSetting layerSetting = new AdvLayerSetting();

		private AdvParamManager defaultParam = new AdvParamManager();

		private AdvSceneGallerySetting sceneGallerySetting = new AdvSceneGallerySetting();

		private AdvLocalizeSetting localizeSetting = new AdvLocalizeSetting();

		private AdvAnimationSetting animationSetting = new AdvAnimationSetting();

		private AdvEyeBlinkSetting eyeBlinkSetting = new AdvEyeBlinkSetting();

		private AdvLipSynchSetting lipSynchSetting = new AdvLipSynchSetting();

		private AdvParticleSetting advParticleSetting = new AdvParticleSetting();

		private AdvVideoSetting videoSetting = new AdvVideoSetting();

		private List<IAdvSetting> settingDataList;

		public AdvImportScenarios ImportedScenarios { get; set; }

		public AdvBootSetting BootSetting
		{
			get
			{
				return bootSetting;
			}
		}

		public AdvCharacterSetting CharacterSetting
		{
			get
			{
				return characterSetting;
			}
		}

		public AdvTextureSetting TextureSetting
		{
			get
			{
				return textureSetting;
			}
		}

		public AdvSoundSetting SoundSetting
		{
			get
			{
				return soundSetting;
			}
		}

		public AdvLayerSetting LayerSetting
		{
			get
			{
				return layerSetting;
			}
		}

		public AdvParamManager DefaultParam
		{
			get
			{
				return defaultParam;
			}
		}

		public AdvSceneGallerySetting SceneGallerySetting
		{
			get
			{
				return sceneGallerySetting;
			}
		}

		public AdvLocalizeSetting LocalizeSetting
		{
			get
			{
				return localizeSetting;
			}
		}

		public AdvAnimationSetting AnimationSetting
		{
			get
			{
				return animationSetting;
			}
		}

		public AdvEyeBlinkSetting EyeBlinkSetting
		{
			get
			{
				return eyeBlinkSetting;
			}
		}

		public AdvLipSynchSetting LipSynchSetting
		{
			get
			{
				return lipSynchSetting;
			}
		}

		public AdvParticleSetting ParticleSetting
		{
			get
			{
				return advParticleSetting;
			}
		}

		public AdvVideoSetting VideoSetting
		{
			get
			{
				return videoSetting;
			}
		}

		private List<IAdvSetting> SettingDataList
		{
			get
			{
				if (settingDataList == null)
				{
					settingDataList = new List<IAdvSetting>();
					settingDataList.Add(LayerSetting);
					settingDataList.Add(CharacterSetting);
					settingDataList.Add(TextureSetting);
					settingDataList.Add(SoundSetting);
					settingDataList.Add(DefaultParam);
					settingDataList.Add(SceneGallerySetting);
					settingDataList.Add(LocalizeSetting);
					settingDataList.Add(AnimationSetting);
					settingDataList.Add(EyeBlinkSetting);
					settingDataList.Add(LipSynchSetting);
					settingDataList.Add(ParticleSetting);
				}
				return settingDataList;
			}
		}

		public void BootInit(string rootDirResource)
		{
			BootSetting.BootInit(rootDirResource);
			if (!(ImportedScenarios != null))
			{
				return;
			}
			foreach (AdvChapterData chapter in ImportedScenarios.Chapters)
			{
				chapter.BootInit(this);
			}
		}

		internal void DownloadAll()
		{
			SettingDataList.ForEach(delegate(IAdvSetting x)
			{
				x.DownloadAll();
			});
		}
	}
}
