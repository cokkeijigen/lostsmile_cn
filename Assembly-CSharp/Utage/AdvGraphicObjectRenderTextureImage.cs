using System;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/RenderTextureImage")]
	public class AdvGraphicObjectRenderTextureImage : AdvGraphicObjectUguiBase
	{
		private RenderTexture copyTemporary;

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

		public AdvRenderTextureSpace RenderTextureSpace { get; private set; }

		private RawImage RawImage { get; set; }

		private void ReleaseTemporary()
		{
			if (copyTemporary != null)
			{
				RenderTexture.ReleaseTemporary(copyTemporary);
				copyTemporary = null;
			}
		}

		private void OnDestroy()
		{
			ReleaseTemporary();
		}

		protected override void AddGraphicComponentOnInit()
		{
		}

		internal void Init(AdvRenderTextureSpace renderTextureSpace)
		{
			RenderTextureSpace = renderTextureSpace;
			RawImage = base.gameObject.GetComponentCreateIfMissing<RawImage>();
			if (renderTextureSpace.RenderTextureType == AdvRenderTextureMode.Image)
			{
				Material = new Material(ShaderManager.DrawByRenderTexture);
			}
			RawImage.texture = RenderTextureSpace.RenderTexture;
			RawImage.SetNativeSize();
			RawImage.rectTransform.localScale = Vector3.one;
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo graphic)
		{
			return false;
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo graphic, float fadeTime)
		{
			bool num = TryCreateCrossFadeImage(fadeTime, graphic);
			if (!num)
			{
				base.gameObject.RemoveComponent<UguiCrossFadeRawImage>();
				ReleaseTemporary();
			}
			RawImage.texture = RenderTextureSpace.RenderTexture;
			RawImage.SetNativeSize();
			if (!num && base.LastResource == null)
			{
				base.ParentObject.FadeIn(fadeTime, delegate
				{
				});
			}
		}

		public override void RuleFadeIn(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			UguiTransition transition = base.gameObject.AddComponent<UguiTransition>();
			transition.RuleFadeIn(engine.EffectManager.FindRuleTexture(data.TextureName), data.Vague, RenderTextureSpace.RenderTextureType == AdvRenderTextureMode.Image, data.GetSkippedTime(engine), delegate
			{
				transition.RemoveComponentMySelf(false);
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		public override void RuleFadeOut(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			UguiTransition transition = base.gameObject.AddComponent<UguiTransition>();
			transition.RuleFadeOut(engine.EffectManager.FindRuleTexture(data.TextureName), data.Vague, RenderTextureSpace.RenderTextureType == AdvRenderTextureMode.Image, data.GetSkippedTime(engine), delegate
			{
				transition.RemoveComponentMySelf(false);
				RawImage.SetAlpha(0f);
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		protected bool TryCreateCrossFadeImage(float time, AdvGraphicInfo graphic)
		{
			if (base.LastResource == null)
			{
				return false;
			}
			if (RawImage.texture == null)
			{
				return false;
			}
			ReleaseTemporary();
			Material material = Material;
			copyTemporary = RenderTextureSpace.RenderTexture.CreateCopyTemporary(0);
			UguiCrossFadeRawImage crossFade = base.gameObject.AddComponent<UguiCrossFadeRawImage>();
			crossFade.Material = material;
			crossFade.CrossFade(copyTemporary, time, delegate
			{
				ReleaseTemporary();
				crossFade.RemoveComponentMySelf(false);
			});
			return true;
		}
	}
}
