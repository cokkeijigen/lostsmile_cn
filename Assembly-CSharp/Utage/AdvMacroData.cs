using System.Collections.Generic;

namespace Utage
{
	public class AdvMacroData
	{
		public string Name { get; private set; }

		public StringGridRow Header { get; private set; }

		public List<StringGridRow> DataList { get; private set; }

		public AdvMacroData(string name, StringGridRow header, List<StringGridRow> dataList)
		{
			Name = name;
			Header = header;
			DataList = dataList;
		}

		public List<StringGridRow> MacroExpansion(StringGridRow args, string debugMsg)
		{
			List<StringGridRow> list = new List<StringGridRow>();
			if (DataList.Count <= 0)
			{
				return list;
			}
			for (int i = 0; i < DataList.Count; i++)
			{
				StringGridRow stringGridRow = DataList[i];
				string[] array = new string[args.Grid.ColumnIndexTbl.Count];
				foreach (KeyValuePair<string, int> item in args.Grid.ColumnIndexTbl)
				{
					string key = item.Key;
					int value = item.Value;
					array[value] = ParaseMacroArg(stringGridRow.ParseCellOptional(key, ""), args);
				}
				StringGridRow stringGridRow2 = new StringGridRow(args.Grid, args.RowIndex);
				stringGridRow2.InitFromStringArray(array);
				list.Add(stringGridRow2);
				stringGridRow2.DebugInfo = debugMsg + " : " + (stringGridRow.RowIndex + 1) + " ";
			}
			return list;
		}

		private string ParaseMacroArg(string str, StringGridRow args)
		{
			int i = 0;
			string text = "";
			for (; i < str.Length; i++)
			{
				bool flag = false;
				if (str[i] == '%')
				{
					foreach (string key in Header.Grid.ColumnIndexTbl.Keys)
					{
						if (key.Length <= 0)
						{
							continue;
						}
						for (int j = 0; j < key.Length && key[j] == str[i + 1 + j]; j++)
						{
							if (j == key.Length - 1)
							{
								flag = true;
							}
						}
						if (flag)
						{
							string defaultVal = Header.ParseCellOptional(key, "");
							text += args.ParseCellOptional(key, defaultVal);
							i += key.Length;
							break;
						}
					}
				}
				if (!flag)
				{
					text += str[i];
				}
			}
			return text;
		}
	}
}
