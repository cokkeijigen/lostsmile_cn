using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/Localize")]
	public class UguiLocalize : UguiLocalizeBase
	{
		[SerializeField]
		protected string key;

		[NonSerialized]
		protected string defaultText;

		private Text cachedText;

		public string Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
				ForceRefresh();
			}
		}

		protected Text CachedText
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

		protected override void RefreshSub()
		{
			Text text = CachedText;
			if (text != null && !LanguageManagerBase.Instance.IgnoreLocalizeUiText)
			{
				string text2;
				if (LanguageManagerBase.Instance.TryLocalizeText(key, out text2))
				{
					text.text = text2;
				}
				else
				{
					Debug.LogError(key + " is not found in localize key", this);
				}
			}
		}

		protected override void InitDefault()
		{
			Text text = CachedText;
			if (text != null)
			{
				defaultText = text.text;
			}
		}

		public override void ResetDefault()
		{
			Text text = CachedText;
			if (text != null)
			{
				text.text = defaultText;
			}
		}
	}
}
