using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Utage
{
	public class AdvMacroManager
	{
		private Dictionary<string, AdvMacroData> macroDataTbl = new Dictionary<string, AdvMacroData>();

		private const string SheetNamePattern = "Macro[0-9]";

		private static readonly Regex SheetNameRegex = new Regex("Macro[0-9]", RegexOptions.IgnorePatternWhitespace);

		public bool TryAddMacroData(string name, StringGrid grid)
		{
			if (!IsMacroName(name))
			{
				return false;
			}
			int num = 0;
			while (num < grid.Rows.Count)
			{
				StringGridRow stringGridRow = grid.Rows[num];
				num++;
				if (stringGridRow.RowIndex < grid.DataTopRow || stringGridRow.IsEmptyOrCommantOut || !TryParseMacoBegin(stringGridRow, out var macroName))
				{
					continue;
				}
				List<StringGridRow> list = new List<StringGridRow>();
				while (num < grid.Rows.Count)
				{
					StringGridRow stringGridRow2 = grid.Rows[num];
					num++;
					if (!stringGridRow2.IsEmptyOrCommantOut)
					{
						if (AdvParser.ParseCellOptional(stringGridRow2, AdvColumnName.Command, "") == "EndMacro")
						{
							break;
						}
						list.Add(stringGridRow2);
					}
				}
				if (macroDataTbl.ContainsKey(macroName))
				{
					Debug.LogError(stringGridRow.ToErrorString(macroName + " is already contains "));
				}
				else
				{
					macroDataTbl.Add(macroName, new AdvMacroData(macroName, stringGridRow, list));
				}
			}
			return true;
		}

		public bool TryMacroExpansion(StringGridRow row, List<StringGridRow> outputList, string debugMsg)
		{
			string key = AdvParser.ParseCellOptional(row, AdvColumnName.Command, "");
			if (!macroDataTbl.TryGetValue(key, out var value))
			{
				return false;
			}
			if (string.IsNullOrEmpty(debugMsg))
			{
				debugMsg = row.Grid.Name + ":" + (row.RowIndex + 1);
			}
			debugMsg = debugMsg + " -> MACRO " + value.Header.Grid.Name;
			foreach (StringGridRow item in value.MacroExpansion(row, debugMsg))
			{
				if (!TryMacroExpansion(item, outputList, item.DebugInfo))
				{
					outputList.Add(item);
				}
			}
			return true;
		}

		public static bool IsMacroName(string sheetName)
		{
			if (sheetName == "Macro")
			{
				return true;
			}
			return SheetNameRegex.Match(sheetName).Success;
		}

		private bool TryParseMacoBegin(StringGridRow row, out string macroName)
		{
			return AdvCommandParser.TryParseScenarioLabel(row, AdvColumnName.Command, out macroName);
		}
	}
}
