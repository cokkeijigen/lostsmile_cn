using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/Image Effects/Color Adjustments/Grayscale")]
	public class Grayscale : ImageEffectSingelShaderBase, IImageEffectStrength
	{
		[Range(0f, 1f)]
		public float strength = 1f;

		public Texture textureRamp;

		[Range(-1f, 1f)]
		public float rampOffset;

		private Texture tmpTextureRamp;

		public float Strength
		{
			get
			{
				return strength;
			}
			set
			{
				strength = value;
			}
		}

		protected override void RenderImage(RenderTexture source, RenderTexture destination)
		{
			base.Material.SetFloat("_Strength", strength);
			base.Material.SetTexture("_RampTex", textureRamp);
			base.Material.SetFloat("_RampOffset", rampOffset);
			Graphics.Blit(source, destination, base.Material);
		}

		protected override void StoreObjectsOnJsonRead()
		{
			base.StoreObjectsOnJsonRead();
			tmpTextureRamp = textureRamp;
		}

		protected override void RestoreObjectsOnJsonRead()
		{
			base.RestoreObjectsOnJsonRead();
			textureRamp = tmpTextureRamp;
		}
	}
}
