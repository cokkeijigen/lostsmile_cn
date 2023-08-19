using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/ChapterTitle")]
public class SampleChapterTitle : MonoBehaviour
{
	[SerializeField]
	protected AdvEngine engine;

	public UtageUguiTitle title;

	public UtageUguiLoadWait loadWait;

	public UtageUguiMainGame mainGame;

	public List<string> chapterUrlList = new List<string>();

	public List<string> startLabel = new List<string>();

	public bool resetParam = true;

	public bool readSystemSaveData = true;

	public bool readAutoSaveDataParamOnly;

	public bool isAutoSaveOnQuit = true;

	public AdvEngine Engine
	{
		get
		{
			return engine ?? (engine = Object.FindObjectOfType<AdvEngine>());
		}
	}

	private void Start()
	{
	}

	public void OnClickDownLoadChpater(int chapterIndex)
	{
		StartCoroutine(LoadChaptersAsync(chapterIndex));
	}

	private IEnumerator LoadChaptersAsync(int chapterIndex)
	{
		if (Engine.SystemSaveData.IsAutoSaveOnQuit)
		{
			Debug.LogError("Check Off AdvEnigne SystemSaveData IsAutoSaveOnQuit");
			Engine.SystemSaveData.IsAutoSaveOnQuit = false;
		}
		byte[] bufferDefaultParam = null;
		byte[] bufferSystemParam = null;
		if (!resetParam)
		{
			bufferDefaultParam = BinaryUtil.BinaryWrite(delegate(BinaryWriter writer)
			{
				engine.Param.Write(writer, AdvParamData.FileType.Default);
			});
			bufferSystemParam = BinaryUtil.BinaryWrite(delegate(BinaryWriter writer)
			{
				engine.Param.Write(writer, AdvParamData.FileType.System);
			});
		}
		int i = 0;
		while (i < chapterIndex + 1)
		{
			string url = chapterUrlList[i];
			if (!Engine.ExitsChapter(url))
			{
				yield return Engine.LoadChapterAsync(url);
			}
			int num = i + 1;
			i = num;
		}
		Engine.GraphicManager.Remake(Engine.DataManager.SettingDataManager.LayerSetting);
		Engine.Param.InitDefaultAll(Engine.DataManager.SettingDataManager.DefaultParam);
		if (!resetParam)
		{
			BinaryUtil.BinaryRead(bufferDefaultParam, delegate(BinaryReader reader)
			{
				engine.Param.Read(reader, AdvParamData.FileType.Default);
			});
			BinaryUtil.BinaryRead(bufferSystemParam, delegate(BinaryReader reader)
			{
				engine.Param.Read(reader, AdvParamData.FileType.System);
			});
		}
		if (readAutoSaveDataParamOnly)
		{
			Engine.SaveManager.ReadAutoSaveData();
			AdvSaveData autoSaveData = Engine.SaveManager.AutoSaveData;
			if (autoSaveData != null && autoSaveData.IsSaved)
			{
				autoSaveData.Buffer.Overrirde(Engine.Param.DefaultData);
			}
		}
		if (readSystemSaveData)
		{
			Engine.SystemSaveData.Init(Engine);
		}
		Engine.SystemSaveData.IsAutoSaveOnQuit = isAutoSaveOnQuit;
		foreach (AssetFile item in Engine.DataManager.GetAllFileSet())
		{
			AssetFileBase assetFileBase = item as AssetFileBase;
			if (assetFileBase == null)
			{
				Debug.LogError("Not Support Type");
			}
			else if (!assetFileBase.CheckCacheOrLocal())
			{
				Debug.Log(item.FileName);
			}
		}
		Engine.DataManager.DownloadAll();
		title.Close();
		loadWait.OpenOnChapter();
		loadWait.onClose.RemoveAllListeners();
		loadWait.onClose.AddListener(delegate
		{
			mainGame.Open();
			if (resetParam && !readAutoSaveDataParamOnly)
			{
				Engine.StartGame(startLabel[chapterIndex]);
			}
			else
			{
				Engine.JumpScenario(startLabel[chapterIndex]);
			}
		});
	}
}
