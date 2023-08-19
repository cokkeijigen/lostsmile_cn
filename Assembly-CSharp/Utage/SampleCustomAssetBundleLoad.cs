using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Sample/CustomAssetBundleLoad")]
	public class SampleCustomAssetBundleLoad : MonoBehaviour
	{
		[Serializable]
		public class SampleAssetBundleVersionInfo
		{
			public string resourcePath;

			public string url;

			public int version;

			public int size;
		}

		[SerializeField]
		private string startScenario = "";

		[SerializeField]
		private AdvEngine engine;

		private List<SampleAssetBundleVersionInfo> assetBundleList = new List<SampleAssetBundleVersionInfo>
		{
			new SampleAssetBundleVersionInfo
			{
				resourcePath = "Sample.scenarios.asset",
				url = "http://madnesslabo.net/Utage3CustomLoad/Windows/sample.scenarios.asset",
				version = 0,
				size = 128
			},
			new SampleAssetBundleVersionInfo
			{
				resourcePath = "Texture/Character/Utako/utako.png",
				url = "http://madnesslabo.net/Utage3CustomLoad/Windows/texture/character/utako/utako.asset",
				version = 0,
				size = 256
			},
			new SampleAssetBundleVersionInfo
			{
				resourcePath = "Texture/BG/TutorialBg1.png",
				url = "http://madnesslabo.net/Utage3Download/Sample/Windows/texture/bg/tutorialbg1.asset",
				version = 0,
				size = 512
			},
			new SampleAssetBundleVersionInfo
			{
				resourcePath = "Sound/BGM/MainTheme.wav",
				url = "http://madnesslabo.net/Utage3Download/Sample/Windows/sound/bgm/maintheme.asset",
				version = 0,
				size = 1024
			}
		};

		public AdvEngine Engine
		{
			get
			{
				return engine ?? (engine = UnityEngine.Object.FindObjectOfType<AdvEngine>());
			}
		}

		private AdvImportScenarios Scenarios { get; set; }

		private void Awake()
		{
			StartCoroutine(LoadEngineAsync());
		}

		private IEnumerator LoadEngineAsync()
		{
			foreach (SampleAssetBundleVersionInfo assetBundle in assetBundleList)
			{
				AssetFileManager.GetInstance().AssetBundleInfoManager.AddAssetBundleInfo(assetBundle.resourcePath, assetBundle.url, assetBundle.version, assetBundle.size);
			}
			if (!string.IsNullOrEmpty(startScenario))
			{
				Engine.StartScenarioLabel = startScenario;
			}
			AssetFileManager.InitLoadTypeSetting(AssetFileManagerSettings.LoadType.Server);
			yield return LoadScenariosAsync("Sample.scenarios.asset");
			if (Scenarios == null)
			{
				Debug.LogError("Scenarios is Blank. Please set .scenarios Asset", this);
				yield break;
			}
			Engine.BootFromExportData(Scenarios, "");
			StartEngine();
		}

		private IEnumerator LoadScenariosAsync(string url)
		{
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

		private void StartEngine()
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
