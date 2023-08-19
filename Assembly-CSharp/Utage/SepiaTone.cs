using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/Image Effects/Color Adjustments/Sepia Tone")]
	public class SepiaTone : ImageEffectSingelShaderBase, IImageEffectStrength
	{
		[Range(0f, 1f)]
		public float strength = 1f;

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
			Graphics.Blit(source, destination, base.Material);
		}
	}
}
