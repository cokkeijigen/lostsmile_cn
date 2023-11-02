using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public class LanguageData
	{
		public class LanguageStrings
		{
			public List<string> Strings { get; private set; }

			public LanguageStrings()
			{
				Strings = new List<string>();
			}

			internal void SetData(List<string> strings)
			{
				Strings = strings;
			}
		}

		private List<string> languages = new List<string>();

		private Dictionary<string, LanguageStrings> dataTbl = new Dictionary<string, LanguageStrings>();

		public List<string> Languages => languages;

		public bool ContainsKey(string key)
		{
			return dataTbl.ContainsKey(key);
		}

		internal bool TryLocalizeText(out string text, string CurrentLanguage, string DefaultLanguage, string key, string dataName = "")
		{
			text = key;
			if (!ContainsKey(key))
			{
				Debug.LogError(key + ": is not found in Language data");
				return false;
			}
			string item = CurrentLanguage;
			if (!Languages.Contains(CurrentLanguage))
			{
				if (!Languages.Contains(DefaultLanguage))
				{
					return false;
				}
				item = DefaultLanguage;
			}
			int num = Languages.IndexOf(item);
			LanguageStrings languageStrings = dataTbl[key];
			if (num >= languageStrings.Strings.Count)
			{
				return false;
			}
			text = languageStrings.Strings[num];
			return true;
		}

		internal void OverwriteData(TextAsset tsv)
		{
			OverwriteData(new StringGrid(tsv.name, CsvType.Tsv, tsv.text));
		}

		internal void OverwriteData(StringGrid grid)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			StringGridRow stringGridRow = grid.Rows[0];
			for (int i = 0; i < stringGridRow.Length; i++)
			{
				if (i == 0)
				{
					continue;
				}
				string text = stringGridRow.Strings[i];
				if (!string.IsNullOrEmpty(text))
				{
					if (!languages.Contains(text))
					{
						languages.Add(text);
					}
					int key = languages.IndexOf(text);
					if (dictionary.ContainsKey(key))
					{
						Debug.LogError(text + " already exists in  " + grid.Name);
					}
					else
					{
						dictionary.Add(key, i);
					}
				}
			}
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.IsEmptyOrCommantOut || row.RowIndex == 0)
				{
					continue;
				}
				string text2 = row.Strings[0];
				if (string.IsNullOrEmpty(text2))
				{
					continue;
				}
				if (!dataTbl.ContainsKey(text2))
				{
					dataTbl.Add(text2, new LanguageStrings());
				}
				int count = languages.Count;
				List<string> list = new List<string>(count);
				for (int j = 0; j < count; j++)
				{
					string item = "";
					if (dictionary.ContainsKey(j))
					{
						int num = dictionary[j];
						if (num < row.Strings.Length)
						{
							item = row.Strings[num].Replace("\\n", "\n");
						}
					}
					list.Add(item);
				}
				dataTbl[text2].SetData(list);
			}
		}
	}
}
