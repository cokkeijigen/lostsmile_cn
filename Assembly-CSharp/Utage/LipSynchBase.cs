using UnityEngine;

namespace Utage
{
	public abstract class LipSynchBase : MonoBehaviour
	{
		[SerializeField]
		private LipSynchType type = LipSynchType.TextAndVoice;

		public LipSynchEvent OnCheckTextLipSync = new LipSynchEvent();

		private string characterLabel;

		public LipSynchType Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public bool EnableTextLipSync { get; set; }

		public LipSynchMode LipSynchMode { get; set; }

		public string CharacterLabel
		{
			get
			{
				if (string.IsNullOrEmpty(characterLabel))
				{
					return base.gameObject.name;
				}
				return characterLabel;
			}
			set
			{
				characterLabel = value;
			}
		}

		public bool IsEnable { get; set; }

		public bool IsPlaying { get; set; }

		public void Play()
		{
			IsEnable = true;
		}

		public void Cancel()
		{
			IsEnable = false;
			IsPlaying = false;
			OnStopLipSync();
		}

		protected virtual void Update()
		{
			bool flag = CheckVoiceLipSync();
			bool flag2 = CheckTextLipSync();
			LipSynchMode = (flag ? LipSynchMode.Voice : LipSynchMode.Text);
			if (IsEnable && (flag || flag2))
			{
				if (!IsPlaying)
				{
					IsPlaying = true;
					OnStartLipSync();
				}
				OnUpdateLipSync();
			}
			else if (IsPlaying)
			{
				IsPlaying = false;
				OnStopLipSync();
			}
		}

		protected bool CheckVoiceLipSync()
		{
			LipSynchType lipSynchType = Type;
			if ((uint)(lipSynchType - 1) <= 1u)
			{
				SoundManager instance = SoundManager.GetInstance();
				if (instance != null && instance.IsPlayingVoice(CharacterLabel))
				{
					return true;
				}
			}
			return false;
		}

		protected bool CheckTextLipSync()
		{
			LipSynchType lipSynchType = Type;
			if (lipSynchType == LipSynchType.Text || lipSynchType == LipSynchType.TextAndVoice)
			{
				OnCheckTextLipSync.Invoke(this);
				return EnableTextLipSync;
			}
			return false;
		}

		protected abstract void OnStartLipSync();

		protected abstract void OnUpdateLipSync();

		protected abstract void OnStopLipSync();
	}
}
