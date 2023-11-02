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

		public AdvBootSetting BootSetting => bootSetting;

		public AdvCharacterSetting CharacterSetting => characterSetting;

		public AdvTextureSetting TextureSetting => textureSetting;

		public AdvSoundSetting SoundSetting => soundSetting;

		public AdvLayerSetting LayerSetting => layerSetting;

		public AdvParamManager DefaultParam => defaultParam;

		public AdvSceneGallerySetting SceneGallerySetting => sceneGallerySetting;

		public AdvLocalizeSetting LocalizeSetting => localizeSetting;

		public AdvAnimationSetting AnimationSetting => animationSetting;

		public AdvEyeBlinkSetting EyeBlinkSetting => eyeBlinkSetting;

		public AdvLipSynchSetting LipSynchSetting => lipSynchSetting;

		public AdvParticleSetting ParticleSetting => advParticleSetting;

		public AdvVideoSetting VideoSetting => videoSetting;

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
