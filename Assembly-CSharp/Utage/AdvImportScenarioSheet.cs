using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AdvImportScenarioSheet : StringGrid
	{
		public class StringGridRowMacroed
		{
			public StringGridRow row;

			public AdvEntityData entityData;

			public StringGridRowMacroed(StringGridRow row)
			{
				this.row = row;
			}

			public StringGridRowMacroed(StringGridRow row, AdvEntityData entityData)
			{
				this.row = row;
				this.entityData = entityData;
			}
		}

		[SerializeField]
		private List<int> entityIndexTbl = new List<int>();

		[SerializeField]
		private List<AdvEntityData> entityDataList = new List<AdvEntityData>();

		public AdvImportScenarioSheet(StringGrid original, AdvSettingDataManager dataManager, AdvMacroManager macroManager)
			: base(original.Name, original.SheetName, original.Type)
		{
			headerRow = original.HeaderRow;
			for (int i = 0; i < original.DataTopRow; i++)
			{
				AddRow(original.Rows[i].Strings);
			}
			List<StringGridRow> list = new List<StringGridRow>();
			foreach (StringGridRow row in original.Rows)
			{
				if (row.RowIndex >= original.DataTopRow && !row.IsEmptyOrCommantOut && !macroManager.TryMacroExpansion(row, list, ""))
				{
					list.Add(row);
				}
			}
			foreach (StringGridRow item2 in list)
			{
				string[] stringArray;
				if (AdvEntityData.ContainsEntitySimple(item2))
				{
					string[] strings;
					if (AdvEntityData.TryCreateEntityStrings(item2, dataManager.DefaultParam.GetParameter, out strings))
					{
						AdvEntityData item = new AdvEntityData(item2.Strings);
						stringArray = strings;
						entityDataList.Add(item);
						entityIndexTbl.Add(base.Rows.Count);
					}
					else
					{
						stringArray = item2.Strings;
					}
				}
				else
				{
					stringArray = item2.Strings;
				}
				StringGridRow stringGridRow = AddRow(stringArray);
				stringGridRow.DebugIndex = item2.DebugIndex;
				stringGridRow.DebugInfo = item2.DebugInfo;
			}
			InitLink();
		}

		public List<AdvCommand> CreateCommandList(AdvSettingDataManager dataManager)
		{
			List<AdvCommand> list = new List<AdvCommand>();
			foreach (StringGridRow row in base.Rows)
			{
                if (row.RowIndex >= base.DataTopRow && !row.IsEmptyOrCommantOut)
				{
                    AdvCommand advCommand = AdvCommandParser.CreateCommand(row, dataManager);
					int entityIndex = GetEntityIndex(row.RowIndex);
					if (entityIndex >= 0)
					{
						advCommand.EntityData = entityDataList[entityIndex];
					}
					if (advCommand != null)
					{
						list.Add(advCommand);
					}
				}
			}
			return list;
		}

		private int GetEntityIndex(int index)
		{
			for (int i = 0; i < entityIndexTbl.Count; i++)
			{
				if (entityIndexTbl[i] == index)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
