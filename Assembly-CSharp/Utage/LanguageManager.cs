using UnityEngine;

namespace Utage
{
	public class LanguageManager : LanguageManagerBase
	{
		protected override void OnRefreshCurrentLanguage()
		{
			if (!base.IgnoreLocalizeUiText)
			{
				UguiLocalizeBase[] array = Object.FindObjectsOfType<UguiLocalizeBase>();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].OnLocalize();
				}
			}
		}
	}
}
