using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AssetFileSetting
	{
		[SerializeField]
		[HideInInspector]
		private AssetFileType fileType;

		[SerializeField]
		private bool isStreamingAssets;

		[SerializeField]
		private List<string> extensions;

		[NonSerialized]
		private AssetFileManagerSettings settings;

		public AssetFileType FileType
		{
			get
			{
				return fileType;
			}
		}

		public bool IsStreamingAssets
		{
			get
			{
				switch (LoadType)
				{
				case AssetFileManagerSettings.LoadType.Local:
				case AssetFileManagerSettings.LoadType.Server:
					return false;
				case AssetFileManagerSettings.LoadType.StreamingAssets:
					return true;
				default:
					return isStreamingAssets;
				}
			}
			set
			{
				isStreamingAssets = value;
			}
		}

		private AssetFileManagerSettings Settings
		{
			get
			{
				return settings;
			}
		}

		public AssetFileManagerSettings.LoadType LoadType
		{
			get
			{
				return Settings.LoadTypeSetting;
			}
		}

		public AssetFileSetting(AssetFileType fileType, string[] extensions)
		{
			this.fileType = fileType;
			this.extensions = new List<string>(extensions);
		}

		public void AddExtensions(string[] extensions)
		{
			this.extensions.AddRange(extensions);
		}

		internal bool ContainsExtensions(string path)
		{
			string item = FilePathUtil.GetExtensionWithOutDouble(path, ".utage").ToLower();
			return extensions.Contains(item);
		}

		public void InitLink(AssetFileManagerSettings settings)
		{
			this.settings = settings;
		}
	}
}
