using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AssetFileManagerSettings
	{
		public enum LoadType
		{
			Local,
			Server,
			StreamingAssets,
			Advanced
		}

		[SerializeField]
		private LoadType loadType;

		[SerializeField]
		private List<AssetFileSetting> fileSettings = new List<AssetFileSetting>
		{
			new AssetFileSetting(AssetFileType.Text, new string[11]
			{
				".txt", ".json", ".html", ".htm", ".xml", ".yaml", ".fnt", ".bin", ".bytes", ".csv",
				".tsv"
			}),
			new AssetFileSetting(AssetFileType.Texture, new string[9] { ".png", ".jpg", ".bmp", ".psd", ".tif", ".tga", ".gif", ".iff", ".pict" }),
			new AssetFileSetting(AssetFileType.Sound, new string[9] { ".mp3", ".ogg", ".wav", ".aif", ".aiff", ".xm", ".mod", ".it", ".s3m" }),
			new AssetFileSetting(AssetFileType.UnityObject, new string[1] { "" })
		};

		[NonSerialized]
		private List<AssetFileSetting> rebuildFileSettings;

		public LoadType LoadTypeSetting
		{
			get
			{
				return loadType;
			}
			private set
			{
				loadType = value;
			}
		}

		public List<AssetFileSetting> FileSettings
		{
			get
			{
				RebuildFileSettings();
				return rebuildFileSettings;
			}
		}

		private void RebuildFileSettings()
		{
			if (rebuildFileSettings != null)
			{
				return;
			}
			if (fileSettings.Count != Enum.GetValues(typeof(AssetFileType)).Length)
			{
				rebuildFileSettings = (fileSettings = DefaultFileSettings());
			}
			else
			{
				rebuildFileSettings = fileSettings;
			}
			foreach (AssetFileSetting rebuildFileSetting in rebuildFileSettings)
			{
				rebuildFileSetting.InitLink(this);
			}
		}

		private List<AssetFileSetting> DefaultFileSettings()
		{
			List<AssetFileSetting> list = new List<AssetFileSetting>();
			list.Add(new AssetFileSetting(AssetFileType.Text, new string[11]
			{
				".txt", ".json", ".html", ".htm", ".xml", ".yaml", ".fnt", ".bin", ".bytes", ".csv",
				".tsv"
			}));
			list.Add(new AssetFileSetting(AssetFileType.Texture, new string[9] { ".png", ".jpg", ".bmp", ".psd", ".tif", ".tga", ".gif", ".iff", ".pict" }));
			list.Add(new AssetFileSetting(AssetFileType.Sound, new string[9] { ".mp3", ".ogg", ".wav", ".aif", ".aiff", ".xm", ".mod", ".it", ".s3m" }));
			list.Add(new AssetFileSetting(AssetFileType.UnityObject, new string[1] { "" }));
			return list;
		}

		public void BootInit(LoadType loadType)
		{
			this.loadType = loadType;
			foreach (AssetFileSetting fileSetting in FileSettings)
			{
				fileSetting.InitLink(this);
			}
		}

		public void AddExtensions(AssetFileType type, string[] extensions)
		{
			Find(type).AddExtensions(extensions);
		}

		public AssetFileSetting Find(AssetFileType type)
		{
			return FileSettings.Find((AssetFileSetting x) => x.FileType == type);
		}

		public AssetFileSetting FindSettingFromPath(string path)
		{
			AssetFileSetting assetFileSetting = fileSettings.Find((AssetFileSetting x) => x.ContainsExtensions(path));
			if (assetFileSetting == null)
			{
				assetFileSetting = Find(AssetFileType.UnityObject);
			}
			return assetFileSetting;
		}
	}
}
