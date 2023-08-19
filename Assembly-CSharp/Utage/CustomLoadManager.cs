using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/File/CustomLoadManager")]
	public class CustomLoadManager : MonoBehaviour
	{
		public delegate void FindAsset(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData, ref AssetFileBase asset);

		public FindAsset OnFindAsset { get; set; }

		public AssetFileBase Find(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData)
		{
			if (OnFindAsset != null)
			{
				AssetFileBase asset = null;
				OnFindAsset(mangager, fileInfo, settingData, ref asset);
				if (asset != null)
				{
					return asset;
				}
			}
			return null;
		}
	}
}
