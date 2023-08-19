using UnityEngine;

namespace UnityChan
{
	public class IKLookAt : MonoBehaviour
	{
		private Animator avator;

		private MeshRenderer target;

		public bool ikActive;

		public Transform lookAtObj;

		public float lookAtWeight = 1f;

		public float bodyWeight = 0.3f;

		public float headWeight = 0.8f;

		public float eyesWeight = 1f;

		public float clampWeight = 0.5f;

		public bool isGUI = true;

		private void Start()
		{
			avator = GetComponent<Animator>();
			if (lookAtObj != null)
			{
				target = lookAtObj.GetComponentInParent<MeshRenderer>();
				target.enabled = false;
			}
		}

		private void OnGUI()
		{
			if (isGUI)
			{
				Rect position = new Rect(Screen.width - 120, Screen.height - 40, 100f, 30f);
				ikActive = GUI.Toggle(position, ikActive, "Look at Target");
			}
		}

		private void OnAnimatorIK(int layorIndex)
		{
			if (!avator)
			{
				return;
			}
			if (ikActive)
			{
				avator.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyesWeight, clampWeight);
				if (lookAtObj != null)
				{
					target.enabled = true;
					avator.SetLookAtPosition(lookAtObj.position);
				}
				else
				{
					avator.SetLookAtWeight(0f);
				}
			}
			else
			{
				avator.SetLookAtWeight(0f);
			}
		}

		private void Update()
		{
			if ((bool)avator && !ikActive && lookAtObj != null)
			{
				target.enabled = false;
			}
		}
	}
}
