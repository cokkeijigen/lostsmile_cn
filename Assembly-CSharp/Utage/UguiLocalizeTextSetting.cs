using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/Localize/TextSetting")]
	public class UguiLocalizeTextSetting : UguiLocalizeBase
	{
		[Serializable]
		public class Setting
		{
			public string language;

			public Font font;

			public UguiNovelTextSettings setting;

			public int fontSize = 20;

			public float lineSpacing = 1f;
		}

		[SerializeField]
		private List<Setting> settingList = new List<Setting>();

		[NonSerialized]
		private Setting defaultSetting;

		private Text cachedText;

		private UguiNovelText novelText;

		private Text CachedText
		{
			get
			{
				if (null == cachedText)
				{
					cachedText = GetComponent<Text>();
				}
				return cachedText;
			}
		}

		private UguiNovelText NovelText
		{
			get
			{
				if (null == novelText)
				{
					novelText = GetComponent<UguiNovelText>();
				}
				return novelText;
			}
		}

		protected override void RefreshSub()
		{
			if (!(CachedText != null))
			{
				return;
			}
			Setting setting = settingList.Find((Setting x) => x.language == currentLanguage);
			if (setting == null)
			{
				setting = defaultSetting;
			}
			if (setting != null)
			{
				CachedText.font = ((setting.font != null) ? setting.font : defaultSetting.font);
				CachedText.fontSize = setting.fontSize;
				CachedText.lineSpacing = setting.lineSpacing;
				if (NovelText != null && setting.setting != null)
				{
					NovelText.TextGenerator.TextSettings = setting.setting;
				}
			}
		}

		protected override void InitDefault()
		{
			defaultSetting = new Setting();
			defaultSetting.font = CachedText.font;
			defaultSetting.fontSize = CachedText.fontSize;
			defaultSetting.lineSpacing = CachedText.lineSpacing;
			if (NovelText != null)
			{
				defaultSetting.setting = NovelText.TextGenerator.TextSettings;
			}
		}

		public override void ResetDefault()
		{
			CachedText.font = defaultSetting.font;
			CachedText.fontSize = defaultSetting.fontSize;
			CachedText.lineSpacing = defaultSetting.lineSpacing;
			if (NovelText != null)
			{
				NovelText.TextGenerator.TextSettings = defaultSetting.setting;
			}
		}
	}
}
