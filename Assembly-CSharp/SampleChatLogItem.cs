using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utage;
using UtageExtensions;

[AddComponentMenu("Utage/ADV/Examples/ChatLogItem")]
[RequireComponent(typeof(Button))]
public class SampleChatLogItem : MonoBehaviour
{
	public UguiNovelText text;

	public Text characterName;

	public GameObject soundIcon;

	private Button button;

	public bool isMultiTextInPage;

	private AdvBacklog data;

	public Button Button
	{
		get
		{
			return button ?? (button = GetComponent<Button>());
		}
	}

	public AdvBacklog Data
	{
		get
		{
			return data;
		}
	}

	private void OnInitData(AdvBacklog data)
	{
		this.data = data;
		if (isMultiTextInPage)
		{
			float height = text.rectTransform.rect.height;
			text.text = data.Text;
			float preferredHeight = text.preferredHeight;
			(text.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
			float height2 = (base.transform as RectTransform).rect.height;
			float num = text.transform.lossyScale.y / base.transform.lossyScale.y;
			height2 += (preferredHeight - height) * num;
			(base.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height2);
		}
		else
		{
			text.text = data.Text;
		}
		characterName.text = data.MainCharacterNameText;
		int countVoice = data.CountVoice;
		if (countVoice <= 0)
		{
			soundIcon.SetActive(false);
			Button.interactable = false;
			return;
		}
		Button.onClick.AddListener(delegate
		{
			OnClicked(data.MainVoiceFileName);
		});
		if (countVoice > 2 || isMultiTextInPage)
		{
			text.gameObject.GetComponentCreateIfMissing<UguiNovelTextEventTrigger>().OnClick.AddListener(delegate(UguiNovelTextHitArea x)
			{
				OnClickHitArea(x, OnClicked);
			});
		}
	}

	private void OnClickHitArea(UguiNovelTextHitArea hitGroup, Action<string> OnClicked)
	{
		if (hitGroup.HitEventType == CharData.HitEventType.Sound)
		{
			OnClicked(hitGroup.Arg);
		}
	}

	private void OnClicked(string voiceFileName)
	{
		if (!string.IsNullOrEmpty(voiceFileName))
		{
			StartCoroutine(CoPlayVoice(voiceFileName, Data.FindCharacerLabel(voiceFileName)));
		}
	}

	private IEnumerator CoPlayVoice(string voiceFileName, string characterLabel)
	{
		AssetFile file = AssetFileManager.Load(voiceFileName, this);
		if (file == null)
		{
			Debug.LogError("Backlog voiceFile is NULL");
			yield break;
		}
		while (!file.IsLoadEnd)
		{
			yield return null;
		}
		SoundManager instance = SoundManager.GetInstance();
		if ((bool)instance)
		{
			instance.PlayVoice(characterLabel, file);
		}
		file.Unuse(this);
	}
}
