using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utage;

[AddComponentMenu("Utage/TemplateUI/SoundRoom")]
public class UtageUguiSoundRoom : UguiView
{
	[SerializeField]
	protected UtageUguiGallery gallery;

	public UguiListView listView;

	protected List<AdvSoundSettingData> itemDataList = new List<AdvSoundSettingData>();

	[SerializeField]
	protected AdvEngine engine;

	protected bool isInit;

	protected bool isChangedBgm;

	public UtageUguiGallery Gallery
	{
		get
		{
			return gallery ?? (gallery = Object.FindObjectOfType<UtageUguiGallery>());
		}
	}

	public AdvEngine Engine
	{
		get
		{
			return engine ?? (engine = Object.FindObjectOfType<AdvEngine>());
		}
	}

	protected virtual void OnOpen()
	{
		isInit = false;
		isChangedBgm = false;
		listView.ClearItems();
		StartCoroutine(CoWaitOpen());
	}

	protected virtual void OnClose()
	{
		isInit = false;
		listView.ClearItems();
		if (isChangedBgm)
		{
			Engine.SoundManager.StopAll(0.2f);
		}
		isChangedBgm = false;
	}

	protected virtual IEnumerator CoWaitOpen()
	{
		while (Engine.IsWaitBootLoading)
		{
			yield return null;
		}
		itemDataList = Engine.DataManager.SettingDataManager.SoundSetting.GetSoundRoomList();
		listView.CreateItems(itemDataList.Count, CallBackCreateItem);
		isInit = true;
	}

	protected virtual void CallBackCreateItem(GameObject go, int index)
	{
		UtageUguiSoundRoomItem component = go.GetComponent<UtageUguiSoundRoomItem>();
		AdvSoundSettingData data = itemDataList[index];
		component.Init(data, OnTap, index);
	}

	protected virtual void Update()
	{
		if (isInit && InputUtil.IsMouseRightButtonDown())
		{
			Gallery.Back();
		}
	}

	protected virtual void OnTap(UtageUguiSoundRoomItem item)
	{
		AdvSoundSettingData data = item.Data;
		string path = Engine.DataManager.SettingDataManager.SoundSetting.LabelToFilePath(data.Key, SoundType.Bgm);
		StartCoroutine(CoPlaySound(path));
	}

	protected virtual IEnumerator CoPlaySound(string path)
	{
		isChangedBgm = true;
		AssetFile file = AssetFileManager.Load(path, this);
		while (!file.IsLoadEnd)
		{
			yield return null;
		}
		Engine.SoundManager.PlayBgm(file);
		file.Unuse(this);
	}
}
