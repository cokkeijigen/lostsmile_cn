using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utage;
using System;

// iTsukezigen++ CHS资源加载器
namespace CHSPatch
{
    public class AssetPatchManager
    {

        private static readonly List<AssetBundle> AssetBundles = new List<AssetBundle>();

        static AssetPatchManager()
        {
            string cnBundlesDir = Directory.GetCurrentDirectory();
            cnBundlesDir = Path.Combine(cnBundlesDir, "LOSTSMILE_CN");
            if (Directory.Exists(cnBundlesDir))
            {
                foreach (string filePath in Directory.GetFiles(cnBundlesDir))
                {
                    try
                    {
                        if (filePath.EndsWith(".dll")) continue;
                        AssetBundle assetBundle = AssetBundle.LoadFromFile(filePath);
                        if (assetBundle == null) continue;
                        AssetBundles.Add(assetBundle);
                    }
                    catch (Exception e)
                    {
                        Logger.OutMessage($"[ERRO] AssetPatchManager: {e.Message}");
                    }
                }
            }
        }

        public static bool GetAssetIfExists(string fileName, out StaticAsset staticAsset)
        {
            staticAsset = null;
            //Logger.OutMessage($"查找文件：{fileName}");
            foreach (AssetBundle bundle in AssetBundles)
            {
                try
                {
                    if (bundle.Contains(fileName))
                    {
                        staticAsset = new StaticAsset
                        {
                            Asset = bundle.LoadAsset<UnityEngine.Object>(fileName)
                        };
                        Logger.OutMessage($"找到文件：{fileName}");
                        return staticAsset.Asset != null;
                    }
                }
                catch (Exception e)
                {
                    Logger.OutMessage($"[ERRO] AssetPatchManager::GetAssetIfExists: {e.Message}");
                }
            }
            return false;
        }
    }
}
