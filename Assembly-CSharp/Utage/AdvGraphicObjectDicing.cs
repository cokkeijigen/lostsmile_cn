using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/Dicing")]
	public class AdvGraphicObjectDicing : AdvGraphicObjectUguiBase
	{
		private AssetFileReference crossFadeReference;

		protected DicingImage Dicing { get; set; }

		protected EyeBlinkDicing EyeBlink { get; set; }

		protected LipSynchDicing LipSynch { get; set; }

		protected AdvAnimationPlayer Animation { get; set; }

		protected override Material Material
		{
			get
			{
				return Dicing.material;
			}
			set
			{
				Dicing.material = value;
			}
		}

		private void ReleaseCrossFadeReference()
		{
			if (crossFadeReference != null)
			{
				crossFadeReference.RemoveComponentMySelf();
				crossFadeReference = null;
			}
		}

		protected override void AddGraphicComponentOnInit()
		{
			Dicing = base.gameObject.AddComponent<DicingImage>();
			EyeBlink = base.gameObject.AddComponent<EyeBlinkDicing>();
			LipSynch = base.gameObject.AddComponent<LipSynchDicing>();
			Animation = base.gameObject.AddComponent<AdvAnimationPlayer>();
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo graphic)
		{
			return !EnableCrossFade(graphic);
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo graphic, float fadeTime)
		{
			Dicing.material = graphic.RenderTextureSetting.GetRenderMaterialIfEnable(Dicing.material);
			bool num = TryCreateCrossFadeImage(fadeTime, graphic);
			if (!num)
			{
				ReleaseCrossFadeReference();
				base.gameObject.RemoveComponent<UguiCrossFadeDicing>();
			}
			DicingTextures dicingData = graphic.File.UnityObject as DicingTextures;
			string subFileName = graphic.SubFileName;
			Dicing.DicingData = dicingData;
			Dicing.ChangePattern(subFileName);
			Dicing.SetNativeSize();
			SetEyeBlinkSync(graphic.EyeBlinkData);
			SetLipSynch(graphic.LipSynchData);
			SetAnimation(graphic.AnimationData);
			if (!num)
			{
				base.ParentObject.FadeIn(fadeTime, delegate
				{
				});
			}
		}

		public override void ChangePattern(string pattern)
		{
			Dicing.ChangePattern(pattern);
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

		protected bool TryCreateCrossFadeImage(float time, AdvGraphicInfo graphic)
		{
			if (base.LastResource == null)
			{
				return false;
			}
			if (EnableCrossFade(graphic))
			{
				ReleaseCrossFadeReference();
				crossFadeReference = base.gameObject.AddComponent<AssetFileReference>();
				crossFadeReference.Init(base.LastResource.File);
				UguiCrossFadeDicing crossFade = base.gameObject.GetComponentCreateIfMissing<UguiCrossFadeDicing>();
				crossFade.CrossFade(Dicing.PatternData, Dicing.mainTexture, time, delegate
				{
					crossFade.RemoveComponentMySelf();
				});
				return true;
			}
			return false;
		}

		private bool EnableCrossFade(AdvGraphicInfo graphic)
		{
			DicingTextures dicingTextures = graphic.File.UnityObject as DicingTextures;
			string subFileName = graphic.SubFileName;
			DicingTextureData textureData = Dicing.DicingData.GetTextureData(subFileName);
			if (textureData == null)
			{
				return false;
			}
			if (Dicing.DicingData == dicingTextures && Dicing.rectTransform.pivot == graphic.Pivot && Dicing.PatternData.Width == textureData.Width)
			{
				return Dicing.PatternData.Height == textureData.Height;
			}
			return false;
		}
	}
}
