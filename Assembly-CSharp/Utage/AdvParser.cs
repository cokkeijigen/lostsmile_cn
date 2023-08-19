namespace Utage
{
	public class AdvParser
	{
		public static string Localize(AdvColumnName name)
		{
			return name.QuickToString();
		}

		public static T ParseCell<T>(StringGridRow row, AdvColumnName name)
		{
			return row.ParseCell<T>(Localize(name));
		}

		public static T ParseCellOptional<T>(StringGridRow row, AdvColumnName name, T defaultVal)
		{
			return row.ParseCellOptional(Localize(name), defaultVal);
		}

		public static bool TryParseCell<T>(StringGridRow row, AdvColumnName name, out T val)
		{
			return row.TryParseCell<T>(Localize(name), out val);
		}

		public static bool IsEmptyCell(StringGridRow row, AdvColumnName name)
		{
			return row.IsEmptyCell(Localize(name));
		}

		public static string ParseCellLocalizedText(StringGridRow row, AdvColumnName defaultColumnName)
		{
			return ParseCellLocalizedText(row, defaultColumnName.QuickToString());
		}

		public static string ParseCellLocalizedText(StringGridRow row, string defaultColumnName)
		{
			string columnName = defaultColumnName;
			if (LanguageManagerBase.Instance != null)
			{
				string currentLanguage = LanguageManagerBase.Instance.CurrentLanguage;
				if (row.Grid.ContainsColumn(currentLanguage))
				{
					columnName = currentLanguage;
				}
				else
				{
					string dataLanguage = LanguageManagerBase.Instance.DataLanguage;
					if (!string.IsNullOrEmpty(dataLanguage))
					{
						columnName = ((!(currentLanguage == dataLanguage)) ? LanguageManagerBase.Instance.DefaultLanguage : defaultColumnName);
					}
				}
			}
			if (row.IsEmptyCell(columnName))
			{
				return row.ParseCellOptional(defaultColumnName, "");
			}
			return row.ParseCellOptional(columnName, "");
		}
	}
}
