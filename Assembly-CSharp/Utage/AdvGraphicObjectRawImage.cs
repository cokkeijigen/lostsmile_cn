using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/RawImage")]
	public class AdvGraphicObjectRawImage : AdvGraphicObjectUguiBase
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

		private void ReleaseCrossFadeReference()
		{
			if (crossFadeReference != null)
			{
				Object.DestroyImmediate(crossFadeReference);
				crossFadeReference = null;
			}
		}

		protected override void AddGraphicComponentOnInit()
		{
			RawImage = this.GetComponentCreateIfMissing<RawImage>();
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo graphic)
		{
			return !EnableCrossFade(graphic);
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo graphic, float fadeTime)
		{
			Material = graphic.RenderTextureSetting.GetRenderMaterialIfEnable(Material);
			bool num = TryCreateCrossFadeImage(fadeTime, graphic);
			if (!num)
			{
				ReleaseCrossFadeReference();
				base.gameObject.RemoveComponent<UguiCrossFadeRawImage>();
			}
			RawImage.texture = graphic.File.Texture;
			RawImage.SetNativeSize();
			if (!num)
			{
				base.ParentObject.FadeIn(fadeTime, delegate
				{
				});
			}
		}

		protected bool TryCreateCrossFadeImage(float fadeTime, AdvGraphicInfo graphic)
		{
			if (base.LastResource == null)
			{
				return false;
			}
			if (EnableCrossFade(graphic))
			{
				StartCrossFadeImage(fadeTime);
				return true;
			}
			return false;
		}

		protected bool EnableCrossFade(AdvGraphicInfo graphic)
		{
			Texture texture = graphic.File.Texture;
			if (texture == null)
			{
				return false;
			}
			if (RawImage.texture == null)
			{
				return false;
			}
			if (RawImage.rectTransform.pivot == graphic.Pivot && RawImage.texture.width == texture.width)
			{
				return RawImage.texture.height == texture.height;
			}
			return false;
		}

		internal void StartCrossFadeImage(float time)
		{
			Texture texture = base.LastResource.File.Texture;
			ReleaseCrossFadeReference();
			crossFadeReference = base.gameObject.AddComponent<AssetFileReference>();
			crossFadeReference.Init(base.LastResource.File);
			UguiCrossFadeRawImage crossFade = base.gameObject.GetComponentCreateIfMissing<UguiCrossFadeRawImage>();
			crossFade.CrossFade(texture, time, delegate
			{
				ReleaseCrossFadeReference();
				crossFade.RemoveComponentMySelf();
			});
		}

		internal void CaptureCamera(Camera camera)
		{
			RawImage.enabled = false;
			CaptureCamera componentCreateIfMissing = camera.GetComponentCreateIfMissing<CaptureCamera>();
			componentCreateIfMissing.enabled = true;
			componentCreateIfMissing.OnCaptured.AddListener(OnCaptured);
		}

		private void OnCaptured(CaptureCamera captureCamera)
		{
			RawImage.enabled = true;
			RawImage.texture = captureCamera.CaptureImage;
			LetterBoxCamera component = captureCamera.GetComponent<LetterBoxCamera>();
			if (component != null)
			{
				RawImage.rectTransform.SetSize(component.CurrentSize);
				if (component.Zoom2D != 1f)
				{
					int num = 1 << base.gameObject.layer;
					if ((component.CachedCamera.cullingMask & num) != 0)
					{
						Vector2 zoom2DCenter = component.Zoom2DCenter;
						zoom2DCenter.x /= component.CurrentSize.x;
						zoom2DCenter.y /= component.CurrentSize.y;
						zoom2DCenter = -zoom2DCenter + Vector2.one * 0.5f;
						RawImage.rectTransform.pivot = zoom2DCenter;
						RawImage.rectTransform.localScale = Vector3.one / component.Zoom2D;
					}
				}
			}
			else
			{
				RawImage.rectTransform.SetSize(Screen.width, Screen.height);
			}
			captureCamera.OnCaptured.RemoveListener(OnCaptured);
			captureCamera.enabled = false;
		}
	}
}
