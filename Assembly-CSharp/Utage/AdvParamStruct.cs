using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	public class AdvParamStruct
	{
		private Dictionary<string, AdvParamData> tbl = new Dictionary<string, AdvParamData>();

		private const int Version = 0;

		public Dictionary<string, AdvParamData> Tbl => tbl;

		public AdvParamStruct()
		{
		}

		public void AddData(StringGrid grid)
		{
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex >= grid.DataTopRow && !row.IsEmptyOrCommantOut)
				{
					AdvParamData advParamData = new AdvParamData();
					if (!advParamData.TryParse(row))
					{
						Debug.LogError(row.ToErrorString(" Parse Error"));
					}
					else if (Tbl.ContainsKey(advParamData.Key))
					{
						Debug.LogError(row.ToErrorString(advParamData.Key + " is already contaisn"));
					}
					else
					{
						Tbl.Add(advParamData.Key, advParamData);
					}
				}
			}
		}

		public AdvParamStruct(StringGridRow names, StringGridRow types, StringGridRow fileTypes)
		{
			for (int i = 1; i < names.Length; i++)
			{
				string text = names.Strings[i];
				if (!text.StartsWith("//") && text.Length > 0)
				{
					AdvParamData advParamData = new AdvParamData();
					string type = ((i < types.Length) ? types.Strings[i] : "");
					string fileType = ((i < fileTypes.Length) ? fileTypes.Strings[i] : "");
					if (!advParamData.TryParse(text, type, fileType))
					{
						Debug.LogError($"{names.Grid.Name} Header [<b>{i}</b>]: ");
					}
					else
					{
						Tbl.Add(advParamData.Key, advParamData);
					}
				}
			}
		}

		public AdvParamStruct(AdvParamStruct header, StringGridRow values)
		{
			int num = 0;
			foreach (KeyValuePair<string, AdvParamData> item in header.Tbl)
			{
				int num2 = ToIndexCommentOuted(values.Grid.Rows[0], num + 1);
				string text = ((num2 < values.Strings.Length) ? values.Strings[num2] : "");
				AdvParamData advParamData = new AdvParamData();
				if (!advParamData.TryParse(item.Value, text))
				{
					Debug.LogError(values.ToErrorString(" Parse Error <b>" + text + "</b>type = " + item.Value.Type));
				}
				else
				{
					Tbl.Add(advParamData.Key, advParamData);
					num++;
				}
			}
		}

		private int ToIndexCommentOuted(StringGridRow row, int index0)
		{
			int num = 0;
			for (num = 0; num < row.Strings.Length; num++)
			{
				string text = row.Strings[num];
				if (text.Length <= 1 || text[0] != '/' || text[1] != '/')
				{
					if (index0 <= 0)
					{
						break;
					}
					index0--;
				}
			}
			return num;
		}

		internal AdvParamStruct Clone()
		{
			AdvParamStruct advParamStruct = new AdvParamStruct();
			foreach (KeyValuePair<string, AdvParamData> item in Tbl)
			{
				AdvParamData advParamData = new AdvParamData();
				advParamData.Copy(item.Value);
				advParamStruct.Tbl.Add(item.Key, advParamData);
			}
			return advParamStruct;
		}

		internal void InitDefaultNormal(AdvParamStruct src)
		{
			foreach (KeyValuePair<string, AdvParamData> item in src.Tbl)
			{
				if (item.Value.SaveFileType != AdvParamData.FileType.System)
				{
					if (Tbl.TryGetValue(item.Key, out var value))
					{
						value.Copy(item.Value);
					}
					else
					{
						Debug.LogError("Param: " + item.Key + "  is not found in default param");
					}
				}
			}
		}

		public int CountFileType(AdvParamData.FileType fileType)
		{
			int num = 0;
			foreach (KeyValuePair<string, AdvParamData> item in Tbl)
			{
				if (item.Value.SaveFileType == fileType)
				{
					num++;
				}
			}
			return num;
		}

		public List<AdvParamData> GetFileTypeList(AdvParamData.FileType fileType)
		{
			List<AdvParamData> list = new List<AdvParamData>();
			foreach (KeyValuePair<string, AdvParamData> item in Tbl)
			{
				if (item.Value.SaveFileType == fileType)
				{
					list.Add(item.Value);
				}
			}
			return list;
		}

		public void Write(BinaryWriter writer, AdvParamData.FileType fileType)
		{
			int value = CountFileType(fileType);
			writer.Write(0);
			writer.Write(value);
			foreach (KeyValuePair<string, AdvParamData> item in Tbl)
			{
				if (item.Value.SaveFileType == fileType)
				{
					writer.Write(item.Value.Key);
					writer.Write(item.Value.ParameterString);
				}
			}
		}

		public void Read(BinaryReader reader, AdvParamData.FileType fileType)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				string key = reader.ReadString();
				string paramString = reader.ReadString();
				if (Tbl.TryGetValue(key, out var value) && value.SaveFileType == fileType)
				{
					value.Read(paramString);
				}
			}
		}
	}
}
