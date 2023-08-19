namespace Utage
{
	internal class AdvCommandShake : AdvCommandTween
	{
		public AdvCommandShake(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row, dataManager)
		{
		}

		protected override void InitTweenData()
		{
			string text = " x=30 y=30";
			string text2 = ParseCellOptional(AdvColumnName.Arg3, text);
			if (!text2.Contains("x=") && !text2.Contains("y="))
			{
				text2 += text;
			}
			string easeType = ParseCellOptional(AdvColumnName.Arg4, "");
			string loopType = ParseCellOptional(AdvColumnName.Arg5, "");
			tweenData = new iTweenData(iTweenType.ShakePosition.ToString(), text2, easeType, loopType);
		}
	}
}
