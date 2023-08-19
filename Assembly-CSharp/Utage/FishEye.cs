using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Utage/Lib/Image Effects/Displacement/FishEye")]
	public class FishEye : ImageEffectSingelShaderBase
	{
		[Range(0f, 1.5f)]
		public float strengthX = 0.5f;

		[Range(0f, 1.5f)]
		public float strengthY = 0.5f;

		protected override void RenderImage(RenderTexture source, RenderTexture destination)
		{
			float num = 5f / 32f;
			float num2 = (float)source.width * 1f / ((float)source.height * 1f);
			base.Material.SetVector("intensity", new Vector4(strengthX * num2 * num, strengthY * num, strengthX * num2 * num, strengthY * num));
			Graphics.Blit(source, destination, base.Material);
		}
	}
}
