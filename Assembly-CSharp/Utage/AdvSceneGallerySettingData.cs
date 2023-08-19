namespace Utage
{
	public class AdvSceneGallerySettingData : AdvSettingDictinoayItemBase
	{
		private string title;

		private string category;

		private string thumbnailName;

		private string thumbnailPath;

		private int thumbnailVersion;

		public string ScenarioLabel
		{
			get
			{
				return base.Key;
			}
		}

		public string Title
		{
			get
			{
				return title;
			}
		}

		public string LocalizedTitle
		{
			get
			{
				return AdvParser.ParseCellLocalizedText(base.RowData, AdvColumnName.Title);
			}
		}

		public string Category
		{
			get
			{
				return category;
			}
		}

		public string ThumbnailPath
		{
			get
			{
				return thumbnailPath;
			}
		}

		public int ThumbnailVersion
		{
			get
			{
				return thumbnailVersion;
			}
		}

		public override bool InitFromStringGridRow(StringGridRow row)
		{
			string text = AdvCommandParser.ParseScenarioLabel(row, AdvColumnName.ScenarioLabel);
			InitKey(text);
			title = AdvParser.ParseCellOptional(row, AdvColumnName.Title, "");
			thumbnailName = AdvParser.ParseCell<string>(row, AdvColumnName.Thumbnail);
			thumbnailVersion = AdvParser.ParseCellOptional(row, AdvColumnName.ThumbnailVersion, 0);
			category = AdvParser.ParseCellOptional(row, AdvColumnName.Categolly, "");
			base.RowData = row;
			return true;
		}

		public void BootInit(AdvSettingDataManager dataManager)
		{
			thumbnailPath = dataManager.BootSetting.ThumbnailDirInfo.FileNameToPath(thumbnailName);
		}
	}
}
