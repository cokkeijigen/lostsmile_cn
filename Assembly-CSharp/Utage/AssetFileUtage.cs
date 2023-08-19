using System;
using System.Collections;
using UnityEngine;

namespace Utage
{
	internal class AssetFileUtage : AssetFileBase
	{
		protected string LoadPath { get; set; }

		protected AssetBundle AssetBundle { get; set; }

		public AssetFileUtage(AssetFileManager assetFileManager, AssetFileInfo fileInfo, IAssetFileSettingData settingData)
			: base(assetFileManager, fileInfo, settingData)
		{
			LoadPath = ParseLoadPath();
		}

		public override bool CheckCacheOrLocal()
		{
			if (base.FileInfo.StrageType == AssetFileStrageType.Server)
			{
				return Caching.IsVersionCached(LoadPath, base.FileInfo.AssetBundleInfo.Hash);
			}
			return true;
		}

		public override IEnumerator LoadAsync(Action onComplete, Action onFailed)
		{
			base.IsLoadEnd = false;
			base.IsLoadError = false;
			yield return LoadAsyncSub(LoadPath, delegate
			{
				if (base.Priority != AssetFileLoadPriority.DownloadOnly)
				{
					base.IsLoadEnd = true;
				}
				onComplete();
			}, delegate
			{
				base.IsLoadError = true;
				onFailed();
			});
		}

		private IEnumerator LoadAsyncSub(string path, Action onComplete, Action onFailed)
		{
			AssetFileStrageType strageType = base.FileInfo.StrageType;
			if (strageType == AssetFileStrageType.Resources)
			{
				if (base.FileManager.EnableResourcesLoadAsync)
				{
					yield return LoadResourceAsync(path, onComplete, onFailed);
				}
				else
				{
					LoadResource(path, onComplete, onFailed);
				}
			}
			else
			{
				yield return LoadAssetBundleAsync(path, onComplete, onFailed);
			}
		}

		private Type GetResourceType()
		{
			switch (FileType)
			{
			case AssetFileType.Text:
				return typeof(TextAsset);
			case AssetFileType.Texture:
				return typeof(Texture2D);
			case AssetFileType.Sound:
				return typeof(AudioClip);
			default:
				return typeof(UnityEngine.Object);
			}
		}

		private void SetLoadError(string errorMsg)
		{
			base.LoadErrorMsg = errorMsg + " : load from " + LoadPath;
			base.IsLoadError = true;
		}

		private void LoadResource(string loadPath, Action onComplete, Action onFailed)
		{
			loadPath = FilePathUtil.GetPathWithoutExtension(loadPath);
			UnityEngine.Object asset = Resources.Load(loadPath, GetResourceType());
			LoadAsset(asset, onComplete, onFailed);
		}

		private IEnumerator LoadResourceAsync(string loadPath, Action onComplete, Action onFailed)
		{
			loadPath = FilePathUtil.GetPathWithoutExtension(loadPath);
			ResourceRequest request = Resources.LoadAsync(loadPath, GetResourceType());
			while (!request.isDone)
			{
				yield return null;
			}
			LoadAsset(request.asset, onComplete, onFailed);
		}

		private IEnumerator LoadAssetBundleAsync(string path, Action onComplete, Action onFailed)
		{
			WWWEx wWWEx = MakeWWWEx(path);
			wWWEx.RetryCount = base.FileManager.AutoRetryCountOnDonwloadError;
			wWWEx.TimeOut = base.FileManager.TimeOutDownload;
			AssetBundle = null;
			if (base.Priority == AssetFileLoadPriority.DownloadOnly)
			{
				yield return wWWEx.DownLoadAssetBundleAsync(onComplete, onFailed);
				yield break;
			}
			AssetBundle assetBundle = null;
			yield return wWWEx.LoadFromCacheOrDownloadAssetBundleAsync(delegate(AssetBundle x)
			{
				assetBundle = x;
			}, onFailed);
			if (assetBundle != null)
			{
				yield return LoadAssetBundleAsync(assetBundle, onComplete, onFailed);
			}
		}

		private WWWEx MakeWWWEx(string path)
		{
			if (base.FileInfo.AssetBundleInfo == null)
			{
				return new WWWEx(path);
			}
			if (base.FileInfo.AssetBundleInfo.Hash.isValid)
			{
				return new WWWEx(path, base.FileInfo.AssetBundleInfo.Hash);
			}
			return new WWWEx(path, base.FileInfo.AssetBundleInfo.Version);
		}

		private IEnumerator LoadAssetBundleAsync(AssetBundle assetBundle, Action onComplete, Action onFailed)
		{
			AssetBundleRequest request = assetBundle.LoadAllAssetsAsync(GetResourceType());
			while (!request.isDone)
			{
				yield return null;
			}
			UnityEngine.Object[] allAssets = request.allAssets;
			if (allAssets == null || allAssets.Length == 0)
			{
				SetLoadError("AssetBundleType Error");
				assetBundle.Unload(true);
				onFailed();
				yield break;
			}
			LoadAsset(allAssets[0], onComplete, onFailed);
			if (FileType == AssetFileType.UnityObject && base.FileManager.UnloadUnusedType == AssetFileManager.UnloadType.NoneAndUnloadAssetBundleTrue)
			{
				AssetBundle = assetBundle;
			}
			else
			{
				assetBundle.Unload(false);
			}
		}

		private void LoadAsset(UnityEngine.Object asset, Action onComplete, Action onFailed)
		{
			if (asset == null)
			{
				SetLoadError("LoadResource Error");
				onFailed();
				return;
			}
			switch (FileType)
			{
			case AssetFileType.Text:
				base.Text = asset as TextAsset;
				if (null == base.Text)
				{
					SetLoadError("LoadResource Error");
				}
				break;
			case AssetFileType.Texture:
				base.Texture = asset as Texture2D;
				if (null == base.Texture)
				{
					SetLoadError("LoadResource Error");
				}
				break;
			case AssetFileType.Sound:
				base.Sound = asset as AudioClip;
				if (null == base.Sound)
				{
					SetLoadError("LoadResource Error");
				}
				break;
			default:
				base.UnityObject = asset;
				if (null == base.UnityObject)
				{
					SetLoadError("LoadResource Error");
				}
				break;
			}
			if (base.IsLoadError)
			{
				onFailed();
			}
			else
			{
				onComplete();
			}
		}

		public override void Unload()
		{
			switch (FileType)
			{
			case AssetFileType.Text:
				Resources.UnloadAsset(base.Text);
				break;
			case AssetFileType.Texture:
				Resources.UnloadAsset(base.Texture);
				break;
			case AssetFileType.Sound:
				Resources.UnloadAsset(base.Sound);
				break;
			}
			base.Text = null;
			base.Texture = null;
			base.Sound = null;
			base.UnityObject = null;
			if (AssetBundle != null)
			{
				AssetBundle.Unload(true);
				AssetBundle = null;
			}
			base.IsLoadEnd = false;
			base.Priority = AssetFileLoadPriority.DownloadOnly;
		}
	}
}
