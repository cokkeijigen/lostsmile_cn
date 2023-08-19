using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class AdvScenarioPageData
	{
		public List<AdvCommand> CommandList { get; private set; }

		public List<AdvCommandText> TextDataList { get; private set; }

		public AdvScenarioLabelData ScenarioLabelData { get; private set; }

		private List<AdvScenarioLabelData> JumpLabelList { get; set; }

		private List<AdvScenarioLabelData> AutoJumpLabelList { get; set; }

		public int PageNo { get; private set; }

		public string MessageWindowName { get; set; }

		internal int IndexTextTopCommand { get; private set; }

		internal bool EnableSave { get; private set; }

		public bool IsEmptyText
		{
			get
			{
				return TextDataList.Count <= 0;
			}
		}

		public AdvScenarioPageData(AdvScenarioLabelData scenarioLabelData, int pageNo, List<AdvCommand> commandList)
		{
			TextDataList = new List<AdvCommandText>();
			ScenarioLabelData = scenarioLabelData;
			PageNo = pageNo;
			CommandList = commandList;
		}

		internal void Init()
		{
			CommandList.ForEach(delegate(AdvCommand command)
			{
				command.InitFromPageData(this);
			});
			EnableSave = true;
			for (int i = 0; i < CommandList.Count; i++)
			{
				if (CommandList[i].IsTypePage())
				{
					IndexTextTopCommand = i;
					break;
				}
			}
		}

		public List<AdvScenarioLabelData> GetJumpScenarioLabelDataList(AdvDataManager dataManager)
		{
			if (JumpLabelList != null)
			{
				return JumpLabelList;
			}
			JumpLabelList = new List<AdvScenarioLabelData>();
			CommandList.ForEach(delegate(AdvCommand command)
			{
				string[] jumpLabels = command.GetJumpLabels();
				if (jumpLabels != null)
				{
					string[] array = jumpLabels;
					foreach (string scenarioLabel in array)
					{
						JumpLabelList.Add(dataManager.FindScenarioLabelData(scenarioLabel));
					}
				}
			});
			return JumpLabelList;
		}

		internal List<AdvScenarioLabelData> GetAutoJumpLabels(AdvDataManager dataManager)
		{
			if (AutoJumpLabelList != null)
			{
				return AutoJumpLabelList;
			}
			AutoJumpLabelList = new List<AdvScenarioLabelData>();
			CommandList.ForEach(delegate(AdvCommand command)
			{
				string[] jumpLabels = command.GetJumpLabels();
				if (jumpLabels != null && (command is AdvCommandJump || command is AdvCommandJumpRandom || command is AdvCommandJumpSubroutine || command is AdvCommandJumpSubroutineRandom))
				{
					string[] array = jumpLabels;
					foreach (string scenarioLabel in array)
					{
						AutoJumpLabelList.Add(dataManager.FindScenarioLabelData(scenarioLabel));
					}
				}
			});
			return AutoJumpLabelList;
		}

		public void Download(AdvDataManager dataManager)
		{
			CommandList.ForEach(delegate(AdvCommand item)
			{
				item.Download(dataManager);
			});
		}

		public AdvCommand GetCommand(int index)
		{
			if (index >= CommandList.Count)
			{
				return null;
			}
			return CommandList[index];
		}

		public void AddToFileSet(HashSet<AssetFile> fileSet)
		{
			foreach (AdvCommand command in CommandList)
			{
				if (command.IsExistLoadFile())
				{
					command.LoadFileList.ForEach(delegate(AssetFile item)
					{
						fileSet.Add(item);
					});
				}
			}
		}

		internal void AddTextData(AdvCommandText command)
		{
			TextDataList.Add(command);
		}

		internal void ChangeTextDataOnCreateEntity(int index, AdvCommandText entity)
		{
			if (TextDataList.Count < index)
			{
				Debug.LogError("  Index error On CreateEntity ");
			}
			else
			{
				TextDataList[index] = entity;
			}
		}

		internal void InitMessageWindowName(AdvCommand command, string messageWindowName)
		{
			if (!string.IsNullOrEmpty(messageWindowName))
			{
				if (string.IsNullOrEmpty(MessageWindowName))
				{
					MessageWindowName = messageWindowName;
				}
				else if (MessageWindowName != messageWindowName)
				{
					Debug.LogError(command.ToErrorString(messageWindowName + ": WindowName already set is this page"));
				}
			}
		}

		internal bool EnableSaveTextTop(AdvCommand command)
		{
			if (command == null)
			{
				return false;
			}
			if (!EnableSave)
			{
				return false;
			}
			if (command == GetCommand(0))
			{
				return false;
			}
			return command == CommandList[IndexTextTopCommand];
		}

		internal int GetIfSkipCommandIndex(int index)
		{
			for (int i = index; i < CommandList.Count; i++)
			{
				AdvCommand advCommand = CommandList[i];
				if (!advCommand.IsIfCommand)
				{
					continue;
				}
				if (advCommand.GetType() == typeof(AdvCommandIf))
				{
					return index;
				}
				for (int j = index + 1; j < CommandList.Count; j++)
				{
					if (CommandList[j].GetType() == typeof(AdvCommandEndIf))
					{
						return j;
					}
				}
			}
			return index;
		}
	}
}
