using System;
using System.IO;
using UnityEngine;

namespace Utage
{
	public class AdvGraphicInfo : IAssetFileSettingData
	{
		public delegate void CreateCustom(string fileType, ref Type type);

		public const string TypeCharacter = "Character";

		public const string TypeTexture = "Texture";

		public const string TypeParticle = "Particle";

		public const string TypeCapture = "Capture";

		public const string TypeVideo = "Video";

		public static CreateCustom CallbackCreateCustom;

		public static Func<string, bool> CallbackExpression;

		public const string FileType2D = "2D";

		public const string FileTypeAvatar = "Avatar";

		public const string FileTypeDicing = "Dicing";

		public const string FileTypeVideo = "Video";

		public const string FileType2DPrefab = "2DPrefab";

		public const string FileTypeParticle = "Particle";

		public const string FileType3D = "3D";

		public const string FileType3DPrefab = "3DPrefab";

		public const string FileTypeCustom = "Custom";

		public const string FileTypeCustom2D = "Custom2D";

		private AssetFile file;

		private AdvRenderTextureSetting renderTextureSetting = new AdvRenderTextureSetting();

		private const int SaveVersion = 0;

		public string DataType { get; protected set; }

		private int Index { get; set; }

		public string Key { get; protected set; }

		public string FileType { get; protected set; }

		public StringGridRow RowData { get; protected set; }

		public IAdvSettingData SettingData { get; protected set; }

		public string FileName { get; protected set; }

		public AssetFile File
		{
			get
			{
				return file;
			}
			set
			{
				file = value;
			}
		}

		public Vector2 Pivot { get; private set; }

		public Vector3 Scale { get; private set; }

		public Vector3 Position { get; private set; }

		public string SubFileName { get; private set; }

		public AdvAnimationData AnimationData { get; private set; }

		public AdvEyeBlinkData EyeBlinkData { get; set; }

		public AdvLipSynchData LipSynchData { get; private set; }

		public bool CheckConditionalExpression
		{
			get
			{
				if (CallbackExpression == null)
				{
					Debug.LogError("GraphicInfo CallbackExpression is nul");
					return false;
				}
				return CallbackExpression(ConditionalExpression);
			}
		}

		public string ConditionalExpression { get; private set; }

		public AdvRenderTextureSetting RenderTextureSetting => renderTextureSetting;

		internal bool IsUguiComponentType => GetComponentType().IsSubclassOf(typeof(AdvGraphicObjectUguiBase));

		public AdvGraphicInfo(string dataType, int index, string key, StringGridRow row, IAdvSettingData advSettindData)
		{
			DataType = dataType;
			Index = index;
			Key = key;
			SettingData = advSettindData;
			RowData = row;
			string dataType2 = DataType;
			if (dataType2 == "Particle")
			{
				FileType = "Particle";
			}
			else
			{
				FileType = AdvParser.ParseCellOptional(row, AdvColumnName.FileType, "");
			}
			FileName = AdvParser.ParseCell<string>(row, AdvColumnName.FileName);
			try
			{
				Pivot = ParserUtil.ParsePivotOptional(AdvParser.ParseCellOptional(row, AdvColumnName.Pivot, ""), new Vector2(0.5f, 0.5f));
			}
			catch (Exception ex)
			{
				Debug.LogError(row.ToErrorString(ex.Message));
			}
			try
			{
				Scale = ParserUtil.ParseScale3DOptional(AdvParser.ParseCellOptional(row, AdvColumnName.Scale, ""), Vector3.one);
			}
			catch (Exception ex2)
			{
				Debug.LogError(row.ToErrorString(ex2.Message));
			}
			Vector3 position = default(Vector3);
			position.x = AdvParser.ParseCellOptional(row, AdvColumnName.X, 0f);
			position.y = AdvParser.ParseCellOptional(row, AdvColumnName.Y, 0f);
			position.z = AdvParser.ParseCellOptional(row, AdvColumnName.Z, 0f);
			Position = position;
			SubFileName = AdvParser.ParseCellOptional(row, AdvColumnName.SubFileName, "");
			ConditionalExpression = AdvParser.ParseCellOptional(row, AdvColumnName.Conditional, "");
			RenderTextureSetting.Parse(row);
		}

		public AdvGraphicInfo(string dataType, string key, string fileType)
		{
			DataType = dataType;
			Key = key;
			FileType = fileType;
			FileName = "";
			Pivot = new Vector2(0.5f, 0.5f);
			Scale = Vector3.one;
			Position = Vector3.zero;
			ConditionalExpression = "";
			SubFileName = "";
		}

		public void BootInit(Func<string, string, string> FileNameToPath, AdvSettingDataManager dataManager)
		{
			File = AssetFileManager.GetFileCreateIfMissing(FileNameToPath(FileName, FileType), this);
			string text = AdvParser.ParseCellOptional(RowData, AdvColumnName.Animation, "");
			if (!string.IsNullOrEmpty(text))
			{
				AnimationData = dataManager.AnimationSetting.Find(text);
				if (AnimationData == null)
				{
					Debug.LogError(RowData.ToErrorString("Animation [ " + text + " ] is not found"));
				}
			}
			string text2 = AdvParser.ParseCellOptional(RowData, AdvColumnName.EyeBlink, "");
			if (!string.IsNullOrEmpty(text2))
			{
				if (dataManager.EyeBlinkSetting.Dictionary.TryGetValue(text2, out var value))
				{
					EyeBlinkData = value;
				}
				else
				{
					Debug.LogError(RowData.ToErrorString("EyeBlinkLabel [ " + text2 + " ] is not found"));
				}
			}
			string text3 = AdvParser.ParseCellOptional(RowData, AdvColumnName.LipSynch, "");
			if (!string.IsNullOrEmpty(text3))
			{
				if (dataManager.LipSynchSetting.Dictionary.TryGetValue(text3, out var value2))
				{
					LipSynchData = value2;
				}
				else
				{
					Debug.LogError(RowData.ToErrorString("LipSynchLabel [ " + text3 + " ] is not found"));
				}
			}
		}

		internal bool TryGetAdvGraphicObjectPrefab(out GameObject prefab)
		{
			prefab = null;
			if (File == null)
			{
				return false;
			}
			if (File.FileType != AssetFileType.UnityObject)
			{
				return false;
			}
			GameObject gameObject = File.UnityObject as GameObject;
			if (gameObject == null)
			{
				return false;
			}
			if (gameObject.GetComponent<AdvGraphicObject>() == null)
			{
				return false;
			}
			prefab = gameObject;
			return true;
		}

		internal Type GetComponentType()
		{
			if (CallbackCreateCustom != null)
			{
				Type type = null;
				CallbackCreateCustom(FileType, ref type);
				if (type != null)
				{
					return type;
				}
			}
			switch (FileType)
			{
			case "3D":
			case "3DPrefab":
				return typeof(AdvGraphicObject3DPrefab);
			case "Particle":
				return typeof(AdvGraphicObjectParticle);
			case "2DPrefab":
				return typeof(AdvGraphicObject2DPrefab);
			case "Custom":
				return typeof(AdvGraphicObjectCustom);
			case "Avatar":
				return typeof(AdvGraphicObjectAvatar);
			case "Dicing":
				return typeof(AdvGraphicObjectDicing);
			case "Video":
				return typeof(AdvGraphicObjectVideo);
			case "Custom2D":
				return typeof(AdvGraphicObjectCustom2D);
			default:
				return typeof(AdvGraphicObjectRawImage);
			}
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(DataType);
			writer.Write(Key);
			writer.Write(Index);
		}

		public static AdvGraphicInfo ReadGraphicInfo(AdvEngine engine, BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return null;
			}
			string text = reader.ReadString();
			string text2 = reader.ReadString();
			int num2 = reader.ReadInt32();
			AdvGraphicInfoList advGraphicInfoList;
			switch (text)
			{
			case "Character":
				advGraphicInfoList = engine.DataManager.SettingDataManager.CharacterSetting.KeyToGraphicInfo(text2);
				break;
			case "Particle":
				return engine.DataManager.SettingDataManager.ParticleSetting.LabelToGraphic(text2);
			case "Texture":
				advGraphicInfoList = engine.DataManager.SettingDataManager.TextureSetting.LabelToGraphic(text2);
				break;
			case "Capture":
				Debug.LogError("Caputure image not support on save");
				return null;
			default:
				return new AdvGraphicInfo(text, text2, "2D");
			}
			if (advGraphicInfoList != null && num2 < advGraphicInfoList.InfoList.Count)
			{
				return advGraphicInfoList.InfoList[num2];
			}
			return null;
		}
	}
}
