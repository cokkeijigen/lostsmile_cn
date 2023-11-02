using System;
using System.Text;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AdvEntityData
	{
		[SerializeField]
		private string[] originalStrings;

		public AdvEntityData(string[] originalStrings)
		{
			this.originalStrings = originalStrings;
		}

		public static AdvCommand CreateEntityCommand(AdvCommand original, AdvEngine engine, AdvScenarioPageData pageData)
		{
			StringGridRow stringGridRow = new StringGridRow(original.RowData.Grid, original.RowData.RowIndex);
			stringGridRow.DebugIndex = original.RowData.DebugIndex;
			string[] strings = original.EntityData.CreateCommandStrings(engine.Param.GetParameter);
			stringGridRow.InitFromStringArray(strings);
			AdvCommand advCommand = AdvCommandParser.CreateCommand(original.Id, stringGridRow, engine.DataManager.SettingDataManager);
			if (advCommand is AdvCommandText)
			{
				(advCommand as AdvCommandText).InitOnCreateEntity(original as AdvCommandText);
			}
			return advCommand;
		}

		public string[] CreateCommandStrings(Func<string, object> GetParameter)
		{
			string[] array = new string[originalStrings.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text = (array[i] = originalStrings[i]);
				if (text.Length <= 1 || text.IndexOf('&') < 0)
				{
					continue;
				}
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				while (num < text.Length)
				{
					if (text[num] == '&')
					{
						bool flag = false;
						for (int j = num + 1; j < text.Length; j++)
						{
							if (j == text.Length - 1 || CheckEntitySeparator(text[j + 1]))
							{
								string arg = text.Substring(num + 1, j - num);
								object obj = GetParameter(arg);
								if (obj != null)
								{
									stringBuilder.Append(obj.ToString());
									num = j + 1;
									flag = true;
								}
								break;
							}
						}
						if (flag)
						{
							continue;
						}
					}
					stringBuilder.Append(text[num]);
					num++;
				}
				array[i] = stringBuilder.ToString();
			}
			return array;
		}

		public static bool ContainsEntitySimple(StringGridRow row)
		{
			for (int i = 0; i < row.Strings.Length; i++)
			{
				int num = row.Strings[i].IndexOf('&');
				if (num >= 0)
				{
					string text = row.Strings[i];
					if (num + 1 >= text.Length || text[num + 1] != '&')
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool TryCreateEntityStrings(StringGridRow original, Func<string, object> GetParameter, out string[] strings)
		{
			bool result = false;
			strings = new string[original.Strings.Length];
			for (int i = 0; i < original.Strings.Length; i++)
			{
				string text = (strings[i] = original.Strings[i]);
				if (text.Length <= 1 || text.IndexOf('&') < 0)
				{
					continue;
				}
				if (original.Grid.TryGetColumnIndex(AdvColumnName.WindowType.QuickToString(), out var index) && i == index)
				{
					Debug.LogError(" Can not use entity in " + AdvColumnName.WindowType.QuickToString());
					return false;
				}
				if (original.Grid.TryGetColumnIndex(AdvColumnName.PageCtrl.QuickToString(), out var index2) && i == index2)
				{
					Debug.LogError(" Can not use entity in " + AdvColumnName.PageCtrl.QuickToString());
					return false;
				}
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				while (num < text.Length)
				{
					if (text[num] == '&')
					{
						bool flag = false;
						for (int j = num + 1; j < text.Length; j++)
						{
							if (j == text.Length - 1 || CheckEntitySeparator(text[j + 1]))
							{
								string arg = text.Substring(num + 1, j - num);
								object obj = GetParameter(arg);
								if (obj != null)
								{
									stringBuilder.Append(obj.ToString());
									num = j + 1;
									flag = true;
								}
								break;
							}
						}
						if (flag)
						{
							result = true;
							continue;
						}
					}
					stringBuilder.Append(text[num]);
					num++;
				}
				strings[i] = stringBuilder.ToString();
			}
			return result;
		}

		private static bool CheckEntitySeparator(char c)
		{
			if (c == '.' || c == '[' || c == ']')
			{
				return true;
			}
			return ExpressionToken.CheckSeparator(c);
		}
	}
}
