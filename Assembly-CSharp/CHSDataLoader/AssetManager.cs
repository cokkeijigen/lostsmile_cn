using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utage;
using System;
using UnityEngine.Video;

// iTsukezigen, CHS资源加载器
namespace CHS{
    public class AssetManager
    {

        private static List<AssetBundle> CHSAssetBundles;
        private static bool IsInitialized = false;

        private static void CHSAssetBundlesLoadIfNotInitialized()
        {
            string cn_BundlesDir = Directory.GetCurrentDirectory();
            cn_BundlesDir = Path.Combine(cn_BundlesDir, "LOSTSMILE_Data", "StreamingAssets", "cn_data");
            if (Directory.Exists(cn_BundlesDir))
            {
                if (CHSAssetBundles == null) CHSAssetBundles = new List<AssetBundle>();
                foreach (string filePath in Directory.GetFiles(cn_BundlesDir))
                {
                    try
                    {
                        AssetBundle assetBundle = AssetBundle.LoadFromFile(filePath);
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
                        staticAsset = new StaticAsset();
                        staticAsset.Asset = bundle.LoadAsset<VideoClip>(fileName);
                        LogPrinter.Puts($"找到文件：{fileName}");
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
