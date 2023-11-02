using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class StringGrid
	{
		[SerializeField]
		private List<StringGridRow> rows;

		[SerializeField]
		private string name;

		private string sheetName;

		[SerializeField]
		private CsvType type;

		[SerializeField]
		private int textLength;

		private Dictionary<string, int> columnIndexTbl;

		[SerializeField]
		protected int headerRow;

		public List<StringGridRow> Rows => rows ?? (rows = new List<StringGridRow>());

		public string Name => name;

		public string SheetName
		{
			get
			{
				if (string.IsNullOrEmpty(sheetName))
				{
					int num = Name.LastIndexOf(":");
					sheetName = Name;
					if (num > 0)
					{
						sheetName = sheetName.Remove(0, num + 1);
					}
					if (sheetName.Contains("."))
					{
						sheetName = FilePathUtil.GetFileNameWithoutDoubleExtension(Name).Replace("%7B", "{").Replace("%7D", "}");
					}
				}
				return sheetName;
			}
		}

		public CsvType Type => type;

		public char CsvSeparator
		{
			get
			{
				if (Type != 0)
				{
					return '\t';
				}
				return ',';
			}
		}

		public int TextLength => textLength;

		public Dictionary<string, int> ColumnIndexTbl
		{
			get
			{
				return columnIndexTbl;
			}
			set
			{
				columnIndexTbl = value;
			}
		}

		public int HeaderRow => headerRow;

		public int DataTopRow => HeaderRow + 1;

		public StringGrid(string name, string sheetName, CsvType type)
		{
			this.name = name;
			this.sheetName = sheetName;
			this.type = type;
		}

		public StringGrid(string name, CsvType type, string csvText, int headerRow)
		{
			Create(name, type, csvText, headerRow);
		}

		public StringGrid(string name, CsvType type, string csvText)
		{
			Create(name, type, csvText, 0);
		}

		private void Create(string name, CsvType type, string csvText, int headerRow)
		{
			this.name = name;
			this.type = type;
			Rows.Clear();
			string[] separator = new string[2] { "\r\n", "\n" };
			string[] array = csvText.Split(separator, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				StringGridRow stringGridRow = new StringGridRow(this, Rows.Count);
				stringGridRow.InitFromCsvText(type, array[i]);
				Rows.Add(stringGridRow);
			}
			ParseHeader(headerRow);
			textLength = csvText.Length;
		}

		public void InitLink()
		{
			foreach (StringGridRow row in Rows)
			{
				row.InitLink(this);
			}
			ParseHeader(headerRow);
		}

		internal bool IsCommentOutCoulmn(int column)
		{
			if (headerRow >= Rows.Count)
			{
				return false;
			}
			StringGridRow stringGridRow = Rows[headerRow];
			if (column >= stringGridRow.Strings.Length)
			{
				return false;
			}
			return stringGridRow.Strings[column].StartsWith("//");
		}

		public void AddRow(List<string> stringList)
		{
			StringGridRow stringGridRow = new StringGridRow(this, Rows.Count);
			stringGridRow.InitFromStringList(stringList);
			Rows.Add(stringGridRow);
			foreach (string @string in stringList)
			{
				textLength += @string.Length;
			}
		}

		public StringGridRow AddRow(string[] stringArray)
		{
			StringGridRow stringGridRow = new StringGridRow(this, Rows.Count);
			stringGridRow.InitFromStringArray(stringArray);
			Rows.Add(stringGridRow);
			foreach (string text in stringArray)
			{
				textLength += text.Length;
			}
			return stringGridRow;
		}

		public void ParseHeader(int headerRow)
		{
			this.headerRow = headerRow;
			ColumnIndexTbl = new Dictionary<string, int>();
			if (headerRow < Rows.Count)
			{
				StringGridRow stringGridRow = Rows[headerRow];
				for (int i = 0; i < stringGridRow.Strings.Length; i++)
				{
					string text = stringGridRow.Strings[i];
					if (ColumnIndexTbl.ContainsKey(text))
					{
						string text2 = "";
						if (!string.IsNullOrEmpty(text))
						{
							text2 += stringGridRow.ToErrorString(ColorUtil.AddColorTag(text, Color.red) + "  is already contains");
							Debug.LogError(text2);
						}
					}
					else
					{
						ColumnIndexTbl.Add(text, i);
					}
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridParseHaeder, headerRow, name));
			}
		}

		public void ParseHeader()
		{
			ParseHeader(0);
		}

		public bool ContainsColumn(string name)
		{
			return ColumnIndexTbl.ContainsKey(name);
		}

		public int GetColumnIndex(string name)
		{
			if (TryGetColumnIndex(name, out var index))
			{
				return index;
			}
			Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridGetColumnIndex, name, this.name));
			return 0;
		}

		public bool TryGetColumnIndex(string name, out int index)
		{
			return ColumnIndexTbl.TryGetValue(name, out index);
		}

		public string ToText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			char csvSeparator = CsvSeparator;
			foreach (StringGridRow row in Rows)
			{
				for (int i = 0; i < row.Strings.Length; i++)
				{
					string value = row.Strings[i].Replace("\n", "\\n");
					stringBuilder.Append(value);
					if (i < row.Strings.Length - 1)
					{
						stringBuilder.Append(csvSeparator);
					}
				}
				stringBuilder.Append("\n");
			}
			return stringBuilder.ToString();
		}
	}
}
