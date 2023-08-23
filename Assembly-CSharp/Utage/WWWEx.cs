using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Utage
{
	public class WWWEx
	{
		public enum Type
		{
			Default,
			Cache
		}

		public string Url { get; private set; }

		public Hash128 AssetBundleHash { get; private set; }

		public int AssetBundleVersion { get; private set; }

		public Type LoadType { get; private set; }

		public int RetryCount { get; set; }

		public float TimeOut { get; set; }

		public float Progress { get; private set; }

		public Action<WWWEx> OnUpdate { get; set; }

		public bool IgnoreDebugLog { get; set; }

		public bool WriteLocal { get; set; }

		public string WritePath { get; set; }

		public FileIOManager IoManager { get; set; }

		public WWWEx(string url)
		{
			LoadType = Type.Default;
			InitSub(url);
		}

		public WWWEx(string url, Hash128 assetBundleHash)
		{
			AssetBundleHash = assetBundleHash;
			LoadType = Type.Cache;
			InitSub(url);
		}

		public WWWEx(string url, int assetBundleVersion)
		{
            AssetBundleVersion = assetBundleVersion;
			LoadType = Type.Cache;
			InitSub(url);
		}

		private void InitSub(string url)
		{

            Url = url;
			RetryCount = 5;
			TimeOut = 5f;
			Progress = 0f;
		}

		public IEnumerator DownLoadAssetBundleAsync(Action onComplete, Action onFailed)
		{
			yield return LoadAsync(delegate
			{
				onComplete();
			}, delegate
			{
				onFailed();
			});
		}

		public IEnumerator LoadFromCacheOrDownloadAssetBundleAsync(Action<AssetBundle> onComplete, Action onFailed)
		{
			yield return LoadAssetBundleAsync(delegate(UnityWebRequest www, AssetBundle assetBundle)
			{
				onComplete(assetBundle);
			}, delegate
			{
				onFailed();
			});
		}

		private IEnumerator LoadAsync(Action<UnityWebRequest> onComplete, Action<UnityWebRequest> onFailed = null)
		{
			return LoadAsync(delegate(UnityWebRequest www)
			{
				onComplete(www);
			}, delegate(UnityWebRequest www)
			{
				if (!IgnoreDebugLog)
				{
					Debug.LogError("WWW load error " + www.url + "\n" + www.error);
				}
				if (onFailed != null)
				{
					onFailed(www);
				}
			}, delegate(UnityWebRequest www)
			{
				if (!IgnoreDebugLog)
				{
					Debug.LogError("WWW timeout " + www.url);
				}
				if (onFailed != null)
				{
					onFailed(www);
				}
			});
		}

		private IEnumerator LoadAsync(Action<UnityWebRequest> onComplete, Action<UnityWebRequest> onFailed, Action<UnityWebRequest> onTimeOut)
		{
			return LoadAsyncSub(onComplete, onFailed, onTimeOut, RetryCount);
		}

		private IEnumerator LoadAsyncSub(Action<UnityWebRequest> onComplete, Action<UnityWebRequest> onFailed, Action<UnityWebRequest> onTimeOut, int retryCount)
		{
			if (LoadType == Type.Cache)
			{
				while (!Caching.ready)
				{
					yield return null;
				}
			}
			bool retry = false;
			using (UnityWebRequest uwr = CreateWebRequest())
			{
				if (0f < TimeOut)
				{
					uwr.timeout = (int)TimeOut;
				}
				UnityWebRequestAsyncOperation request = uwr.SendWebRequest();
				float time = 0f;
				bool isTimeOut = false;
				Progress = 0f;
				while (!request.isDone && !isTimeOut)
				{
					if (0f < TimeOut)
					{
						if (Progress == request.progress)
						{
							time += Time.deltaTime;
							if (time >= TimeOut)
							{
								isTimeOut = true;
							}
						}
						else
						{
							time = 0f;
						}
					}
					Progress = request.progress;
					if (OnUpdate != null)
					{
						OnUpdate(this);
					}
					yield return null;
				}
				if (isTimeOut || uwr.error == "Request timeout")
				{
					if (retryCount <= 0)
					{
						if (onTimeOut != null)
						{
							onTimeOut(uwr);
						}
					}
					else
					{
						retry = true;
					}
				}
				else if (uwr.isNetworkError || uwr.isHttpError)
				{
					if (retryCount <= 0)
					{
						if (onFailed != null)
						{
							onFailed(uwr);
						}
					}
					else
					{
						retry = true;
					}
				}
				else
				{
					Progress = request.progress;
					if (OnUpdate != null)
					{
						OnUpdate(this);
					}
					if (onComplete != null)
					{
						onComplete(uwr);
					}
				}
			}
			if (retry)
			{
				yield return LoadAsyncSub(onComplete, onFailed, onTimeOut, retryCount - 1);
			}
		}

		private UnityWebRequest CreateWebRequest()
		{
			Type loadType = LoadType;
			if (loadType == Type.Cache)
			{
				if (AssetBundleHash.isValid)
				{
					return UnityWebRequestAssetBundle.GetAssetBundle(Url, AssetBundleHash);
				}
				return UnityWebRequestAssetBundle.GetAssetBundle(Url, (uint)AssetBundleVersion);
			}
			if (WriteLocal)
			{
				return UnityWebRequest.Get(Url);
			}
			return UnityWebRequestAssetBundle.GetAssetBundle(Url);
		}

		private IEnumerator LoadAssetBundleAsync(Action<UnityWebRequest, AssetBundle> onComplete, Action<UnityWebRequest> onFailed)
		{
			return LoadAsync(delegate(UnityWebRequest www)
			{
				AssetBundle assetBundle = null;
				if (WriteLocal)
				{
					IoManager.CreateDirectory(FilePathUtil.GetDirectoryPath(WritePath) + "/");
					IoManager.Write(WritePath, www.downloadHandler.data);
					assetBundle = AssetBundle.LoadFromFile(WritePath);
				}
				else
				{
					assetBundle = DownloadHandlerAssetBundle.GetContent(www);
				}
				if (assetBundle != null)
				{
					if (onComplete != null)
					{
						onComplete(www, assetBundle);
					}
				}
				else
				{
					if (!IgnoreDebugLog)
					{
						Debug.LogError(www.url + " is not assetBundle");
					}
					if (onFailed != null)
					{
						onFailed(www);
					}
				}
			}, delegate(UnityWebRequest www)
			{
				if (onFailed != null)
				{
					onFailed(www);
				}
			});
		}

		public IEnumerator LoadAssetBundleByNameAsync<T>(string assetName, bool unloadAllLoadedObjects, Action<T> onComplete, Action onFailed) where T : UnityEngine.Object
		{
			AssetBundle assetBundle = null;
			yield return LoadAssetBundleAsync(delegate(UnityWebRequest _www, AssetBundle _assetBundle)
			{
				assetBundle = _assetBundle;
			}, delegate
			{
				if (onFailed != null)
				{
					onFailed();
				}
			});
			if (assetBundle == null)
			{
				yield break;
			}
			AssetBundleRequest request = assetBundle.LoadAssetAsync<T>(assetName);
			while (!request.isDone)
			{
				yield return null;
			}
			T val = request.asset as T;
			if ((UnityEngine.Object)val == (UnityEngine.Object)null)
			{
				if (!IgnoreDebugLog)
				{
					Debug.LogError(Url + "  " + assetName + " is not AssetBundle of " + typeof(T).Name);
				}
				if (onFailed != null)
				{
					onFailed();
				}
			}
			else if (onComplete != null)
			{
				onComplete(val);
			}
			assetBundle.Unload(unloadAllLoadedObjects);
		}

		public IEnumerator LoadAssetBundleAllAsync<T>(bool unloadAllLoadedObjects, Action<T[]> onComplete, Action onFailed) where T : UnityEngine.Object
		{
			AssetBundle assetBundle = null;
			yield return LoadAssetBundleAsync(delegate(UnityWebRequest _www, AssetBundle _assetBundle)
			{
				assetBundle = _assetBundle;
			}, delegate
			{
				if (onFailed != null)
				{
					onFailed();
				}
			});
			if (assetBundle == null)
			{
				yield break;
			}
			AssetBundleRequest request = assetBundle.LoadAllAssetsAsync<T>();
			while (!request.isDone)
			{
				yield return null;
			}
			T[] array = request.allAssets as T[];
			if (array == null || array.Length == 0)
			{
				if (!IgnoreDebugLog)
				{
					Debug.LogError(Url + "   is not AssetBundle of " + typeof(T).Name);
				}
				if (onFailed != null)
				{
					onFailed();
				}
			}
			else if (onComplete != null)
			{
				onComplete(array);
			}
			assetBundle.Unload(unloadAllLoadedObjects);
		}
	}
}
