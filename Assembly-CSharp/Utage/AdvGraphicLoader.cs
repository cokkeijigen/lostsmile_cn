using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/AdvGraphicLoader")]
	public class AdvGraphicLoader : MonoBehaviour
	{
		public UnityEvent OnComplete = new UnityEvent();

		private AdvGraphicInfo graphic;

		public bool IsLoading
		{
			get
			{
				if (graphic == null)
				{
					return false;
				}
				return !graphic.File.IsLoadEnd;
			}
		}

		public void LoadGraphic(AdvGraphicInfo graphic, Action onComplete)
		{
			Unload();
			this.graphic = graphic;
			AssetFileManager.Load(graphic.File, this);
			StartCoroutine(CoLoadWait(onComplete));
		}

		private IEnumerator CoLoadWait(Action onComplete)
		{
			while (IsLoading)
			{
				yield return null;
			}
			OnComplete.Invoke();
			onComplete?.Invoke();
		}

		public void Unload()
		{
			if (graphic != null)
			{
				graphic.File.Unuse(this);
				graphic = null;
			}
		}

		private void OnDestroy()
		{
			Unload();
		}
	}
}
