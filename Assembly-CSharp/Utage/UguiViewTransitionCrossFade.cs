using System.Collections;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/ViewTransition CrossFade")]
	[RequireComponent(typeof(UguiView))]
	public class UguiViewTransitionCrossFade : MonoBehaviour, ITransition
	{
		private UguiView uguiView;

		private bool isPlaying;

		public float time = 1f;

		public UguiView UguiView
		{
			get
			{
				return uguiView ?? (uguiView = GetComponent<UguiView>());
			}
		}

		public bool IsPlaying
		{
			get
			{
				return isPlaying;
			}
		}

		public void Open()
		{
			StopCoroutine(CoClose());
			StartCoroutine(CoOpen());
		}

		public void Close()
		{
			StopCoroutine(CoOpen());
			StartCoroutine(CoClose());
		}

		public void CancelClosing()
		{
			StopCoroutine(CoClose());
			EndClose();
			isPlaying = false;
		}

		private IEnumerator CoOpen()
		{
			isPlaying = true;
			CanvasGroup canvasGroup = UguiView.CanvasGroup;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			float currentTime = 0f;
			while (currentTime < time)
			{
				canvasGroup.alpha = currentTime / time;
				currentTime += Time.deltaTime;
				yield return null;
			}
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			isPlaying = false;
		}

		private IEnumerator CoClose()
		{
			isPlaying = true;
			CanvasGroup canvasGroup = UguiView.CanvasGroup;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			float currentTime = 0f;
			while (currentTime < time)
			{
				canvasGroup.alpha = 1f - currentTime / time;
				currentTime += Time.deltaTime;
				yield return null;
			}
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			EndClose();
		}

		private void EndClose()
		{
			UguiView.CanvasGroup.alpha = 0f;
			isPlaying = false;
		}
	}
}
