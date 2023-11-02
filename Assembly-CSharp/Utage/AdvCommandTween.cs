using UnityEngine;

namespace Utage
{
	public class AdvCommandTween : AdvCommandEffectBase
	{
		protected iTweenData tweenData;

		public AdvCommandTween(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			InitTweenData();
			if (tweenData.Type == iTweenType.Stop)
			{
				base.WaitType = AdvCommandWaitType.Add;
			}
			if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
			{
				Debug.LogError(ToErrorString(tweenData.ErrorMsg));
			}
		}

		protected override void OnParse()
		{
			ParseEffectTarget(AdvColumnName.Arg1);
			if (!IsEmptyCell(AdvColumnName.WaitType))
			{
				ParseWait(AdvColumnName.WaitType);
			}
			else if (!IsEmptyCell(AdvColumnName.Arg6))
			{
				ParseWait(AdvColumnName.Arg6);
			}
			else
			{
				ParseWait(AdvColumnName.WaitType);
			}
		}

		protected virtual void InitTweenData()
		{
			string type = ParseCell<string>(AdvColumnName.Arg2);
			string arg = ParseCellOptional(AdvColumnName.Arg3, "");
			string easeType = ParseCellOptional(AdvColumnName.Arg4, "");
			string loopType = ParseCellOptional(AdvColumnName.Arg5, "");
			tweenData = new iTweenData(type, arg, easeType, loopType);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
			{
				Debug.LogError(tweenData.ErrorMsg);
				OnComplete(thread);
				return;
			}
			AdvITweenPlayer advITweenPlayer = target.AddComponent<AdvITweenPlayer>();
			advITweenPlayer.Init(skipSpeed: engine.Page.CheckSkip() ? engine.Config.SkipSpped : 0f, data: tweenData, isUnder2DSpace: IsUnder2DSpace(target), pixelsToUnits: engine.GraphicManager.PixelsToUnits, callbackComplete: delegate
			{
				OnComplete(thread);
			});
			advITweenPlayer.Play();
			_ = advITweenPlayer.IsEndlessLoop;
		}

		private bool IsUnder2DSpace(GameObject target)
		{
			switch (targetType)
			{
			case AdvEffectManager.TargetType.MessageWindow:
				return true;
			case AdvEffectManager.TargetType.Default:
				return target.GetComponent<AdvGraphicObject>() != null;
			default:
				return false;
			}
		}
	}
}
