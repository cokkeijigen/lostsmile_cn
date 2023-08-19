using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/LocalizeRectTransform")]
	public class UguiLocalizeRectTransform : UguiLocalizeBase
	{
		[Serializable]
		public class Setting
		{
			public string language;

			public Vector2 anchoredPosition;

			public Vector2 size = new Vector2(100f, 100f);
		}

		[SerializeField]
		private List<Setting> settingList = new List<Setting>();

		[NonSerialized]
		private Setting defaultSetting;

		private RectTransform cachedRectTransform;

		private RectTransform CachedRectTransform
		{
			get
			{
				if (null == cachedRectTransform)
				{
					cachedRectTransform = GetComponent<RectTransform>();
				}
				return cachedRectTransform;
			}
		}

		private bool HasChanged { get; set; }

		protected override void RefreshSub()
		{
			HasChanged = true;
		}

		protected override void InitDefault()
		{
		}

		public override void ResetDefault()
		{
			if (defaultSetting != null)
			{
				CachedRectTransform.anchoredPosition = defaultSetting.anchoredPosition;
				CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, defaultSetting.size.x);
				CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, defaultSetting.size.y);
			}
		}

		private void Update()
		{
			if (defaultSetting == null)
			{
				InitDefaultSetting();
			}
			if (HasChanged)
			{
				Setting setting = settingList.Find((Setting x) => x.language == currentLanguage);
				if (setting == null)
				{
					setting = defaultSetting;
				}
				if (setting != null)
				{
					CachedRectTransform.anchoredPosition = setting.anchoredPosition;
					CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, setting.size.x);
					CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, setting.size.y);
				}
			}
		}

		private void InitDefaultSetting()
		{
			defaultSetting = new Setting();
			defaultSetting.anchoredPosition = CachedRectTransform.anchoredPosition;
			defaultSetting.size = CachedRectTransform.rect.size;
		}
	}
}
