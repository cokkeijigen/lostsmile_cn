using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
    [AddComponentMenu("Utage/ADV/Internal/SaveManager")]
    public class AdvSaveManager : MonoBehaviour
    {
        public enum SaveType
        {
            Default,
            SavePoint,
            Disable
        }

        [Serializable]
        protected class SaveSetting
        {
            [SerializeField]
            private int captureWidth = 256;

            [SerializeField]
            private int captureHeight = 256;

            [SerializeField]
            private int saveMax = 9;

            public int CaptureWidth => captureWidth;

            public int CaptureHeight => captureHeight;

            public int SaveMax => saveMax;
        }

        [SerializeField]
        protected FileIOManager fileIOManager;

        [SerializeField]
        protected SaveType type;

        [SerializeField]
        protected bool isAutoSave = true;

        [SerializeField]
        protected string directoryName = "Save";

        [SerializeField]
        protected string fileName = "save";

        [SerializeField]
        protected SaveSetting defaultSetting = new SaveSetting();

        [SerializeField]
        protected SaveSetting webPlayerSetting;

        public List<GameObject> CustomSaveDataObjects;

        protected List<IBinaryIO> saveIoList;

        protected AdvSaveData autoSaveData;

        protected AdvSaveData currentAutoSaveData;

        protected AdvSaveData quickSaveData;

        protected List<AdvSaveData> saveDataList;

        protected Texture2D captureTexture;

        protected virtual FileIOManager FileIOManager => fileIOManager ?? (fileIOManager = UnityEngine.Object.FindObjectOfType<FileIOManager>());

        public virtual SaveType Type => type;

        public virtual bool IsAutoSave => isAutoSave;

        public virtual string DirectoryName
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

        public virtual string FileName
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

        public int CaptureWidth => defaultSetting.CaptureWidth;

        public int CaptureHeight => defaultSetting.CaptureHeight;

        protected virtual int SaveMax => defaultSetting.SaveMax;

        public virtual List<IBinaryIO> CustomSaveDataIOList
        {
            get
            {
                List<IBinaryIO> list = new List<IBinaryIO>();
                foreach (GameObject customSaveDataObject in CustomSaveDataObjects)
                {
                    IAdvSaveData component = customSaveDataObject.GetComponent<IAdvSaveData>();
                    if (component == null)
                    {
                        Debug.LogError(customSaveDataObject.name + "is not contains IAdvCustomSaveDataIO ", customSaveDataObject);
                    }
                    else
                    {
                        list.Add(component);
                    }
                }
                return list;
            }
        }

        public virtual AdvSaveData AutoSaveData => autoSaveData;

        public virtual AdvSaveData CurrentAutoSaveData => currentAutoSaveData;

        public virtual AdvSaveData QuickSaveData => quickSaveData;

        public virtual List<AdvSaveData> SaveDataList => saveDataList;

        public virtual Texture2D CaptureTexture
        {
            get
            {
                return captureTexture;
            }
            set
            {
                ClearCaptureTexture();
                captureTexture = value;
            }
        }

        public virtual List<IBinaryIO> GetSaveIoListCreateIfMissing(AdvEngine engine)
        {
            if (saveIoList == null)
            {
                saveIoList = new List<IBinaryIO>();
                saveIoList.AddRange(GetComponentsInChildren<IAdvSaveData>(true));
            }
            return saveIoList;
        }

        public void ClearCaptureTexture()
        {
            if (captureTexture != null)
            {
                UnityEngine.Object.Destroy(captureTexture);
                captureTexture = null;
            }
        }

        public virtual void Init()
        {
            FileIOManager.CreateDirectory(ToDirPath());
            autoSaveData = new AdvSaveData(AdvSaveData.SaveDataType.Auto, ToFilePath("Auto"));
            currentAutoSaveData = new AdvSaveData(AdvSaveData.SaveDataType.Auto, ToFilePath("Auto"));
            quickSaveData = new AdvSaveData(AdvSaveData.SaveDataType.Quick, ToFilePath("Quick"));
            saveDataList = new List<AdvSaveData>();
            for (int i = 0; i < SaveMax; i++)
            {
                AdvSaveData item = new AdvSaveData(AdvSaveData.SaveDataType.Default, ToFilePath(string.Concat(i + 1)));
                saveDataList.Add(item);
            }
        }

        protected virtual string ToFilePath(string id)
        {
            return FilePathUtil.Combine(ToDirPath(), FileName + id);
        }

        protected virtual string ToDirPath()
        {
            return FilePathUtil.Combine(FileIOManagerBase.SdkPersistentDataPath, DirectoryName + "/");
        }

        public virtual bool ReadAutoSaveData()
        {
            if (!isAutoSave)
            {
                return false;
            }
            return ReadSaveData(AutoSaveData);
        }

        public virtual bool ReadQuickSaveData()
        {
            return ReadSaveData(QuickSaveData);
        }

        public virtual void ReadAllSaveData()
        {
            ReadAutoSaveData();
            ReadQuickSaveData();
            foreach (AdvSaveData saveData in SaveDataList)
            {
                ReadSaveData(saveData);
            }
        }

        protected virtual bool ReadSaveData(AdvSaveData saveData)
        {
            if (FileIOManager.Exists(saveData.Path))
            {
                return FileIOManager.ReadBinaryDecode(saveData.Path, saveData.Read);
            }
            return false;
        }

        public virtual void UpdateAutoSaveData(AdvEngine engine)
        {
            CurrentAutoSaveData.UpdateAutoSaveData(engine, null, CustomSaveDataIOList, GetSaveIoListCreateIfMissing(engine));
        }

        public virtual void WriteSaveData(AdvEngine engine, AdvSaveData saveData)
        {
            if (!CurrentAutoSaveData.IsSaved)
            {
                Debug.LogError("SaveData is Disabled");
                return;
            }
            saveData.SaveGameData(CurrentAutoSaveData, engine, UtageToolKit.CreateResizeTexture(CaptureTexture, CaptureWidth, CaptureHeight));
            FileIOManager.WriteBinaryEncode(saveData.Path, saveData.Write);
        }

        protected virtual void OnDeleteAllSaveDataAndQuit()
        {
            DeleteAllSaveData();
            isAutoSave = false;
        }

        public virtual void DeleteAllSaveData()
        {
            DeleteSaveData(AutoSaveData);
            DeleteSaveData(QuickSaveData);
            foreach (AdvSaveData saveData in SaveDataList)
            {
                DeleteSaveData(saveData);
            }
        }

        public virtual void DeleteSaveData(AdvSaveData saveData)
        {
            if (FileIOManager.Exists(saveData.Path))
            {
                FileIOManager.Delete(saveData.Path);
            }
            saveData.Clear();
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
            if (IsAutoSave && CurrentAutoSaveData != null && CurrentAutoSaveData.IsSaved)
            {
                FileIOManager.WriteBinaryEncode(CurrentAutoSaveData.Path, CurrentAutoSaveData.Write);
            }
        }
    }
}
