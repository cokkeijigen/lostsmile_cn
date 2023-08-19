using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public abstract class LanguageManagerBase : ScriptableObject
	{
		private static LanguageManagerBase instance;

		private const string Auto = "Auto";

		[SerializeField]
		protected string language = "Auto";

		[SerializeField]
		protected string defaultLanguage = "Japanese";

		[SerializeField]
		protected string dataLanguage = "";

		[SerializeField]
		private List<TextAsset> languageData = new List<TextAsset>();

		[SerializeField]
		private bool ignoreLocalizeUiText;

		[SerializeField]
		private bool ignoreLocalizeVoice = true;

		[SerializeField]
		private List<string> voiceLanguages = new List<string>();

		private string currentLanguage;

		public static LanguageManagerBase Instance
		{
			get
			{
				if (instance == null)
				{
					if ((bool)CustomProjectSetting.Instance)
					{
						instance = CustomProjectSetting.Instance.Language;
					}
					if (instance != null)
					{
						instance.Init();
					}
				}
				return instance;
			}
		}

		public string Language
		{
			get
			{
				return language;
			}
		}

		public string DefaultLanguage
		{
			get
			{
				return defaultLanguage;
			}
		}

		public string DataLanguage
		{
			get
			{
				return dataLanguage;
			}
		}

		public bool IgnoreLocalizeUiText
		{
			get
			{
				return ignoreLocalizeUiText;
			}
		}

		public bool IgnoreLocalizeVoice
		{
			get
			{
				return ignoreLocalizeVoice;
			}
		}

		public List<string> VoiceLanguages
		{
			get
			{
				return voiceLanguages;
			}
		}

		public Action OnChangeLanugage { get; set; }

		public string CurrentLanguage
		{
			get
			{
				return currentLanguage;
			}
			set
			{
				if (currentLanguage != value)
				{
					currentLanguage = value;
					RefreshCurrentLanguage();
				}
			}
		}

		private LanguageData Data { get; set; }

		public List<string> Languages
		{
			get
			{
				return Data.Languages;
			}
		}

		private void OnEnable()
		{
			Init();
		}

		private void Init()
		{
			Data = new LanguageData();
			foreach (TextAsset languageDatum in languageData)
			{
				if (!(languageDatum == null))
				{
					Data.OverwriteData(languageDatum);
				}
			}
			currentLanguage = ((string.IsNullOrEmpty(language) || language == "Auto") ? Application.systemLanguage.ToString() : language);
			RefreshCurrentLanguage();
		}

		protected void RefreshCurrentLanguage()
		{
			if (!(Instance != this))
			{
				if (OnChangeLanugage != null)
				{
					OnChangeLanugage();
				}
				OnRefreshCurrentLanguage();
			}
		}

		protected abstract void OnRefreshCurrentLanguage();

		public string LocalizeText(string dataName, string key)
		{
			string text;
			if (Data.ContainsKey(key) && Data.TryLocalizeText(out text, CurrentLanguage, DefaultLanguage, key, dataName))
			{
				return text;
			}
			Debug.LogError(key + " is not found in " + dataName);
			return key;
		}

		public string LocalizeText(string key)
		{
			string text = key;
			TryLocalizeText(key, out text);
			return text;
		}

		public bool TryLocalizeText(string key, out string text)
		{
			text = key;
			if (Data.ContainsKey(key) && Data.TryLocalizeText(out text, CurrentLanguage, DefaultLanguage, key))
			{
				return true;
			}
			return false;
		}

		internal void OverwriteData(StringGrid grid)
		{
			Data.OverwriteData(grid);
			RefreshCurrentLanguage();
		}
	}
}
