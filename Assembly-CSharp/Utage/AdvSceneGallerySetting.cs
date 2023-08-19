using System.Collections.Generic;

namespace Utage
{
	public class AdvSceneGallerySetting : AdvSettingDataDictinoayBase<AdvSceneGallerySettingData>
	{
		public override void BootInit(AdvSettingDataManager dataManager)
		{
			foreach (AdvSceneGallerySettingData item in base.List)
			{
				item.BootInit(dataManager);
			}
		}

		public override void DownloadAll()
		{
			foreach (AdvSceneGallerySettingData item in base.List)
			{
				AssetFileManager.Download(item.ThumbnailPath);
			}
		}

		public List<AdvSceneGallerySettingData> CreateGalleryDataList(string category)
		{
			List<AdvSceneGallerySettingData> list = new List<AdvSceneGallerySettingData>();
			foreach (AdvSceneGallerySettingData item in base.List)
			{
				if (item.Category == category)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public List<string> CreateCategoryList()
		{
			List<string> list = new List<string>();
			foreach (AdvSceneGallerySettingData item in base.List)
			{
				if (!string.IsNullOrEmpty(item.ThumbnailPath) && !list.Contains(item.Category))
				{
					list.Add(item.Category);
				}
			}
			return list;
		}

		public bool Contains(string key)
		{
			return base.Dictionary.ContainsKey(key);
		}
	}
}
