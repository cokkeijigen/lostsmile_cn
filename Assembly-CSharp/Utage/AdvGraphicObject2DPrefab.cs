using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/2DPrefab")]
	public class AdvGraphicObject2DPrefab : AdvGraphicObjectPrefabBase
	{
		protected SpriteRenderer sprite;

		protected override void ChangeResourceOnDrawSub(AdvGraphicInfo graphic)
		{
			sprite = currentObject.GetComponent<SpriteRenderer>();
		}

		internal override void OnEffectColorsChange(AdvEffectColor color)
		{
			if (!(sprite == null))
			{
				sprite.color = color.MulColor;
			}
		}
	}
}
