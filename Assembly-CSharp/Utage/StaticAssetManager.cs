using CHSPatch;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/File/StaticAssetManager")]
	public class StaticAssetManager : MonoBehaviour
	{
		[SerializeField]
		private List<StaticAsset> assets = new List<StaticAsset>();

		private List<StaticAsset> Assets => assets;

		public AssetFileBase FindAssetFile(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData)
		{

            if (Assets == null)
			{
				return null;
			}
			string assetName = FilePathUtil.GetFileNameWithoutExtension(fileInfo.FileName);
            // iTsukeziegn++ 
            StaticAsset staticAsset; // 尝试替换资源文件
            if (!AssetPatchManager.GetAssetIfExists(assetName.ToLower(), out staticAsset))
            {
                staticAsset = Assets.Find(x => x.Asset.name == assetName);
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
