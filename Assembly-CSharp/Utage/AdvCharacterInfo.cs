using UnityEngine;

namespace Utage
{
	public class AdvCharacterInfo
	{
		public string Label { get; private set; }

		public string NameText { get; private set; }

		public string Pattern { get; private set; }

		public bool IsHide { get; private set; }

		public AdvGraphicInfoList Graphic { get; private set; }

		public string LocalizeNameText
		{
			get
			{
				return LanguageManagerBase.Instance.LocalizeText(TextParser.MakeLogText(NameText));
			}
		}

		public static AdvCharacterInfo Create(AdvCommand command, AdvSettingDataManager dataManager)
		{
			if (command.IsEmptyCell(AdvColumnName.Arg1))
			{
				return null;
			}
			string characterLabel;
			string text = (characterLabel = command.ParseCell<string>(AdvColumnName.Arg1));
			bool isHide = false;
			string erroMsg = "";
			string text2 = ParserUtil.ParseTagTextToString(command.ParseCellOptional(AdvColumnName.Arg2, ""), delegate(string tagName, string arg)
			{
				bool flag = false;
				if (!(tagName == "Off"))
				{
					if (tagName == "Character")
					{
						characterLabel = arg;
					}
					else
					{
						erroMsg = "Unkownn Tag <" + tagName + ">";
						flag = true;
					}
				}
				else
				{
					isHide = true;
				}
				return !flag;
			});
			if (!string.IsNullOrEmpty(erroMsg))
			{
				Debug.LogError(erroMsg);
				return null;
			}
			if (!dataManager.CharacterSetting.Contains(characterLabel))
			{
				return new AdvCharacterInfo(characterLabel, text, text2, isHide, null);
			}
			AdvCharacterSettingData characterData = dataManager.CharacterSetting.GetCharacterData(characterLabel, text2);
			if (characterData == null)
			{
				Debug.LogError(command.ToErrorString(characterLabel + ", " + text2 + " is not contained in Chactecter Sheet"));
				return null;
			}
			if (!string.IsNullOrEmpty(characterData.NameText) && text == characterLabel)
			{
				text = characterData.NameText;
			}
			return new AdvCharacterInfo(characterLabel, text, text2, isHide, characterData.Graphic);
		}

		private AdvCharacterInfo(string label, string nameText, string pattern, bool isHide, AdvGraphicInfoList graphic)
		{
			Label = label;
			NameText = nameText;
			Pattern = pattern;
			IsHide = isHide;
			Graphic = graphic;
		}
	}
}
