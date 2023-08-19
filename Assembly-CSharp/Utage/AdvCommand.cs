using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public abstract class AdvCommand
	{
		private static bool isEditorErrorCheck;

		private List<AssetFile> loadFileList;

		public static bool IsEditorErrorCheck
		{
			get
			{
				return isEditorErrorCheck;
			}
			set
			{
				isEditorErrorCheck = value;
			}
		}

		public static bool IsEditorErrorCheckWaitType { get; set; }

		public StringGridRow RowData { get; set; }

		internal AdvEntityData EntityData { get; set; }

		internal string Id { get; set; }

		public bool IsEntityType
		{
			get
			{
				return EntityData != null;
			}
		}

		public List<AssetFile> LoadFileList
		{
			get
			{
				return loadFileList;
			}
		}

		public AdvScenarioThread CurrentTread { get; set; }

		public virtual bool IsIfCommand
		{
			get
			{
				return false;
			}
		}

		protected AdvCommand(StringGridRow row)
		{
			RowData = row;
		}

		public virtual string[] GetJumpLabels()
		{
			return null;
		}

		public bool IsExistLoadFile()
		{
			if (loadFileList != null)
			{
				return loadFileList.Count > 0;
			}
			return false;
		}

		public AssetFile AddLoadFile(string path, IAssetFileSettingData settingData)
		{
			if (IsEntityType)
			{
				return null;
			}
			return AddLoadFileSub(AssetFileManager.GetFileCreateIfMissing(path, settingData));
		}

		public void AddLoadGraphic(AdvGraphicInfoList graphic)
		{
			foreach (AdvGraphicInfo info in graphic.InfoList)
			{
				AddLoadGraphic(info);
			}
		}

		public void AddLoadGraphic(AdvGraphicInfo graphic)
		{
			if (graphic == null || IsEntityType)
			{
				return;
			}
			AddLoadFileSub(graphic.File);
			if (graphic.SettingData is AdvCharacterSettingData)
			{
				AdvCharacterSettingData advCharacterSettingData = graphic.SettingData as AdvCharacterSettingData;
				if (advCharacterSettingData.Icon != null && advCharacterSettingData.Icon.File != null)
				{
					AddLoadFileSub(advCharacterSettingData.Icon.File);
				}
			}
		}

		public void AddLoadFile(AssetFile file)
		{
			if (!IsEntityType)
			{
				AddLoadFileSub(file);
			}
		}

		private AssetFile AddLoadFileSub(AssetFile file)
		{
			if (loadFileList == null)
			{
				loadFileList = new List<AssetFile>();
			}
			if (file == null)
			{
				if (!IsEditorErrorCheck)
				{
					Debug.LogError("file is not found");
				}
			}
			else
			{
				loadFileList.Add(file);
			}
			return file;
		}

		public void Download(AdvDataManager dataManager)
		{
			if (loadFileList == null)
			{
				return;
			}
			foreach (AssetFile loadFile in loadFileList)
			{
				AssetFileManager.Download(loadFile);
			}
		}

		public void Load()
		{
			if (loadFileList == null)
			{
				return;
			}
			foreach (AssetFile loadFile in loadFileList)
			{
				AssetFileManager.Load(loadFile, this);
			}
		}

		public bool IsLoadEnd()
		{
			if (loadFileList != null)
			{
				foreach (AssetFile loadFile in loadFileList)
				{
					if (!loadFile.IsLoadEnd)
					{
						return false;
					}
				}
			}
			return true;
		}

		public abstract void DoCommand(AdvEngine engine);

		public void Unload()
		{
			if (loadFileList == null)
			{
				return;
			}
			foreach (AssetFile loadFile in loadFileList)
			{
				loadFile.Unuse(this);
			}
		}

		public virtual bool Wait(AdvEngine engine)
		{
			return false;
		}

		public virtual bool IsTypePage()
		{
			return false;
		}

		public virtual bool IsTypePageEnd()
		{
			return false;
		}

		public virtual void InitFromPageData(AdvScenarioPageData pageData)
		{
		}

		public virtual string[] GetExtraCommandIdArray(AdvCommand next)
		{
			return null;
		}

		public string ToErrorString(string msg)
		{
			return RowData.ToErrorString(msg);
		}

		public bool IsEmptyCell(AdvColumnName name)
		{
			return IsEmptyCell(name.QuickToString());
		}

		public bool IsEmptyCell(string name)
		{
			return RowData.IsEmptyCell(name);
		}

		public T ParseCell<T>(AdvColumnName name)
		{
			return ParseCell<T>(name.QuickToString());
		}

		public T ParseCell<T>(string name)
		{
			return RowData.ParseCell<T>(name);
		}

		public T ParseCellOptional<T>(AdvColumnName name, T defaultVal)
		{
			return ParseCellOptional(name.QuickToString(), defaultVal);
		}

		public T ParseCellOptional<T>(string name, T defaultVal)
		{
			return RowData.ParseCellOptional(name, defaultVal);
		}

		public bool TryParseCell<T>(AdvColumnName name, out T val)
		{
			return TryParseCell<T>(name.QuickToString(), out val);
		}

		public bool TryParseCell<T>(string name, out T val)
		{
			return RowData.TryParseCell<T>(name, out val);
		}

		public T[] ParseCellArray<T>(AdvColumnName name)
		{
			return ParseCellArray<T>(name.QuickToString());
		}

		public T[] ParseCellArray<T>(string name)
		{
			return RowData.ParseCellArray<T>(name);
		}

		public T[] ParseCellOptionalArray<T>(AdvColumnName name, T[] defaultArray)
		{
			return ParseCellOptionalArray(name.QuickToString(), defaultArray);
		}

		public T[] ParseCellOptionalArray<T>(string name, T[] defaultArray)
		{
			return RowData.ParseCellOptionalArray(name, defaultArray);
		}

		public bool TryParseCellArray<T>(AdvColumnName name, out T[] array)
		{
			return TryParseCellArray<T>(name.QuickToString(), out array);
		}

		public bool TryParseCellArray<T>(string name, out T[] array)
		{
			return RowData.TryParseCellArray<T>(name, out array);
		}

		public T? ParseCellOptionalNull<T>(AdvColumnName name) where T : struct
		{
			return ParseCellOptionalNull<T>(name.QuickToString());
		}

		public T? ParseCellOptionalNull<T>(string name) where T : struct
		{
			if (IsEmptyCell(name))
			{
				return null;
			}
			return RowData.ParseCell<T>(name);
		}

		public string ParseCellLocalizedText()
		{
			return ParseCellLocalized(AdvColumnName.Text.QuickToString());
		}

		public string ParseCellLocalized(string defaultColumnName)
		{
			return AdvParser.ParseCellLocalizedText(RowData, defaultColumnName);
		}

		public virtual string ParseScenarioLabel(AdvColumnName name)
		{
			string scenarioLabel;
			if (!AdvCommandParser.TryParseScenarioLabel(RowData, name, out scenarioLabel))
			{
				Debug.LogError(ToErrorString(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotScenarioLabel, ParseCell<string>(name))));
			}
			return scenarioLabel;
		}
	}
}
