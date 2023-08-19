using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Effect/ParticleAutomaticDestroyer")]
	public class ParticleAutomaticDestroyer : MonoBehaviour
	{
		private bool isPlalyed;

		private void Update()
		{
			if (CheckPlaying())
			{
				isPlalyed = true;
			}
			else if (isPlalyed)
			{
				Object.Destroy(base.gameObject);
			}
		}

		private bool CheckPlaying()
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].isPlaying)
				{
					return true;
				}
			}
			return false;
		}
	}
}
