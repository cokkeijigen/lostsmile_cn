using UnityEngine;

namespace Utage
{
	public class AdvCharacterSetting : AdvSettingDataDictinoayBase<AdvCharacterSettingData>
	{
		private DictionaryString defaultKey = new DictionaryString();

		protected override bool TryParseContinues(AdvCharacterSettingData last, StringGridRow row)
		{
			if (last == null)
			{
				return false;
			}
			string value = AdvParser.ParseCellOptional(row, AdvColumnName.CharacterName, "");
			string value2 = AdvParser.ParseCellOptional(row, AdvColumnName.Pattern, "");
			if (string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value2))
			{
				last.AddGraphicInfo(row);
				return true;
			}
			return false;
		}

		protected override AdvCharacterSettingData ParseFromStringGridRow(AdvCharacterSettingData last, StringGridRow row)
		{
			string text = AdvParser.ParseCellOptional(row, AdvColumnName.CharacterName, "");
			string pattern = AdvParser.ParseCellOptional(row, AdvColumnName.Pattern, "");
			string text2 = AdvParser.ParseCellOptional(row, AdvColumnName.NameText, "");
			if (string.IsNullOrEmpty(text))
			{
				if (last == null)
				{
					Debug.LogError(row.ToErrorString("Not Found Chacter Name"));
					return null;
				}
				text = last.Name;
			}
			if (string.IsNullOrEmpty(text2))
			{
				text2 = ((last == null || !(text == last.Name)) ? text : last.NameText);
			}
			AdvCharacterSettingData advCharacterSettingData = new AdvCharacterSettingData();
			advCharacterSettingData.Init(text, pattern, text2, row);
			if (!base.Dictionary.ContainsKey(advCharacterSettingData.Key))
			{
				AddData(advCharacterSettingData);
				if (!defaultKey.ContainsKey(text))
				{
					defaultKey.Add(text, advCharacterSettingData.Key);
				}
				return advCharacterSettingData;
			}
			Debug.LogError("" + row.ToErrorString(ColorUtil.AddColorTag(advCharacterSettingData.Key, Color.red) + "  is already contains"));
			return null;
		}

		public override void BootInit(AdvSettingDataManager dataManager)
		{
			foreach (AdvCharacterSettingData item in base.List)
			{
				item.BootInit(dataManager);
			}
		}

		public override void DownloadAll()
		{
			foreach (AdvCharacterSettingData item in base.List)
			{
				item.Graphic.DownloadAll();
			}
		}

		public bool Contains(string name)
		{
			return defaultKey.ContainsKey(name);
		}

		public AdvGraphicInfoList KeyToGraphicInfo(string key)
		{
			AdvCharacterSettingData advCharacterSettingData = FindData(key);
			if (advCharacterSettingData == null)
			{
				Debug.LogError("Not contains " + key + " in Character sheet");
				return null;
			}
			return advCharacterSettingData.Graphic;
		}

		internal AdvCharacterSettingData GetCharacterData(string characterLabel, string patternLabel)
		{
			if (string.IsNullOrEmpty(patternLabel))
			{
				return FindData(defaultKey.Get(characterLabel));
			}
			AdvCharacterSettingData advCharacterSettingData = FindData(ToDataKey(characterLabel, patternLabel));
			if (advCharacterSettingData == null)
			{
				advCharacterSettingData = FindData(defaultKey.Get(characterLabel));
				if (advCharacterSettingData != null && advCharacterSettingData.Graphic.IsDefaultFileType)
				{
					return null;
				}
			}
			return advCharacterSettingData;
		}

		private AdvCharacterSettingData FindData(string key)
		{
			AdvCharacterSettingData value;
			if (!base.Dictionary.TryGetValue(key, out value))
			{
				return null;
			}
			return value;
		}

		internal static string ToDataKey(string name, string label)
		{
			return string.Format("{0},{1}", name, label);
		}
	}
}
