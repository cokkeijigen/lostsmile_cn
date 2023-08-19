using System;
using UnityEngine;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/TemplateUI/SaveLoadItem")]
[RequireComponent(typeof(Button))]
public class UtageUguiSaveLoadItem : MonoBehaviour
{
	public Text text;

	public Text no;

	public Text date;

	public RawImage captureImage;

	public Texture2D autoSaveIcon;

	public string textEmpty = "Empty";

	protected Button button;

	protected AdvSaveData data;

	protected int index;

	protected Color defaultColor;

	public AdvSaveData Data
	{
		get
		{
			return data;
		}
	}

	public int Index
	{
		get
		{
			return index;
		}
	}

	public virtual void Init(AdvSaveData data, Action<UtageUguiSaveLoadItem> ButtonClickedEvent, int index, bool isSave)
	{
		this.data = data;
		this.index = index;
		button = GetComponent<Button>();
		button.onClick.AddListener(delegate
		{
			ButtonClickedEvent(this);
		});
		Refresh(isSave);
	}

	public virtual void Refresh(bool isSave)
	{
		no.text = string.Format("No.{0,3}", index);
		if (data.IsSaved)
		{
			if (data.Type == AdvSaveData.SaveDataType.Auto || data.Texture == null)
			{
				if (data.Type == AdvSaveData.SaveDataType.Auto && autoSaveIcon != null)
				{
					captureImage.texture = autoSaveIcon;
					captureImage.color = Color.white;
				}
				else
				{
					captureImage.texture = null;
					captureImage.color = Color.black;
				}
			}
			else
			{
				captureImage.texture = data.Texture;
				captureImage.color = Color.white;
			}
			text.text = data.Title;
			date.text = UtageToolKit.DateToStringJp(data.Date);
			button.interactable = true;
		}
		else
		{
			text.text = textEmpty;
			date.text = "";
			button.interactable = isSave;
		}
		if (data.Type == AdvSaveData.SaveDataType.Auto)
		{
			no.text = "Auto";
			if (isSave)
			{
				button.interactable = false;
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if (captureImage != null && captureImage.texture != null)
		{
			captureImage.texture = null;
		}
	}
}
