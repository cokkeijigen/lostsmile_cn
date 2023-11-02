using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class AdvChapterData : ScriptableObject
	{
		[SerializeField]
		private string chapterName = "";

		[SerializeField]
		private List<AdvImportBook> dataList = new List<AdvImportBook>();

		[SerializeField]
		private List<StringGrid> settingList = new List<StringGrid>();

		public string ChapterName => chapterName;

		public List<AdvImportBook> DataList => dataList;

		public List<StringGrid> SettingList => settingList;

		public bool IsInited { get; set; }

		public void Init(string name)
		{
			chapterName = name;
		}

		public void BootInit(AdvSettingDataManager settingDataManager)
		{
			IsInited = true;
			foreach (StringGrid setting in settingList)
			{
				AdvSheetParser.FindSettingData(settingDataManager, setting.SheetName)?.ParseGrid(setting);
			}
			foreach (StringGrid setting2 in settingList)
			{
				AdvSheetParser.FindSettingData(settingDataManager, setting2.SheetName)?.BootInit(settingDataManager);
			}
		}

		public void AddScenario(Dictionary<string, AdvScenarioData> scenarioDataTbl)
		{
			foreach (AdvImportBook data in DataList)
			{
				foreach (AdvImportScenarioSheet importGrid in data.ImportGridList)
				{
					if (scenarioDataTbl.ContainsKey(importGrid.SheetName))
					{
						Debug.LogErrorFormat("{0} is already contains", importGrid.SheetName);
					}
					else
					{
						importGrid.InitLink();
						AdvScenarioData value = new AdvScenarioData(importGrid);
						scenarioDataTbl.Add(importGrid.SheetName, value);
					}
				}
			}
		}
	}
}
