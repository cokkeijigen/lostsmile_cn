using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/Image Effects/Displacement/Twirl")]
	public class Twirl : ImageEffectSingelShaderBase
	{
		public Vector2 radius = new Vector2(0.3f, 0.3f);

		[Range(0f, 360f)]
		public float angle = 50f;

		public Vector2 center = new Vector2(0.5f, 0.5f);

		protected override void RenderImage(RenderTexture source, RenderTexture destination)
		{
			ImageEffectUtil.RenderDistortion(base.Material, source, destination, angle, center, radius);
		}
	}
}
