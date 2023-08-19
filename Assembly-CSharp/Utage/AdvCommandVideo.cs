using UnityEngine;

namespace Utage
{
	internal class AdvCommandVideo : AdvCommand
	{
		private bool isEndPlay = true;

		private CameraClearFlags cameraClearFlags;

		private Color cameraClearColor;

		private AssetFile file;

		private string label;

		private bool loop;

		private bool cancel;

		private string cameraName;

		public AdvCommandVideo(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			label = ParseCell<string>(AdvColumnName.Arg1);
			cameraName = ParseCell<string>(AdvColumnName.Arg2);
			loop = ParseCellOptional(AdvColumnName.Arg3, false);
			cancel = ParseCellOptional(AdvColumnName.Arg4, true);
			string text = FilePathUtil.Combine(dataManager.BootSetting.ResourceDir, "Video");
			text = FilePathUtil.Combine(text, label);
			file = AddLoadFile(text, new AdvCommandSetting(this));
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.GraphicManager.VideoManager.Play(label, cameraName, file, loop, cancel);
			isEndPlay = false;
		}

		public override bool Wait(AdvEngine engine)
		{
			if (!isEndPlay)
			{
				if (engine.UiManager.IsInputTrig)
				{
					engine.GraphicManager.VideoManager.Cancel(label);
				}
				isEndPlay = engine.GraphicManager.VideoManager.IsEndPlay(label);
				if (isEndPlay)
				{
					engine.GraphicManager.VideoManager.Complete(label);
					Camera componentInChildren = engine.EffectManager.FindTarget(AdvEffectManager.TargetType.Camera, cameraName).GetComponentInChildren<Camera>();
					cameraClearFlags = componentInChildren.clearFlags;
					cameraClearColor = componentInChildren.backgroundColor;
					componentInChildren.clearFlags = CameraClearFlags.Color;
					componentInChildren.backgroundColor = Color.black;
				}
				return true;
			}
			Camera componentInChildren2 = engine.EffectManager.FindTarget(AdvEffectManager.TargetType.Camera, cameraName).GetComponentInChildren<Camera>();
			componentInChildren2.clearFlags = cameraClearFlags;
			componentInChildren2.backgroundColor = cameraClearColor;
			return false;
		}
	}
}
