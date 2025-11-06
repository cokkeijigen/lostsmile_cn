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

			string text = command.ParseCell<string>(AdvColumnName.Arg1);
            string characterLabel = GetRawCharacterName(text); // iTsukezigen++ 

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
            return text switch
            {
                "海洋之神" => "沖ツ御神",
                "棉花糖" => "わたあめ",
                "由希子" => "由希子",
                "美铃" => "美鈴",
                "春纪" => "春紀",
                "胡桃" => "胡桃",
                "结李" => "結李",
                "利夫" => "利夫",
                "有纪" => "有紀",
                "沙希" => "サキ",
                "隼人" => "隼人",
                "村长" => "村長",
                "彩芽" => "あやめ",
                "美广" => "みひろ",
                "珠珠" => "シュシュ",
                "巡" => "めぐる",
                "少" => "スクナ",
                "纯" => "純",
                "凑" => "湊",
                _ => text,
            };
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
