using System.Text.RegularExpressions;
using UnityEngine;

namespace Utage
{
	public class AdvSheetParser
	{
		public const string SheetNameBoot = "Boot";

		public const string SheetNameScenario = "Scenario";

		private const string SheetNameCharacter = "Character";

		private const string SheetNameTexture = "Texture";

		private const string SheetNameSound = "Sound";

		private const string SheetNameLayer = "Layer";

		private const string SheetNameSceneGallery = "SceneGallery";

		private const string SheetNameLocalize = "Localize";

		private const string SheetNameAnimation = "Animation";

		private const string SheetNameLipSynch = "LipSynch";

		private const string SheetNameEyeBlink = "EyeBlink";

		private const string SheetNameParticle = "Particle";

		private static readonly Regex SheetNameRegex = new Regex("(.+)\\{\\}", RegexOptions.IgnorePatternWhitespace);

		private static readonly Regex AnimationSheetNameRegix = new Regex("(.+)\\[\\]", RegexOptions.IgnorePatternWhitespace);

		public static bool IsDisableSheetName(string sheetName)
		{
			if (sheetName == "Boot" || sheetName == "Scenario")
			{
				return true;
			}
			return false;
		}

		public static bool IsSettingsSheet(string sheetName)
		{
			switch (sheetName)
			{
			case "Scenario":
			case "Character":
			case "Texture":
			case "Sound":
			case "Layer":
			case "SceneGallery":
			case "Localize":
			case "EyeBlink":
			case "LipSynch":
			case "Particle":
				return true;
			default:
				if (!IsParamSheetName(sheetName))
				{
					return IsAnimationSheetName(sheetName);
				}
				return true;
			}
		}

		public static bool IsScenarioSheet(string sheetName)
		{
			if (IsDisableSheetName(sheetName))
			{
				return false;
			}
			if (IsSettingsSheet(sheetName))
			{
				return false;
			}
			return true;
		}

		public static bool IsParamSheetName(string sheetName)
		{
			if (sheetName == "Param")
			{
				return true;
			}
			return SheetNameRegex.Match(sheetName).Success;
		}

		private static bool IsAnimationSheetName(string sheetName)
		{
			if (sheetName == "Animation")
			{
				return true;
			}
			return AnimationSheetNameRegix.Match(sheetName).Success;
		}

		public static string ToBootTsvTagName(string sheetName)
		{
			string text = sheetName;
			if (IsParamSheetName(sheetName))
			{
				text = "Param";
			}
			else if (IsAnimationSheetName(sheetName))
			{
				text = "Animation";
			}
			return text + "Setting";
		}

		public static IAdvSetting FindSettingData(AdvSettingDataManager settingManager, string sheetName)
		{
			switch (sheetName)
			{
			case "Character":
				return settingManager.CharacterSetting;
			case "Texture":
				return settingManager.TextureSetting;
			case "Sound":
				return settingManager.SoundSetting;
			case "Layer":
				return settingManager.LayerSetting;
			case "SceneGallery":
				return settingManager.SceneGallerySetting;
			case "Localize":
				return settingManager.LocalizeSetting;
			case "EyeBlink":
				return settingManager.EyeBlinkSetting;
			case "LipSynch":
				return settingManager.LipSynchSetting;
			case "Particle":
				return settingManager.ParticleSetting;
			default:
				if (IsParamSheetName(sheetName))
				{
					return settingManager.DefaultParam;
				}
				if (IsAnimationSheetName(sheetName))
				{
					return settingManager.AnimationSetting;
				}
				Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotSettingSheet, sheetName));
				return null;
			}
		}
	}
}
