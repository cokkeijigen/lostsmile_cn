using System;
using UnityEngine;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/TemplateUI/CgGalleryItem")]
public class UtageUguiCgGalleryItem : MonoBehaviour
{
	public AdvUguiLoadGraphicFile texture;

	public Text count;

	private AdvCgGalleryData data;

	public AdvCgGalleryData Data => data;

	public virtual void Init(AdvCgGalleryData data, Action<UtageUguiCgGalleryItem> ButtonClickedEvent)
	{
		this.data = data;
		Button component = GetComponent<Button>();
		component.onClick.AddListener(delegate
		{
			ButtonClickedEvent(this);
		});
		if (component.interactable = data.NumOpen > 0)
		{
			texture.gameObject.SetActive(true);
			texture.LoadTextureFile(data.ThumbnailPath);
			count.text = $"{data.NumOpen,2}/{data.NumTotal,2}";
		}
		else
		{
			texture.gameObject.SetActive(false);
			count.text = "";
		}
	}
}
