using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Other/DontDestoryOnLoad")]
	public class DontDestoryOnLoad : MonoBehaviour
	{
		[SerializeField]
		private bool dontDestoryOnLoad;

		private void Awake()
		{
			if (dontDestoryOnLoad)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}
	}
}
