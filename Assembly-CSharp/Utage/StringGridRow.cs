using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class StringGridRow
	{
		[NonSerialized]
		private StringGrid grid;

		[SerializeField]
        public int rowIndex;

		[NonSerialized]
		private int debugIndex = -1;

		[SerializeField]
        public string[] strings;

		[SerializeField]
        public bool isEmpty;

		[SerializeField]
		public bool isCommentOut;

		[NonSerialized]
		private string debugInfo;

		public StringGrid Grid
		{
			get
			{
				return grid;
			}
		}

		public int RowIndex
		{
			get
			{
				return rowIndex;
			}
		}

		public int DebugIndex
		{
			get
			{
				return debugIndex;
			}
			set
			{
				debugIndex = value;
			}
		}

		public string[] Strings
		{
			get
			{
				return strings;
			}
		}

		public int Length
		{
			get
			{
				return strings.Length;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return isEmpty;
			}
		}

		public bool IsCommentOut
		{
			get
			{
				return isCommentOut;
			}
		}

		public bool IsEmptyOrCommantOut
		{
			get
			{
				if (!IsEmpty)
				{
					return IsCommentOut;
				}
				return true;
			}
		}

		internal string DebugInfo
		{
			get
			{
				return debugInfo;
			}
			set
			{
				debugInfo = value;
			}
		}

		public StringGridRow(StringGrid gird, int rowIndex)
		{
			this.rowIndex = (DebugIndex = rowIndex);
			InitLink(gird);
		}

		public void InitLink(StringGrid grid)
		{
			this.grid = grid;
		}

		public void InitFromCsvText(CsvType type, string text)
		{
			strings = text.Split((type == CsvType.Tsv) ? '\t' : ',');
			isEmpty = CheckEmpty();
			isCommentOut = CheckCommentOut();
		}

		public void InitFromStringList(List<string> stringList)
		{
			InitFromStringArray(stringList.ToArray());
		}

		public void InitFromStringArray(string[] strings)
		{
			this.strings = strings;
			isEmpty = CheckEmpty();
			isCommentOut = CheckCommentOut();
		}

		private bool CheckEmpty()
		{
			string[] array = strings;
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckCommentOut()
		{
			if (Strings.Length == 0)
			{
				return false;
			}
			return Strings[0].StartsWith("//");
		}

		public bool IsEmptyCell(string columnName)
		{
			int index;
			if (Grid.TryGetColumnIndex(columnName, out index))
			{
				return IsEmptyCell(index);
			}
			return true;
		}

		internal bool IsAllEmptyCellNamedColumn()
		{
			foreach (KeyValuePair<string, int> item in Grid.ColumnIndexTbl)
			{
				if (!IsEmptyCell(item.Value) && !Grid.IsCommentOutCoulmn(item.Value))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsEmptyCell(int index)
		{
			if (index < Length)
			{
				return string.IsNullOrEmpty(strings[index]);
			}
			return true;
		}

		public T ParseCell<T>(string columnName)
		{
			T val;
			if (!TryParseCell<T>(columnName, out val))
			{
				Debug.LogError(ToErrorStringWithPraseColumnName(columnName));
			}
			return val;
		}

		public T ParseCell<T>(int index)
		{
			T val;
			if (!TryParseCell<T>(index, out val))
			{
				Debug.LogError(ToErrorStringWithPraseColumnIndex(index));
			}
			return val;
		}

		public T ParseCellOptional<T>(string columnName, T defaultVal)
		{
			T val;
			if (!TryParseCell<T>(columnName, out val))
			{
				return defaultVal;
			}
			return val;
		}

		public T ParseCellOptional<T>(int index, T defaultVal)
		{
			T val;
			if (!TryParseCell<T>(index, out val))
			{
				return defaultVal;
			}
			return val;
		}

		public bool TryParseCell<T>(string columnName, out T val)
		{
			int index;
			if (Grid.TryGetColumnIndex(columnName, out index))
			{
				return TryParseCell<T>(index, out val);
			}
			val = default(T);
			return false;
		}

		public bool TryParseCell<T>(int index, out T val)
		{
			if (!IsEmptyCell(index))
			{
				if (TryParse<T>(strings[index], out val))
				{
					return true;
				}
				Debug.LogError(ToErrorStringWithPrase(strings[index], index));
				return false;
			}
			val = default(T);
			return false;
		}

		public bool TryParseCellTypeOptional<T>(int index, T defaultVal, out T val)
		{
			if (!IsEmptyCell(index))
			{
				if (TryParse<T>(strings[index], out val))
				{
					return true;
				}
				val = defaultVal;
				return false;
			}
			val = defaultVal;
			return false;
		}

		public static bool TryParse<T>(string str, out T val)
		{
			try
			{
				Type typeFromHandle = typeof(T);
				if (typeFromHandle == typeof(string))
				{
					val = (T)(object)str;
				}
				else if (typeFromHandle.IsEnum)
				{
					val = (T)Enum.Parse(typeof(T), str);
				}
				else
				{
					if (typeFromHandle == typeof(Color))
					{
						Color color = Color.white;
						bool flag = ColorUtil.TryParseColor(str, ref color);
						val = (flag ? ((T)(object)color) : default(T));
						return flag;
					}
					if (typeFromHandle == typeof(int))
					{
						val = (T)(object)int.Parse(str);
					}
					else if (typeFromHandle == typeof(float))
					{
						val = (T)(object)WrapperUnityVersion.ParseFloatGlobal(str);
					}
					else if (typeFromHandle == typeof(double))
					{
						val = (T)(object)WrapperUnityVersion.ParseDoubleGlobal(str);
					}
					else if (typeFromHandle == typeof(bool))
					{
						val = (T)(object)bool.Parse(str);
					}
					else
					{
						TypeConverter converter = TypeDescriptor.GetConverter(typeFromHandle);
						val = (T)converter.ConvertFromString(str);
					}
				}
				return true;
			}
			catch
			{
				val = default(T);
				return false;
			}
		}

		public T[] ParseCellArray<T>(string columnName)
		{
			T[] val;
			if (!TryParseCellArray<T>(columnName, out val))
			{
				Debug.LogError(ToErrorStringWithPraseColumnName(columnName));
			}
			return val;
		}

		public T[] ParseCellArray<T>(int index)
		{
			T[] val;
			if (!TryParseCellArray<T>(index, out val))
			{
				Debug.LogError(ToErrorStringWithPraseColumnIndex(index));
			}
			return val;
		}

		public T[] ParseCellOptionalArray<T>(string columnName, T[] defaultVal)
		{
			T[] val;
			if (!TryParseCellArray<T>(columnName, out val))
			{
				return defaultVal;
			}
			return val;
		}

		public T[] ParseCellOptionalArray<T>(int index, T[] defaultVal)
		{
			T[] val;
			if (!TryParseCellArray<T>(index, out val))
			{
				return defaultVal;
			}
			return val;
		}

		public bool TryParseCellArray<T>(string columnName, out T[] val)
		{
			int index;
			if (Grid.TryGetColumnIndex(columnName, out index))
			{
				return TryParseCellArray<T>(index, out val);
			}
			val = null;
			return false;
		}

		public bool TryParseCellArray<T>(int index, out T[] val)
		{
			if (!IsEmptyCell(index))
			{
				if (TryParseArray<T>(strings[index], out val))
				{
					return true;
				}
				Debug.LogError(ToErrorStringWithPrase(strings[index], index));
				return false;
			}
			val = null;
			return false;
		}

		private bool TryParseArray<T>(string str, out T[] val)
		{
			string[] array = str.Split(',');
			int num = array.Length;
			val = new T[num];
			for (int i = 0; i < num; i++)
			{
				T val2;
				if (!TryParse<T>(array[i].Trim(), out val2))
				{
					return false;
				}
				val[i] = val2;
			}
			return true;
		}

		internal string ToDebugString()
		{
			char csvSeparator = Grid.CsvSeparator;
			string text = "";
			string[] array = strings;
			foreach (string text2 in array)
			{
				text = text + " " + text2 + csvSeparator;
			}
			return text;
		}

		public string ToErrorString(string msg)
		{
			if (!msg.EndsWith("\n"))
			{
				msg += "\n";
			}
			int num = DebugIndex + 1;
			if (string.IsNullOrEmpty(DebugInfo))
			{
				string sheetName = Grid.SheetName;
				msg = msg + sheetName + ":" + num + " ";
			}
			else
			{
				msg += DebugInfo;
			}
			return msg + ColorUtil.AddColorTag(ToDebugString(), Color.red) + "\n<b>" + Grid.Name + "</b>  : " + num;
		}

		public string ToStringOfFileSheetLine()
		{
			int num = rowIndex + 1;
			return "<b>" + Grid.Name + "</b>  : " + num;
		}

		private string ToErrorStringWithPraseColumnName(string columnName)
		{
			return ToErrorString(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridRowPraseColumnName, columnName));
		}

		private string ToErrorStringWithPraseColumnIndex(int index)
		{
			return ToErrorString(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridRowPraseColumnIndex, index));
		}

		private string ToErrorStringWithPrase(string column, int index)
		{
			return ToErrorString(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.StringGridRowPrase, index, column));
		}
	}
}
