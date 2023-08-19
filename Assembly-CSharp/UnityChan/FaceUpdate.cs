using UnityEngine;

namespace UnityChan
{
	public class FaceUpdate : MonoBehaviour
	{
		public AnimationClip[] animations;

		private Animator anim;

		public float delayWeight;

		public bool isKeepFace;

		public bool isGUI = true;

		private float current;

		private void Start()
		{
			anim = GetComponent<Animator>();
		}

		private void OnGUI()
		{
			if (!isGUI)
			{
				return;
			}
			GUILayout.Box("Face Update", GUILayout.Width(170f), GUILayout.Height(25 * (animations.Length + 2)));
			GUILayout.BeginArea(new Rect(10f, 25f, 150f, 25 * (animations.Length + 1)));
			AnimationClip[] array = animations;
			foreach (AnimationClip animationClip in array)
			{
				if (GUILayout.RepeatButton(animationClip.name))
				{
					anim.CrossFade(animationClip.name, 0f);
				}
			}
			isKeepFace = GUILayout.Toggle(isKeepFace, " Keep Face");
			GUILayout.EndArea();
		}

		private void Update()
		{
			if (Input.GetMouseButton(0))
			{
				current = 1f;
			}
			else if (!isKeepFace)
			{
				current = Mathf.Lerp(current, 0f, delayWeight);
			}
			anim.SetLayerWeight(1, current);
		}

		public void OnCallChangeFace(string str)
		{
			int num = 0;
			AnimationClip[] array = animations;
			foreach (AnimationClip animationClip in array)
			{
				if (str == animationClip.name)
				{
					ChangeFace(str);
					break;
				}
				if (num <= animations.Length)
				{
					num++;
					continue;
				}
				str = "default@unitychan";
				ChangeFace(str);
			}
		}

		private void ChangeFace(string str)
		{
			isKeepFace = true;
			current = 1f;
			anim.CrossFade(str, 0f);
		}
	}
}
