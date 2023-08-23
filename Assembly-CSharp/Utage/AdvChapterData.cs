using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Utage.MiniAnimationData;

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

		public string ChapterName
		{
			get
			{
				return chapterName;
			}
		}

		public List<AdvImportBook> DataList
		{
			get
			{
				return dataList;
			}
		}

		public List<StringGrid> SettingList
		{
			get
			{
				return settingList;
			}
		}

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
				IAdvSetting advSetting = AdvSheetParser.FindSettingData(settingDataManager, setting.SheetName);
				if (advSetting != null)
				{
					advSetting.ParseGrid(setting);
				}
			}
			foreach (StringGrid setting2 in settingList)
			{
				IAdvSetting advSetting2 = AdvSheetParser.FindSettingData(settingDataManager, setting2.SheetName);
				if (advSetting2 != null)
				{
					advSetting2.BootInit(settingDataManager);
				}
			}
		}

		public void AddScenario(Dictionary<string, AdvScenarioData> scenarioDataTbl)
        {

			foreach (AdvImportBook data in DataList)
			{
                //LogPrinter.Dump(data.ImportGridList, data.name);
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
