using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/Custom")]
	public class AdvGraphicObjectCustom : AdvGraphicObjectPrefabBase
	{
		protected SpriteRenderer sprite;

		protected override void ChangeResourceOnDrawSub(AdvGraphicInfo graphic)
		{
			IAdvGraphicObjectCustom[] componentsInChildren = GetComponentsInChildren<IAdvGraphicObjectCustom>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ChangeResourceOnDrawSub(graphic);
			}
		}

		internal override void OnEffectColorsChange(AdvEffectColor color)
		{
			IAdvGraphicObjectCustom[] componentsInChildren = GetComponentsInChildren<IAdvGraphicObjectCustom>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnEffectColorsChange(color);
			}
		}

		internal override void SetCommandArg(AdvCommand command)
		{
			base.SetCommandArg(command);
			IAdvGraphicObjectCustomCommand[] componentsInChildren = GetComponentsInChildren<IAdvGraphicObjectCustomCommand>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetCommandArg(command);
			}
		}
	}
}
