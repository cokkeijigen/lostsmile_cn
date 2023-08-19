using UnityEngine;

namespace Utage
{
	internal class AdvCommandWaitInput : AdvCommand
	{
		private float time;

		private float waitEndTime;

		public AdvCommandWaitInput(StringGridRow row)
			: base(row)
		{
			time = ParseCellOptional(AdvColumnName.Arg6, -1f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (base.CurrentTread.IsMainThread)
			{
				engine.Page.IsWaitingInputCommand = true;
			}
			waitEndTime = Time.time + (engine.Page.CheckSkip() ? (time / engine.Config.SkipSpped) : time);
		}

		public override bool Wait(AdvEngine engine)
		{
			if (IsWaitng(engine))
			{
				return true;
			}
			if (engine.Config.VoiceStopType == VoiceStopType.OnClick)
			{
				engine.SoundManager.StopVoiceIgnoreLoop();
			}
			engine.UiManager.ClearPointerDown();
			if (base.CurrentTread.IsMainThread)
			{
				engine.Page.IsWaitingInputCommand = false;
			}
			return false;
		}

		private bool IsWaitng(AdvEngine engine)
		{
			if (engine.Page.CheckSkip())
			{
				return false;
			}
			if (engine.UiManager.IsInputTrig)
			{
				return false;
			}
			if (time > 0f)
			{
				return Time.time < waitEndTime;
			}
			return true;
		}
	}
}
