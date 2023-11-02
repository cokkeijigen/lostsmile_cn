using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/TemplateUI/MainGame")]
public class UtageUguiMainGame : UguiView
{
	protected enum BootType
	{
		Default,
		Start,
		Load,
		SceneGallery,
		StartLabel
	}

	[SerializeField]
	protected AdvEngine engine;

	[SerializeField]
	protected LetterBoxCamera letterBoxCamera;

	public UtageUguiTitle title;

	public UtageUguiConfig config;

	public UtageUguiSaveLoad saveLoad;

	public UtageUguiGallery gallery;

	public GameObject buttons;

	public Toggle checkSkip;

	public Toggle checkAuto;

	protected BootType bootType;

	protected AdvSaveData loadData;

	protected bool isInit;

	protected string scenarioLabel;

	public virtual AdvEngine Engine => engine ?? (engine = Object.FindObjectOfType<AdvEngine>());

	public virtual LetterBoxCamera LetterBoxCamera => letterBoxCamera ?? (letterBoxCamera = Object.FindObjectOfType<LetterBoxCamera>());

	protected virtual void Awake()
	{
		Engine.Page.OnEndText.AddListener(delegate(AdvPage page)
		{
			CaptureScreenOnSavePoint(page);
		});
	}

	public override void Close()
	{
		base.Close();
		Engine.UiManager.Close();
		Engine.Config.IsSkip = false;
	}

	protected virtual void ClearBootData()
	{
		bootType = BootType.Default;
		isInit = false;
		loadData = null;
	}

	public virtual void OpenStartGame()
	{
		ClearBootData();
		bootType = BootType.Start;
		Open();
	}

	public virtual void OpenStartLabel(string label)
	{
		ClearBootData();
		bootType = BootType.StartLabel;
		scenarioLabel = label;
		Open();
	}

	public virtual void OpenLoadGame(AdvSaveData loadData)
	{
		ClearBootData();
		bootType = BootType.Load;
		this.loadData = loadData;
		Open();
	}

	public virtual void OpenSceneGallery(string scenarioLabel)
	{
		ClearBootData();
		bootType = BootType.SceneGallery;
		this.scenarioLabel = scenarioLabel;
		Open();
	}

	protected virtual void OnOpen()
	{
		if (Engine.SaveManager.Type != AdvSaveManager.SaveType.SavePoint)
		{
			Engine.SaveManager.ClearCaptureTexture();
		}
		StartCoroutine(CoWaitOpen());
	}

	protected virtual IEnumerator CoWaitOpen()
	{
		while (Engine.IsWaitBootLoading)
		{
			yield return null;
		}
		switch (bootType)
		{
		case BootType.Default:
			Engine.UiManager.Open();
			break;
		case BootType.Start:
			Engine.StartGame();
			break;
		case BootType.Load:
			Engine.OpenLoadGame(loadData);
			break;
		case BootType.SceneGallery:
			Engine.StartSceneGallery(scenarioLabel);
			break;
		case BootType.StartLabel:
			Engine.StartGame(scenarioLabel);
			break;
		}
		ClearBootData();
		loadData = null;
		Engine.Config.IsSkip = false;
		isInit = true;
	}

	protected virtual void Update()
	{
		if (!isInit)
		{
			return;
		}
		if ((bool)SystemUi.GetInstance())
		{
			if (Engine.IsLoading)
			{
				SystemUi.GetInstance().StartIndicator(this);
			}
			else
			{
				SystemUi.GetInstance().StopIndicator(this);
			}
		}
		if (Engine.IsEndScenario)
		{
			Close();
			if (Engine.IsSceneGallery)
			{
				gallery.Open();
			}
			else
			{
				title.Open(this);
			}
		}
	}

	protected virtual void LateUpdate()
	{
		buttons.SetActive(Engine.UiManager.IsShowingMenuButton && Engine.UiManager.Status == AdvUiManager.UiStatus.Default);
		if ((bool)checkSkip && checkSkip.isOn != Engine.Config.IsSkip)
		{
			checkSkip.isOn = Engine.Config.IsSkip;
		}
		if ((bool)checkAuto && checkAuto.isOn != Engine.Config.IsAutoBrPage)
		{
			checkAuto.isOn = Engine.Config.IsAutoBrPage;
		}
	}

	protected virtual void CaptureScreenOnSavePoint(AdvPage page)
	{
		if (Engine.SaveManager.Type == AdvSaveManager.SaveType.SavePoint && page.IsSavePoint)
		{
			Debug.Log("Capture");
			StartCoroutine(CoCaptureScreen());
		}
	}

	protected virtual IEnumerator CoCaptureScreen()
	{
		yield return new WaitForEndOfFrame();
		Engine.SaveManager.CaptureTexture = CaptureScreen();
	}

	public virtual void OnTapSkip(bool isOn)
	{
		Engine.Config.IsSkip = isOn;
	}

	public virtual void OnTapAuto(bool isOn)
	{
		Engine.Config.IsAutoBrPage = isOn;
	}

	public virtual void OnTapConfig()
	{
		Close();
		config.Open(this);
	}

	public virtual void OnTapSave()
	{
		if (!Engine.IsSceneGallery)
		{
			StartCoroutine(CoSave());
		}
	}

	protected virtual IEnumerator CoSave()
	{
		if (Engine.SaveManager.Type != AdvSaveManager.SaveType.SavePoint)
		{
			yield return new WaitForEndOfFrame();
			Engine.SaveManager.CaptureTexture = CaptureScreen();
		}
		Close();
		saveLoad.OpenSave(this);
	}

	public virtual void OnTapLoad()
	{
		if (!Engine.IsSceneGallery)
		{
			Close();
			saveLoad.OpenLoad(this);
		}
	}

	public virtual void OnTapQSave()
	{
		if (!Engine.IsSceneGallery)
		{
			Engine.Config.IsSkip = false;
			StartCoroutine(CoQSave());
		}
	}

	protected virtual IEnumerator CoQSave()
	{
		if (Engine.SaveManager.Type != AdvSaveManager.SaveType.SavePoint)
		{
			yield return new WaitForEndOfFrame();
			Engine.SaveManager.CaptureTexture = CaptureScreen();
		}
		Engine.QuickSave();
		if (Engine.SaveManager.Type != AdvSaveManager.SaveType.SavePoint)
		{
			Engine.SaveManager.ClearCaptureTexture();
		}
	}

	public virtual void OnTapQLoad()
	{
		if (!Engine.IsSceneGallery)
		{
			Engine.Config.IsSkip = false;
			Engine.QuickLoad();
		}
	}

	protected virtual Texture2D CaptureScreen()
	{
		Rect rect = LetterBoxCamera.CachedCamera.rect;
		int num = Mathf.CeilToInt(rect.x * (float)Screen.width);
		int num2 = Mathf.CeilToInt(rect.y * (float)Screen.height);
		int num3 = Mathf.FloorToInt(rect.width * (float)Screen.width);
		int num4 = Mathf.FloorToInt(rect.height * (float)Screen.height);
		return UtageToolKit.CaptureScreen(new Rect(num, num2, num3, num4));
	}
}
