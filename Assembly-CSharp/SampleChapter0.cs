using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/Chapter0")]
public class SampleChapter0 : MonoBehaviour
{
	[SerializeField]
	protected AdvEngine engine;

	public UtageUguiTitle title;

	public UtageUguiLoadWait loadWait;

	public UtageUguiMainGame mainGame;

	public List<string> chapterUrlList = new List<string>();

	public List<string> startLabel = new List<string>();

	public AdvEngine Engine => engine ?? (engine = Object.FindObjectOfType<AdvEngine>());

	private void Start()
	{
	}

	public void OnClickDownLoadChpater(int chapterIndex)
	{
		StartCoroutine(LoadChaptersAsync(chapterIndex));
	}

	private IEnumerator LoadChaptersAsync(int chapterIndex)
	{
		string url = chapterUrlList[chapterIndex];
		if (!Engine.ExitsChapter(url))
		{
			yield return Engine.LoadChapterAsync(url);
		}
		Engine.GraphicManager.Remake(Engine.DataManager.SettingDataManager.LayerSetting);
		int num = 0;
		foreach (AssetFile item in Engine.DataManager.GetAllFileSet())
		{
			if (!(item is AssetFileBase assetFileBase))
			{
				Debug.LogError("Not Support Type");
			}
			else if (!assetFileBase.CheckCacheOrLocal())
			{
				num += assetFileBase.FileInfo.AssetBundleInfo.Size;
			}
		}
		Debug.Log("DownLoad Size = 0");
		Engine.DataManager.DownloadAllFileUsed();
		title.Close();
		loadWait.OpenOnChapter();
		loadWait.onClose.RemoveAllListeners();
		loadWait.onClose.AddListener(delegate
		{
			mainGame.Open();
			Engine.JumpScenario(startLabel[chapterIndex]);
		});
	}
}
