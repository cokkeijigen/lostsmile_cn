using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/MainEngine")]
	[RequireComponent(typeof(DontDestoryOnLoad))]
	[RequireComponent(typeof(AdvDataManager))]
	[RequireComponent(typeof(AdvScenarioPlayer))]
	[RequireComponent(typeof(AdvPage))]
	[RequireComponent(typeof(AdvMessageWindowManager))]
	[RequireComponent(typeof(AdvSelectionManager))]
	[RequireComponent(typeof(AdvBacklogManager))]
	[RequireComponent(typeof(AdvConfig))]
	[RequireComponent(typeof(AdvSystemSaveData))]
	[RequireComponent(typeof(AdvSaveManager))]
	public class AdvEngine : MonoBehaviour
	{
		private string startScenarioLabel = "Start";

		private AdvDataManager dataManager;

		private AdvScenarioPlayer scenarioPlayer;

		private AdvPage page;

		private AdvSelectionManager selectionManager;

		private AdvMessageWindowManager messageWindowManager;

		private AdvBacklogManager backlogManager;

		private AdvConfig config;

		private AdvSystemSaveData systemSaveData;

		private AdvSaveManager saveManager;

		[SerializeField]
		private AdvGraphicManager graphicManager;

		[SerializeField]
		private AdvEffectManager effectManager;

		[SerializeField]
		private AdvUiManager uiManager;

		[SerializeField]
		[FormerlySerializedAs("soundManger")]
		private SoundManager soundManager;

		[SerializeField]
		private CameraManager cameraManager;

		private AdvParamManager param = new AdvParamManager();

		[SerializeField]
		private bool bootAsync;

		[SerializeField]
		private bool isStopSoundOnStart = true;

		[SerializeField]
		private bool isStopSoundOnEnd = true;

		private List<AdvCustomCommandManager> customCommandManagerList;

		public UnityEvent onPreInit;

		[SerializeField]
		private OpenDialogEvent onOpenDialog;

		[SerializeField]
		private AdvEvent onPageTextChange = new AdvEvent();

		public AdvEvent OnClear;

		[SerializeField]
		public AdvEvent onChangeLanguage = new AdvEvent();

		private bool isWaitBootLoading = true;

		private bool isStarted;

		private bool isSceneGallery;

		public string StartScenarioLabel
		{
			get
			{
				return startScenarioLabel;
			}
			set
			{
				startScenarioLabel = value;
			}
		}

		public AdvDataManager DataManager
		{
			get
			{
				return dataManager ?? (dataManager = GetComponent<AdvDataManager>());
			}
		}

		public AdvScenarioPlayer ScenarioPlayer
		{
			get
			{
				return scenarioPlayer ?? (scenarioPlayer = GetComponent<AdvScenarioPlayer>());
			}
		}

		public AdvPage Page
		{
			get
			{
				return page ?? (page = GetComponent<AdvPage>());
			}
		}

		public AdvSelectionManager SelectionManager
		{
			get
			{
				return selectionManager ?? (selectionManager = GetComponent<AdvSelectionManager>());
			}
		}

		public AdvMessageWindowManager MessageWindowManager
		{
			get
			{
				return messageWindowManager ?? (messageWindowManager = base.gameObject.GetComponentCreateIfMissing<AdvMessageWindowManager>());
			}
		}

		public AdvBacklogManager BacklogManager
		{
			get
			{
				return backlogManager ?? (backlogManager = GetComponent<AdvBacklogManager>());
			}
		}

		public AdvConfig Config
		{
			get
			{
				return config ?? (config = GetComponent<AdvConfig>());
			}
		}

		public AdvSystemSaveData SystemSaveData
		{
			get
			{
				return systemSaveData ?? (systemSaveData = GetComponent<AdvSystemSaveData>());
			}
		}

		public AdvSaveManager SaveManager
		{
			get
			{
				return saveManager ?? (saveManager = GetComponent<AdvSaveManager>());
			}
		}

		public AdvGraphicManager GraphicManager
		{
			get
			{
				if (graphicManager == null)
				{
					graphicManager = base.transform.GetCompoentInChildrenCreateIfMissing<AdvGraphicManager>();
					graphicManager.transform.localPosition = new Vector3(0f, 0f, 20f);
				}
				return graphicManager;
			}
		}

		public AdvEffectManager EffectManager
		{
			get
			{
				return effectManager ?? (effectManager = base.transform.GetCompoentInChildrenCreateIfMissing<AdvEffectManager>());
			}
		}

		public AdvUiManager UiManager
		{
			get
			{
				return uiManager ?? (uiManager = UnityEngine.Object.FindObjectOfType<AdvUiManager>());
			}
		}

		public SoundManager SoundManager
		{
			get
			{
				return soundManager ?? (soundManager = UnityEngine.Object.FindObjectOfType<SoundManager>());
			}
		}

		public CameraManager CameraManager
		{
			get
			{
				return cameraManager ?? (cameraManager = UnityEngine.Object.FindObjectOfType<CameraManager>());
			}
		}

		public AdvParamManager Param
		{
			get
			{
				return param;
			}
		}

		public List<AdvCustomCommandManager> CustomCommandManagerList
		{
			get
			{
				if (customCommandManagerList == null)
				{
					customCommandManagerList = new List<AdvCustomCommandManager>();
					GetComponentsInChildren(true, customCommandManagerList);
				}
				return customCommandManagerList;
			}
		}

		public OpenDialogEvent OnOpenDialog
		{
			get
			{
				if (onOpenDialog.GetPersistentEventCount() == 0 && SystemUi.GetInstance() != null)
				{
					onOpenDialog.AddListener(SystemUi.GetInstance().OpenDialog);
				}
				return onOpenDialog;
			}
			set
			{
				onOpenDialog = value;
			}
		}

		public AdvEvent OnPageTextChange
		{
			get
			{
				return onPageTextChange;
			}
		}

		public AdvEvent OnChangeLanguage
		{
			get
			{
				return onChangeLanguage;
			}
		}

		public bool IsWaitBootLoading
		{
			get
			{
				return isWaitBootLoading;
			}
		}

		public bool IsStarted
		{
			get
			{
				return isStarted;
			}
		}

		public bool IsSceneGallery
		{
			get
			{
				return isSceneGallery;
			}
		}

		public bool IsLoading
		{
			get
			{
				if (IsWaitBootLoading)
				{
					return true;
				}
				if (GraphicManager.IsLoading)
				{
					return true;
				}
				return ScenarioPlayer.IsLoading;
			}
		}

		public bool IsEndScenario
		{
			get
			{
				if (ScenarioPlayer == null)
				{
					return false;
				}
				if (IsLoading)
				{
					return false;
				}
				return ScenarioPlayer.IsEndScenario;
			}
		}

		public bool IsPausingScenario
		{
			get
			{
				return ScenarioPlayer.IsPausing;
			}
		}

		public bool IsEndOrPauseScenario
		{
			get
			{
				if (!IsEndScenario)
				{
					return IsPausingScenario;
				}
				return true;
			}
		}

		public void BootFromExportData(AdvImportScenarios scenarios, string resourceDir)
		{

            base.gameObject.SetActive(true);
			StopAllCoroutines();
			StartCoroutine(CoBootFromExportData(scenarios, resourceDir));
		}

		private IEnumerator CoBootFromExportData(AdvImportScenarios scenarios, string resourceDir)
		{
			ClearSub(false);
			isStarted = true;
			isWaitBootLoading = true;
			onPreInit.Invoke();
			while (!AssetFileManager.IsInitialized())
			{
				yield return null;
			}
			yield return null;
			DataManager.SettingDataManager.ImportedScenarios = scenarios;
			yield return CoBootInit(resourceDir);
			isWaitBootLoading = false;
		}

		public bool ExitsChapter(string url)
		{
			string chapterAssetName = FilePathUtil.GetFileNameWithoutExtension(url);
			return DataManager.SettingDataManager.ImportedScenarios.Chapters.Exists((AdvChapterData x) => x.name == chapterAssetName);
		}

		public IEnumerator LoadChapterAsync(string url)
		{
			AssetFile file = AssetFileManager.Load(url, this);
			while (!file.IsLoadEnd)
			{
				yield return null;
			}
			AdvChapterData advChapterData = file.UnityObject as AdvChapterData;
			if (advChapterData == null)
			{
				Debug.LogError(url + " is  not scenario file");
				yield break;
			}
			if (DataManager.SettingDataManager.ImportedScenarios == null)
			{
				DataManager.SettingDataManager.ImportedScenarios = new AdvImportScenarios();
			}
			if (DataManager.SettingDataManager.ImportedScenarios.TryAddChapter(advChapterData))
			{
				DataManager.BootInitChapter(advChapterData);
			}
		}

		private void OnClicked()
		{
		}

		private void ChangeLanguage()
		{
			Page.OnChangeLanguage();
			OnChangeLanguage.Invoke(this);
		}

		public void ClearOnStart()
		{
			ClearSub(isStopSoundOnStart);
		}

		public void ClearOnEnd()
		{
			ClearSub(isStopSoundOnEnd);
		}

		private void ClearOnLaod()
		{
			ClearSub(true);
		}

		private void ClearSub(bool isStopSound)
		{
			Page.Clear();
			SelectionManager.Clear();
			BacklogManager.Clear();
			GraphicManager.Clear();
			GraphicManager.gameObject.SetActive(true);
			if (UiManager != null)
			{
				UiManager.Close();
			}
			ClearCustomCommand();
			ScenarioPlayer.Clear();
			if (isStopSound && SoundManager != null)
			{
				SoundManager.StopBgm();
				SoundManager.StopAmbience();
				SoundManager.StopAllLoop();
			}
			if (MessageWindowManager == null)
			{
				Debug.LogError("MessageWindowManager is Missing");
			}
			CameraManager.OnClear();
			SaveManager.GetSaveIoListCreateIfMissing(this).ForEach(delegate(IBinaryIO x)
			{
				((IAdvSaveData)x).OnClear();
			});
			SaveManager.CustomSaveDataIOList.ForEach(delegate(IBinaryIO x)
			{
				((IAdvSaveData)x).OnClear();
			});
			OnClear.Invoke(this);
		}

		public void EndScenario()
		{
			ScenarioPlayer.EndScenario();
		}

		private IEnumerator CoBootInit(string rootDirResource)
		{

            BootInitCustomCommand();
			DataManager.BootInit(rootDirResource);
			GraphicManager.BootInit(this, DataManager.SettingDataManager.LayerSetting);
			Param.InitDefaultAll(DataManager.SettingDataManager.DefaultParam);
			AdvGraphicInfo.CallbackExpression = Param.CalcExpressionBoolean;
			TextParser.CallbackCalcExpression = (Func<string, object>)Delegate.Combine(TextParser.CallbackCalcExpression, new Func<string, object>(Param.CalcExpressionNotSetParam));
			iTweenData.CallbackGetValue = (Func<string, object>)Delegate.Combine(iTweenData.CallbackGetValue, new Func<string, object>(Param.GetParameter));
			LanguageManagerBase.Instance.OnChangeLanugage = ChangeLanguage;
			SystemSaveData.Init(this);
			SaveManager.Init();
			if (bootAsync)
			{
				yield return StartCoroutine(DataManager.CoBootInitScenariodData());
				yield break;
			}
			DataManager.BootInitScenariodData();
			DataManager.StartBackGroundDownloadResource();
		}

		public void BootInitCustomCommand()
		{
			AdvCommandParser.OnCreateCustomCommandFromID = null;
			foreach (AdvCustomCommandManager customCommandManager in CustomCommandManagerList)
			{
				customCommandManager.OnBootInit();
			}
		}

		public void ClearCustomCommand()
		{
			foreach (AdvCustomCommandManager customCommandManager in CustomCommandManagerList)
			{
				customCommandManager.OnClear();
			}
		}

		public void WriteSystemData()
		{
			systemSaveData.Write();
		}

		public void WriteSaveData(AdvSaveData saveData)
		{
			SaveManager.WriteSaveData(this, saveData);
		}

		private void LoadSaveData(AdvSaveData saveData)
		{
			ClearOnLaod();
			StartCoroutine(CoStartSaveData(saveData));
		}

		public void QuickSave()
		{
			WriteSaveData(SaveManager.QuickSaveData);
		}

		public bool QuickLoad()
		{
			if (SaveManager.ReadQuickSaveData())
			{
				LoadSaveData(SaveManager.QuickSaveData);
				return true;
			}
			return false;
		}

		public void StartGame()
		{
			StartGame(StartScenarioLabel);
		}

		public void StartGame(string scenarioLabel)
		{
			isSceneGallery = false;
			StartGameSub(scenarioLabel);
		}

		private void StartGameSub(string scenarioLabel)
		{
			StartCoroutine(CoStartGameSub(scenarioLabel));
		}

		private IEnumerator CoStartGameSub(string scenarioLabel)
		{
			while (IsWaitBootLoading)
			{
				yield return null;
			}
			Param.InitDefaultNormal(DataManager.SettingDataManager.DefaultParam);
			ClearOnStart();
			StartScenario(scenarioLabel, 0);
		}

		public void OpenLoadGame(AdvSaveData saveData)
		{
			isSceneGallery = false;
			LoadSaveData(saveData);
		}

		public void StartSceneGallery(string label)
		{
			isSceneGallery = true;
			StartGameSub(label);
		}

		public bool ResumeScenario()
		{
			if (!ScenarioPlayer.IsPausing)
			{
				return false;
			}
			ScenarioPlayer.Resume();
			return true;
		}

		public void JumpScenario(string label)
		{
			if (ScenarioPlayer.MainThread.IsPlaying)
			{
				if (ScenarioPlayer.IsPausing)
				{
					ScenarioPlayer.Resume();
				}
				ScenarioPlayer.MainThread.JumpManager.RegistoreLabel(label);
			}
			else
			{
				StartScenario(label, 0);
			}
		}

		private void StartScenario(string label, int page)
		{
			StartCoroutine(CoStartScenario(label, page));
		}

		private IEnumerator CoStartScenario(string label, int page)
		{
			while (IsWaitBootLoading)
			{
				yield return null;
			}
			while (GraphicManager.IsLoading)
			{
				yield return null;
			}
			while (SoundManager.IsLoading)
			{
				yield return null;
			}
			if (UiManager != null)
			{
				UiManager.Open();
			}
			if (label.Length > 1 && label[0] == '*')
			{
				label = label.Substring(1);
			}
			ScenarioPlayer.StartScenario(label, page);
		}

		private IEnumerator CoStartSaveData(AdvSaveData saveData)
		{
			while (IsWaitBootLoading)
			{
				yield return null;
			}
			while (GraphicManager.IsLoading)
			{
				yield return null;
			}
			while (SoundManager.IsLoading)
			{
				yield return null;
			}
			if (UiManager != null)
			{
				UiManager.Open();
			}
			yield return ScenarioPlayer.CoStartSaveData(saveData);
		}
	}
}
