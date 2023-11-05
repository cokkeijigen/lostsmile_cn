using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utage;
using System;

// iTsukezigen++ CHS资源加载器
namespace CHS {
    public class AssetManager
    {

        private static List<AssetBundle> CHSAssetBundles;
        private static bool IsInitialized = false;

        private static void CHSAssetBundlesLoadIfNotInitialized()
        {
            string cnBundlesDir = Directory.GetCurrentDirectory();
            cnBundlesDir = Path.Combine(cnBundlesDir, "LOSTSMILE_CN");
            if (Directory.Exists(cnBundlesDir))
            {
                if (CHSAssetBundles == null) CHSAssetBundles = new List<AssetBundle>();
                foreach (string filePath in Directory.GetFiles(cnBundlesDir))
                {
                    try
                    {
                        if (filePath.EndsWith(".dll")) continue;
                        AssetBundle assetBundle = AssetBundle.LoadFromFile(filePath);
                        if (assetBundle == null) continue;
                        CHSAssetBundles.Add(assetBundle);
                    }
                    catch (Exception e)
                    {
                        LogPrinter.Puts($"CHSAssetBundlesLoad: {e.Message}", "err");
                    }
                }
            }
            AssetManager.IsInitialized = true;
        }

        public static bool GetCHSAssetFileIfExists(string fileName, out StaticAsset staticAsset)
        {
            staticAsset = null;
            if(!AssetManager.IsInitialized) CHSAssetBundlesLoadIfNotInitialized();
            if (CHSAssetBundles == null || CHSAssetBundles.Count == 0) return false;
            foreach (AssetBundle bundle in AssetManager.CHSAssetBundles) {
                try {
                    if (bundle.Contains(fileName))
                    {
                        staticAsset = new StaticAsset
                        {
                            Asset = bundle.LoadAsset<UnityEngine.Object>(fileName)
                        };
                        //LogPrinter.Puts($"找到文件：{fileName}");
                        return staticAsset.Asset != null;
                    }
                } catch (Exception e) {
                    LogPrinter.Puts($"GetCHSAssetFileIfExists: {e.Message}", "err");
                }
            }
            return false;
        }
    }
}
