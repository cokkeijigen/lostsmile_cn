using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/File/AssetBundleInfoManager")]
	public class AssetBundleInfoManager : MonoBehaviour
	{
		[SerializeField]
		private int retryCount = 5;

		[SerializeField]
		private float timeOut = 5f;

		[SerializeField]
		private bool useCacheManifest = true;

		[SerializeField]
		private string cacheDirectoryName = "Cache";

		[SerializeField]
		private AssetFileManager assetFileManager;

		private Dictionary<string, AssetBundleInfo> dictionary = new Dictionary<string, AssetBundleInfo>(StringComparer.OrdinalIgnoreCase);

		private const string AssetBundleManifestName = "assetbundlemanifest";

		public int RetryCount
		{
			get
			{
				return retryCount;
			}
			set
			{
				retryCount = value;
			}
		}

		public int TimeOut
		{
			get
			{
				return retryCount;
			}
			set
			{
				retryCount = value;
			}
		}

		public bool UseCacheManifest
		{
			get
			{
				return useCacheManifest;
			}
			set
			{
				useCacheManifest = value;
			}
		}

		public string CacheDirectoryName
		{
			get
			{
				return cacheDirectoryName;
			}
			set
			{
				cacheDirectoryName = value;
			}
		}

		private AssetFileManager AssetFileManager
		{
			get
			{
				return this.GetComponentCache(ref assetFileManager);
			}
		}

		private FileIOManager FileIOManager
		{
			get
			{
				return AssetFileManager.FileIOManager;
			}
		}

		public AssetBundleInfo FindAssetBundleInfo(string path)
		{
			AssetBundleInfo value;
			if (!dictionary.TryGetValue(path, out value))
			{
				string key = FilePathUtil.ChangeExtension(path, ".asset");
				if (!dictionary.TryGetValue(key, out value))
				{
					return null;
				}
			}
			return value;
		}

		public void AddAssetBundleInfo(string resourcePath, string assetBunleUrl, int assetBunleVersion, int assetBunleSize = 0)
		{
			try
			{
				dictionary.Add(resourcePath, new AssetBundleInfo(assetBunleUrl, assetBunleVersion, assetBunleSize));
			}
			catch
			{
				Debug.LogError(resourcePath + "is already contains in assetbundleManger");
			}
		}

		public void AddAssetBundleInfo(string resourcePath, string assetBunleUrl, Hash128 assetBunleHash, int assetBunleSize = 0)
		{
			try
			{
				dictionary.Add(resourcePath, new AssetBundleInfo(assetBunleUrl, assetBunleHash, assetBunleSize));
			}
			catch
			{
				Debug.LogError(resourcePath + "is already contains in assetbundleManger");
			}
		}

		public void AddAssetBundleManifest(string rootUrl, AssetBundleManifest manifest)
		{
			string[] allAssetBundles = manifest.GetAllAssetBundles();
			foreach (string text in allAssetBundles)
			{
				Hash128 assetBundleHash = manifest.GetAssetBundleHash(text);
				string text2 = FilePathUtil.Combine(rootUrl, text);
				try
				{
					dictionary.Add(text2, new AssetBundleInfo(text2, assetBundleHash));
				}
				catch
				{
					Debug.LogError(text2 + "is already contains in assetbundleManger");
				}
			}
		}

		public IEnumerator DownloadManifestAsync(string rootUrl, string relativeUrl, Action onComplete, Action onFailed)
		{
			WWWEx wWWEx = new WWWEx(FilePathUtil.ToCacheClearUrl(FilePathUtil.Combine(rootUrl, relativeUrl)));
			if (UseCacheManifest)
			{
				wWWEx.IoManager = FileIOManager;
				wWWEx.WriteLocal = true;
				wWWEx.WritePath = GetCachePath(relativeUrl);
			}
			wWWEx.OnUpdate = OnDownloadingManifest;
			wWWEx.RetryCount = retryCount;
			wWWEx.TimeOut = timeOut;
			return wWWEx.LoadAssetBundleByNameAsync("assetbundlemanifest", false, delegate(AssetBundleManifest manifest)
			{
				AddAssetBundleManifest(rootUrl, manifest);
				if (onComplete != null)
				{
					onComplete();
				}
			}, delegate
			{
				if (onFailed != null)
				{
					onFailed();
				}
			});
		}

		private void OnDownloadingManifest(WWWEx wwwEx)
		{
		}

		public IEnumerator LoadCacheManifestAsync(string rootUrl, string relativeUrl, Action onComplete, Action onFailed)
		{
			return new WWWEx(FilePathUtil.AddFileProtocol(GetCachePath(relativeUrl)))
			{
				OnUpdate = OnDownloadingManifest,
				RetryCount = 0,
				TimeOut = 0.1f
			}.LoadAssetBundleByNameAsync("assetbundlemanifest", false, delegate(AssetBundleManifest manifest)
			{
				AddAssetBundleManifest(rootUrl, manifest);
				if (onComplete != null)
				{
					onComplete();
				}
			}, delegate
			{
				if (onFailed != null)
				{
					onFailed();
				}
			});
		}

		private string GetCachePath(string relativeUrl)
		{
			return FilePathUtil.Combine(FileIOManagerBase.SdkTemporaryCachePath, cacheDirectoryName, relativeUrl);
		}

		public void DeleteAllCache()
		{
			FileIOManager.DeleteDirectory(FilePathUtil.Combine(FileIOManagerBase.SdkTemporaryCachePath, cacheDirectoryName) + "/");
			WrapperUnityVersion.CleanCache();
		}
	}
}
