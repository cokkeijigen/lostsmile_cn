using System.Collections.Generic;

namespace Utage
{
	public class AdvAnimationSetting : AdvSettingBase
	{
		private List<AdvAnimationData> DataList = new List<AdvAnimationData>();

		protected override void OnParseGrid(StringGrid grid)
		{
			int index = 0;
			while (index < grid.Rows.Count)
			{
				if (grid.Rows[index].IsEmpty)
				{
					index++;
					continue;
				}
				AdvAnimationData item = new AdvAnimationData(grid, ref index, true);
				DataList.Add(item);
			}
		}

		public AdvAnimationData Find(string name)
		{
			return DataList.Find((AdvAnimationData x) => x.Clip.name == name);
		}
	}
}
