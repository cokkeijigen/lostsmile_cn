# lostsmile unity 中文本地化项目
## 本项目仅供学习交流使用，严禁一切商业或特殊用途！！！ ![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_00.png)<br>
- LOSTSMILE_CN：程序`hook`相关，实现了资源重定向，可以与原版资源共存**
- Assembly-CSharp：原版`Assembly-CSharp.dll`反编译源码，修复存档路径问题，以及添加自定义AssetBundle加载和资源替换（注：`release`分支为未修改过的源码）

## 0x00 如何打包使用Unity打包Assetbundle
首先要知道游戏的unity版本，这个可以通过在游戏`主程序.exe`的属性或者`UnityPlayer.dll`属性中查看，再或者用`ida`打开`UnityPlayer.dll`查找字符串`version`或者`UnityPlayer`等字眼找到。
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_01.png)<br>
从[UnityArchive](https://unity.com/cn/releases/editor/archive)中下载对应版本的Unity，这个游戏的版本为`2018.4.15f1`，相信聪明的你，一定不用多说也知道怎么下载安装了吧。
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_02.png)<br>
接着随便新建一个项目，`Template`随意，用默认的就行了
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_03.png)<br>
来到项目`Assets`目录下新建一个`Editor`的目录，进入里面接着创建一个C#脚本，名称随意，我这里叫`BuildAssetBundle`，内容如下
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_04.png)<br>
```cs
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildAssetBundles : MonoBehaviour
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        // 设置AB包输出路径
        string assetBundleDirectory = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        // 构建StandaloneWindows的AB包
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}
```
回到`Assets`，创建一个目录用于存档要打包的资源，名称也随意，我这里叫`LostSmile`，点击目录，可以看到右下角有个`Asset Lables`，针对这个文件新建一个，第一个是`输出的文件名`，第二个是`后缀名`
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_05.png)<br>
进入`LostSmile`，将要打包的资源放入
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_06.png)<br>
回到外面，在`LostSmile`右键`Build AssetBundles`即可
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_07.png)<br>
输出的`AssetBundles`就在项目目录下的`Assets\StreamingAssets\AssetBundles`
![Image text](https://raw.githubusercontent.com/cokkeijigen/lostsmile_cn/master/Pictures/lostsmile_08.png)<br>

##  0x01 加载自己打包的`AssetBundles`并替换
首先创建一个工具类，用来管理我们自己的AssetBundle：[CHS::AssetManager](https://github.com/cokkeijigen/lostsmile_cn/blob/master/Assembly-CSharp/CHSDataLoader/AssetManager.cs)
```cs
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Utage;
using System;

namespace CHS {
    public class AssetManager
    {
        private static List<AssetBundle> CHSAssetBundles;
        private static bool IsInitialized = false;

        // 加载所有Assetbundle
        private static void CHSAssetBundlesLoadIfNotInitialized()
        {
            string cnBundlesDir = Directory.GetCurrentDirectory();
            // Assetbundle存档位置为：`游戏目录/LOSTSMILE_CN/`
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
                     catch (Exception e) {}
                }
            }
            IsInitialized = true
        }
        
        // 从AssetBundle中获取资源
        public static bool GetCHSAssetFileIfExists(string fileName, out StaticAsset staticAsset)
        {
            staticAsset = null;
            if(!AssetManager.IsInitialized) CHSAssetBundlesLoadIfNotInitialized();
            if (CHSAssetBundles == null || CHSAssetBundles.Count == 0) return false;
            foreach (AssetBundle bundle in AssetManager.CHSAssetBundles) {
                try
                {
                    if (bundle.Contains(fileName))
                    {
                         staticAsset = new StaticAsset
                         {
                             Asset = bundle.LoadAsset<UnityEngine.Object>(fileName)
                         }
                         return staticAsset.Asset != null;
                    }
                } catch (Exception e){}
            }
        }
    }
}
```
找到游戏获取`AssetFile`的地方插入自己写的方法调用代码，位置在： [Utage::StaticAssetManager::FindAssetFile](https://github.com/cokkeijigen/lostsmile_cn/blob/master/Assembly-CSharp/Utage/StaticAssetManager.cs#L23)
```cs
public AssetFileBase FindAssetFile(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData)
{

    if (Assets == null)
    {
         return null;
    }
    string assetName = FilePathUtil.GetFileNameWithoutExtension(fileInfo.FileName);
    StaticAsset staticAsset; // 尝试获取LOSTSMILE_CN目录下的资源
    if (!CHS.AssetManager.GetCHSAssetFileIfExists(assetName.ToLower(), out staticAsset))
    {
        staticAsset = Assets.Find((StaticAsset x) => x.Asset.name == assetName);
        if (staticAsset == null)
        {
            return null;
        }
    }
    return new StaticAssetFile(staticAsset, mangager, fileInfo, settingData);
}
```
## 0x02 修复存档中的绝对路径
