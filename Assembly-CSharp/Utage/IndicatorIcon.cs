using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/System UI/IndicatorIcon")]
	public class IndicatorIcon : MonoBehaviour
	{
		[SerializeField]
		private GameObject icon;

		[SerializeField]
		private float animTime = 1f / 12f;

		[SerializeField]
		private float animRotZ = -36f;

		[SerializeField]
		private bool isDeviceIndicator;

		private bool isStarting;

		private float rotZ;

		private List<object> objList = new List<object>();

		private void Awake()
		{
			if (IsDeviceIndicator())
			{
				WrapperUnityVersion.SetActivityIndicatorStyle();
				icon.SetActive(false);
			}
		}

		public void StartIndicator(object obj)
		{
			IncRef(obj);
			if (objList.Count > 0 && !isStarting)
			{
				base.gameObject.SetActive(true);
				isStarting = true;
				if (!IsDeviceIndicator())
				{
					InvokeRepeating("RotIcon", 0f, animTime);
				}
			}
		}

		public void StopIndicator(object obj)
		{
			DecRef(obj);
			if (objList.Count <= 0 && isStarting)
			{
				if (!IsDeviceIndicator())
				{
					CancelInvoke();
				}
				base.gameObject.SetActive(false);
				isStarting = false;
			}
		}

		private void RotIcon()
		{
			icon.transform.eulerAngles = new Vector3(0f, 0f, rotZ);
			rotZ += animRotZ;
		}

		private void IncRef(object obj)
		{
			if (!objList.Contains(obj))
			{
				objList.Add(obj);
			}
		}

		private void DecRef(object obj)
		{
			if (objList.Contains(obj))
			{
				objList.Remove(obj);
			}
		}

		private bool IsDeviceIndicator()
		{
			isDeviceIndicator = false;
			return isDeviceIndicator;
		}
	}
}
