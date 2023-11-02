using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public class AdvScenarioData
	{
		private string name;

		private bool isInit;

		private bool isAlreadyBackGroundLoad;

		private List<AdvScenarioJumpData> jumpDataList = new List<AdvScenarioJumpData>();

		private Dictionary<string, AdvScenarioLabelData> scenarioLabels = new Dictionary<string, AdvScenarioLabelData>();

		private string Name => name;

		public AdvImportScenarioSheet DataGrid { get; private set; }

		public string DataGridName => DataGrid.Name;

		public bool IsInit => isInit;

		public bool IsAlreadyBackGroundLoad => isAlreadyBackGroundLoad;

		public List<AdvScenarioJumpData> JumpDataList => jumpDataList;

		public Dictionary<string, AdvScenarioLabelData> ScenarioLabels => scenarioLabels;

		public AdvScenarioData(AdvImportScenarioSheet grid)
		{
			name = grid.SheetName;
			DataGrid = grid;
		}

		public void Init(AdvSettingDataManager dataManager)
		{
			isInit = false;
			List<AdvCommand> commandList = DataGrid.CreateCommandList(dataManager);
			AddExtraCommand(commandList, dataManager);
			MakeScanerioLabelData(commandList);
			MakeJumpDataList(commandList);
			isInit = true;
		}

		private void AddExtraCommand(List<AdvCommand> commandList, AdvSettingDataManager dataManager)
		{
			int num = 0;
			while (num < commandList.Count)
			{
				AdvCommand advCommand = commandList[num];
				AdvCommand next = ((num + 1 < commandList.Count) ? commandList[num + 1] : null);
				num++;
				string[] extraCommandIdArray = advCommand.GetExtraCommandIdArray(next);
				if (extraCommandIdArray == null)
				{
					continue;
				}
				string[] array = extraCommandIdArray;
				for (int i = 0; i < array.Length; i++)
				{
					AdvCommand advCommand2 = AdvCommandParser.CreateCommand(array[i], advCommand.RowData, dataManager);
					if (advCommand.IsEntityType)
					{
						advCommand2.EntityData = advCommand.EntityData;
					}
					commandList.Insert(num, advCommand2);
					num++;
				}
			}
		}

		private void MakeScanerioLabelData(List<AdvCommand> commandList)
		{
			if (commandList.Count <= 0)
			{
				return;
			}
			string scenarioLabel = Name;
			AdvCommandScenarioLabel advCommandScenarioLabel = null;
			AdvScenarioLabelData advScenarioLabelData = null;
			int i = 0;
			while (true)
			{
				int num = i;
				for (; i < commandList.Count && !(commandList[i] is AdvCommandScenarioLabel); i++)
				{
				}
				if (IsContainsScenarioLabel(scenarioLabel))
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.RedefinitionScenarioLabel, scenarioLabel, DataGridName));
				}
				else
				{
					AdvScenarioLabelData advScenarioLabelData2 = new AdvScenarioLabelData(scenarioLabel, advCommandScenarioLabel, commandList.GetRange(num, i - num));
					if (advScenarioLabelData != null)
					{
						advScenarioLabelData.Next = advScenarioLabelData2;
					}
					advScenarioLabelData = advScenarioLabelData2;
					scenarioLabels.Add(scenarioLabel, advScenarioLabelData2);
				}
				if (i < commandList.Count)
				{
					advCommandScenarioLabel = commandList[i] as AdvCommandScenarioLabel;
					scenarioLabel = advCommandScenarioLabel.ScenarioLabel;
					i++;
					continue;
				}
				break;
			}
		}

		private void MakeJumpDataList(List<AdvCommand> commandList)
		{
			JumpDataList.Clear();
			commandList.ForEach(delegate(AdvCommand command)
			{
				string[] jumpLabels = command.GetJumpLabels();
				if (jumpLabels != null)
				{
					string[] array = jumpLabels;
					foreach (string toLabel in array)
					{
						JumpDataList.Add(new AdvScenarioJumpData(toLabel, command.RowData));
					}
				}
			});
		}

		public void Download(AdvDataManager dataManager)
		{
			foreach (KeyValuePair<string, AdvScenarioLabelData> scenarioLabel in ScenarioLabels)
			{
				scenarioLabel.Value.Download(dataManager);
			}
			isAlreadyBackGroundLoad = true;
		}

		public void AddToFileSet(HashSet<AssetFile> fileSet)
		{
			foreach (KeyValuePair<string, AdvScenarioLabelData> scenarioLabel in ScenarioLabels)
			{
				scenarioLabel.Value.AddToFileSet(fileSet);
			}
		}

		public bool IsContainsScenarioLabel(string scenarioLabel)
		{
			return FindScenarioLabelData(scenarioLabel) != null;
		}

		public AdvScenarioLabelData FindScenarioLabelData(string scenarioLabel)
		{
			return ScenarioLabels.GetValueOrGetNullIfMissing(scenarioLabel);
		}

		public AdvScenarioLabelData FindNextScenarioLabelData(string scenarioLabel)
		{
			return FindScenarioLabelData(scenarioLabel)?.Next;
		}
	}
}
