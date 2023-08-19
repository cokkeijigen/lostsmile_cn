namespace Utage
{
	public class AdvVoiceSetting : IAssetFileSoundSettingData, IAssetFileSettingData
	{
		public StringGridRow RowData { get; private set; }

		public float IntroTime
		{
			get
			{
				return 0f;
			}
		}

		public float Volume
		{
			get
			{
				return 1f;
			}
		}

		public AdvVoiceSetting(StringGridRow row)
		{
			RowData = row;
		}
	}
}
