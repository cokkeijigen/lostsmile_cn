using System;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/VideoLoadPathChanger")]
	public class AdvVideoLoadPathChanger : MonoBehaviour
	{
		[SerializeField]
		private string rootPath = "";

		public string RootPath
		{
			get
			{
				return rootPath;
			}
		}

		private void Awake()
		{
			CustomLoadManager customLoadManager = AssetFileManager.GetCustomLoadManager();
			customLoadManager.OnFindAsset = (CustomLoadManager.FindAsset)Delegate.Combine(customLoadManager.OnFindAsset, new CustomLoadManager.FindAsset(FindAsset));
		}

		private void FindAsset(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData, ref AssetFileBase asset)
		{
			if (IsVideoType(fileInfo, settingData))
			{
				asset = new AdvLocalVideoFile(this, mangager, fileInfo, settingData);
			}
		}

		private bool IsVideoType(AssetFileInfo fileInfo, IAssetFileSettingData settingData)
		{
			if (fileInfo.FileType != AssetFileType.UnityObject)
			{
				return false;
			}
			if (settingData is AdvCommandSetting)
			{
				return (settingData as AdvCommandSetting).Command is AdvCommandVideo;
			}
			AdvGraphicInfo advGraphicInfo = settingData as AdvGraphicInfo;
			if (advGraphicInfo != null)
			{
				return advGraphicInfo.FileType == "Video";
			}
			return false;
		}
	}
}
