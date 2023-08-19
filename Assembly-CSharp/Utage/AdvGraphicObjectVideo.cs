using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/Video")]
	public class AdvGraphicObjectVideo : AdvGraphicObjectUguiBase
	{
		private AssetFileReference crossFadeReference;

		protected override Material Material
		{
			get
			{
				return RawImage.material;
			}
			set
			{
				RawImage.material = value;
			}
		}

		private RawImage RawImage { get; set; }

		private VideoClip VideoClip { get; set; }

		private VideoPlayer VideoPlayer { get; set; }

		protected Timer FadeTimer { get; set; }

		private RenderTexture RenderTexture { get; set; }

		private int Width { get; set; }

		private int Height { get; set; }

		private void ReleaseCrossFadeReference()
		{
			if (crossFadeReference != null)
			{
				Object.Destroy(crossFadeReference);
				crossFadeReference = null;
			}
		}

		protected override void AddGraphicComponentOnInit()
		{
			RawImage = this.GetComponentCreateIfMissing<RawImage>();
			FadeTimer = base.gameObject.AddComponent<Timer>();
			FadeTimer.AutoDestroy = false;
			VideoPlayer = base.gameObject.AddComponent<VideoPlayer>();
		}

		private void OnDisable()
		{
			VideoPlayer.Stop();
		}

		private void OnDestroy()
		{
			ReleaseTexture();
		}

		private void ReleaseTexture()
		{
			if ((bool)RenderTexture)
			{
				if (VideoPlayer.isPlaying)
				{
					VideoPlayer.Stop();
				}
				if (RenderTexture.active == RenderTexture)
				{
					RenderTexture.active = null;
				}
				RenderTexture.Release();
				Object.Destroy(RenderTexture);
			}
		}

		internal override void OnEffectColorsChange(AdvEffectColor color)
		{
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo graphic)
		{
			return true;
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo graphic, float fadeTime)
		{
			VideoClip = graphic.File.UnityObject as VideoClip;
			VideoPlayer.clip = VideoClip;
			VideoPlayer.isLooping = true;
			float volume = base.Engine.SoundManager.BgmVolume * base.Engine.SoundManager.MasterVolume;
			VideoPlayer.SetDirectAudioVolume(0, volume);
			VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
			ReleaseTexture();
			RenderTexture = new RenderTexture((int)VideoClip.width, (int)VideoClip.height, 16, RenderTextureFormat.ARGB32);
			VideoPlayer.targetTexture = RenderTexture;
			VideoPlayer.Play();
			RawImage.texture = RenderTexture;
			RawImage.SetNativeSize();
		}

		private void Update()
		{
			VideoPlayer videoPlayer = VideoPlayer;
			if (!(videoPlayer == null) && videoPlayer.isPlaying)
			{
				float volume = base.Engine.SoundManager.BgmVolume * base.Engine.SoundManager.MasterVolume;
				videoPlayer.SetDirectAudioVolume(0, volume);
			}
		}
	}
}
