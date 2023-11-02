namespace Utage
{
	public class AdvVoiceSetting : IAssetFileSoundSettingData, IAssetFileSettingData
	{
		public StringGridRow RowData { get; private set; }

		public float IntroTime => 0f;

		public float Volume => 1f;

		public AdvVoiceSetting(StringGridRow row)
		{
			RowData = row;
		}
	}
}
