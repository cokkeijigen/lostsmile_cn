namespace Utage
{
	public class AdvTextureSettingData : AdvSettingDictinoayItemBase
	{
		public delegate void ParseCustomFileTypeRootDir(string fileType, ref string rootDir);

		public enum Type
		{
			Bg,
			Event,
			Sprite
		}

		public static ParseCustomFileTypeRootDir CallbackParseCustomFileTypeRootDir;

		private string thumbnailName;

		public Type TextureType { get; private set; }

		public AdvGraphicInfoList Graphic { get; private set; }

		public string ThumbnailPath { get; private set; }

		public int ThumbnailVersion { get; private set; }

		public string CgCategory { get; private set; }

		public override bool InitFromStringGridRow(StringGridRow row)
		{
			base.RowData = row;
			string text = AdvParser.ParseCell<string>(row, AdvColumnName.Label);
			InitKey(text);
			TextureType = AdvParser.ParseCell<Type>(row, AdvColumnName.Type);
			Graphic = new AdvGraphicInfoList(text);
			thumbnailName = AdvParser.ParseCellOptional(row, AdvColumnName.Thumbnail, "");
			ThumbnailVersion = AdvParser.ParseCellOptional(row, AdvColumnName.ThumbnailVersion, 0);
			CgCategory = AdvParser.ParseCellOptional(row, AdvColumnName.CgCategolly, "");
			AddGraphicInfo(row);
			return true;
		}

		internal void BootInit(AdvSettingDataManager dataManager)
		{
			Graphic.BootInit((string fileName, string fileType) => FileNameToPath(fileName, fileType, dataManager.BootSetting), dataManager);
			ThumbnailPath = dataManager.BootSetting.ThumbnailDirInfo.FileNameToPath(thumbnailName);
			if (!string.IsNullOrEmpty(ThumbnailPath))
			{
				AssetFileManager.GetFileCreateIfMissing(ThumbnailPath);
			}
		}

		private string FileNameToPath(string fileName, string fileType, AdvBootSetting settingData)
		{
			string rootDir = null;
			if (CallbackParseCustomFileTypeRootDir != null)
			{
				CallbackParseCustomFileTypeRootDir(fileType, ref rootDir);
				if (rootDir != null)
				{
					return FilePathUtil.Combine(settingData.ResourceDir, rootDir, fileName);
				}
			}
			switch (TextureType)
			{
			case Type.Event:
				return settingData.EventDirInfo.FileNameToPath(fileName);
			case Type.Sprite:
				return settingData.SpriteDirInfo.FileNameToPath(fileName);
			default:
				return settingData.BgDirInfo.FileNameToPath(fileName);
			}
		}

		internal void AddGraphicInfo(StringGridRow row)
		{
			Graphic.Add("Texture", row, this);
		}
	}
}
