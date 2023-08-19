using UnityEngine;

namespace Utage
{
	internal class AdvCommandImageEffectBase : AdvCommandEffectBase
	{
		private string animationName;

		private float time;

		private bool inverse;

		private string imageEffectType { get; set; }

		public AdvCommandImageEffectBase(StringGridRow row, AdvSettingDataManager dataManager, bool inverse)
			: base(row)
		{
			this.inverse = inverse;
			targetType = AdvEffectManager.TargetType.Camera;
			imageEffectType = base.RowData.ParseCell<string>(AdvColumnName.Arg2.ToString());
			animationName = ParseCellOptional(AdvColumnName.Arg3, "");
			time = ParseCellOptional(AdvColumnName.Arg6, 0f);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			Camera componentInChildren = target.GetComponentInChildren<Camera>(true);
			ImageEffectBase imageEffect;
			bool alreadyEnabled;
			if (!ImageEffectUtil.TryGetComonentCreateIfMissing(imageEffectType, out imageEffect, out alreadyEnabled, componentInChildren.gameObject))
			{
				Complete(imageEffect, thread);
				return;
			}
			if (!inverse)
			{
				imageEffect.enabled = true;
			}
			bool enableAnimation = !string.IsNullOrEmpty(animationName);
			bool flag = imageEffect is IImageEffectStrength;
			if (!flag && !enableAnimation)
			{
				Complete(imageEffect, thread);
				return;
			}
			if (flag)
			{
				IImageEffectStrength fade = imageEffect as IImageEffectStrength;
				float start = (inverse ? fade.Strength : 0f);
				float end = ((!inverse) ? 1 : 0);
				Timer timer = componentInChildren.gameObject.AddComponent<Timer>();
				timer.AutoDestroy = true;
				timer.StartTimer(engine.Page.ToSkippedTime(time), delegate(Timer x)
				{
					fade.Strength = x.GetCurve(start, end);
				}, delegate
				{
					if (!enableAnimation)
					{
						Complete(imageEffect, thread);
					}
				});
			}
			if (!enableAnimation)
			{
				return;
			}
			AdvAnimationData advAnimationData = engine.DataManager.SettingDataManager.AnimationSetting.Find(animationName);
			if (advAnimationData == null)
			{
				Debug.LogError(base.RowData.ToErrorString("Animation " + animationName + " is not found"));
				Complete(imageEffect, thread);
				return;
			}
			AdvAnimationPlayer advAnimationPlayer = componentInChildren.gameObject.AddComponent<AdvAnimationPlayer>();
			advAnimationPlayer.AutoDestory = true;
			advAnimationPlayer.EnableSave = true;
			advAnimationPlayer.Play(advAnimationData.Clip, engine.Page.SkippedSpeed, delegate
			{
				Complete(imageEffect, thread);
			});
		}

		private void Complete(ImageEffectBase imageEffect, AdvScenarioThread thread)
		{
			if (inverse)
			{
				Object.DestroyImmediate(imageEffect);
			}
			OnComplete(thread);
		}
	}
}
