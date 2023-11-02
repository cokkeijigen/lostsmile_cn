using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/TextSound")]
	public class AdvTextSound : MonoBehaviour
	{
		public enum Type
		{
			Time,
			CharacterCount
		}

		[Serializable]
		public class SoundInfo
		{
			public string key;

			public AudioClip sound;
		}

		[SerializeField]
		private bool disable;

		[SerializeField]
		protected AdvEngine engine;

		public Type type;

		public AudioClip defaultSound;

		public List<SoundInfo> soundInfoList = new List<SoundInfo>();

		public int intervalCount = 3;

		public float intervalTime = 0.1f;

		private float lastTime;

		private int lastCharacterCount;

		public bool Disable
		{
			get
			{
				return disable;
			}
			set
			{
				disable = value;
			}
		}

		public AdvEngine Engine => engine ?? (engine = UnityEngine.Object.FindObjectOfType<AdvEngine>());

		private void Awake()
		{
			Engine.Page.OnBeginPage.AddListener(OnBeginPage);
			Engine.Page.OnUpdateSendChar.AddListener(OnUpdateSendChar);
		}

		private void OnBeginPage(AdvPage page)
		{
			lastTime = 0f;
			lastCharacterCount = -intervalCount;
		}

		private void OnUpdateSendChar(AdvPage page)
		{
			if (CheckPlaySe(page))
			{
				AudioClip se = GetSe(page);
				if (se != null)
				{
					SoundManager.GetInstance().PlaySe(se);
					lastCharacterCount = page.CurrentTextLength;
					lastTime = Time.time;
				}
			}
		}

		private bool CheckPlaySe(AdvPage page)
		{
			if (Disable)
			{
				return false;
			}
			if (page.CurrentTextLength == lastCharacterCount)
			{
				return false;
			}
			switch (type)
			{
			case Type.Time:
				if (Time.time - lastTime > intervalTime)
				{
					return true;
				}
				break;
			case Type.CharacterCount:
				if (page.CurrentTextLength >= lastCharacterCount + intervalCount)
				{
					return true;
				}
				break;
			}
			return false;
		}

		private AudioClip GetSe(AdvPage page)
		{
			SoundInfo soundInfo = soundInfoList.Find((SoundInfo x) => x.key == page.CharacterLabel);
			if (soundInfo == null)
			{
				return defaultSound;
			}
			return soundInfo.sound;
		}
	}
}
