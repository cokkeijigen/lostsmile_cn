using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/Image Effects/Blur/Motion Blur")]
	[RequireComponent(typeof(Camera))]
	public class MotionBlur : ImageEffectSingelShaderBase
	{
		[Range(0f, 0.92f)]
		public float blurAmount = 0.8f;

		public bool extraBlur;

		private RenderTexture accumTexture;

		protected override bool NeedRenderTexture => true;

		private void OnDisable()
		{
			Object.DestroyImmediate(accumTexture);
		}

		protected override void RenderImage(RenderTexture source, RenderTexture destination)
		{
			if (accumTexture == null || accumTexture.width != source.width || accumTexture.height != source.height)
			{
				Object.DestroyImmediate(accumTexture);
				accumTexture = new RenderTexture(source.width, source.height, 0);
				accumTexture.hideFlags = HideFlags.HideAndDontSave;
				Graphics.Blit(source, accumTexture);
			}
			if (extraBlur)
			{
				RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
				accumTexture.MarkRestoreExpected();
				Graphics.Blit(accumTexture, temporary);
				Graphics.Blit(temporary, accumTexture);
				RenderTexture.ReleaseTemporary(temporary);
			}
			blurAmount = Mathf.Clamp(blurAmount, 0f, 0.92f);
			base.Material.SetTexture("_MainTex", accumTexture);
			base.Material.SetFloat("_AccumOrig", 1f - blurAmount);
			accumTexture.MarkRestoreExpected();
			Graphics.Blit(source, accumTexture, base.Material);
			Graphics.Blit(accumTexture, destination);
		}
	}
}
