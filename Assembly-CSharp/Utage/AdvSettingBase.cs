using System.Collections.Generic;

namespace Utage
{
	public abstract class AdvSettingBase : IAdvSetting
	{
		private List<StringGrid> gridList = new List<StringGrid>();

		public List<StringGrid> GridList
		{
			get
			{
				return gridList;
			}
		}

		public virtual void ParseGrid(StringGrid grid)
		{
			GridList.Add(grid);
			grid.InitLink();
			OnParseGrid(grid);
		}

		protected abstract void OnParseGrid(StringGrid grid);

		public virtual void BootInit(AdvSettingDataManager dataManager)
		{
		}

		public virtual void DownloadAll()
		{
		}
	}
}
