﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/File/StaticAssetManager")]
	public class StaticAssetManager : MonoBehaviour
	{
		[SerializeField]
		private List<StaticAsset> assets = new List<StaticAsset>();

		private List<StaticAsset> Assets
		{
			get
			{
				return assets;
			}
		}

		public AssetFileBase FindAssetFile(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData)
		{
			if (Assets == null)
			{
				return null;
			}
			string assetName = FilePathUtil.GetFileNameWithoutExtension(fileInfo.FileName);
			StaticAsset staticAsset;
			// iTsukeziegn， 尝试获取cn_data目录下的资源
			if (!CHS.AssetManager.GetCHSAssetFileIfExists(assetName.ToLower(), out staticAsset))
			{
				staticAsset = Assets.Find((StaticAsset x) => x.Asset.name == assetName);
				if (staticAsset == null)
				{
					return null;
				}
			}
            // end
            return new StaticAssetFile(staticAsset, mangager, fileInfo, settingData);
		}

		public bool Contains(Object asset)
		{
			foreach (StaticAsset asset2 in Assets)
			{
				if (asset2.Asset == asset)
				{
					return true;
				}
			}
			return false;
		}
	}
}
