namespace Utage
{
	public class AssetFileInfo
	{
		public string FileName { get; private set; }

		public AssetBundleInfo AssetBundleInfo { get; set; }

		public AssetFileType FileType
		{
			get
			{
				return Setting.FileType;
			}
		}

		public AssetFileSetting Setting { get; private set; }

		public AssetFileStrageType StrageType { get; set; }

		public AssetFileInfo(string path, AssetFileManagerSettings settings, AssetBundleInfo assetBundleInfo)
		{
			FileName = path;
			Setting = settings.FindSettingFromPath(path);
			AssetBundleInfo = assetBundleInfo;
			StrageType = ParseStrageType();
		}

		private AssetFileStrageType ParseStrageType()
		{
			if (Setting.IsStreamingAssets)
			{
				return AssetFileStrageType.StreamingAssets;
			}
			if (FilePathUtil.IsAbsoluteUri(FileName))
			{
				return AssetFileStrageType.Server;
			}
			if (Setting.LoadType == AssetFileManagerSettings.LoadType.Server)
			{
				return AssetFileStrageType.Server;
			}
			return AssetFileStrageType.Resources;
		}
	}
}
