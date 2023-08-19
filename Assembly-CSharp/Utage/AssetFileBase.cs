using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public abstract class AssetFileBase : AssetFile
	{
		protected HashSet<object> referenceSet = new HashSet<object>();

		protected AssetFileManager FileManager { get; private set; }

		public AssetFileInfo FileInfo { get; private set; }

		public virtual string FileName
		{
			get
			{
				return FileInfo.FileName;
			}
		}

		public IAssetFileSettingData SettingData { get; private set; }

		public virtual AssetFileType FileType { get; protected set; }

		public bool IsLoadEnd { get; protected set; }

		public bool IsLoadError { get; protected set; }

		public string LoadErrorMsg { get; protected set; }

		public TextAsset Text { get; protected set; }

		public Texture2D Texture { get; protected set; }

		public AudioClip Sound { get; protected set; }

		public UnityEngine.Object UnityObject { get; protected set; }

		protected internal AssetFileLoadPriority Priority { get; protected set; }

		protected internal bool IgnoreUnload { get; protected set; }

		internal int ReferenceCount
		{
			get
			{
				if (referenceSet.Contains(null))
				{
					referenceSet.RemoveWhere((object s) => s == null);
					Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.FileReferecedIsNull));
				}
				return referenceSet.Count;
			}
		}

		public AssetFileBase(AssetFileManager mangager, AssetFileInfo fileInfo, IAssetFileSettingData settingData)
		{
			FileManager = mangager;
			FileInfo = fileInfo;
			FileType = fileInfo.FileType;
			SettingData = settingData;
			Priority = AssetFileLoadPriority.DownloadOnly;
		}

		internal virtual void ReadyToLoad(AssetFileLoadPriority loadPriority, object referenceObj)
		{
			if (loadPriority < Priority)
			{
				Priority = loadPriority;
			}
			Use(referenceObj);
		}

		public virtual void Use(object referenceObj)
		{
			if (referenceObj != null)
			{
				referenceSet.Add(referenceObj);
			}
		}

		public virtual void Unuse(object referenceObj)
		{
			if (referenceObj != null)
			{
				referenceSet.Remove(referenceObj);
			}
		}

		public virtual void AddReferenceComponent(GameObject go)
		{
			go.AddComponent<AssetFileReference>().Init(this);
		}

		internal void LoadDummy(AssetFileDummyOnLoadError dummyFiles)
		{
			IgnoreUnload = true;
			IsLoadEnd = true;
			IsLoadError = false;
			switch (FileType)
			{
			case AssetFileType.Text:
				Text = dummyFiles.text;
				break;
			case AssetFileType.Texture:
				Texture = dummyFiles.texture;
				break;
			case AssetFileType.Sound:
				Sound = dummyFiles.sound;
				break;
			case AssetFileType.UnityObject:
				UnityObject = dummyFiles.asset;
				break;
			}
		}

		protected virtual string ParseLoadPath()
		{
			switch (FileInfo.StrageType)
			{
			case AssetFileStrageType.Server:
			case AssetFileStrageType.StreamingAssets:
				if (FileInfo.AssetBundleInfo == null)
				{
					Debug.LogError("Not found in assetbundle list " + FileName);
					return FilePathUtil.EncodeUrl(FileName);
				}
				return FilePathUtil.EncodeUrl(FilePathUtil.ToCacheClearUrl(FileInfo.AssetBundleInfo.Url));
			default:
				return FileName;
			}
		}

		public abstract bool CheckCacheOrLocal();

		public abstract IEnumerator LoadAsync(Action onComplete, Action onFailed);

		public abstract void Unload();
	}
}
