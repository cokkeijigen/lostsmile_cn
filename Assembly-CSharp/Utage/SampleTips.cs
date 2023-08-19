using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Examples/Tips")]
	public class SampleTips : MonoBehaviour
	{
		public void OnClickTips(UguiNovelTextHitArea hit)
		{
			Debug.Log(hit.Arg);
		}
	}
}
