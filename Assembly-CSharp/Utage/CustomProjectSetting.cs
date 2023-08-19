using UnityEngine;

namespace Utage
{
	public class CustomProjectSetting : ScriptableObject
	{
		private static CustomProjectSetting instance;

		[SerializeField]
		private LanguageManager language;

		public static CustomProjectSetting Instance
		{
			get
			{
				if (instance == null)
				{
					BootCustomProjectSetting bootCustomProjectSetting = Object.FindObjectOfType<BootCustomProjectSetting>();
					if (bootCustomProjectSetting != null)
					{
						instance = bootCustomProjectSetting.CustomProjectSetting;
						if (instance == null)
						{
							Debug.LogError("CustomProjectSetting is NONE", bootCustomProjectSetting);
						}
					}
				}
				return instance;
			}
		}

		public LanguageManager Language
		{
			get
			{
				return language;
			}
			set
			{
				language = value;
			}
		}
	}
}
