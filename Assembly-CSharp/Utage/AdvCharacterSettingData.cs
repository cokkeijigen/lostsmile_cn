using System;
using UnityEngine;

namespace Utage
{
	public class AdvCharacterSettingData : AdvSettingDictinoayItemBase
	{
		public delegate void ParseCustomFileTypeRootDir(string fileType, ref string rootDir);

		public class IconInfo
		{
			public enum Type
			{
				None,
				IconImage,
				DicingPattern,
				RectImage
			}

			public Type IconType { get; internal set; }

			public string FileName { get; internal set; }

			public AssetFile File { get; set; }

			public Rect IconRect { get; internal set; }

			public string IconSubFileName { get; internal set; }

			public IconInfo(StringGridRow row)
			{
				FileName = AdvParser.ParseCellOptional(row, AdvColumnName.Icon, "");
				if (!string.IsNullOrEmpty(FileName))
				{
					if (!AdvParser.IsEmptyCell(row, AdvColumnName.IconSubFileName))
					{
						IconType = Type.DicingPattern;
						IconSubFileName = AdvParser.ParseCell<string>(row, AdvColumnName.IconSubFileName);
					}
					else
					{
						IconType = Type.IconImage;
					}
				}
				else if (!AdvParser.IsEmptyCell(row, AdvColumnName.IconRect))
				{
					float[] array = row.ParseCellArray<float>(AdvColumnName.IconRect.QuickToString());
					if (array.Length == 4)
					{
						IconType = Type.RectImage;
						IconRect = new Rect(array[0], array[1], array[2], array[3]);
					}
					else
					{
						Debug.LogError(row.ToErrorString("IconRect. Array size is not 4"));
					}
				}
				else
				{
					IconType = Type.None;
				}
			}

			public void BootInit(Func<string, string> FileNameToPath)
			{
				if (!string.IsNullOrEmpty(FileName))
				{
					File = AssetFileManager.GetFileCreateIfMissing(FileNameToPath(FileName));

                }
			}
		}

		public static ParseCustomFileTypeRootDir CallbackParseCustomFileTypeRootDir;

		private string name;

		private string pattern;

		private string nameText;

		private AdvGraphicInfoList graphic;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string Pattern
		{
			get
			{
				return pattern;
			}
		}

		public string NameText
		{
			get
			{
				return nameText;
			}
		}

		public AdvGraphicInfoList Graphic
		{
			get
			{
				return graphic;
			}
		}

		public IconInfo Icon { get; private set; }

		public override bool InitFromStringGridRow(StringGridRow row)
		{
			Debug.LogError("Not Use");
			return false;
		}

		internal void Init(string name, string pattern, string nameText, StringGridRow row)
		{
			this.name = name;
			this.pattern = pattern;
			base.RowData = row;
			InitKey(AdvCharacterSetting.ToDataKey(name, pattern));
			this.nameText = nameText;
			graphic = new AdvGraphicInfoList(base.Key);
			if (!AdvParser.IsEmptyCell(row, AdvColumnName.FileName))
			{
				AddGraphicInfo(row);
			}
			Icon = new IconInfo(row);
		}

		internal void BootInit(AdvSettingDataManager dataManager)
		{
			Graphic.BootInit((string fileName, string fileType) => FileNameToPath(fileName, fileType, dataManager.BootSetting), dataManager);
			Icon.BootInit((string fileName) => dataManager.BootSetting.CharacterDirInfo.FileNameToPath(fileName));
		}

		private string FileNameToPath(string fileName, string fileType, AdvBootSetting settingData)
		{
			string rootDir = null;
			if (CallbackParseCustomFileTypeRootDir != null)
			{
				CallbackParseCustomFileTypeRootDir(fileType, ref rootDir);
				if (rootDir != null)
				{
					return FilePathUtil.Combine(settingData.ResourceDir, rootDir, fileName);
				}
			}
			return settingData.CharacterDirInfo.FileNameToPath(fileName);
		}

		internal void AddGraphicInfo(StringGridRow row)
		{
			Graphic.Add("Character", row, this);
		}
	}
}
