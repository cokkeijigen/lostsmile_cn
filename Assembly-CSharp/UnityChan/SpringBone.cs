using UnityEngine;

namespace UnityChan
{
	public class SpringBone : MonoBehaviour
	{
		public Transform child;

		public Vector3 boneAxis = new Vector3(-1f, 0f, 0f);

		public float radius = 0.05f;

		public bool isUseEachBoneForceSettings;

		public float stiffnessForce = 0.01f;

		public float dragForce = 0.4f;

		public Vector3 springForce = new Vector3(0f, -0.0001f, 0f);

		public SpringCollider[] colliders;

		public bool debug = true;

		public float threshold = 0.01f;

		private float springLength;

		private Quaternion localRotation;

		private Transform trs;

		private Vector3 currTipPos;

		private Vector3 prevTipPos;

		private Transform org;

		private SpringManager managerRef;

		private void Awake()
		{
			trs = base.transform;
			localRotation = base.transform.localRotation;
			managerRef = GetParentSpringManager(base.transform);
		}

		private SpringManager GetParentSpringManager(Transform t)
		{
			SpringManager component = t.GetComponent<SpringManager>();
			if (component != null)
			{
				return component;
			}
			if (t.parent != null)
			{
				return GetParentSpringManager(t.parent);
			}
			return null;
		}

		private void Start()
		{
			springLength = Vector3.Distance(trs.position, child.position);
			currTipPos = child.position;
			prevTipPos = child.position;
		}

		public void UpdateSpring()
		{
			org = trs;
			trs.localRotation = Quaternion.identity * localRotation;
			float num = Time.deltaTime * Time.deltaTime;
			Vector3 vector = trs.rotation * (boneAxis * stiffnessForce) / num;
			vector += (prevTipPos - currTipPos) * dragForce / num;
			vector += springForce / num;
			Vector3 vector2 = currTipPos;
			currTipPos = currTipPos - prevTipPos + currTipPos + vector * num;
			currTipPos = (currTipPos - trs.position).normalized * springLength + trs.position;
			for (int i = 0; i < colliders.Length; i++)
			{
				if (Vector3.Distance(currTipPos, colliders[i].transform.position) <= radius + colliders[i].radius)
				{
					Vector3 normalized = (currTipPos - colliders[i].transform.position).normalized;
					currTipPos = colliders[i].transform.position + normalized * (radius + colliders[i].radius);
					currTipPos = (currTipPos - trs.position).normalized * springLength + trs.position;
				}
			}
			prevTipPos = vector2;
			Quaternion b = Quaternion.FromToRotation(trs.TransformDirection(boneAxis), currTipPos - trs.position) * trs.rotation;
			trs.rotation = Quaternion.Lerp(org.rotation, b, managerRef.dynamicRatio);
		}

		private void OnDrawGizmos()
		{
			if (debug)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(currTipPos, radius);
			}
		}
	}
}
