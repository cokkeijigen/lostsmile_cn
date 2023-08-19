using System.Collections;
using UnityEngine;

namespace UnityChan
{
	[RequireComponent(typeof(Animator))]
	public class IdleChanger : MonoBehaviour
	{
		private Animator anim;

		private AnimatorStateInfo currentState;

		private AnimatorStateInfo previousState;

		public bool _random;

		public float _threshold = 0.5f;

		public float _interval = 10f;

		public bool isGUI = true;

		private void Start()
		{
			anim = GetComponent<Animator>();
			currentState = anim.GetCurrentAnimatorStateInfo(0);
			previousState = currentState;
			StartCoroutine("RandomChange");
		}

		private void Update()
		{
			if (Input.GetKeyDown("up") || Input.GetButton("Jump"))
			{
				anim.SetBool("Next", true);
			}
			if (Input.GetKeyDown("down"))
			{
				anim.SetBool("Back", true);
			}
			if (anim.GetBool("Next"))
			{
				currentState = anim.GetCurrentAnimatorStateInfo(0);
				if (previousState.fullPathHash != currentState.fullPathHash)
				{
					anim.SetBool("Next", false);
					previousState = currentState;
				}
			}
			if (anim.GetBool("Back"))
			{
				currentState = anim.GetCurrentAnimatorStateInfo(0);
				if (previousState.fullPathHash != currentState.fullPathHash)
				{
					anim.SetBool("Back", false);
					previousState = currentState;
				}
			}
		}

		private void OnGUI()
		{
			if (isGUI)
			{
				GUI.Box(new Rect(Screen.width - 110, 10f, 100f, 90f), "Change Motion");
				if (GUI.Button(new Rect(Screen.width - 100, 40f, 80f, 20f), "Next"))
				{
					anim.SetBool("Next", true);
				}
				if (GUI.Button(new Rect(Screen.width - 100, 70f, 80f, 20f), "Back"))
				{
					anim.SetBool("Back", true);
				}
			}
		}

		private IEnumerator RandomChange()
		{
			while (true)
			{
				if (_random)
				{
					float num = Random.Range(0f, 1f);
					if (num < _threshold)
					{
						anim.SetBool("Back", true);
					}
					else if (num >= _threshold)
					{
						anim.SetBool("Next", true);
					}
				}
				yield return new WaitForSeconds(_interval);
			}
		}
	}
}
