using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/MessageWindowManager")]
	public class AdvUguiMessageWindowManager : MonoBehaviour
	{
		internal virtual void Close()
		{
			base.gameObject.SetActive(false);
		}

		internal virtual void Open()
		{
			base.gameObject.SetActive(true);
		}
	}
}
