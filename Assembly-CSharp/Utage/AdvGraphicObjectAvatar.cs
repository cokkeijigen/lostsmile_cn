using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/Avatar")]
	public class AdvGraphicObjectAvatar : AdvGraphicObjectUguiBase
	{
		protected Timer FadeTimer { get; set; }

		protected AvatarImage Avatar { get; set; }

		protected EyeBlinkAvatar EyeBlink { get; set; }

		protected LipSynchAvatar LipSynch { get; set; }

		protected AdvAnimationPlayer Animation { get; set; }

		protected CanvasGroup Group { get; set; }

		protected override Material Material
		{
			get
			{
				return Avatar.Material;
			}
			set
			{
				Avatar.Material = value;
			}
		}

		protected override void AddGraphicComponentOnInit()
		{
			Avatar = base.gameObject.AddComponent<AvatarImage>();
			Avatar.OnPostRefresh.AddListener(OnPostRefresh);
			EyeBlink = base.gameObject.AddComponent<EyeBlinkAvatar>();
			LipSynch = base.gameObject.AddComponent<LipSynchAvatar>();
			Animation = base.gameObject.AddComponent<AdvAnimationPlayer>();
			Group = base.gameObject.AddComponent<CanvasGroup>();
			FadeTimer = base.gameObject.AddComponent<Timer>();
			FadeTimer.AutoDestroy = false;
		}

		internal override void OnEffectColorsChange(AdvEffectColor color)
		{
			Graphic[] componentsInChildren = GetComponentsInChildren<Graphic>();
			foreach (Graphic graphic in componentsInChildren)
			{
				if (graphic != null)
				{
					graphic.color = color.MulColor;
				}
			}
		}

		private void OnPostRefresh()
		{
			if (!base.LastResource.RenderTextureSetting.EnableRenderTexture)
			{
				OnEffectColorsChange(base.ParentObject.EffectColor);
			}
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo graphic)
		{
			AvatarData avatarData = graphic.File.UnityObject as AvatarData;
			return Avatar.AvatarData != avatarData;
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo graphic, float fadeTime)
		{
			Avatar.Material = graphic.RenderTextureSetting.GetRenderMaterialIfEnable(Avatar.Material);
			AvatarData avatarData = graphic.File.UnityObject as AvatarData;
			Avatar.AvatarData = avatarData;
			Avatar.CachedRectTransform.sizeDelta = avatarData.Size;
			Avatar.AvatarPattern.SetPattern(graphic.RowData);
			SetEyeBlinkSync(graphic.EyeBlinkData);
			SetLipSynch(graphic.LipSynchData);
			SetAnimation(graphic.AnimationData);
			if (base.LastResource == null)
			{
				base.ParentObject.FadeIn(fadeTime, delegate
				{
				});
			}
		}

		internal override void Flip(bool flipX, bool flipY)
		{
			Avatar.Flip(flipX, flipY);
		}

		protected void SetAnimation(AdvAnimationData data)
		{
			Animation.Cancel();
			if (data != null)
			{
				Animation.Play(data.Clip, base.Engine.Page.SkippedSpeed);
			}
		}

		protected void SetEyeBlinkSync(AdvEyeBlinkData data)
		{
			if (data == null)
			{
				EyeBlink.AnimationData = new MiniAnimationData();
				return;
			}
			EyeBlink.IntervalTime.Min = data.IntervalMin;
			EyeBlink.IntervalTime.Max = data.IntervalMax;
			EyeBlink.RandomDoubleEyeBlink = data.RandomDoubleEyeBlink;
			EyeBlink.EyeTag = data.Tag;
			EyeBlink.AnimationData = data.AnimationData;
		}

		protected void SetLipSynch(AdvLipSynchData data)
		{
			LipSynch.Cancel();
			if (data == null)
			{
				LipSynch.AnimationData = new MiniAnimationData();
				return;
			}
			LipSynch.Type = data.Type;
			LipSynch.Interval = data.Interval;
			LipSynch.ScaleVoiceVolume = data.ScaleVoiceVolume;
			LipSynch.LipTag = data.Tag;
			LipSynch.AnimationData = data.AnimationData;
			LipSynch.Play();
		}
	}
}
