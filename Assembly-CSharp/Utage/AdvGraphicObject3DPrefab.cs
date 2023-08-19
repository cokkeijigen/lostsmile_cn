using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/3DModel")]
	public class AdvGraphicObject3DPrefab : AdvGraphicObjectPrefabBase
	{
		public override void Init(AdvGraphicObject parentObject)
		{
			base.Init(parentObject);
			base.transform.localEulerAngles = Vector3.up * 180f;
		}

		protected override void ChangeResourceOnDrawSub(AdvGraphicInfo grapic)
		{
		}

		internal override void OnEffectColorsChange(AdvEffectColor color)
		{
			if ((bool)currentObject)
			{
				Color mulColor = color.MulColor;
				mulColor.a = 1f;
				Renderer[] componentsInChildren = currentObject.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].material.color = mulColor;
				}
			}
		}
	}
}
