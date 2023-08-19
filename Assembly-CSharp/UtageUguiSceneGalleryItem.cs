using System;
using UnityEngine;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/TemplateUI/SceneGalleryItem")]
public class UtageUguiSceneGalleryItem : MonoBehaviour
{
	public AdvUguiLoadGraphicFile texture;

	public Text title;

	protected AdvSceneGallerySettingData data;

	public AdvSceneGallerySettingData Data
	{
		get
		{
			return data;
		}
	}

	public virtual void Init(AdvSceneGallerySettingData data, Action<UtageUguiSceneGalleryItem> ButtonClickedEvent, AdvSystemSaveData saveData)
	{
		this.data = data;
		Button component = GetComponent<Button>();
		component.onClick.AddListener(delegate
		{
			ButtonClickedEvent(this);
		});
		if (!(component.interactable = saveData.GalleryData.CheckSceneLabels(data.ScenarioLabel)))
		{
			texture.gameObject.SetActive(false);
			if ((bool)title)
			{
				title.text = "";
			}
			return;
		}
		texture.gameObject.SetActive(true);
		texture.LoadTextureFile(data.ThumbnailPath);
		if ((bool)title)
		{
			title.text = data.LocalizedTitle;
		}
	}
}
