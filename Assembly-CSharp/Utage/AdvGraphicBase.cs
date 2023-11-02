using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	public abstract class AdvGraphicBase : MonoBehaviour
	{
		internal AdvGraphicObject ParentObject { get; set; }

		internal AdvGraphicLayer Layer => ParentObject.Layer;

		internal AdvEngine Engine => Layer.Manager.Engine;

		protected float PixelsToUnits => Layer.Manager.PixelsToUnits;

		protected AdvGraphicInfo LastResource => ParentObject.LastResource;

		public virtual void Init(AdvGraphicObject parentObject)
		{
			ParentObject = parentObject;
		}

		internal abstract bool CheckFailedCrossFade(AdvGraphicInfo graphic);

		internal abstract void ChangeResourceOnDraw(AdvGraphicInfo graphic, float fadeTime);

		internal virtual void SetCommandArg(AdvCommand command)
		{
		}

		internal abstract void Scale(AdvGraphicInfo graphic);

		internal abstract void Alignment(Alignment alignment, AdvGraphicInfo graphic);

		internal virtual void Flip(bool flipX, bool flipY)
		{
			if (flipX || flipY)
			{
				UguiFlip component = GetComponent<UguiFlip>();
				if (component != null)
				{
					component.RemoveComponentMySelf();
				}
				component = base.gameObject.AddComponent<UguiFlip>();
				component.FlipX = flipX;
				component.FlipY = flipY;
			}
		}

		internal virtual void OnEffectColorsChange(AdvEffectColor color)
		{
			Graphic component = GetComponent<Graphic>();
			if (component != null)
			{
				component.color = color.MulColor;
			}
		}

		public virtual void ChangePattern(string pattern)
		{
		}

		public virtual void RuleFadeIn(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			UguiTransition transition = base.gameObject.AddComponent<UguiTransition>();
			transition.RuleFadeIn(engine.EffectManager.FindRuleTexture(data.TextureName), data.Vague, false, data.GetSkippedTime(engine), delegate
			{
				transition.RemoveComponentMySelf(false);
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		public virtual void RuleFadeOut(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			UguiTransition transition = base.gameObject.AddComponent<UguiTransition>();
			transition.RuleFadeOut(engine.EffectManager.FindRuleTexture(data.TextureName), data.Vague, false, data.GetSkippedTime(engine), delegate
			{
				transition.RemoveComponentMySelf(false);
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		public virtual void Read(BinaryReader reader)
		{
		}

		public virtual void Write(BinaryWriter writer)
		{
		}
	}
}
