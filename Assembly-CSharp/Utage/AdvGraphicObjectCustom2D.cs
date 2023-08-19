using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/Custom2D")]
	public class AdvGraphicObjectCustom2D : AdvGraphicObjectUguiBase
	{
		protected override Material Material { get; set; }

		protected override void AddGraphicComponentOnInit()
		{
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo grapic)
		{
			return base.LastResource != grapic;
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo grapic, float fadeTime)
		{
			if (base.LastResource != grapic)
			{
				GameObject obj = Object.Instantiate(grapic.File.UnityObject) as GameObject;
				Vector3 localPosition = obj.transform.localPosition;
				Vector3 localEulerAngles = obj.transform.localEulerAngles;
				Vector3 localScale = obj.transform.localScale;
				obj.transform.SetParent(base.transform);
				obj.transform.localPosition = localPosition;
				obj.transform.localScale = localScale;
				obj.transform.localEulerAngles = localEulerAngles;
				obj.ChangeLayerDeep(base.gameObject.layer);
				obj.gameObject.SetActive(true);
				ChangeResourceOnDrawSub(grapic);
			}
			if (base.LastResource == null)
			{
				base.ParentObject.FadeIn(fadeTime, delegate
				{
				});
			}
		}

		internal virtual void ChangeResourceOnDrawSub(AdvGraphicInfo graphic)
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
