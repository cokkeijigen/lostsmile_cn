using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utage
{
	[RequireComponent(typeof(RawImage))]
	[AddComponentMenu("Utage/Lib/UI/FadeTextureStream")]
	public class UguiFadeTextureStream : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[Serializable]
		public class FadeTextureInfo
		{
			public Texture texture;

			public string moviePath;

			public float fadeInTime = 0.5f;

			public float duration = 3f;

			public float fadeOutTime = 0.5f;

			public bool allowSkip;
		}

		public bool allowSkip = true;

		public bool allowAllSkip;

		public FadeTextureInfo[] fadeTextures = new FadeTextureInfo[1];

		private bool isInput;

		private bool isPlaying;

		private bool IsInputAllSkip
		{
			get
			{
				if (isInput)
				{
					return allowAllSkip;
				}
				return false;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return isPlaying;
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			isInput = true;
		}

		private bool IsInputSkip(FadeTextureInfo info)
		{
			if (isInput)
			{
				if (!allowSkip)
				{
					return info.allowSkip;
				}
				return true;
			}
			return false;
		}

		private void LateUpdate()
		{
			isInput = false;
		}

		public void Play()
		{
			StartCoroutine(CoPlay());
		}

		private IEnumerator CoPlay()
		{
			isPlaying = true;
			RawImage rawImage = GetComponent<RawImage>();
			rawImage.CrossFadeAlpha(0f, 0f, true);
			FadeTextureInfo[] array = fadeTextures;
			foreach (FadeTextureInfo info in array)
			{
				rawImage.texture = info.texture;
				bool allSkip = false;
				if ((bool)info.texture)
				{
					rawImage.CrossFadeAlpha(1f, info.fadeInTime, true);
					float time2 = 0f;
					while (!IsInputSkip(info))
					{
						yield return null;
						time2 += Time.deltaTime;
						if (time2 > info.fadeInTime)
						{
							break;
						}
					}
					time2 = 0f;
					while (!IsInputSkip(info))
					{
						yield return null;
						time2 += Time.deltaTime;
						if (time2 > info.duration)
						{
							break;
						}
					}
					allSkip = IsInputAllSkip;
					rawImage.CrossFadeAlpha(0f, info.fadeOutTime, true);
					yield return new WaitForSeconds(info.fadeOutTime);
				}
				else if (!string.IsNullOrEmpty(info.moviePath))
				{
					WrapperMoviePlayer.Play(info.moviePath);
					while (WrapperMoviePlayer.IsPlaying())
					{
						yield return null;
						if (IsInputSkip(info))
						{
							WrapperMoviePlayer.Cancel();
						}
						allSkip = IsInputAllSkip;
					}
				}
				if (allSkip)
				{
					break;
				}
				yield return null;
			}
			isPlaying = false;
		}
	}
}
