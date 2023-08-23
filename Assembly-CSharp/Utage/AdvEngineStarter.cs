using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/EngineStarter")]
	public class AdvEngineStarter : MonoBehaviour
	{
		public enum StrageType
		{
			Local,
			StreamingAssets,
			Server,
			StreamingAssetsAndLocalScenario,
			ServerAndLocalScenario,
			LocalAndServerScenario
		}

		[SerializeField]
		private bool isLoadOnAwake = true;

		[SerializeField]
		private bool isAutomaticPlay;

		[SerializeField]
		private string startScenario = "";

		[SerializeField]
		private AdvEngine engine;

		[SerializeField]
		private StrageType strageType;

		[SerializeField]
		private AdvImportScenarios scenarios;

		[SerializeField]
		private string rootResourceDir;

		[SerializeField]
		private string serverUrl;

		[SerializeField]
		private string scenariosName;

		[SerializeField]
		private bool useChapter;

		[SerializeField]
		private List<string> chapterNames = new List<string>();

		public AdvEngine Engine
		{
			get
			{
				return engine ?? (engine = UnityEngine.Object.FindObjectOfType<AdvEngine>());
			}
		}

		public StrageType Strage
		{
			get
			{
				return strageType;
			}
			set
			{
				strageType = value;
			}
		}

		public AdvImportScenarios Scenarios
		{
			get
			{
				return scenarios;
			}
			set
			{
				scenarios = value;
			}
		}

		public string RootResourceDir
		{
			get
			{
				return rootResourceDir;
			}
			set
			{
				rootResourceDir = value;
			}
		}

		public string ServerUrl
		{
			get
			{
				return serverUrl;
			}
			set
			{
				serverUrl = value;
			}
		}

		public string ScenariosName
		{
			get
			{
				return scenariosName;
			}
			set
			{
				scenariosName = value;
			}
		}

		public bool UseChapter
		{
			get
			{
				return useChapter;
			}
			set
			{
				useChapter = value;
			}
		}

		public List<string> ChapterNames
		{
			get
			{
				return chapterNames;
			}
		}

		public bool IsLoadStart { get; set; }

		public bool IsLoadErrorOnAwake { get; set; }

		private void Awake()
		{
			if (isLoadOnAwake)
			{
				StartCoroutine(LoadEngineAsync(delegate
				{
					IsLoadErrorOnAwake = true;
				}));
			}
		}

		public IEnumerator LoadEngineAsync(Action onFailed)
		{
			yield return LoadEngineAsyncSub(false, onFailed);
		}

		public IEnumerator LoadEngineAsyncFromCacheManifest(Action onFailed)
		{
			yield return LoadEngineAsyncSub(true, onFailed);
		}

		private IEnumerator LoadEngineAsyncSub(bool loadManifestFromCache, Action onFailed)
		{
			IsLoadStart = true;
			bool isFailed = false;
			if (Strage != 0)
			{
				yield return LoadAssetBundleManifestAsync(loadManifestFromCache, delegate
				{
					isFailed = true;
				});
			}
			if (isFailed)
			{
				onFailed();
			}
			else
			{
				yield return LoadEngineAsyncSub();
			}
		}

		private IEnumerator LoadEngineAsyncSub()
		{
			if (!string.IsNullOrEmpty(startScenario))
			{
				Engine.StartScenarioLabel = startScenario;
			}
			switch (Strage)
			{
			case StrageType.Local:
			case StrageType.LocalAndServerScenario:
				AssetFileManager.InitLoadTypeSetting(AssetFileManagerSettings.LoadType.Local);
				break;
			case StrageType.StreamingAssets:
			case StrageType.StreamingAssetsAndLocalScenario:
				AssetFileManager.InitLoadTypeSetting(AssetFileManagerSettings.LoadType.StreamingAssets);
				break;
			case StrageType.Server:
			case StrageType.ServerAndLocalScenario:
				AssetFileManager.InitLoadTypeSetting(AssetFileManagerSettings.LoadType.Server);
				break;
			default:
				Debug.LogError("Unkonw Strage" + Strage);
				break;
			}
			bool flag = false;
			StrageType strage = Strage;
			if (strage != 0 && (uint)(strage - 3) > 1u)
			{
				flag = true;
			}
			if (flag)
			{
				if (UseChapter)
				{
					if (Engine.SystemSaveData.IsAutoSaveOnQuit)
					{
						Debug.LogError("Check Off AdvEnigne SystemSaveData IsAutoSaveOnQuit\n「AdvEnigne SystemSaveData IsAutoSaveOnQuit」のチェックをオフにして起動してください\nチャプター機能を使う場合、追加シナリオをDLする前にシステムセーブデータを上書きされないように、アプリ終了・スリープでのオートセーブを無効にする必要があります");
						Engine.SystemSaveData.IsAutoSaveOnQuit = false;
					}
					yield return LoadChaptersAsync(GetDynamicStrageRoot());
				}
				else
				{
					yield return LoadScenariosAsync(GetDynamicStrageRoot());
				}
			}
			if (Scenarios == null)
			{
				Debug.LogError("Scenarios is Blank. Please set .scenarios Asset", this);
				yield break;
			}
			strage = Strage;
			if (strage == StrageType.Local || strage == StrageType.LocalAndServerScenario)
			{
				Engine.BootFromExportData(Scenarios, RootResourceDir);

            }
            else
			{
				Engine.BootFromExportData(Scenarios, GetDynamicStrageRoot());
			}
			if (isAutomaticPlay)
			{
				StartEngine();
			}
		}

		private string GetDynamicStrageRoot()
		{
			switch (Strage)
			{
			case StrageType.Server:
			case StrageType.ServerAndLocalScenario:
			case StrageType.LocalAndServerScenario:
				return FilePathUtil.Combine(ServerUrl, AssetBundleHelper.RuntimeAssetBundleTarget().ToString());
			case StrageType.StreamingAssets:
			case StrageType.StreamingAssetsAndLocalScenario:
				return FilePathUtil.ToStreamingAssetsPath(FilePathUtil.Combine(RootResourceDir, AssetBundleHelper.RuntimeAssetBundleTarget().ToString()));
			default:
				Debug.LogError("UnDefine");
				return "";
			}
		}

		private IEnumerator LoadAssetBundleManifestAsync(bool fromCache, Action onFailed)
		{
			if (Strage == StrageType.Local)
			{
				yield break;
			}
			if (fromCache)
			{
				yield return AssetFileManager.GetInstance().AssetBundleInfoManager.LoadCacheManifestAsync(GetDynamicStrageRoot(), AssetBundleHelper.RuntimeAssetBundleTarget().ToString(), delegate
				{
				}, delegate
				{
					onFailed();
				});
			}
			else
			{
				yield return AssetFileManager.GetInstance().AssetBundleInfoManager.DownloadManifestAsync(GetDynamicStrageRoot(), AssetBundleHelper.RuntimeAssetBundleTarget().ToString(), delegate
				{
				}, delegate
				{
					onFailed();
				});
			}
		}

		private IEnumerator LoadScenariosAsync(string rootDir)
		{
			string url = ToScenariosFilePath(rootDir);
			AssetFile file = AssetFileManager.Load(url, this);
			while (!file.IsLoadEnd)
			{
				yield return null;
			}
			AdvImportScenarios advImportScenarios = file.UnityObject as AdvImportScenarios;
			if (advImportScenarios == null)
			{
				Debug.LogError(url + " is  not scenario file");
			}
			else
			{
				Scenarios = advImportScenarios;
			}
		}

		private string ToScenariosFilePath(string rootDir)
		{
			string text = ScenariosName;
			if (string.IsNullOrEmpty(text))
			{
				text = RootResourceDir + ".scenarios.asset";
			}
			return FilePathUtil.Combine(rootDir, text);
		}

		private IEnumerator LoadChaptersAsync(string rootDir)
		{
			AdvImportScenarios scenarios = ScriptableObject.CreateInstance<AdvImportScenarios>();
			foreach (string chapterName in ChapterNames)
			{
				string url = FilePathUtil.Combine(rootDir, chapterName) + ".chapter.asset";
				AssetFile file = AssetFileManager.Load(url, this);
				while (!file.IsLoadEnd)
				{
					yield return null;
				}
				AdvChapterData chapterData = file.UnityObject as AdvChapterData;
				if (scenarios == null)
				{
					Debug.LogError(url + " is  not scenario file");
					yield break;
				}
				scenarios.AddChapter(chapterData);
			}
			Scenarios = scenarios;
		}

		public void StartEngine()
		{
			StartCoroutine(CoPlayEngine());
		}

		private IEnumerator CoPlayEngine()
		{
			while (Engine.IsWaitBootLoading)
			{
				yield return null;
			}
			if (string.IsNullOrEmpty(startScenario))
			{
				Engine.StartGame();
			}
			else
			{
				Engine.StartGame(startScenario);
			}
		}
	}
}
