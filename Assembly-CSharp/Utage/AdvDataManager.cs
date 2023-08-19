using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/DataManager ")]
	public class AdvDataManager : MonoBehaviour
	{
		[SerializeField]
		private bool isBackGroundDownload = true;

		[SerializeField]
		private int maxSenarioCountOnInitAsync = 1;

		private AdvSettingDataManager settingDataManager = new AdvSettingDataManager();

		private Dictionary<string, AdvScenarioData> scenarioDataTbl = new Dictionary<string, AdvScenarioData>();

		private AdvMacroManager macroManager = new AdvMacroManager();

		public bool IsBackGroundDownload
		{
			get
			{
				return isBackGroundDownload;
			}
			set
			{
				isBackGroundDownload = value;
			}
		}

		public int MaxSenarioCountOnInitAsync
		{
			get
			{
				return maxSenarioCountOnInitAsync;
			}
			set
			{
				maxSenarioCountOnInitAsync = value;
			}
		}

		public AdvSettingDataManager SettingDataManager
		{
			get
			{
				return settingDataManager;
			}
		}

		public Dictionary<string, AdvScenarioData> ScenarioDataTbl
		{
			get
			{
				return scenarioDataTbl;
			}
		}

		public bool IsReadySettingData
		{
			get
			{
				return settingDataManager != null;
			}
		}

		public AdvMacroManager MacroManager
		{
			get
			{
				return macroManager;
			}
		}

		public virtual void BootInit(string rootDirResource)
		{
			settingDataManager.BootInit(rootDirResource);
		}

		public virtual void BootInitScenario(bool async)
		{
			if (async)
			{
				StartCoroutine(CoBootInitScenariodData());
				return;
			}
			BootInitScenariodData();
			StartBackGroundDownloadResource();
		}

		public virtual void BootInitChapter(AdvChapterData chapter)
		{
			chapter.BootInit(SettingDataManager);
			Dictionary<string, AdvScenarioData> dictionary = new Dictionary<string, AdvScenarioData>();
			chapter.AddScenario(dictionary);
			foreach (KeyValuePair<string, AdvScenarioData> item in dictionary)
			{
				scenarioDataTbl.Add(item.Key, item.Value);
			}
			foreach (KeyValuePair<string, AdvScenarioData> item2 in dictionary)
			{
				item2.Value.Init(settingDataManager);
			}
		}

		public virtual void BootInitScenariodData()
		{
			if (settingDataManager.ImportedScenarios != null)
			{
				settingDataManager.ImportedScenarios.Chapters.ForEach(delegate(AdvChapterData x)
				{
					x.AddScenario(scenarioDataTbl);
				});
			}
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				value.Init(settingDataManager);
			}
		}

		public virtual IEnumerator CoBootInitScenariodData()
		{
			if (settingDataManager.ImportedScenarios != null)
			{
				settingDataManager.ImportedScenarios.Chapters.ForEach(delegate(AdvChapterData x)
				{
					x.AddScenario(scenarioDataTbl);
				});
			}
			int countScenario = 0;
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				value.Init(settingDataManager);
				int num = countScenario + 1;
				countScenario = num;
				if (countScenario >= MaxSenarioCountOnInitAsync)
				{
					countScenario = 0;
					yield return null;
				}
			}
			StartBackGroundDownloadResource();
		}

		public virtual void StartBackGroundDownloadResource()
		{
			if (isBackGroundDownload)
			{
				DownloadAll();
			}
		}

		public virtual HashSet<AssetFile> GetAllFileSet()
		{
			HashSet<AssetFile> hashSet = new HashSet<AssetFile>();
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				value.AddToFileSet(hashSet);
			}
			return hashSet;
		}

		public virtual void DownloadAll()
		{
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				value.Download(this);
			}
			SettingDataManager.DownloadAll();
		}

		public virtual void DownloadAllFileUsed()
		{
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				value.Download(this);
			}
		}

		public virtual bool IsLoadEndScenarioLabel(AdvScenarioJumpData jumpData)
		{
			return IsLoadEndScenarioLabel(jumpData.ToLabel);
		}

		public virtual bool IsLoadEndScenarioLabel(string label)
		{
			AdvScenarioData advScenarioData = FindScenarioData(label);
			if (advScenarioData != null)
			{
				return true;
			}
			Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotFoundScnarioLabel, label));
			return false;
		}

		public virtual AdvScenarioData FindScenarioData(string label)
		{
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				if (value.IsContainsScenarioLabel(label))
				{
					return value;
				}
			}
			return null;
		}

		public virtual AdvScenarioLabelData FindScenarioLabelData(string scenarioLabel)
		{
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				AdvScenarioLabelData advScenarioLabelData = value.FindScenarioLabelData(scenarioLabel);
				if (advScenarioLabelData != null)
				{
					return advScenarioLabelData;
				}
			}
			return null;
		}

		public virtual AdvScenarioLabelData NextScenarioLabelData(string scenarioLabel)
		{
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				AdvScenarioLabelData advScenarioLabelData = value.FindNextScenarioLabelData(scenarioLabel);
				if (advScenarioLabelData != null)
				{
					return advScenarioLabelData;
				}
			}
			return null;
		}

		public virtual void SetSubroutineRetunInfo(string scenarioLabel, int subroutineCommandIndex, SubRoutineInfo info)
		{
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				AdvScenarioLabelData advScenarioLabelData = value.FindScenarioLabelData(scenarioLabel);
				if (advScenarioLabelData != null)
				{
					if (!advScenarioLabelData.TrySetSubroutineRetunInfo(subroutineCommandIndex, info))
					{
						AdvScenarioLabelData advScenarioLabelData2 = NextScenarioLabelData(scenarioLabel);
						info.ReturnLabel = advScenarioLabelData2.ScenarioLabel;
						info.ReturnPageNo = 0;
						info.ReturnCommand = null;
					}
					break;
				}
			}
		}

		public virtual HashSet<AssetFile> MakePreloadFileList(string scenarioLabel, int page, int maxFilePreload, int preloadDeep)
		{
			foreach (AdvScenarioData value in scenarioDataTbl.Values)
			{
				if (value.IsContainsScenarioLabel(scenarioLabel))
				{
					AdvScenarioLabelData advScenarioLabelData = value.FindScenarioLabelData(scenarioLabel);
					if (advScenarioLabelData == null)
					{
						return null;
					}
					return advScenarioLabelData.MakePreloadFileListSub(this, page, maxFilePreload, preloadDeep);
				}
			}
			return null;
		}
	}
}
