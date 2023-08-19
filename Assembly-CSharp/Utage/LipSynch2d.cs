using System.Collections;
using UnityEngine;

namespace Utage
{
	public abstract class LipSynch2d : LipSynchBase
	{
		[SerializeField]
		private float duration = 0.2f;

		[SerializeField]
		private float interval = 0.2f;

		[SerializeField]
		private float scaleVoiceVolume = 1f;

		[SerializeField]
		private string lipTag = "lip";

		[SerializeField]
		private MiniAnimationData animationData = new MiniAnimationData();

		[SerializeField]
		private GameObject target;

		protected Coroutine coLypSync;

		public float Duration
		{
			get
			{
				return duration;
			}
			set
			{
				duration = value;
			}
		}

		public float Interval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
			}
		}

		public float ScaleVoiceVolume
		{
			get
			{
				return scaleVoiceVolume;
			}
			set
			{
				scaleVoiceVolume = value;
			}
		}

		public string LipTag
		{
			get
			{
				return lipTag;
			}
			set
			{
				lipTag = value;
			}
		}

		public MiniAnimationData AnimationData
		{
			get
			{
				return animationData;
			}
			set
			{
				animationData = value;
			}
		}

		public float LipSyncVolume { get; set; }

		public GameObject Target
		{
			get
			{
				if (target == null)
				{
					target = base.gameObject;
				}
				return target;
			}
			set
			{
				target = value;
			}
		}

		protected override void OnStartLipSync()
		{
			if (coLypSync == null)
			{
				coLypSync = StartCoroutine(CoUpdateLipSync());
			}
		}

		protected override void OnUpdateLipSync()
		{
			if (CheckVoiceLipSync())
			{
				LipSyncVolume = SoundManager.GetInstance().GetVoiceSamplesVolume(base.CharacterLabel) * ScaleVoiceVolume;
			}
			else
			{
				LipSyncVolume = -1f;
			}
		}

		protected override void OnStopLipSync()
		{
			if (coLypSync != null)
			{
				StopCoroutine(coLypSync);
				coLypSync = null;
			}
		}

		protected abstract IEnumerator CoUpdateLipSync();
	}
}
