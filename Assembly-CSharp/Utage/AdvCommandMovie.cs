using UnityEngine;

namespace Utage
{
	internal class AdvCommandMovie : AdvCommand
	{
		private string label;

		private bool loop;

		private bool cancel;

		private float waitTime;

		private float time;

		public AdvCommandMovie(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			label = ParseCell<string>(AdvColumnName.Arg1);
			loop = ParseCellOptional(AdvColumnName.Arg2, false);
			cancel = ParseCellOptional(AdvColumnName.Arg3, true);
			waitTime = ParseCellOptional(AdvColumnName.Arg6, -1f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (WrapperMoviePlayer.GetInstance().OverrideRootDirectory)
			{
				WrapperMoviePlayer.Play(FilePathUtil.Combine(WrapperMoviePlayer.GetInstance().RootDirectory, label), loop, cancel);
			}
			else
			{
				string text = FilePathUtil.Combine(engine.DataManager.SettingDataManager.BootSetting.ResourceDir, "Movie");
				WrapperMoviePlayer.Play(FilePathUtil.Combine(text, label), loop, cancel);
			}
			time = 0f;
		}

		public override bool Wait(AdvEngine engine)
		{
			if (engine.UiManager.IsInputTrig)
			{
				WrapperMoviePlayer.Cancel();
			}
			bool result = WrapperMoviePlayer.IsPlaying();
			if (waitTime >= 0f)
			{
				if (time >= waitTime)
				{
					result = false;
				}
				time += Time.deltaTime;
			}
			return result;
		}
	}
}
