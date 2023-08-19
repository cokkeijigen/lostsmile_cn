using System;
using UnityEngine;

namespace Utage
{
	public abstract class UguiLocalizeBase : MonoBehaviour
	{
		[NonSerialized]
		protected string currentLanguage;

		[NonSerialized]
		protected bool isInit;

		public virtual void OnLocalize()
		{
			ForceRefresh();
		}

		protected virtual void OnEnable()
		{
			if (!isInit)
			{
				isInit = true;
				InitDefault();
			}
			ForceRefresh();
		}

		protected virtual void OnValidate()
		{
			ForceRefresh();
		}

		protected virtual void ForceRefresh()
		{
			currentLanguage = "";
			Refresh();
		}

		protected virtual bool IsEnable()
		{
			if (!Application.isPlaying)
			{
				return false;
			}
			if (!base.gameObject.activeInHierarchy)
			{
				return false;
			}
			return true;
		}

		protected virtual void Refresh()
		{
			LanguageManagerBase instance = LanguageManagerBase.Instance;
			if (!(instance == null) && !(currentLanguage == instance.CurrentLanguage) && IsEnable())
			{
				currentLanguage = instance.CurrentLanguage;
				RefreshSub();
			}
		}

		protected abstract void RefreshSub();

		public virtual void EditorRefresh()
		{
			LanguageManagerBase instance = LanguageManagerBase.Instance;
			if (!(instance == null))
			{
				currentLanguage = instance.CurrentLanguage;
				if (!isInit)
				{
					isInit = true;
					InitDefault();
				}
				RefreshSub();
			}
		}

		protected abstract void InitDefault();

		public abstract void ResetDefault();
	}
}
