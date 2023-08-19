using System;
using UnityEngine;

namespace Utage
{
	public static class AdvCommandParser
	{
		public delegate void CreateCustomCommandFromID(string id, StringGridRow row, AdvSettingDataManager dataManager, ref AdvCommand command);

		public static CreateCustomCommandFromID OnCreateCustomCommandFromID;

		[Obsolete("Use OnCreateCustomCommandFromID  instead")]
		public static CreateCustomCommandFromID OnCreateCustomCommnadFromID;

		public const string IdError = "Error";

		public const string IdComment = "Comment";

		public const string IdCharacter = "Character";

		public const string IdCharacterOff = "CharacterOff";

		public const string IdText = "Text";

		public const string IdBg = "Bg";

		public const string IdBgOff = "BgOff";

		public const string IdBgEvent = "BgEvent";

		public const string IdBgEventOff = "BgEventOff";

		public const string IdSprite = "Sprite";

		public const string IdSpriteOff = "SpriteOff";

		public const string IdParticle = "Particle";

		public const string IdParticleOff = "ParticleOff";

		public const string IdLayerReset = "LayerReset";

		public const string IdLayerOff = "LayerOff";

		public const string IdMovie = "Movie";

		public const string IdVideo = "Video";

		public const string IdSe = "Se";

		public const string IdStopSe = "StopSe";

		public const string IdBgm = "Bgm";

		public const string IdStopBgm = "StopBgm";

		public const string IdAmbience = "Ambience";

		public const string IdStopAmbience = "StopAmbience";

		public const string IdVoice = "Voice";

		public const string IdStopVoice = "StopVoice";

		public const string IdStopSound = "StopSound";

		public const string IdChangeSoundVolume = "ChangeSoundVolume";

		public const string IdSelection = "Selection";

		public const string IdSelectionEnd = "SelectionEnd";

		public const string IdSelectionClick = "SelectionClick";

		public const string IdJump = "Jump";

		public const string IdJumpRandom = "JumpRandom";

		public const string IdJumpRandomEnd = "JumpRandomEnd";

		public const string IdJumpSubroutine = "JumpSubroutine";

		public const string IdJumpSubroutineRandom = "JumpSubroutineRandom";

		public const string IdJumpSubroutineRandomEnd = "JumpSubroutineRandomEnd";

		public const string IdEndSubroutine = "EndSubroutine";

		public const string IdBeginMacro = "BeginMacro";

		public const string IdEndMacro = "EndMacro";

		public const string IdEffect = "Effect";

		public const string IdWait = "Wait";

		public const string IdWaitInput = "WaitInput";

		public const string IdWaitCustom = "WaitCustom";

		public const string IdTween = "Tween";

		public const string IdFadeIn = "FadeIn";

		public const string IdFadeOut = "FadeOut";

		public const string IdShake = "Shake";

		public const string IdZoomCamera = "ZoomCamera";

		public const string IdVibrate = "Vibrate";

		public const string IdPlayAnimation = "PlayAnimation";

		public const string IdRuleFadeIn = "RuleFadeIn";

		public const string IdRuleFadeOut = "RuleFadeOut";

		public const string IdCaptureImage = "CaptureImage";

		public const string IdImageEffect = "ImageEffect";

		public const string IdImageEffectOff = "ImageEffectOff";

		public const string IdParam = "Param";

		public const string IdIf = "If";

		public const string IdElseIf = "ElseIf";

		public const string IdElse = "Else";

		public const string IdEndIf = "EndIf";

		public const string IdShowMessageWindow = "ShowMessageWindow";

		public const string IdHideMessageWindow = "HideMessageWindow";

		public const string IdShowMenuButton = "ShowMenuButton";

		public const string IdHideMenuButton = "HideMenuButton";

		public const string IdChangeMessageWindow = "ChangeMessageWindow";

		public const string IdInitMessageWindow = "InitMessageWindow";

		public const string IdGuiReset = "GuiReset";

		public const string IdGuiActive = "GuiActive";

		public const string IdGuiPosition = "GuiPosition";

		public const string IdGuiSize = "GuiSize";

		public const string IdSendMessage = "SendMessage";

		public const string IdSendMessageByName = "SendMessageByName";

		public const string IdEndScenario = "EndScenario";

		public const string IdPauseScenario = "PauseScenario";

		public const string IdEndSceneGallery = "EndSceneGallery";

		public const string IdScenarioLabel = "ScenarioLabel";

		public const string IdPageControler = "PageControl";

		public const string IdThread = "Thread";

		public const string IdWaitThread = "WaitThread";

		public const string IdEndPage = "EndPage";

		public const string IdEndThread = "EndThread";

		public static AdvCommand CreateCommand(StringGridRow row, AdvSettingDataManager dataManager)
		{
			if (row.IsCommentOut || IsComment(row))
			{
				return null;
			}
			AdvCommand advCommand = CreateCommand(ParseCommandID(row), row, dataManager);
			if (advCommand == null && !row.IsAllEmptyCellNamedColumn())
			{
				Debug.LogError(row.ToErrorString(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.CommandParseNull)));
				return new AdvCommandError(row);
			}
			return advCommand;
		}

		public static AdvCommand CreateCommand(string id, StringGridRow row, AdvSettingDataManager dataManager)
		{
			AdvCommand command = null;
			if (OnCreateCustomCommandFromID != null)
			{
				OnCreateCustomCommandFromID(id, row, dataManager, ref command);
			}
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			if (command == null)
			{
				command = CreateCommandDefault(id, row, dataManager);
			}
			if (command != null)
			{
				command.Id = id;
			}
			return command;
		}

		private static AdvCommand CreateCommandDefault(string id, StringGridRow row, AdvSettingDataManager dataManager)
		{
			switch (id)
			{
			case "Character":
				return new AdvCommandCharacter(row, dataManager);
			case "Text":
				return new AdvCommandText(row, dataManager);
			case "CharacterOff":
				return new AdvCommandCharacterOff(row);
			case "Bg":
				return new AdvCommandBg(row, dataManager);
			case "BgOff":
				return new AdvCommandBgOff(row);
			case "BgEvent":
				return new AdvCommandBgEvent(row, dataManager);
			case "BgEventOff":
				return new AdvCommandBgEventOff(row);
			case "Sprite":
				return new AdvCommandSprite(row, dataManager);
			case "SpriteOff":
				return new AdvCommandSpriteOff(row);
			case "Particle":
				return new AdvCommandParticle(row, dataManager);
			case "ParticleOff":
				return new AdvCommandParticleOff(row);
			case "LayerOff":
				return new AdvCommandLayerOff(row, dataManager);
			case "LayerReset":
				return new AdvCommandLayerReset(row, dataManager);
			case "Movie":
				Debug.LogWarning("Movie command has been abolished due to Unity specification change. In Unity2018.2 or newer. ");
				return new AdvCommandMovie(row, dataManager);
			case "Video":
				return new AdvCommandVideo(row, dataManager);
			case "Tween":
				return new AdvCommandTween(row, dataManager);
			case "FadeIn":
				return new AdvCommandFadeIn(row);
			case "FadeOut":
				return new AdvCommandFadeOut(row);
			case "Shake":
				return new AdvCommandShake(row, dataManager);
			case "Vibrate":
				return new AdvCommandVibrate(row, dataManager);
			case "ZoomCamera":
				return new AdvCommandZoomCamera(row, dataManager);
			case "PlayAnimation":
				return new AdvCommandPlayAnimatin(row, dataManager);
			case "RuleFadeIn":
				return new AdvCommandRuleFadeIn(row);
			case "RuleFadeOut":
				return new AdvCommandRuleFadeOut(row);
			case "CaptureImage":
				return new AdvCommandCaptureImage(row);
			case "ImageEffect":
				return new AdvCommandImageEffect(row, dataManager);
			case "ImageEffectOff":
				return new AdvCommandImageEffectOff(row, dataManager);
			case "Se":
				return new AdvCommandSe(row, dataManager);
			case "StopSe":
				return new AdvCommandStopSe(row, dataManager);
			case "Bgm":
				return new AdvCommandBgm(row, dataManager);
			case "StopBgm":
				return new AdvCommandStopBgm(row);
			case "Ambience":
				return new AdvCommandAmbience(row, dataManager);
			case "StopAmbience":
				return new AdvCommandStopAmbience(row);
			case "Voice":
				return new AdvCommandVoice(row, dataManager);
			case "StopVoice":
				return new AdvCommandStopVoice(row);
			case "StopSound":
				return new AdvCommandStopSound(row);
			case "ChangeSoundVolume":
				return new AdvCommandChangeSoundVolume(row);
			case "Wait":
				return new AdvCommandWait(row);
			case "WaitInput":
				return new AdvCommandWaitInput(row);
			case "WaitCustom":
				return new AdvCommandWaitCustom(row);
			case "Param":
				return new AdvCommandParam(row, dataManager);
			case "Selection":
				return new AdvCommandSelection(row, dataManager);
			case "SelectionEnd":
				return new AdvCommandSelectionEnd(row, dataManager);
			case "SelectionClick":
				return new AdvCommandSelectionClick(row, dataManager);
			case "Jump":
				return new AdvCommandJump(row, dataManager);
			case "JumpRandom":
				return new AdvCommandJumpRandom(row, dataManager);
			case "JumpRandomEnd":
				return new AdvCommandJumpRandomEnd(row, dataManager);
			case "JumpSubroutine":
				return new AdvCommandJumpSubroutine(row, dataManager);
			case "JumpSubroutineRandom":
				return new AdvCommandJumpSubroutineRandom(row, dataManager);
			case "JumpSubroutineRandomEnd":
				return new AdvCommandJumpSubroutineRandomEnd(row, dataManager);
			case "EndSubroutine":
				return new AdvCommandEndSubroutine(row, dataManager);
			case "If":
				return new AdvCommandIf(row, dataManager);
			case "ElseIf":
				return new AdvCommandElseIf(row, dataManager);
			case "Else":
				return new AdvCommandElse(row);
			case "EndIf":
				return new AdvCommandEndIf(row);
			case "ShowMessageWindow":
				return new AdvCommandShowMessageWindow(row);
			case "HideMessageWindow":
				return new AdvCommandHideMessageWindow(row);
			case "ShowMenuButton":
				return new AdvCommandShowMenuButton(row);
			case "HideMenuButton":
				return new AdvCommandHideMenuButton(row);
			case "ChangeMessageWindow":
				return new AdvCommandMessageWindowChangeCurrent(row);
			case "InitMessageWindow":
				return new AdvCommandMessageWindowInit(row);
			case "GuiReset":
				return new AdvCommandGuiReset(row);
			case "GuiActive":
				return new AdvCommandGuiActive(row);
			case "GuiPosition":
				return new AdvCommandGuiPosition(row);
			case "GuiSize":
				return new AdvCommandGuiSize(row);
			case "SendMessage":
				return new AdvCommandSendMessage(row);
			case "SendMessageByName":
				return new AdvCommandSendMessageByName(row);
			case "EndScenario":
				return new AdvCommandEndScenario(row);
			case "PauseScenario":
				return new AdvCommandPauseScenario(row);
			case "EndSceneGallery":
				return new AdvCommandEndSceneGallery(row);
			case "PageControl":
				return new AdvCommandPageControler(row, dataManager);
			case "ScenarioLabel":
				return new AdvCommandScenarioLabel(row);
			case "Thread":
				return new AdvCommandThread(row);
			case "WaitThread":
				return new AdvCommandWaitThread(row);
			case "EndThread":
				return new AdvCommandEndThread(row);
			case "EndPage":
				return new AdvCommandEndPage(row);
			default:
				return null;
			}
		}

		private static string ParseCommandID(StringGridRow row)
		{
			string text = AdvParser.ParseCellOptional(row, AdvColumnName.Command, "");
			if (string.IsNullOrEmpty(text))
			{
				if (!AdvParser.IsEmptyCell(row, AdvColumnName.Arg1))
				{
					return "Character";
				}
				if (!AdvParser.IsEmptyCell(row, AdvColumnName.Text) || !AdvParser.IsEmptyCell(row, AdvColumnName.PageCtrl))
				{
					return "Text";
				}
				return null;
			}
			if (IsScenarioLabel(text))
			{
				text = "ScenarioLabel";
			}
			return text;
		}

		public static bool IsScenarioLabel(string str)
		{
			if (!string.IsNullOrEmpty(str) && str.Length >= 2)
			{
				return str[0] == '*';
			}
			return false;
		}

		public static bool TryParseScenarioLabel(StringGridRow row, AdvColumnName columnName, out string scenarioLabel)
		{
			string text = AdvParser.ParseCell<string>(row, columnName);
			if (!IsScenarioLabel(text))
			{
				scenarioLabel = text;
				return false;
			}
			if (text.Length >= 3 && text[1] == '*')
			{
				scenarioLabel = row.Grid.SheetName + "*" + text.Substring(2);
			}
			else
			{
				scenarioLabel = text.Substring(1);
			}
			return true;
		}

		public static string ParseScenarioLabel(StringGridRow row, AdvColumnName name)
		{
			string scenarioLabel;
			if (!TryParseScenarioLabel(row, name, out scenarioLabel))
			{
				Debug.LogError(row.ToErrorString(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotScenarioLabel, scenarioLabel)));
			}
			return scenarioLabel;
		}

		private static bool IsComment(StringGridRow row)
		{
			string text = AdvParser.ParseCellOptional(row, AdvColumnName.Command, "");
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (text == "Comment")
			{
				return true;
			}
			if (text.Length >= 2 && text[0] == '/' && text[1] == '/')
			{
				return true;
			}
			return false;
		}
	}
}
