namespace Utage
{
	internal class AdvLocalVideoFile : AssetFileUtage
	{
		public AdvLocalVideoFile(AdvVideoLoadPathChanger pathChanger, AssetFileManager assetFileManager, AssetFileInfo fileInfo, IAssetFileSettingData settingData)
			: base(assetFileManager, fileInfo, settingData)
		{
			fileInfo.StrageType = AssetFileStrageType.Resources;
			if (settingData is AdvCommandSetting)
			{
				string text = (settingData as AdvCommandSetting).Command.ParseCell<string>(AdvColumnName.Arg1);
				base.LoadPath = FilePathUtil.Combine(pathChanger.RootPath, text);
			}
			else
			{
				string fileName = (settingData as AdvGraphicInfo).FileName;
				base.LoadPath = FilePathUtil.Combine(pathChanger.RootPath, fileName);
			}
		}
	}
}
