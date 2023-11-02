namespace Utage
{
	public class AdvSceneGallerySettingData : AdvSettingDictinoayItemBase
	{
		private string title;

		private string category;

		private string thumbnailName;

		private string thumbnailPath;

		private int thumbnailVersion;

		public string ScenarioLabel => base.Key;

		public string Title => title;

		public string LocalizedTitle => AdvParser.ParseCellLocalizedText(base.RowData, AdvColumnName.Title);

		public string Category => category;

		public string ThumbnailPath => thumbnailPath;

		public int ThumbnailVersion => thumbnailVersion;

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
