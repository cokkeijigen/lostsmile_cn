using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/BackgroundRaycastReciever")]
	public class UguiBackgroundRaycastReciever : MonoBehaviour
	{
		[SerializeField]
		private UguiBackgroundRaycaster raycaster;

		public UguiBackgroundRaycaster Raycaster
		{
			get
			{
				return raycaster ?? (raycaster = Object.FindObjectOfType<UguiBackgroundRaycaster>());
			}
			set
			{
				raycaster = value;
			}
		}

		private void OnEnable()
		{
			Raycaster.AddTarget(base.gameObject);
		}

		private void OnDisable()
		{
			Raycaster.RemoveTarget(base.gameObject);
		}
	}
}
