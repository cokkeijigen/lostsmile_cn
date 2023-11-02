using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/TemplateUI/LoadWait")]
public class UtageUguiLoadWait : UguiView
{
	protected enum State
	{
		Start,
		Downloding,
		DownlodFinished
	}

	protected enum Type
	{
		Default,
		Boot,
		ChapterDownload
	}

	[SerializeField]
	protected AdvEngine engine;

	[SerializeField]
	protected AdvEngineStarter starter;

	public bool isAutoCacheFileLoad;

	public UtageUguiTitle title;

	public string bootSceneName;

	public GameObject buttonSkip;

	public GameObject buttonBack;

	public GameObject buttonDownload;

	public GameObject loadingBarRoot;

	public Image loadingBar;

	public Text textMain;

	public Text textCount;

	[SerializeField]
	protected OpenDialogEvent onOpenDialog;

	public AdvEngine Engine => engine ?? (engine = Object.FindObjectOfType<AdvEngine>());

	public AdvEngineStarter Starter => starter ?? (starter = Object.FindObjectOfType<AdvEngineStarter>());

	public virtual OpenDialogEvent OnOpenDialog
	{
		get
		{
			if (onOpenDialog.GetPersistentEventCount() == 0 && SystemUi.GetInstance() != null)
			{
				onOpenDialog.RemoveAllListeners();
				onOpenDialog.AddListener(SystemUi.GetInstance().OpenDialog);
			}
			return onOpenDialog;
		}
		set
		{
			onOpenDialog = value;
		}
	}

	protected virtual State CurrentState { get; set; }

	protected virtual Type DownloadType { get; set; }

	protected virtual bool AreadyTryReadCache { get; set; }

	public virtual void OpenOnBoot()
	{
		DownloadType = Type.Boot;
		Open();
	}

	public virtual void OpenOnChapter()
	{
		DownloadType = Type.ChapterDownload;
		Open();
	}

	protected virtual void OnClose()
	{
		DownloadType = Type.Default;
	}

	protected virtual void OnOpen()
	{
		switch (DownloadType)
		{
		case Type.Boot:
			if ((bool)buttonBack)
			{
				buttonBack.SetActive(false);
			}
			if ((bool)buttonSkip)
			{
				buttonSkip.SetActive(true);
			}
			if ((bool)buttonDownload)
			{
				buttonDownload.SetActive(true);
			}
			break;
		case Type.Default:
			if ((bool)buttonBack)
			{
				buttonBack.SetActive(true);
			}
			if ((bool)buttonSkip)
			{
				buttonSkip.SetActive(false);
			}
			if ((bool)buttonDownload)
			{
				buttonDownload.SetActive(true);
			}
			break;
		case Type.ChapterDownload:
			if ((bool)buttonBack)
			{
				buttonBack.SetActive(false);
			}
			if ((bool)buttonSkip)
			{
				buttonSkip.SetActive(false);
			}
			if ((bool)buttonDownload)
			{
				buttonDownload.SetActive(false);
			}
			break;
		}
		if (!Starter.IsLoadStart)
		{
			ChangeState(State.Start);
		}
		else
		{
			ChangeState(State.Downloding);
		}
	}

	protected virtual void ChangeState(State state)
	{
		CurrentState = state;
		switch (state)
		{
		case State.Start:
			buttonDownload.SetActive(true);
			loadingBarRoot.SetActive(false);
			textMain.text = "";
			textCount.text = "";
			StartLoadEngine();
			break;
		case State.Downloding:
			buttonDownload.SetActive(false);
			StartCoroutine(CoUpdateLoading());
			break;
		case State.DownlodFinished:
			OnFinished();
			break;
		}
	}

	protected virtual void OnFinished()
	{
		switch (DownloadType)
		{
		case Type.Boot:
			Close();
			title.Open();
			break;
		case Type.Default:
			buttonDownload.SetActive(false);
			loadingBarRoot.SetActive(false);
			textMain.text = LanguageSystemText.LocalizeText(SystemText.DownloadFinished);
			textCount.text = "";
			break;
		case Type.ChapterDownload:
			Close();
			break;
		}
	}

	public virtual void OnTapSkip()
	{
		Close();
		title.Open();
	}

	public virtual void OnTapReDownload()
	{
		AssetFileManager.GetInstance().AssetBundleInfoManager.DeleteAllCache();
		if (string.IsNullOrEmpty(bootSceneName))
		{
			WrapperUnityVersion.LoadScene(0);
		}
		else
		{
			WrapperUnityVersion.LoadScene(bootSceneName);
		}
	}

	protected virtual IEnumerator CoUpdateLoading()
	{
		int maxCountDownLoad = 0;
		loadingBarRoot.SetActive(true);
		loadingBar.fillAmount = 0f;
		textMain.text = LanguageSystemText.LocalizeText(SystemText.Downloading);
		textCount.text = string.Format(LanguageSystemText.LocalizeText(SystemText.DownloadCount), 0, 1);
		while (Engine.IsWaitBootLoading)
		{
			if (Starter.IsLoadErrorOnAwake)
			{
				Starter.IsLoadErrorOnAwake = false;
				OnFailedLoadEngine();
			}
			yield return null;
		}
		yield return null;
		while (!AssetFileManager.IsDownloadEnd())
		{
			yield return null;
			int num = AssetFileManager.CountDownloading();
			maxCountDownLoad = Mathf.Max(maxCountDownLoad, num);
			int num2 = maxCountDownLoad - num;
			textCount.text = string.Format(LanguageSystemText.LocalizeText(SystemText.DownloadCount), num2, maxCountDownLoad);
			if (maxCountDownLoad > 0)
			{
				loadingBar.fillAmount = 1f * (float)(maxCountDownLoad - num) / (float)maxCountDownLoad;
			}
		}
		loadingBarRoot.gameObject.SetActive(false);
		ChangeState(State.DownlodFinished);
	}

	protected virtual void StartLoadEngine()
	{
		StartCoroutine(Starter.LoadEngineAsync(OnFailedLoadEngine));
		ChangeState(State.Downloding);
	}

	protected virtual void OnFailedLoadEngine()
	{
		if (isAutoCacheFileLoad && !AreadyTryReadCache)
		{
			AreadyTryReadCache = true;
			StartCoroutine(Starter.LoadEngineAsyncFromCacheManifest(OnFailedLoadEngine));
			return;
		}
		string arg = LanguageSystemText.LocalizeText(SystemText.WarningNotOnline);
		List<ButtonEventInfo> arg2 = new List<ButtonEventInfo>
		{
			new ButtonEventInfo(LanguageSystemText.LocalizeText(SystemText.Yes), delegate
			{
				StartCoroutine(Starter.LoadEngineAsyncFromCacheManifest(OnFailedLoadEngine));
			}),
			new ButtonEventInfo(LanguageSystemText.LocalizeText(SystemText.Retry), delegate
			{
				StartCoroutine(Starter.LoadEngineAsync(OnFailedLoadEngine));
			})
		};
		OnOpenDialog.Invoke(arg, arg2);
	}

	protected bool IsMobileOffLine()
	{
		switch (Application.internetReachability)
		{
		case NetworkReachability.NotReachable:
			return true;
		default:
			return false;
		}
	}
}
