using UnityEngine;

namespace Utage
{
	public class AdvParamData
	{
		public enum ParamType
		{
			Bool,
			Float,
			Int,
			String
		}

		public enum FileType
		{
			Default,
			System,
			Const
		}

		private string key;

		private ParamType type;

		private object parameter;

		private string parameterString;

		private FileType fileType;

		public string Key => key;

		public ParamType Type => type;

		public object Parameter
		{
			get
			{
				if (parameter == null)
				{
					ParseParameterString();
				}
				return parameter;
			}
			set
			{
				switch (type)
				{
				case ParamType.Bool:
					parameter = (bool)value;
					break;
				case ParamType.Float:
					parameter = ExpressionCast.ToFloat(value);
					break;
				case ParamType.Int:
					parameter = ExpressionCast.ToInt(value);
					break;
				case ParamType.String:
					parameter = (string)value;
					break;
				}
				parameterString = parameter.ToString();
			}
		}

		public string ParameterString => parameterString;

		public FileType SaveFileType => fileType;

		public bool TryParse(string name, string type, string fileType)
		{
			key = name;
			if (!ParserUtil.TryParaseEnum<ParamType>(type, out this.type))
			{
				Debug.LogError(type + " is not ParamType");
				return false;
			}
			if (string.IsNullOrEmpty(fileType))
			{
				this.fileType = FileType.Default;
			}
			else if (!ParserUtil.TryParaseEnum<FileType>(fileType, out this.fileType))
			{
				Debug.LogError(fileType + " is not FileType");
				return false;
			}
			return true;
		}

		public bool TryParse(AdvParamData src, string value)
		{
			key = src.Key;
			type = src.Type;
			fileType = src.SaveFileType;
			parameterString = value;
			try
			{
				ParseParameterString();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool TryParse(StringGridRow row)
		{
			string value = AdvParser.ParseCell<string>(row, AdvColumnName.Label);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			key = value;
			type = AdvParser.ParseCell<ParamType>(row, AdvColumnName.Type);
			parameterString = AdvParser.ParseCellOptional(row, AdvColumnName.Value, "");
			fileType = AdvParser.ParseCellOptional(row, AdvColumnName.FileType, FileType.Default);
			try
			{
				ParseParameterString();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void Copy(AdvParamData src)
		{
			key = src.Key;
			type = src.type;
			parameterString = src.parameterString;
			ParseParameterString();
			fileType = src.fileType;
		}

		public void CopySaveData(AdvParamData src)
		{
			if (key != src.Key)
			{
				Debug.LogError(src.key + "is diffent name of Saved param");
			}
			if (type != src.type)
			{
				Debug.LogError(string.Concat(src.type, "is diffent type of Saved param"));
			}
			if (fileType != src.fileType)
			{
				Debug.LogError(string.Concat(src.fileType, "is diffent fileType of Saved param"));
			}
			parameterString = src.parameterString;
			ParseParameterString();
		}

		public void Read(string paramString)
		{
			parameterString = paramString;
			ParseParameterString();
		}

		private void ParseParameterString()
		{
			switch (type)
			{
			case ParamType.Bool:
				parameter = bool.Parse(parameterString);
				break;
			case ParamType.Float:
				parameter = WrapperUnityVersion.ParseFloatGlobal(parameterString);
				break;
			case ParamType.Int:
				parameter = int.Parse(parameterString);
				break;
			case ParamType.String:
				parameter = parameterString;
				break;
			}
		}
	}
}
