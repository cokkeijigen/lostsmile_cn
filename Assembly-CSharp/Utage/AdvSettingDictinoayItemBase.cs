namespace Utage
{
	public abstract class AdvSettingDictinoayItemBase : IAdvSettingData
	{
		private string key;

		public string Key => key;

		public StringGridRow RowData { get; protected set; }

		internal void InitKey(string key)
		{
			this.key = key;
		}

		internal bool InitFromStringGridRowMain(StringGridRow row)
		{
			RowData = row;
			return InitFromStringGridRow(row);
		}

		public abstract bool InitFromStringGridRow(StringGridRow row);
	}
}
