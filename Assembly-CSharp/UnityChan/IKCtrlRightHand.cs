using UnityEngine;

namespace UnityChan
{
	[RequireComponent(typeof(Animator))]
	public class IKCtrlRightHand : MonoBehaviour
	{
		private Animator anim;

		public Transform targetObj;

		public bool isIkActive;

		public float mixWeight = 1f;

		private void Awake()
		{
			anim = GetComponent<Animator>();
		}

		private void Update()
		{
			if (mixWeight >= 1f)
			{
				mixWeight = 1f;
			}
			else if (mixWeight <= 0f)
			{
				mixWeight = 0f;
			}
		}

		private void OnAnimatorIK(int layerIndex)
		{
			if (isIkActive)
			{
				anim.SetIKPositionWeight(AvatarIKGoal.RightHand, mixWeight);
				anim.SetIKRotationWeight(AvatarIKGoal.RightHand, mixWeight);
				anim.SetIKPosition(AvatarIKGoal.RightHand, targetObj.position);
				anim.SetIKRotation(AvatarIKGoal.RightHand, targetObj.rotation);
			}
		}

		private void OnGUI()
		{
			Rect position = new Rect(10f, Screen.height - 20, 400f, 30f);
			isIkActive = GUI.Toggle(position, isIkActive, "IK Active");
		}
	}
}
