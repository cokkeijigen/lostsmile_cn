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
                    if (filePath.EndsWith(".dll")) continue;
                    try
                    {
                        AssetBundle assetBundle = AssetBundle.LoadFromFile(filePath);
                        if (assetBundle == null) 
                        {
                            continue;
                        }
                        AssetBundles.Add(assetBundle);
                       
                        #if DEBUG
                            Logger.OutMessage($"[AssetPatchManager] ADD: {filePath}");
                        #endif
                    }
                    catch (Exception e)
                    {
                        #if DEBUG
                            Logger.OutMessage($"[AssetPatchManager] ERRO: {e.Message}");
                        #endif
                    }
                }
            }
        }

        public static bool GetAssetIfExists(string fileName, out StaticAsset staticAsset)
        {
            staticAsset = null;
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
                        #if DEBUG
                            Logger.OutMessage($"[AssetPatchManager::GetAssetIfExists] Found：{fileName}");
                        #endif
                        return staticAsset.Asset != null;
                    }
                }
                catch (Exception e)
                {
                    #if DEBUG
                        Logger.OutMessage($"[AssetPatchManager::GetAssetIfExists] ERRO: {e.Message}");
                    #endif
                }
            }
            return false;
        }

        public static bool GetSprite(string fileName, out Sprite sprite) 
        {
            sprite = null;
            foreach (AssetBundle bundle in AssetBundles)
            {
                try
                {
                    if (bundle.Contains(fileName))
                    {
                        sprite = bundle.LoadAsset<Sprite>(fileName);
                        #if DEBUG
                            Logger.OutMessage($"[AssetPatchManager::GetSprite] Found：{fileName}");
                        #endif
                        return sprite != null;
                    }
                }
                catch (Exception e)
                {
                    #if DEBUG
                        Logger.OutMessage($"[AssetPatchManager::GetSprite] ERRO: {e.Message}");
                    #endif
                }
            }
            return false;
        }
    }
}
