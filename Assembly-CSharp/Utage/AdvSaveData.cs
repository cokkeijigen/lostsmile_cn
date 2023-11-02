using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[Serializable]
	public class AdvSaveData
	{
		public enum SaveDataType
		{
			Default,
			Quick,
			Auto
		}

		private Texture2D texture;

		private static readonly int MagicID = FileIOManagerBase.ToMagicID('S', 'a', 'v', 'e');

		public const int Version = 10;

		public BinaryBuffer Buffer = new BinaryBuffer();

		public string Path { get; private set; }

		public SaveDataType Type { get; private set; }

		public string Title { get; private set; }

		public Texture2D Texture
		{
			get
			{
				return texture;
			}
			set
			{
				texture = value;
				if (texture != null && texture.wrapMode != TextureWrapMode.Clamp)
				{
					texture.wrapMode = TextureWrapMode.Clamp;
				}
			}
		}

		public DateTime Date { get; set; }

		public bool IsSaved => !Buffer.IsEmpty;

		public int FileVersion { get; private set; }

		public AdvParamManager ReadParam(AdvEngine engine)
		{
			AdvParamManager advParamManager = new AdvParamManager();
			advParamManager.InitDefaultAll(engine.DataManager.SettingDataManager.DefaultParam);
			Buffer.Overrirde(advParamManager.DefaultData);
			return advParamManager;
		}

		public AdvSaveData(SaveDataType type, string path)
		{
			Type = type;
			Path = path;
			Clear();
		}

		public void Clear()
		{
			Buffer = new BinaryBuffer();
			if (Texture != null)
			{
				UnityEngine.Object.Destroy(Texture);
			}
			Texture = null;
			FileVersion = -1;
			Title = "";
		}

		public void SaveGameData(AdvSaveData autoSave, AdvEngine engine, Texture2D tex)
		{
			Clear();
			Buffer = autoSave.Buffer.Clone<BinaryBuffer>();
			Date = DateTime.Now;
			Texture = tex;
			FileVersion = autoSave.FileVersion;
			Title = autoSave.Title;
		}

		public void UpdateAutoSaveData(AdvEngine engine, Texture2D tex, List<IBinaryIO> customSaveIoList, List<IBinaryIO> saveIoList)
		{
			Clear();
			List<IBinaryIO> list = new List<IBinaryIO>
			{
				engine.ScenarioPlayer,
				engine.Param.DefaultData,
				engine.GraphicManager,
				engine.CameraManager,
				engine.SoundManager
			};
			list.AddRange(customSaveIoList);
			list.AddRange(saveIoList);
			Buffer.MakeBuffer(list);
			Date = DateTime.Now;
			Texture = tex;
			Title = engine.Page.SaveDataTitle;
		}

		public void LoadGameData(AdvEngine engine, List<IBinaryIO> customSaveIoList, List<IBinaryIO> saveIoList)
		{
			Buffer.Overrirde(engine.Param.DefaultData);
			Buffer.Overrirde(engine.GraphicManager);
			Buffer.Overrirde(engine.CameraManager);
			Buffer.Overrirde(engine.SoundManager);
			Buffer.Overrirde(customSaveIoList);
			Buffer.Overrirde(saveIoList);
		}

		public void Read(BinaryReader reader)
		{
			Clear();
			if (reader.ReadInt32() != MagicID)
			{
				throw new Exception("Read File Id Error");
			}
			int num = reader.ReadInt32();
			if (num >= 10)
			{
				FileVersion = num;
				Date = new DateTime(reader.ReadInt64());
				int num2 = reader.ReadInt32();
				if (num2 > 0)
				{
					byte[] data = reader.ReadBytes(num2);
					Texture2D tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
					tex.LoadImage(data);
					Texture = tex;
				}
				else
				{
					Texture = null;
				}
				Title = reader.ReadString();
				Buffer.Read(reader);
				return;
			}
			Clear();
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
		}

		public void Write(BinaryWriter writer)
		{
			Date = DateTime.Now;
			writer.Write(MagicID);
			writer.Write(10);
			writer.Write(Date.Ticks);
			if (Texture != null)
			{
				byte[] bytes = Texture.EncodeToPNG();
				writer.WriteBuffer(bytes);
			}
			else
			{
				writer.Write(0);
			}
			writer.Write(Title);
			Buffer.Write(writer);
		}
	}
}
