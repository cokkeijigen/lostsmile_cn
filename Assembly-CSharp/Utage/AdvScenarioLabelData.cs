using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class AdvScenarioLabelData
	{
		private AdvCommandScenarioLabel scenarioLabelCommand;

		public List<AdvScenarioPageData> PageDataList { get; private set; }

		public string ScenarioLabel { get; private set; }

		public AdvScenarioLabelData Next { get; internal set; }

		public int PageNum
		{
			get
			{
				return PageDataList.Count;
			}
		}

		public bool IsSavePoint
		{
			get
			{
				if (scenarioLabelCommand != null)
				{
					return scenarioLabelCommand.Type == AdvCommandScenarioLabel.ScenarioLabelType.SavePoint;
				}
				return false;
			}
		}

		public string SaveTitle
		{
			get
			{
				if (scenarioLabelCommand != null)
				{
					return scenarioLabelCommand.Title;
				}
				return "";
			}
		}

		public List<AdvCommand> CommandList { get; set; }

		internal AdvScenarioLabelData(string scenarioLabel, AdvCommandScenarioLabel scenarioLabelCommand, List<AdvCommand> commandList)
		{
			ScenarioLabel = scenarioLabel;
			this.scenarioLabelCommand = scenarioLabelCommand;
			CommandList = commandList;
			PageDataList = new List<AdvScenarioPageData>();
			if (CommandList.Count > 0)
			{
				int num = 0;
				do
				{
					int num2 = num;
					int pageEndCommandIndex = GetPageEndCommandIndex(num2);
					PageDataList.Add(new AdvScenarioPageData(this, PageDataList.Count, CommandList.GetRange(num2, pageEndCommandIndex - num2 + 1)));
					num = pageEndCommandIndex + 1;
				}
				while (num < CommandList.Count);
				PageDataList.ForEach(delegate(AdvScenarioPageData x)
				{
					x.Init();
				});
			}
		}

		private int GetPageEndCommandIndex(int begin)
		{
			for (int i = begin; i < CommandList.Count; i++)
			{
				if (!CommandList[i].IsTypePageEnd())
				{
					continue;
				}
				for (int j = i; j < CommandList.Count && !CommandList[j].IsTypePage(); j++)
				{
					if (CommandList[j] is AdvCommandEndPage)
					{
						return j;
					}
				}
				return i;
			}
			return CommandList.Count - 1;
		}

		public void Download(AdvDataManager dataManager)
		{
			PageDataList.ForEach(delegate(AdvScenarioPageData item)
			{
				item.Download(dataManager);
			});
		}

		public void AddToFileSet(HashSet<AssetFile> fileSet)
		{
			foreach (AdvScenarioPageData pageData in PageDataList)
			{
				pageData.AddToFileSet(fileSet);
			}
		}

		public AdvScenarioPageData GetPageData(int page)
		{
			if (page >= PageDataList.Count)
			{
				return null;
			}
			return PageDataList[page];
		}

		public string ToErrorString(string str, string gridName)
		{
			if (scenarioLabelCommand != null)
			{
				return scenarioLabelCommand.RowData.ToErrorString(str);
			}
			return str + " " + gridName;
		}

		internal int CountSubroutineCommandIndex(AdvCommand command)
		{
			int num = 0;
			foreach (AdvScenarioPageData pageData in PageDataList)
			{
				foreach (AdvCommand command2 in pageData.CommandList)
				{
					Type type = command2.GetType();
					if (type == typeof(AdvCommandJumpSubroutine) || type == typeof(AdvCommandJumpSubroutineRandom))
					{
						if (command2 == command)
						{
							return num;
						}
						num++;
					}
				}
			}
			Debug.LogError("Not found Subroutine Command");
			return -1;
		}

		internal bool TrySetSubroutineRetunInfo(int subroutineCommandIndex, SubRoutineInfo info)
		{
			info.ReturnLabel = ScenarioLabel;
			AdvCommand advCommand = null;
			int num = 0;
			foreach (AdvScenarioPageData pageData in PageDataList)
			{
				foreach (AdvCommand command in pageData.CommandList)
				{
					Type type = command.GetType();
					if (advCommand == null)
					{
						if (type == typeof(AdvCommandJumpSubroutine) || type == typeof(AdvCommandJumpSubroutineRandom))
						{
							if (num == subroutineCommandIndex)
							{
								advCommand = command;
							}
							else
							{
								num++;
							}
						}
						continue;
					}
					if (advCommand.GetType() == typeof(AdvCommandJumpSubroutine))
					{
						info.ReturnPageNo = pageData.PageNo;
						info.ReturnCommand = command;
						return true;
					}
					if (!(advCommand.GetType() == typeof(AdvCommandJumpSubroutineRandom)) || !(type != typeof(AdvCommandJumpSubroutineRandom)) || !(type != typeof(AdvCommandJumpSubroutineRandom)))
					{
						continue;
					}
					info.ReturnPageNo = pageData.PageNo;
					info.ReturnCommand = command;
					return true;
				}
			}
			return false;
		}

		internal HashSet<AssetFile> MakePreloadFileListSub(AdvDataManager dataManager, int page, int maxFilePreload, int preloadDeep)
		{
			AdvScenarioLabelData advScenarioLabelData = this;
			HashSet<AssetFile> hashSet = new HashSet<AssetFile>();
			do
			{
				for (int i = page; i < advScenarioLabelData.PageNum; i++)
				{
					advScenarioLabelData.GetPageData(i).AddToFileSet(hashSet);
					if (hashSet.Count >= maxFilePreload)
					{
						return hashSet;
					}
				}
				if (advScenarioLabelData.IsEndPreLoad())
				{
					advScenarioLabelData.PreloadDeep(dataManager, page, hashSet, maxFilePreload, preloadDeep);
					break;
				}
				page = 0;
				advScenarioLabelData = advScenarioLabelData.Next;
			}
			while (advScenarioLabelData != null);
			return hashSet;
		}

		private bool IsEndPreLoad()
		{
			if (CommandList.Count <= 0)
			{
				return false;
			}
			AdvCommand advCommand = CommandList[CommandList.Count - 1];
			if (advCommand is AdvCommandPageControler)
			{
				if (CommandList.Count - 2 < 0)
				{
					return false;
				}
				advCommand = CommandList[CommandList.Count - 2];
			}
			if (advCommand is AdvCommandEndScenario)
			{
				return true;
			}
			if (advCommand is AdvCommandSelectionEnd)
			{
				return true;
			}
			if (advCommand is AdvCommandSelectionClickEnd)
			{
				return true;
			}
			if (advCommand is AdvCommandJumpRandomEnd)
			{
				return true;
			}
			if ((advCommand is AdvCommandJump || advCommand is AdvCommandJumpSubroutine || advCommand is AdvCommandJumpSubroutineRandom) && advCommand.IsEmptyCell(AdvColumnName.Arg2))
			{
				return true;
			}
			return false;
		}

		private void PreloadDeep(AdvDataManager dataManager, int startPage, HashSet<AssetFile> fileSet, int maxFilePreload, int deepLevel)
		{
			if (fileSet.Count >= maxFilePreload || deepLevel <= 0)
			{
				return;
			}
			for (int i = startPage; i < PageNum; i++)
			{
				GetPageData(i).GetJumpScenarioLabelDataList(dataManager).ForEach(delegate(AdvScenarioLabelData x)
				{
					if (x != null)
					{
						x.PreloadDeep(dataManager, fileSet, maxFilePreload, deepLevel);
					}
				});
			}
		}

		private void PreloadDeep(AdvDataManager dataManager, HashSet<AssetFile> fileSet, int maxFilePreload, int deepLevel)
		{
			if (deepLevel <= 0)
			{
				return;
			}
			int num = deepLevel - 1;
			deepLevel = num;
			if (PageNum <= 0 || fileSet.Count >= maxFilePreload)
			{
				return;
			}
			GetPageData(0).AddToFileSet(fileSet);
			if (fileSet.Count >= maxFilePreload)
			{
				return;
			}
			GetPageData(0).GetAutoJumpLabels(dataManager).ForEach(delegate(AdvScenarioLabelData x)
			{
				if (x != null)
				{
					x.PreloadDeep(dataManager, fileSet, maxFilePreload, deepLevel);
				}
			});
		}
	}
}
