using System.Collections.Generic;

namespace Utage
{
	public class AdvCgGalleryData
	{
		private List<AdvTextureSettingData> list;

		private string thumbnailPath;

		private AdvGallerySaveData saveData;

		public string ThumbnailPath => thumbnailPath;

		public int NumTotal => list.Count;

		public int NumOpen
		{
			get
			{
				int num = 0;
				if (saveData == null)
				{
					return 0;
				}
				foreach (AdvTextureSettingData item in list)
				{
					if (saveData.CheckCgLabel(item.Key))
					{
						num++;
					}
				}
				return num;
			}
		}

		public AdvCgGalleryData(string thumbnailPath, AdvGallerySaveData saveData)
		{
			this.thumbnailPath = thumbnailPath;
			list = new List<AdvTextureSettingData>();
			this.saveData = saveData;
		}

		public void AddTextureData(AdvTextureSettingData data)
		{
			list.Add(data);
		}

		public AdvTextureSettingData GetDataOpened(int index)
		{
			int num = 0;
			foreach (AdvTextureSettingData item in list)
			{
				if (saveData.CheckCgLabel(item.Key))
				{
					if (index == num)
					{
						return item;
					}
					num++;
				}
			}
			return null;
		}
	}
}
