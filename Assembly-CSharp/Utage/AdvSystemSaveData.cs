using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/SystemSaveData")]
	public class AdvSystemSaveData : MonoBehaviour
	{
		[SerializeField]
		private bool dontUseSystemSaveData;

		[SerializeField]
		private bool isAutoSaveOnQuit = true;

		[SerializeField]
		private FileIOManager fileIOManager;

		[SerializeField]
		private string directoryName = "Save";

		[SerializeField]
		private string fileName = "system";

		private AdvReadHistorySaveData readData = new AdvReadHistorySaveData();

		private AdvSelectedHistorySaveData selectionData = new AdvSelectedHistorySaveData();

		private AdvGallerySaveData galleryData = new AdvGallerySaveData();

		private AdvEngine engine;

		protected bool isInit;

		protected static readonly int MagicID = FileIOManagerBase.ToMagicID('S', 'y', 's', 't');

		protected const int Version = 4;

		public bool DontUseSystemSaveData
		{
			get
			{
				return dontUseSystemSaveData;
			}
			set
			{
				dontUseSystemSaveData = value;
			}
		}

		public bool IsAutoSaveOnQuit
		{
			get
			{
				return isAutoSaveOnQuit;
			}
			set
			{
				isAutoSaveOnQuit = value;
			}
		}

		private FileIOManager FileIOManager
		{
			get
			{
				return fileIOManager ?? (fileIOManager = UnityEngine.Object.FindObjectOfType<FileIOManager>());
			}
		}

		public string DirectoryName
		{
			get
			{
				return directoryName;
			}
			set
			{
				directoryName = value;
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				fileName = value;
			}
		}

		public string Path { get; private set; }

		public AdvReadHistorySaveData ReadData
		{
			get
			{
				return readData;
			}
		}

		public AdvSelectedHistorySaveData SelectionData
		{
			get
			{
				return selectionData;
			}
		}

		public AdvGallerySaveData GalleryData
		{
			get
			{
				return galleryData;
			}
		}

		protected AdvEngine Engine
		{
			get
			{
				return engine;
			}
		}

		protected virtual List<IBinaryIO> DataList
		{
			get
			{
				return new List<IBinaryIO>
				{
					ReadData,
					SelectionData,
					Engine.Config,
					GalleryData,
					Engine.Param.SystemData
				};
			}
		}

		public virtual void Init(AdvEngine engine)
		{
			this.engine = engine;
			if (!TryReadSaveData())
			{
				InitDefault();
			}
			isInit = true;
		}

		protected virtual void InitDefault()
		{
			engine.Config.InitDefault();
		}

		protected virtual bool TryReadSaveData()
		{
			if (DontUseSystemSaveData)
			{
				return false;
			}
			string text = FilePathUtil.Combine(FileIOManagerBase.SdkPersistentDataPath, DirectoryName);
			FileIOManager.CreateDirectory(text);
			Path = FilePathUtil.Combine(text, FileName);
			if (!FileIOManager.Exists(Path))
			{
				return false;
			}
			return FileIOManager.ReadBinaryDecode(Path, ReadBinary);
		}

		public virtual void Write()
		{
			if (!DontUseSystemSaveData && isInit)
			{
				FileIOManager.WriteBinaryEncode(Path, WriteBinary);
			}
		}

		protected virtual void OnDeleteAllSaveDataAndQuit()
		{
			Delete();
			isAutoSaveOnQuit = false;
		}

		public virtual void Delete()
		{
			FileIOManager.Delete(Path);
		}

		protected virtual void OnApplicationQuit()
		{
			AutoSave();
		}

		protected virtual void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				AutoSave();
			}
		}

		protected virtual void AutoSave()
		{
			if (IsAutoSaveOnQuit)
			{
				Write();
			}
		}

		protected virtual void ReadBinary(BinaryReader reader)
		{
			if (reader.ReadInt32() != MagicID)
			{
				throw new Exception("Read File Id Error");
			}
			int num = reader.ReadInt32();
			if (num == 4)
			{
				BinaryBufferGeneric<IBinaryIO>.Read(reader, DataList);
				return;
			}
			throw new Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
		}

		protected virtual void WriteBinary(BinaryWriter writer)
		{
			writer.Write(MagicID);
			writer.Write(4);
			BinaryBufferGeneric<IBinaryIO>.Write(writer, DataList);
		}
	}
}
