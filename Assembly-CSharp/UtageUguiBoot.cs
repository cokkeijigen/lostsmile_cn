using System.Collections;
using UnityEngine;
using Utage;

[AddComponentMenu("Utage/TemplateUI/Title")]
public class UtageUguiBoot : UguiView
{
	[SerializeField]
	protected AdvEngine engine;

	public UguiFadeTextureStream fadeTextureStream;

	public UtageUguiTitle title;

	public UtageUguiLoadWait loadWait;

	public bool isWaitBoot;

	public bool isWaitDownLoad;

	public bool isWaitSplashScreen = true;

	public AdvEngine Engine
	{
		get
		{
			return engine ?? (engine = Object.FindObjectOfType<AdvEngine>());
		}
	}

	public virtual void Start()
	{
		title.gameObject.SetActive(false);
		StartCoroutine(CoUpdate());
	}

	protected virtual IEnumerator CoUpdate()
	{
		if (isWaitSplashScreen)
		{
			while (!WrapperUnityVersion.IsFinishedSplashScreen())
			{
				yield return null;
			}
		}
		Open();
		if ((bool)fadeTextureStream)
		{
			fadeTextureStream.gameObject.SetActive(true);
			fadeTextureStream.Play();
			while (fadeTextureStream.IsPlaying)
			{
				yield return null;
			}
		}
		if (isWaitBoot)
		{
			while (Engine.IsWaitBootLoading)
			{
				yield return null;
			}
		}
		Close();
		if (isWaitDownLoad && loadWait != null)
		{
			loadWait.OpenOnBoot();
		}
		else
		{
			title.Open();
		}
	}
}
