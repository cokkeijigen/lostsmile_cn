using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public class AdvParamStructTbl
	{
		private Dictionary<string, AdvParamStruct> tbl = new Dictionary<string, AdvParamStruct>();

		private const int Version = 0;

		public Dictionary<string, AdvParamStruct> Tbl => tbl;

		public void AddSingle(StringGrid grid)
		{
			if (!Tbl.TryGetValue("", out var value))
			{
				value = new AdvParamStruct();
				Tbl.Add("", value);
			}
			value.AddData(grid);
		}

		public void AddTbl(StringGrid grid)
		{
			if (grid.Rows.Count < 3)
			{
				Debug.LogError(grid.Name + " is not Param Sheet");
				return;
			}
			StringGridRow names = grid.Rows[0];
			StringGridRow types = grid.Rows[1];
			StringGridRow fileTypes = grid.Rows[2];
			AdvParamStruct header = new AdvParamStruct(names, types, fileTypes);
			for (int i = 3; i < grid.Rows.Count; i++)
			{
				StringGridRow stringGridRow = grid.Rows[i];
				if (!stringGridRow.IsEmptyOrCommantOut)
				{
					AdvParamStruct value = new AdvParamStruct(header, stringGridRow);
					string text = stringGridRow.Strings[0];
					if (Tbl.ContainsKey(text))
					{
						stringGridRow.ToErrorString(text + " is already contains ");
					}
					else
					{
						Tbl.Add(text, value);
					}
				}
			}
		}

		internal AdvParamStructTbl Clone()
		{
			AdvParamStructTbl advParamStructTbl = new AdvParamStructTbl();
			foreach (KeyValuePair<string, AdvParamStruct> item in Tbl)
			{
				advParamStructTbl.Tbl.Add(item.Key, item.Value.Clone());
			}
			return advParamStructTbl;
		}

		internal void InitDefaultNormal(AdvParamStructTbl src)
		{
			foreach (KeyValuePair<string, AdvParamStruct> item in src.Tbl)
			{
				if (Tbl.TryGetValue(item.Key, out var value))
				{
					value.InitDefaultNormal(item.Value);
				}
				else
				{
					Debug.LogError("Param: " + item.Key + "  is not found in default param");
				}
			}
		}

		public void Write(BinaryWriter writer, AdvParamData.FileType fileType)
		{
			writer.Write(0);
			writer.Write(Tbl.Count);
			foreach (KeyValuePair<string, AdvParamStruct> keyValue in Tbl)
			{
				writer.Write(keyValue.Key);
				writer.WriteBuffer(delegate(BinaryWriter x)
				{
					keyValue.Value.Write(x, fileType);
				});
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
				if (Tbl.ContainsKey(key))
				{
					reader.ReadBuffer(delegate(BinaryReader x)
					{
						Tbl[key].Read(x, fileType);
					});
				}
				else
				{
					reader.SkipBuffer();
				}
			}
		}
	}
}
