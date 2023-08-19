using System.Collections;
using UnityEngine;

namespace UnityChan
{
	public class RandomWind : MonoBehaviour
	{
		private SpringBone[] springBones;

		public bool isWindActive;

		private bool isMinus;

		public float threshold = 0.5f;

		public float interval = 5f;

		public float windPower = 1f;

		public float gravity = 0.98f;

		public bool isGUI = true;

		private void Start()
		{
			springBones = GetComponent<SpringManager>().springBones;
			StartCoroutine("RandomChange");
		}

		private void Update()
		{
			Vector3 zero = Vector3.zero;
			if (isWindActive)
			{
				zero = ((!isMinus) ? new Vector3(Mathf.PerlinNoise(Time.time, 0f) * windPower * 0.001f, gravity * -0.001f, 0f) : new Vector3(Mathf.PerlinNoise(Time.time, 0f) * windPower * -0.001f, gravity * -0.001f, 0f));
				for (int i = 0; i < springBones.Length; i++)
				{
					springBones[i].springForce = zero;
				}
			}
		}

		private void OnGUI()
		{
			if (isGUI)
			{
				Rect position = new Rect(10f, Screen.height - 40, 400f, 30f);
				isWindActive = GUI.Toggle(position, isWindActive, "Random Wind");
			}
		}

		private IEnumerator RandomChange()
		{
			while (true)
			{
				if (Random.Range(0f, 1f) > threshold)
				{
					isMinus = true;
				}
				else
				{
					isMinus = false;
				}
				yield return new WaitForSeconds(interval);
			}
		}
	}
}
