using UnityEngine;

namespace UnityChan
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField]
		private Vector3 focus = Vector3.zero;

		[SerializeField]
		private GameObject focusObj;

		public bool showInstWindow = true;

		private Vector3 oldPos;

		private void setupFocusObject(string name)
		{
			GameObject obj = (focusObj = new GameObject(name));
			obj.transform.position = focus;
			obj.transform.LookAt(base.transform.position);
		}

		private void Start()
		{
			if (focusObj == null)
			{
				setupFocusObject("CameraFocusObject");
			}
			Transform obj = base.transform;
			base.transform.parent = focusObj.transform;
			obj.LookAt(focus);
		}

		private void Update()
		{
			mouseEvent();
		}

		private void OnGUI()
		{
			if (showInstWindow)
			{
				GUI.Box(new Rect(Screen.width - 210, Screen.height - 100, 200f, 90f), "Camera Operations");
				GUI.Label(new Rect(Screen.width - 200, Screen.height - 80, 200f, 30f), "RMB / Alt+LMB: Tumble");
				GUI.Label(new Rect(Screen.width - 200, Screen.height - 60, 200f, 30f), "MMB / Alt+Cmd+LMB: Track");
				GUI.Label(new Rect(Screen.width - 200, Screen.height - 40, 200f, 30f), "Wheel / 2 Fingers Swipe: Dolly");
			}
		}

		private void mouseEvent()
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis != 0f)
			{
				mouseWheelEvent(axis);
			}
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
			{
				oldPos = Input.mousePosition;
			}
			mouseDragEvent(Input.mousePosition);
		}

		private void mouseDragEvent(Vector3 mousePos)
		{
			Vector3 vector = mousePos - oldPos;
			if (Input.GetMouseButton(0))
			{
				if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftCommand))
				{
					if (vector.magnitude > 1E-05f)
					{
						cameraTranslate(-vector / 100f);
					}
				}
				else if (Input.GetKey(KeyCode.LeftAlt) && vector.magnitude > 1E-05f)
				{
					cameraRotate(new Vector3(vector.y, vector.x, 0f));
				}
			}
			else if (Input.GetMouseButton(2))
			{
				if (vector.magnitude > 1E-05f)
				{
					cameraTranslate(-vector / 100f);
				}
			}
			else if (Input.GetMouseButton(1) && vector.magnitude > 1E-05f)
			{
				cameraRotate(new Vector3(vector.y, vector.x, 0f));
			}
			oldPos = mousePos;
		}

		public void mouseWheelEvent(float delta)
		{
			Vector3 vector = (base.transform.position - focus) * (1f + delta);
			if ((double)vector.magnitude > 0.01)
			{
				base.transform.position = focus + vector;
			}
		}

		private void cameraTranslate(Vector3 vec)
		{
			Transform transform = focusObj.transform;
			vec.x *= -1f;
			transform.Translate(Vector3.right * vec.x);
			transform.Translate(Vector3.up * vec.y);
			focus = transform.position;
		}

		public void cameraRotate(Vector3 eulerAngle)
		{
			Quaternion identity = Quaternion.identity;
			focusObj.transform.localEulerAngles += eulerAngle;
			identity.SetLookRotation(focus);
		}
	}
}
