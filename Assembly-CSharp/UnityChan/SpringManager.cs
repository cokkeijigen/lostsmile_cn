using UnityEngine;

namespace UnityChan
{
	public class SpringManager : MonoBehaviour
	{
		public float dynamicRatio = 1f;

		public float stiffnessForce;

		public AnimationCurve stiffnessCurve;

		public float dragForce;

		public AnimationCurve dragCurve;

		public SpringBone[] springBones;

		private void Start()
		{
			UpdateParameters();
		}

		private void LateUpdate()
		{
			if (dynamicRatio == 0f)
			{
				return;
			}
			for (int i = 0; i < springBones.Length; i++)
			{
				if (dynamicRatio > springBones[i].threshold)
				{
					springBones[i].UpdateSpring();
				}
			}
		}

		private void UpdateParameters()
		{
			UpdateParameter("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter("dragForce", dragForce, dragCurve);
		}

		private void UpdateParameter(string fieldName, float baseValue, AnimationCurve curve)
		{
		}
	}
}
