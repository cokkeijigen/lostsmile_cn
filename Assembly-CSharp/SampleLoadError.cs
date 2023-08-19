using System.Collections;
using UnityEngine;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/SampleLoadError")]
public class SampleLoadError : MonoBehaviour
{
	private bool isWaitingRetry;

	private void Awake()
	{
		AssetFileManager.SetLoadErrorCallBack(CustomCallbackFileLoadError);
	}

	private void CustomCallbackFileLoadError(AssetFile file)
	{
		string text = "インターネットに接続した状況でプレイしてください";
		if (SystemUi.GetInstance() != null)
		{
			if (isWaitingRetry)
			{
				StartCoroutine(CoWaitRetry(file));
				return;
			}
			isWaitingRetry = true;
			SystemUi.GetInstance().OpenDialog1Button(text, LanguageSystemText.LocalizeText(SystemText.Retry), delegate
			{
				isWaitingRetry = false;
				OnRetry(file);
			});
		}
		else
		{
			OnRetry(file);
		}
	}

	private IEnumerator CoWaitRetry(AssetFile file)
	{
		while (isWaitingRetry)
		{
			yield return null;
		}
		OnRetry(file);
	}

	private void OnRetry(AssetFile file)
	{
		AssetFileManager.ReloadFile(file);
	}
}
