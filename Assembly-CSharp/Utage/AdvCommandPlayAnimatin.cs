using UnityEngine;

namespace Utage
{
	public class AdvCommandPlayAnimatin : AdvCommandEffectBase
	{
		private string animationName;

		public AdvCommandPlayAnimatin(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			animationName = ParseCell<string>(AdvColumnName.Arg2);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			AdvAnimationData advAnimationData = engine.DataManager.SettingDataManager.AnimationSetting.Find(animationName);
			if (advAnimationData == null)
			{
				Debug.LogError(base.RowData.ToErrorString("Animation " + animationName + " is not found"));
				OnComplete(thread);
				return;
			}
			AdvAnimationPlayer advAnimationPlayer = target.AddComponent<AdvAnimationPlayer>();
			advAnimationPlayer.AutoDestory = true;
			advAnimationPlayer.EnableSave = true;
			advAnimationPlayer.Play(advAnimationData.Clip, engine.Page.SkippedSpeed, delegate
			{
				OnComplete(thread);
			});
		}
	}
}
