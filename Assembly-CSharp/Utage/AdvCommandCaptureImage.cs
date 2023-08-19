namespace Utage
{
	internal class AdvCommandCaptureImage : AdvCommand
	{
		private string objName;

		private string cameraName;

		private string layerName;

		private bool isWaiting;

		public AdvCommandCaptureImage(StringGridRow row)
			: base(row)
		{
			objName = ParseCell<string>(AdvColumnName.Arg1);
			cameraName = ParseCell<string>(AdvColumnName.Arg2);
			layerName = ParseCell<string>(AdvColumnName.Arg3);
		}

		public override void DoCommand(AdvEngine engine)
		{
			isWaiting = true;
			engine.GraphicManager.CreateCaptureImageObject(objName, cameraName, layerName);
		}

		public override bool Wait(AdvEngine engine)
		{
			if (!isWaiting)
			{
				return false;
			}
			isWaiting = false;
			return true;
		}
	}
}
