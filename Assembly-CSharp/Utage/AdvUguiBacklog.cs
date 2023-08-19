using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/UiBacklog")]
	[RequireComponent(typeof(Button))]
	public class AdvUguiBacklog : MonoBehaviour
	{
		public UguiNovelText text;

		public Text characterName;

		public GameObject soundIcon;

		private Button button;

		public bool isMultiTextInPage;

		protected AdvBacklog data;

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

		public virtual void Init(AdvBacklog data)
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
			}
			else if (countVoice >= 2 || isMultiTextInPage)
			{
				text.gameObject.GetComponentCreateIfMissing<UguiNovelTextEventTrigger>().OnClick.AddListener(delegate(UguiNovelTextHitArea x)
				{
					OnClickHitArea(x, OnClicked);
				});
			}
			else
			{
				Button.onClick.AddListener(delegate
				{
					OnClicked(data.MainVoiceFileName);
				});
			}
		}

		protected virtual void OnClickHitArea(UguiNovelTextHitArea hitGroup, Action<string> OnClicked)
		{
			if (hitGroup.HitEventType == CharData.HitEventType.Sound)
			{
				OnClicked(hitGroup.Arg);
			}
		}

		protected virtual void OnClicked(string voiceFileName)
		{
			if (!string.IsNullOrEmpty(voiceFileName))
			{
				StartCoroutine(CoPlayVoice(voiceFileName, Data.FindCharacerLabel(voiceFileName)));
			}
		}

		protected virtual IEnumerator CoPlayVoice(string voiceFileName, string characterLabel)
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
}
