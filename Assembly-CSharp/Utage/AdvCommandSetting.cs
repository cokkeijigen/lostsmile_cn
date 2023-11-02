namespace Utage
{
	public class AdvCommandSetting : IAssetFileSettingData
	{
		public AdvCommand Command { get; private set; }

		public StringGridRow RowData => Command.RowData;

		public AdvCommandSetting(AdvCommand command)
		{
			Command = command;
		}
	}
}
