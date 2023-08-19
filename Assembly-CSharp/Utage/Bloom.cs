using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Utage/Lib/Image Effects/Bloom and Glow/Bloom")]
	public class Bloom : ImageEffectSingelShaderBase
	{
		public enum Resolution
		{
			Low,
			High
		}

		public enum BlurType
		{
			Standard,
			Sgx
		}

		[Range(0f, 1.5f)]
		public float threshold = 0.25f;

		[Range(0f, 2.5f)]
		public float intensity = 0.75f;

		[Range(0.25f, 5.5f)]
		public float blurSize = 1f;

		private Resolution resolution;

		[Range(1f, 4f)]
		public int blurIterations = 1;

		public BlurType blurType;

		protected override bool NeedRenderTexture
		{
			get
			{
				return true;
			}
		}

		protected override void RenderImage(RenderTexture source, RenderTexture destination)
		{
			int num = ((resolution == Resolution.Low) ? 4 : 2);
			float num2 = ((resolution == Resolution.Low) ? 0.5f : 1f);
			base.Material.SetVector("_Parameter", new Vector4(blurSize * num2, 0f, threshold, intensity));
			source.filterMode = FilterMode.Bilinear;
			int width = source.width / num;
			int height = source.height / num;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, base.Material, 1);
			int num3 = ((blurType != 0) ? 2 : 0);
			for (int i = 0; i < blurIterations; i++)
			{
				base.Material.SetVector("_Parameter", new Vector4(blurSize * num2 + (float)i * 1f, 0f, threshold, intensity));
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, base.Material, 2 + num3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, base.Material, 3 + num3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			base.Material.SetTexture("_Bloom", renderTexture);
			Graphics.Blit(source, destination, base.Material, 0);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}
}
