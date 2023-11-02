using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[Serializable]
	public class AdvParamManager : AdvSettingBase
	{
		public class IoInerface : IBinaryIO
		{
			private AdvParamData.FileType FileType { get; set; }

			private AdvParamManager Param { get; set; }

			public string SaveKey => "ParamManagerIoInerface";

			public IoInerface(AdvParamManager param, AdvParamData.FileType fileType)
			{
				Param = param;
				FileType = fileType;
			}

			public void OnWrite(BinaryWriter writer)
			{
				Param.Write(writer, FileType);
			}

			public void OnRead(BinaryReader reader)
			{
				Param.Read(reader, FileType);
			}
		}

		public const string DefaultSheetName = "Param";

		private const string KeyPattern = "(.+)\\[(.+)\\]\\.(.+)";

		private static readonly Regex KeyRegix = new Regex("(.+)\\[(.+)\\]\\.(.+)", RegexOptions.IgnorePatternWhitespace);

		private Dictionary<string, AdvParamStructTbl> structTbl = new Dictionary<string, AdvParamStructTbl>();

		private const int Version = 0;

		private IoInerface systemData;

		private IoInerface defaultData;

		public bool IsInit { get; protected set; }

		public Dictionary<string, AdvParamStructTbl> StructTbl => structTbl;

		public bool HasChangedSystemParam { get; set; }

		public AdvParamManager DefaultParameter { get; set; }

		public IoInerface SystemData
		{
			get
			{
				if (systemData == null)
				{
					systemData = new IoInerface(this, AdvParamData.FileType.System);
				}
				return systemData;
			}
		}

		public IoInerface DefaultData
		{
			get
			{
				if (defaultData == null)
				{
					defaultData = new IoInerface(this, AdvParamData.FileType.Default);
				}
				return defaultData;
			}
		}

		internal static bool ParseKey(string key, out string structName, out string indexKey, out string valueKey)
		{
			structName = (indexKey = (valueKey = ""));
			if (!key.Contains("["))
			{
				return false;
			}
			Match match = KeyRegix.Match(key);
			if (!match.Success)
			{
				return false;
			}
			structName = match.Groups[1].Value + "{}";
			indexKey = match.Groups[2].Value;
			valueKey = match.Groups[3].Value;
			return true;
		}

		private bool TryGetParamData(string key, out AdvParamData data)
		{
			data = null;
			if (!ParseKey(key, out var structName, out var indexKey, out var valueKey))
			{
				return GetDefault()?.Tbl.TryGetValue(key, out data) ?? false;
			}
			if (!TryGetParamTbl(structName, indexKey, out var paramStruct))
			{
				return false;
			}
			return paramStruct.Tbl.TryGetValue(valueKey, out data);
		}

		public bool TryGetParamTbl(string structName, string indexKey, out AdvParamStruct paramStruct)
		{
			paramStruct = null;
			if (!StructTbl.ContainsKey(structName))
			{
				return false;
			}
			if (!StructTbl[structName].Tbl.ContainsKey(indexKey))
			{
				return false;
			}
			paramStruct = StructTbl[structName].Tbl[indexKey];
			return true;
		}

		public AdvParamStruct GetDefault()
		{
			if (!StructTbl.ContainsKey("Param"))
			{
				return null;
			}
			return StructTbl["Param"].Tbl[""];
		}

		protected override void OnParseGrid(StringGrid grid)
		{
			if (base.GridList.Count == 0)
			{
				Debug.LogError("Old Version Reimport Excel Scenario Data");
				return;
			}
			string sheetName = grid.SheetName;
			if (!StructTbl.TryGetValue(sheetName, out var value))
			{
				value = new AdvParamStructTbl();
				StructTbl.Add(sheetName, value);
			}
			if (sheetName == "Param")
			{
				value.AddSingle(grid);
			}
			else
			{
				value.AddTbl(grid);
			}
		}

		internal void InitDefaultAll(AdvParamManager src)
		{
			DefaultParameter = src;
			StructTbl.Clear();
			foreach (KeyValuePair<string, AdvParamStructTbl> item in src.StructTbl)
			{
				StructTbl.Add(item.Key, item.Value.Clone());
			}
			IsInit = true;
		}

		internal void InitDefaultNormal(AdvParamManager src)
		{
			foreach (KeyValuePair<string, AdvParamStructTbl> item in src.StructTbl)
			{
				if (StructTbl.TryGetValue(item.Key, out var value))
				{
					value.InitDefaultNormal(item.Value);
				}
				else
				{
					Debug.LogError("Param: " + item.Key + "  is not found in default param");
				}
			}
		}

		public int GetParameterInt(string key)
		{
			return GetParameter<int>(key);
		}

		public void SetParameterInt(string key, int value)
		{
			SetParameter(key, value);
		}

		public float GetParameterFloat(string key)
		{
			return GetParameter<float>(key);
		}

		public void SetParameterFloat(string key, float value)
		{
			SetParameter(key, value);
		}

		public bool GetParameterBoolean(string key)
		{
			return GetParameter<bool>(key);
		}

		public void SetParameterBoolean(string key, bool value)
		{
			SetParameter(key, value);
		}

		public string GetParameterString(string key)
		{
			return GetParameter<string>(key);
		}

		public void SetParameterString(string key, string value)
		{
			SetParameter(key, value);
		}

		public T GetParameter<T>(string key)
		{
			return (T)GetParameter(key);
		}

		public void SetParameter<T>(string key, T value)
		{
			SetParameter(key, (object)value);
		}

		public void SetParameter(string key, object parameter)
		{
			if (!TrySetParameter(key, parameter))
			{
				Debug.LogError(key + " is not parameter name");
			}
		}

		public bool TrySetParameter(string key, object parameter)
		{
			if (!CheckSetParameterSub(key, parameter, out var data))
			{
				return false;
			}
			data.Parameter = parameter;
			if (data.SaveFileType == AdvParamData.FileType.System)
			{
				HasChangedSystemParam = true;
			}
			return true;
		}

		public bool TryGetParameter(string key, out object parameter)
		{
			if (TryGetParamData(key, out var data))
			{
				parameter = data.Parameter;
				return true;
			}
			parameter = null;
			return false;
		}

		public bool CheckSetParameter(string key, object parameter)
		{
			AdvParamData data;
			return CheckSetParameterSub(key, parameter, out data);
		}

		public object GetParameter(string key)
		{
			if (TryGetParameter(key, out var parameter))
			{
				return parameter;
			}
			return null;
		}

		public ExpressionParser CreateExpression(string exp)
		{
			return new ExpressionParser(exp, GetParameter, CheckSetParameter);
		}

		public object CalcExpressionNotSetParam(string exp)
		{
			ExpressionParser expressionParser = CreateExpression(exp);
			if (string.IsNullOrEmpty(expressionParser.ErrorMsg))
			{
				return expressionParser.CalcExp(GetParameter, CheckSetParameter);
			}
			throw new Exception(expressionParser.ErrorMsg);
		}

		public object CalcExpression(ExpressionParser exp)
		{
			return exp.CalcExp(GetParameter, TrySetParameter);
		}

		public float CalcExpressionFloat(ExpressionParser exp)
		{
			object obj = exp.CalcExp(GetParameter, TrySetParameter);
			if (obj.GetType() == typeof(int))
			{
				return (int)obj;
			}
			if (obj.GetType() == typeof(float))
			{
				return (float)obj;
			}
			Debug.LogError("Float Cast error : " + exp.Exp);
			return 0f;
		}

		public int CalcExpressionInt(ExpressionParser exp)
		{
			object obj = exp.CalcExp(GetParameter, TrySetParameter);
			if (obj.GetType() == typeof(int))
			{
				return (int)obj;
			}
			if (obj.GetType() == typeof(float))
			{
				return (int)(float)obj;
			}
			Debug.LogError("Int Cast error : " + exp.Exp);
			return 0;
		}

		public ExpressionParser CreateExpressionBoolean(string exp)
		{
			return new ExpressionParser(exp, GetParameter, CheckSetParameter, true);
		}

		public bool CalcExpressionBoolean(ExpressionParser exp)
		{
			bool result = exp.CalcExpBoolean(GetParameter, TrySetParameter);
			if (!string.IsNullOrEmpty(exp.ErrorMsg))
			{
				Debug.LogError(exp.ErrorMsg);
			}
			return result;
		}

		public bool CalcExpressionBoolean(string exp)
		{
			return CalcExpressionBoolean(CreateExpressionBoolean(exp));
		}

		private bool CheckSetParameterSub(string key, object parameter, out AdvParamData data)
		{
			if (!TryGetParamData(key, out data))
			{
				return false;
			}
			if (data.SaveFileType == AdvParamData.FileType.Const)
			{
				return false;
			}
			if ((data.Type == AdvParamData.ParamType.Bool || parameter is bool) && (data.Type != 0 || !(parameter is bool)))
			{
				return false;
			}
			if (parameter is string && data.Type != AdvParamData.ParamType.String)
			{
				return false;
			}
			if (data.Type == AdvParamData.ParamType.String && parameter is bool)
			{
				return false;
			}
			return true;
		}

		public void Write(BinaryWriter writer, AdvParamData.FileType fileType)
		{
			writer.Write(0);
			writer.Write(StructTbl.Count);
			foreach (KeyValuePair<string, AdvParamStructTbl> keyValue in StructTbl)
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
			if (fileType == AdvParamData.FileType.Default)
			{
				InitDefaultNormal(DefaultParameter);
			}
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
				if (StructTbl.ContainsKey(key))
				{
					reader.ReadBuffer(delegate(BinaryReader x)
					{
						StructTbl[key].Read(x, fileType);
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
