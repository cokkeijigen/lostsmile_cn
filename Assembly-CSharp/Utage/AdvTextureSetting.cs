using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class AdvTextureSetting : AdvSettingDataDictinoayBase<AdvTextureSettingData>
	{
		protected override bool TryParseContinues(AdvTextureSettingData last, StringGridRow row)
		{
			if (last == null)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(AdvParser.ParseCellOptional(row, AdvColumnName.Label, "")))
			{
				return false;
			}
			last.AddGraphicInfo(row);
			return true;
		}

		public override void BootInit(AdvSettingDataManager dataManager)
		{
			foreach (AdvTextureSettingData item in base.List)
			{
				item.BootInit(dataManager);
			}
		}

		public override void DownloadAll()
		{
			foreach (AdvTextureSettingData item in base.List)
			{
				item.Graphic.DownloadAll();
				if (!string.IsNullOrEmpty(item.ThumbnailPath))
				{
					AssetFileManager.Download(item.ThumbnailPath);
				}
			}
		}

		public AdvGraphicInfoList LabelToGraphic(string label)
		{
			AdvTextureSettingData advTextureSettingData = FindData(label);
			if (advTextureSettingData == null)
			{
				Debug.LogError("Not contains " + label + " in Texture sheet");
				return null;
			}
			return advTextureSettingData.Graphic;
		}

		public bool ContainsLabel(string label)
		{
			if (FindData(label) == null)
			{
				return false;
			}
			return true;
		}

		private AdvTextureSettingData FindData(string label)
		{
			AdvTextureSettingData value;
			if (!base.Dictionary.TryGetValue(label, out value))
			{
				return null;
			}
			return value;
		}

		public List<AdvCgGalleryData> CreateCgGalleryList(AdvGallerySaveData saveData)
		{
			return CreateCgGalleryList(saveData, "");
		}

		public List<AdvCgGalleryData> CreateCgGalleryList(AdvGallerySaveData saveData, string category)
		{
			List<AdvCgGalleryData> list = new List<AdvCgGalleryData>();
			AdvCgGalleryData advCgGalleryData = null;
			foreach (AdvTextureSettingData item in base.List)
			{
				if (item.TextureType == AdvTextureSettingData.Type.Event && !string.IsNullOrEmpty(item.ThumbnailPath) && (string.IsNullOrEmpty(category) || !(item.CgCategory != category)))
				{
					string thumbnailPath = item.ThumbnailPath;
					if (advCgGalleryData == null)
					{
						advCgGalleryData = new AdvCgGalleryData(thumbnailPath, saveData);
						list.Add(advCgGalleryData);
					}
					else if (thumbnailPath != advCgGalleryData.ThumbnailPath)
					{
						advCgGalleryData = new AdvCgGalleryData(thumbnailPath, saveData);
						list.Add(advCgGalleryData);
					}
					advCgGalleryData.AddTextureData(item);
				}
			}
			return list;
		}

		public List<string> CreateCgGalleryCategoryList()
		{
			List<string> list = new List<string>();
			foreach (AdvTextureSettingData item in base.List)
			{
				if (item.TextureType == AdvTextureSettingData.Type.Event && !string.IsNullOrEmpty(item.ThumbnailPath) && !string.IsNullOrEmpty(item.CgCategory) && !list.Contains(item.CgCategory))
				{
					list.Add(item.CgCategory);
				}
			}
			return list;
		}
	}
}
