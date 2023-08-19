using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/Image Effects/Color Adjustments/ColorFade")]
	public class ColorFade : ImageEffectSingelShaderBase, IImageEffectStrength
	{
		[Range(0f, 1f)]
		public float strength = 1f;

		public Color color = new Color(0f, 0f, 0f, 0f);

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
			base.Material.SetFloat("_Strength", Strength);
			base.Material.color = color;
			Graphics.Blit(source, destination, base.Material);
		}
	}
}
