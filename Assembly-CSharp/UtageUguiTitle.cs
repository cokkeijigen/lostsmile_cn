using UnityEngine;
using Utage;

[AddComponentMenu("Utage/TemplateUI/Title")]
public class UtageUguiTitle : UguiView
{
	[SerializeField]
	protected AdvEngineStarter starter;

	public UtageUguiMainGame mainGame;

	public UtageUguiConfig config;

	public UtageUguiSaveLoad load;

	public UtageUguiGallery gallery;

	public UtageUguiLoadWait download;

	public GameObject downloadButton;

	public AdvEngineStarter Starter
	{
		get
		{
			return starter ?? (starter = Object.FindObjectOfType<AdvEngineStarter>());
		}
	}

	protected virtual void OnOpen()
	{
		if (downloadButton != null)
		{
			downloadButton.SetActive(false);
		}
	}

	public virtual void OnTapStart()
	{
		Close();
		mainGame.OpenStartGame();
	}

	public virtual void OnTapLoad()
	{
		Close();
		load.OpenLoad(this);
	}

	public virtual void OnTapConfig()
	{
		Close();
		config.Open(this);
	}

	public virtual void OnTapGallery()
	{
		Close();
		gallery.Open(this);
	}

	public virtual void OnTapDownLoad()
	{
		Close();
		download.Open(this);
	}

	public virtual void OnTapStartLabel(string label)
	{
		Close();
		mainGame.OpenStartLabel(label);
	}

	protected virtual void OnCloseLoadChapter(string startLabel)
	{
		download.onClose.RemoveAllListeners();
		Close();
		mainGame.OpenStartLabel(startLabel);
	}
}
