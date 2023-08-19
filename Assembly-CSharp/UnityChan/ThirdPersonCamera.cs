using UnityEngine;

namespace UnityChan
{
	public class ThirdPersonCamera : MonoBehaviour
	{
		public float smooth = 3f;

		private Transform standardPos;

		private Transform frontPos;

		private Transform jumpPos;

		private bool bQuickSwitch;

		private void Start()
		{
			standardPos = GameObject.Find("CamPos").transform;
			if ((bool)GameObject.Find("FrontPos"))
			{
				frontPos = GameObject.Find("FrontPos").transform;
			}
			if ((bool)GameObject.Find("JumpPos"))
			{
				jumpPos = GameObject.Find("JumpPos").transform;
			}
			base.transform.position = standardPos.position;
			base.transform.forward = standardPos.forward;
		}

		private void FixedUpdate()
		{
			if (Input.GetButton("Fire1"))
			{
				setCameraPositionFrontView();
			}
			else if (Input.GetButton("Fire2"))
			{
				setCameraPositionJumpView();
			}
			else
			{
				setCameraPositionNormalView();
			}
		}

		private void setCameraPositionNormalView()
		{
			if (!bQuickSwitch)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, standardPos.position, Time.fixedDeltaTime * smooth);
				base.transform.forward = Vector3.Lerp(base.transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
			}
			else
			{
				base.transform.position = standardPos.position;
				base.transform.forward = standardPos.forward;
				bQuickSwitch = false;
			}
		}

		private void setCameraPositionFrontView()
		{
			bQuickSwitch = true;
			base.transform.position = frontPos.position;
			base.transform.forward = frontPos.forward;
		}

		private void setCameraPositionJumpView()
		{
			bQuickSwitch = false;
			base.transform.position = Vector3.Lerp(base.transform.position, jumpPos.position, Time.fixedDeltaTime * smooth);
			base.transform.forward = Vector3.Lerp(base.transform.forward, jumpPos.forward, Time.fixedDeltaTime * smooth);
		}
	}
}
