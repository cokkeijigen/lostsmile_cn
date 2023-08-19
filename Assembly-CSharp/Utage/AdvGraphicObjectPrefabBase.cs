using System;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public abstract class AdvGraphicObjectPrefabBase : AdvGraphicBase
	{
		private enum SaveType
		{
			Animator,
			Other
		}

		protected GameObject currentObject;

		private Animator animator;

		private const int Version = 1;

		private string AnimationStateName { get; set; }

		public override void Init(AdvGraphicObject parentObject)
		{
			AnimationStateName = "";
			base.Init(parentObject);
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo grapic)
		{
			return base.LastResource != grapic;
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo grapic, float fadeTime)
		{
			if (base.LastResource != grapic)
			{
				currentObject = UnityEngine.Object.Instantiate(grapic.File.UnityObject) as GameObject;
				Vector3 localPosition = currentObject.transform.localPosition;
				Vector3 localEulerAngles = currentObject.transform.localEulerAngles;
				Vector3 localScale = currentObject.transform.localScale;
				currentObject.transform.SetParent(base.transform);
				currentObject.transform.localPosition = localPosition;
				currentObject.transform.localScale = localScale;
				currentObject.transform.localEulerAngles = localEulerAngles;
				currentObject.ChangeLayerDeep(base.gameObject.layer);
				currentObject.gameObject.SetActive(true);
				animator = GetComponentInChildren<Animator>();
				ChangeResourceOnDrawSub(grapic);
			}
			if (base.LastResource == null)
			{
				base.ParentObject.FadeIn(fadeTime, delegate
				{
				});
			}
		}

		protected abstract void ChangeResourceOnDrawSub(AdvGraphicInfo grapic);

		internal override void Scale(AdvGraphicInfo graphic)
		{
			base.transform.localScale = graphic.Scale * base.Layer.Manager.PixelsToUnits;
		}

		internal override void Alignment(Alignment alignment, AdvGraphicInfo graphic)
		{
			base.transform.localPosition = graphic.Position;
		}

		internal override void Flip(bool flipX, bool flipY)
		{
		}

		internal override void SetCommandArg(AdvCommand command)
		{
			string text = command.ParseCellOptional(AdvColumnName.Arg2, "");
			float fadeTime = command.ParseCellOptional(AdvColumnName.Arg6, 0.2f);
			ChangeAnimationState(text, fadeTime);
		}

		private void ChangeAnimationState(string name, float fadeTime)
		{
			AnimationStateName = name;
			if (string.IsNullOrEmpty(AnimationStateName))
			{
				return;
			}
			if ((bool)animator)
			{
				animator.CrossFadeInFixedTime(AnimationStateName, fadeTime);
				return;
			}
			Animation componentInChildren = GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				componentInChildren.CrossFade(AnimationStateName, fadeTime);
			}
		}

		public override void RuleFadeIn(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			Debug.LogError(base.gameObject.name + " is not support RuleFadeIn", base.gameObject);
			if (onComplete != null)
			{
				onComplete();
			}
		}

		public override void RuleFadeOut(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			Debug.LogError(base.gameObject.name + " is not support RuleFadeOut", base.gameObject);
			if (onComplete != null)
			{
				onComplete();
			}
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(1);
			if (animator != null)
			{
				writer.Write(SaveType.Animator.ToString());
				int layerCount = animator.layerCount;
				writer.Write(layerCount);
				for (int i = 0; i < layerCount; i++)
				{
					AnimatorStateInfo animatorStateInfo = (animator.IsInTransition(i) ? animator.GetNextAnimatorStateInfo(i) : animator.GetCurrentAnimatorStateInfo(i));
					writer.Write(animatorStateInfo.fullPathHash);
					writer.Write(animatorStateInfo.normalizedTime);
				}
			}
			else
			{
				writer.Write(SaveType.Other.ToString());
				writer.Write(AnimationStateName);
			}
		}

		public override void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 1)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			string value = reader.ReadString();
			switch ((SaveType)Enum.Parse(typeof(SaveType), value))
			{
			case SaveType.Animator:
			{
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					int stateNameHash = reader.ReadInt32();
					int layer = i;
					float normalizedTime = reader.ReadSingle();
					animator.Play(stateNameHash, layer, normalizedTime);
				}
				break;
			}
			default:
			{
				string text = reader.ReadString();
				ChangeAnimationState(text, 0f);
				break;
			}
			}
		}
	}
}
