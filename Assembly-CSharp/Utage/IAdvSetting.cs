using System.Collections.Generic;

namespace Utage
{
	public interface IAdvSetting
	{
		List<StringGrid> GridList { get; }

		void ParseGrid(StringGrid grid);

		void BootInit(AdvSettingDataManager dataManager);

		void DownloadAll();
	}
}
