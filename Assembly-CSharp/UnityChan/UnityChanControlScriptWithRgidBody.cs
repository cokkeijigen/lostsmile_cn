using UnityEngine;

namespace UnityChan
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class UnityChanControlScriptWithRgidBody : MonoBehaviour
	{
		public float animSpeed = 1.5f;

		public float lookSmoother = 3f;

		public bool useCurves = true;

		public float useCurvesHeight = 0.5f;

		public float forwardSpeed = 7f;

		public float backwardSpeed = 2f;

		public float rotateSpeed = 2f;

		public float jumpPower = 3f;

		private CapsuleCollider col;

		private Rigidbody rb;

		private Vector3 velocity;

		private float orgColHight;

		private Vector3 orgVectColCenter;

		private Animator anim;

		private AnimatorStateInfo currentBaseState;

		private GameObject cameraObject;

		private static int idleState = Animator.StringToHash("Base Layer.Idle");

		private static int locoState = Animator.StringToHash("Base Layer.Locomotion");

		private static int jumpState = Animator.StringToHash("Base Layer.Jump");

		private static int restState = Animator.StringToHash("Base Layer.Rest");

		private void Start()
		{
			anim = GetComponent<Animator>();
			col = GetComponent<CapsuleCollider>();
			rb = GetComponent<Rigidbody>();
			cameraObject = GameObject.FindWithTag("MainCamera");
			orgColHight = col.height;
			orgVectColCenter = col.center;
		}

		private void FixedUpdate()
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			anim.SetFloat("Speed", axis2);
			anim.SetFloat("Direction", axis);
			anim.speed = animSpeed;
			currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
			rb.useGravity = true;
			velocity = new Vector3(0f, 0f, axis2);
			velocity = base.transform.TransformDirection(velocity);
			if ((double)axis2 > 0.1)
			{
				velocity *= forwardSpeed;
			}
			else if ((double)axis2 < -0.1)
			{
				velocity *= backwardSpeed;
			}
			if (Input.GetButtonDown("Jump") && currentBaseState.fullPathHash == locoState && !anim.IsInTransition(0))
			{
				rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
				anim.SetBool("Jump", true);
			}
			base.transform.localPosition += velocity * Time.fixedDeltaTime;
			base.transform.Rotate(0f, axis * rotateSpeed, 0f);
			if (currentBaseState.fullPathHash == locoState)
			{
				if (useCurves)
				{
					resetCollider();
				}
			}
			else if (currentBaseState.fullPathHash == jumpState)
			{
				cameraObject.SendMessage("setCameraPositionJumpView");
				if (anim.IsInTransition(0))
				{
					return;
				}
				if (useCurves)
				{
					float @float = anim.GetFloat("JumpHeight");
					if (anim.GetFloat("GravityControl") > 0f)
					{
						rb.useGravity = false;
					}
					Ray ray = new Ray(base.transform.position + Vector3.up, -Vector3.up);
					RaycastHit hitInfo = default(RaycastHit);
					if (Physics.Raycast(ray, out hitInfo))
					{
						if (hitInfo.distance > useCurvesHeight)
						{
							col.height = orgColHight - @float;
							float y = orgVectColCenter.y + @float;
							col.center = new Vector3(0f, y, 0f);
						}
						else
						{
							resetCollider();
						}
					}
				}
				anim.SetBool("Jump", false);
			}
			else if (currentBaseState.fullPathHash == idleState)
			{
				if (useCurves)
				{
					resetCollider();
				}
				if (Input.GetButtonDown("Jump"))
				{
					anim.SetBool("Rest", true);
				}
			}
			else if (currentBaseState.fullPathHash == restState && !anim.IsInTransition(0))
			{
				anim.SetBool("Rest", false);
			}
		}

		private void OnGUI()
		{
			GUI.Box(new Rect(Screen.width - 260, 10f, 250f, 150f), "Interaction");
			GUI.Label(new Rect(Screen.width - 245, 30f, 250f, 30f), "Up/Down Arrow : Go Forwald/Go Back");
			GUI.Label(new Rect(Screen.width - 245, 50f, 250f, 30f), "Left/Right Arrow : Turn Left/Turn Right");
			GUI.Label(new Rect(Screen.width - 245, 70f, 250f, 30f), "Hit Space key while Running : Jump");
			GUI.Label(new Rect(Screen.width - 245, 90f, 250f, 30f), "Hit Spase key while Stopping : Rest");
			GUI.Label(new Rect(Screen.width - 245, 110f, 250f, 30f), "Left Control : Front Camera");
			GUI.Label(new Rect(Screen.width - 245, 130f, 250f, 30f), "Alt : LookAt Camera");
		}

		private void resetCollider()
		{
			col.height = orgColHight;
			col.center = orgVectColCenter;
		}
	}
}
