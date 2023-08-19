using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/Image Effects/Color Adjustments/Mosaic")]
	public class Mosaic : ImageEffectSingelShaderBase
	{
		[Min(1f)]
		public float size = 8f;

		private LetterBoxCamera letterBoxCamera;

		private LetterBoxCamera LetterBoxCamera
		{
			get
			{
				return this.GetComponentCache(ref letterBoxCamera);
			}
		}

		protected override void RenderImage(RenderTexture source, RenderTexture destination)
		{
			float num = 1f;
			if (LetterBoxCamera != null)
			{
				num = Mathf.Min((float)source.width / LetterBoxCamera.CurrentSize.x, (float)source.height / LetterBoxCamera.CurrentSize.y);
			}
			if (size <= 1f)
			{
				Graphics.Blit(source, destination);
				return;
			}
			base.Material.SetFloat("_Size", Mathf.CeilToInt(size * num));
			Graphics.Blit(source, destination, base.Material);
		}
	}
}
