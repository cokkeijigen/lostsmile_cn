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

		public string LocalizeNameText => LanguageManagerBase.Instance.LocalizeText(TextParser.MakeLogText(NameText));

		public static AdvCharacterInfo Create(AdvCommand command, AdvSettingDataManager dataManager)
		{
			if (command.IsEmptyCell(AdvColumnName.Arg1))
			{
				return null;
			}

			// iTsukezigen++
			string text = command.ParseCell<string>(AdvColumnName.Arg1);
            string characterLabel = GetRawCharacterName(text);

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

        // iTsukezigen++
        private static string GetRawCharacterName(string text)
		{
			switch (text) {
                case "美铃":
                    return "美鈴";
                case "春纪":
                    return "春紀";
                case "少":
                    return "スクナ";
                case "胡桃":
                    return "胡桃";
                case "美广":
                    return "みひろ";
                case "巡":
                    return "めぐる";
                case "结李":
                    return "結李";
                case "利夫":
                    return "利夫";
                case "有纪":
                    return "有紀";
                case "纯":
                    return "純";
                case "沙希":
                    return "サキ";
                case "海洋之神":
                    return "沖ツ御神";
                case "棉花糖":
                    return "わたあめ";
                case "凑":
                    return "湊";
                case "彩芽":
                    return "あやめ";
                case "隼人":
                    return "隼人";
                case "由希子":
                    return "由希子";
                case "珠珠":
                    return "シュシュ";
                default:
					return text;
			}
		}
		// end++

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
