using System;
using UnityEngine;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/TemplateUI/SoundRoomItem")]
public class UtageUguiSoundRoomItem : MonoBehaviour
{
	public Text title;

	protected AdvSoundSettingData data;

	public AdvSoundSettingData Data
	{
		get
		{
			return data;
		}
	}

	public virtual void Init(AdvSoundSettingData data, Action<UtageUguiSoundRoomItem> ButtonClickedEvent, int index)
	{
		this.data = data;
		title.text = data.Title;
		GetComponent<Button>().onClick.AddListener(delegate
		{
			ButtonClickedEvent(this);
		});
	}
}
