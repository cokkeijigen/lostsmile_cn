using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/File/AssetFileManager")]
	[RequireComponent(typeof(StaticAssetManager))]
	public class AssetFileManager : MonoBehaviour
	{
		internal enum UnloadType
		{
			None,
			UnloadUnusedAsset,
			UnloadUnusedAssetAlways,
			NoneAndUnloadAssetBundleTrue
		}

		[SerializeField]
		[FormerlySerializedAs("fileIOManger")]
		private FileIOManager fileIOManager;

		[SerializeField]
		private bool enableResourcesLoadAsync = true;

		[SerializeField]
		private float timeOutDownload = 10f;

		[SerializeField]
		private int autoRetryCountOnDonwloadError = 5;

		[SerializeField]
		private int loadFileMax = 5;

		[SerializeField]
		[MinMax(0f, 100f, "min", "max")]
		private MinMaxInt rangeOfFilesOnMemory = new MinMaxInt
		{
			Min = 10,
			Max = 20
		};

		[SerializeField]
		private UnloadType unloadType = UnloadType.UnloadUnusedAsset;

		[SerializeField]
		internal bool isOutPutDebugLog;

		[SerializeField]
		internal bool isDebugCacheFileName;

		[SerializeField]
		internal bool isDebugBootDeleteChacheTextAndBinary;

		[SerializeField]
		internal bool isDebugBootDeleteChacheAll;

		[SerializeField]
		private AssetFileManagerSettings settings;

		[SerializeField]
		private AssetBundleInfoManager assetBundleInfoManager;

		[SerializeField]
		private AssetFileDummyOnLoadError dummyFiles = new AssetFileDummyOnLoadError();

		private List<AssetFileBase> loadingFileList = new List<AssetFileBase>();

		private List<AssetFileBase> loadWaitFileList = new List<AssetFileBase>();

		private List<AssetFileBase> usingFileList = new List<AssetFileBase>();

		private Dictionary<string, AssetFileBase> fileTbl = new Dictionary<string, AssetFileBase>();

		private CustomLoadManager customLoadManager;

		private StaticAssetManager staticAssetManager;

		private Action<AssetFile> callbackError;

		private bool isWaitingRetry;

		private bool unloadingUnusedAssets;

		private static bool isEditorErrorCheck;

		private static AssetFileManager instance;

		public FileIOManager FileIOManager
		{
			get
			{
				return this.GetComponentCache(ref fileIOManager);
			}
			set
			{
				fileIOManager = value;
			}
		}

		public bool EnableResourcesLoadAsync
		{
			get
			{
				return enableResourcesLoadAsync;
			}
			set
			{
				enableResourcesLoadAsync = value;
			}
		}

		public float TimeOutDownload
		{
			get
			{
				return timeOutDownload;
			}
			set
			{
				timeOutDownload = value;
			}
		}

		public int AutoRetryCountOnDonwloadError
		{
			get
			{
				return autoRetryCountOnDonwloadError;
			}
			set
			{
				autoRetryCountOnDonwloadError = value;
			}
		}

		public int MaxFilesOnMemory => rangeOfFilesOnMemory.Max;

		public int MinFilesOnMemory => rangeOfFilesOnMemory.Min;

		internal UnloadType UnloadUnusedType => unloadType;

		public AssetFileManagerSettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = value;
			}
		}

		public AssetBundleInfoManager AssetBundleInfoManager
		{
			get
			{
				return this.GetComponentCacheCreateIfMissing(ref assetBundleInfoManager);
			}
			set
			{
				assetBundleInfoManager = value;
			}
		}

		private CustomLoadManager CustomLoadManager => this.GetComponentCacheCreateIfMissing(ref customLoadManager);

		private StaticAssetManager StaticAssetManager => this.GetComponentCacheCreateIfMissing(ref staticAssetManager);

		public Action<AssetFile> CallbackError
		{
			get
			{
				return CallbackFileLoadError;
			}
			set
			{
				callbackError = value;
			}
		}

		public static bool IsEditorErrorCheck
		{
			get
			{
				return isEditorErrorCheck;
			}
			set
			{
				isEditorErrorCheck = value;
			}
		}

		private void CallbackFileLoadError(AssetFile file)
		{
			AssetFileBase errorFile = file as AssetFileBase;
			string text = file.LoadErrorMsg + "\n" + file.FileName;
			Debug.LogError(text);
			if (SystemUi.GetInstance() != null)
			{
				if (isWaitingRetry)
				{
					StartCoroutine(CoWaitRetry(errorFile));
					return;
				}
				isWaitingRetry = true;
				SystemUi.GetInstance().OpenDialog1Button(text, LanguageSystemText.LocalizeText(SystemText.Retry), delegate
				{
					isWaitingRetry = false;
					ReloadFileSub(errorFile);
				});
			}
			else
			{
				ReloadFileSub(errorFile);
			}
		}

		private IEnumerator CoWaitRetry(AssetFileBase file)
		{
			while (isWaitingRetry)
			{
				yield return null;
			}
			ReloadFileSub(file);
		}

		private void Awake()
		{
			if (null == instance)
			{
				instance = this;
			}
		}

		private AssetFileBase AddSub(string path, IAssetFileSettingData settingData)
		{
			if (!fileTbl.TryGetValue(path, out var value))
			{
				if (path.Contains(" "))
				{
					Debug.LogWarning("[" + path + "] contains white space");
				}
				AssetBundleInfo assetBundleInfo = AssetBundleInfoManager.FindAssetBundleInfo(path);
				AssetFileInfo fileInfo = new AssetFileInfo(path, settings, assetBundleInfo);
				value = StaticAssetManager.FindAssetFile(this, fileInfo, settingData);
				if (value == null)
				{
					value = CustomLoadManager.Find(this, fileInfo, settingData);
					if (value == null)
					{
						value = new AssetFileUtage(this, fileInfo, settingData);
					}
				}
				fileTbl.Add(path, value);
			}
			return value;
		}

		private void DownloadSub(AssetFileBase file)
		{
			if (!file.CheckCacheOrLocal())
			{
				file.ReadyToLoad(AssetFileLoadPriority.DownloadOnly, null);
				AddLoadFile(file);
			}
		}

		private void PreloadSub(AssetFileBase file, object referenceObj)
		{
			AddUseList(file);
			file.ReadyToLoad(AssetFileLoadPriority.Preload, referenceObj);
			AddLoadFile(file);
		}

		private AssetFile BackGroundLoadSub(AssetFileBase file, object referenceObj)
		{
			AddUseList(file);
			file.ReadyToLoad(AssetFileLoadPriority.BackGround, referenceObj);
			AddLoadFile(file);
			return file;
		}

		private AssetFile LoadSub(AssetFileBase file, object referenceObj)
		{
			AddUseList(file);
			file.ReadyToLoad(AssetFileLoadPriority.Default, referenceObj);
			AddLoadFile(file);
			return file;
		}

		private void AddUseList(AssetFileBase file)
		{
			if (!usingFileList.Contains(file))
			{
				usingFileList.Add(file);
			}
		}

		private void LoadSub(AssetFileBase file, Action<AssetFile> onComplete)
		{
			StartCoroutine(CoLoadWait(file, onComplete));
		}

		private IEnumerator CoLoadWait(AssetFileBase file, Action<AssetFile> onComplete)
		{
			if (file.IsLoadEnd)
			{
				onComplete(file);
				yield break;
			}
			LoadSub(file, this);
			while (!file.IsLoadEnd)
			{
				yield return null;
			}
			onComplete(file);
		}

		private void AddLoadFile(AssetFileBase file)
		{
			TryAddLoadingFileList(file);
		}

		private bool TryAddLoadingFileList(AssetFileBase file)
		{
			if (file.IsLoadEnd)
			{
				return false;
			}
			if (loadingFileList.Contains(file))
			{
				return false;
			}
			if (loadingFileList.Count < loadFileMax)
			{
				loadingFileList.Add(file);
				if (isOutPutDebugLog)
				{
					Debug.Log("Load Start :" + file.FileName);
				}
				StartCoroutine(LoadAsync(file));
				return true;
			}
			if (!loadWaitFileList.Contains(file))
			{
				loadWaitFileList.Add(file);
				return false;
			}
			return false;
		}

		private IEnumerator LoadAsync(AssetFileBase file)
		{
			yield return file.LoadAsync(delegate
			{
				if (isOutPutDebugLog)
				{
					Debug.Log("Load End :" + file.FileName);
				}
				loadingFileList.Remove(file);
				LoadNextFile();
			}, delegate
			{
				if (dummyFiles.isEnable)
				{
					if (dummyFiles.outputErrorLog)
					{
						Debug.LogError("Load Failed. Dummy file loaded:" + file.FileName + "\n" + file.LoadErrorMsg);
					}
					file.LoadDummy(dummyFiles);
					loadingFileList.Remove(file);
					LoadNextFile();
				}
				else
				{
					Debug.LogError("Load Failed :" + file.FileName + "\n" + file.LoadErrorMsg);
					if (CallbackError != null)
					{
						CallbackError(file);
					}
				}
			});
		}

		private void ReloadFileSub(AssetFileBase file)
		{
			StartCoroutine(ReloadFileSubAsync(file));
		}

		private IEnumerator ReloadFileSubAsync(AssetFileBase file)
		{
			yield return null;
			yield return StartCoroutine(LoadAsync(file));
		}

		private void LoadNextFile()
		{
			AssetFileBase assetFileBase = null;
			foreach (AssetFileBase loadWaitFile in loadWaitFileList)
			{
				if (assetFileBase == null)
				{
					assetFileBase = loadWaitFile;
				}
				else if (loadWaitFile.Priority < assetFileBase.Priority)
				{
					assetFileBase = loadWaitFile;
				}
			}
			if (assetFileBase != null)
			{
				if (assetFileBase.IsLoadEnd)
				{
					loadWaitFileList.Remove(assetFileBase);
				}
				else if (TryAddLoadingFileList(assetFileBase))
				{
					loadWaitFileList.Remove(assetFileBase);
				}
				else
				{
					Debug.LogError("Failed To Load file " + assetFileBase.FileName);
				}
			}
		}

		private void LateUpdate()
		{
			int totalOnMemoryFileCount = GetTotalOnMemoryFileCount();
			if (totalOnMemoryFileCount > MaxFilesOnMemory)
			{
				UnloadUnusedFileList(totalOnMemoryFileCount - MinFilesOnMemory);
			}
		}

		private void OnDestroy()
		{
			UnloadUnusedFileList(int.MaxValue);
		}

		private int GetTotalOnMemoryFileCount()
		{
			int num = loadingFileList.Count;
			foreach (AssetFileBase usingFile in usingFileList)
			{
				if (!usingFile.IgnoreUnload && usingFile.IsLoadEnd)
				{
					num++;
				}
			}
			return num;
		}

		private void UnloadUnusedFileList(int count)
		{
			if (usingFileList.Count <= 0 || count <= 0)
			{
				return;
			}
			int num = 0;
			List<AssetFileBase> list = new List<AssetFileBase>();
			foreach (AssetFileBase usingFile in usingFileList)
			{
				if (count <= 0 || usingFile.IgnoreUnload || !usingFile.IsLoadEnd || usingFile.ReferenceCount > 0)
				{
					list.Add(usingFile);
					continue;
				}
				if (isOutPutDebugLog)
				{
					Debug.Log("Unload " + usingFile.FileName);
				}
				usingFile.Unload();
				count--;
				if (usingFile.FileType == AssetFileType.UnityObject)
				{
					num++;
				}
			}
			UnloadUnusedAssets(num);
			usingFileList = list;
		}

		private void UnloadUnusedAssets(int count)
		{
			switch (unloadType)
			{
			default:
				return;
			case UnloadType.UnloadUnusedAsset:
				if (count <= 0)
				{
					return;
				}
				break;
			case UnloadType.None:
			case UnloadType.NoneAndUnloadAssetBundleTrue:
				return;
			case UnloadType.UnloadUnusedAssetAlways:
				break;
			}
			if (!unloadingUnusedAssets && base.gameObject.activeInHierarchy)
			{
				StartCoroutine(UnloadUnusedAssetsAsync());
			}
		}

		private IEnumerator UnloadUnusedAssetsAsync()
		{
			if (isOutPutDebugLog)
			{
				Debug.Log("UnloadUnusedAssets");
			}
			unloadingUnusedAssets = true;
			yield return Resources.UnloadUnusedAssets();
			unloadingUnusedAssets = false;
		}

		private bool IsLoadEnd(AssetFileLoadPriority priority)
		{
			foreach (AssetFileBase loadingFile in loadingFileList)
			{
				if (loadingFile.Priority <= priority && !loadingFile.IsLoadEnd)
				{
					return false;
				}
			}
			foreach (AssetFileBase loadWaitFile in loadWaitFileList)
			{
				if (loadWaitFile.Priority <= priority && !loadWaitFile.IsLoadEnd)
				{
					return false;
				}
			}
			return true;
		}

		private int CountLoading(AssetFileLoadPriority priority)
		{
			int num = 0;
			foreach (AssetFileBase loadingFile in loadingFileList)
			{
				if (loadingFile.Priority <= priority && !loadingFile.IsLoadEnd)
				{
					num++;
				}
			}
			foreach (AssetFileBase loadWaitFile in loadWaitFileList)
			{
				if (loadWaitFile.Priority <= priority && !loadWaitFile.IsLoadEnd)
				{
					num++;
				}
			}
			return num;
		}

		internal static bool IsInitialized()
		{
			return true;
		}

		public static void InitLoadTypeSetting(AssetFileManagerSettings.LoadType loadTypeSetting)
		{
			GetInstance().Settings.BootInit(loadTypeSetting);
		}

		public static void InitError(Action<AssetFile> callbackError)
		{
			GetInstance().CallbackError = callbackError;
		}

		public static AssetFile GetFileCreateIfMissing(string path, IAssetFileSettingData settingData = null)
		{
			if (!IsEditorErrorCheck)
			{
				return GetInstance().AddSub(path, settingData);
			}
			if (path.Contains(" "))
			{
				Debug.LogWarning("[" + path + "] contains white space");
			}
			return null;
		}

		public static AssetFile Load(string path, object referenceObj)
		{
			return Load(GetFileCreateIfMissing(path), referenceObj);
		}

		public static AssetFile Load(AssetFile file, object referenceObj)
		{
			return GetInstance().LoadSub(file as AssetFileBase, referenceObj);
		}

		public static void Load(AssetFile file, Action<AssetFile> onComplete)
		{
			GetInstance().LoadSub(file as AssetFileBase, onComplete);
		}

		public static void Preload(string path, object referenceObj)
		{
			Preload(GetFileCreateIfMissing(path), referenceObj);
		}

		public static void Preload(AssetFile file, object referenceObj)
		{
			GetInstance().PreloadSub(file as AssetFileBase, referenceObj);
		}

		public static AssetFile BackGroundLoad(string path, object referenceObj)
		{
			return BackGroundLoad(GetFileCreateIfMissing(path), referenceObj);
		}

		public static AssetFile BackGroundLoad(AssetFile file, object referenceObj)
		{
			return GetInstance().BackGroundLoadSub(file as AssetFileBase, referenceObj);
		}

		public static void Download(string path)
		{
			Download(GetFileCreateIfMissing(path));
		}

		public static void Download(AssetFile file)
		{
			GetInstance().DownloadSub(file as AssetFileBase);
		}

		public static bool IsLoadEnd()
		{
			return GetInstance().IsLoadEnd(AssetFileLoadPriority.Preload);
		}

		public static bool IsDownloadEnd()
		{
			return GetInstance().IsLoadEnd(AssetFileLoadPriority.DownloadOnly);
		}

		public static int CountLoading()
		{
			return GetInstance().CountLoading(AssetFileLoadPriority.Preload);
		}

		public static int CountDownloading()
		{
			return GetInstance().CountLoading(AssetFileLoadPriority.DownloadOnly);
		}

		public static void UnloadUnusedAll()
		{
			GetInstance().UnloadUnusedAssets(int.MaxValue);
		}

		public static void UnloadUnused(int count)
		{
			GetInstance().UnloadUnusedAssets(count);
		}

		public static void AddAssetFileTypeExtensions(AssetFileType type, string[] extensions)
		{
			GetInstance().Settings.AddExtensions(type, extensions);
		}

		public static bool ContainsStaticAsset(UnityEngine.Object asset)
		{
			return GetInstance().StaticAssetManager.Contains(asset);
		}

		public static CustomLoadManager GetCustomLoadManager()
		{
			return GetInstance().CustomLoadManager;
		}

		public static void SetLoadErrorCallBack(Action<AssetFile> callbackError)
		{
			GetInstance().callbackError = callbackError;
		}

		public static void ReloadFile(AssetFile file)
		{
			GetInstance().ReloadFileSub(file as AssetFileBase);
		}

		public static AssetFileManager GetInstance()
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType<AssetFileManager>();
				if (instance == null)
				{
					Debug.LogError("Not Found AssetFileManager in current scene");
				}
			}
			return instance;
		}
	}
}
