using UnityEngine;

namespace Utage
{
	public class AdvCommandCharacter : AdvCommand
	{
		protected AdvCharacterInfo characterInfo;

		protected string layerName;

		protected float fadeTime;

		public AdvCommandCharacter(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			characterInfo = AdvCharacterInfo.Create(this, dataManager);
			if (characterInfo.Graphic != null)
			{
				AddLoadGraphic(characterInfo.Graphic);
			}
			layerName = ParseCellOptional(AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName, AdvLayerSettingData.LayerType.Character))
			{
				Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
			}
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			bool flag = false;
			if (characterInfo.IsHide)
			{
				engine.GraphicManager.CharacterManager.FadeOut(characterInfo.Label, engine.Page.ToSkippedTime(fadeTime));
			}
			else if (CheckDrawCharacter(engine))
			{
				flag = true;
				engine.GraphicManager.CharacterManager.DrawCharacter(layerName, characterInfo.Label, new AdvGraphicOperaitonArg(this, characterInfo.Graphic.Main, fadeTime));
			}
			if (flag || CheckNewCharacterInfo(engine))
			{
				engine.Page.CharacterInfo = characterInfo;
			}
			AdvGraphicObject advGraphicObject = engine.GraphicManager.CharacterManager.FindObject(characterInfo.Label);
			if (advGraphicObject != null)
			{
				advGraphicObject.SetCommandPostion(this);
				advGraphicObject.TargetObject.SetCommandArg(this);
			}
		}

		private bool CheckDrawCharacter(AdvEngine engine)
		{
			if (characterInfo.Graphic == null || characterInfo.Graphic.Main == null)
			{
				return false;
			}
			if (engine.GraphicManager.IsEventMode)
			{
				return false;
			}
			if (string.IsNullOrEmpty(characterInfo.Pattern) && engine.GraphicManager.CharacterManager.IsContians(layerName, characterInfo.Label))
			{
				return false;
			}
			return true;
		}

		private bool CheckNewCharacterInfo(AdvEngine engine)
		{
			if (engine.Page.CharacterLabel != characterInfo.Label)
			{
				return true;
			}
			if (engine.Page.NameText != characterInfo.NameText)
			{
				return true;
			}
			if (!string.IsNullOrEmpty(characterInfo.Pattern))
			{
				return true;
			}
			return false;
		}

		public override string[] GetExtraCommandIdArray(AdvCommand next)
		{
			if (!IsEmptyCell(AdvColumnName.Text) || !IsEmptyCell(AdvColumnName.PageCtrl))
			{
				return new string[1] { "Text" };
			}
			return null;
		}
	}
}
