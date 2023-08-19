using System.Collections;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Effect/LinkTransform")]
	public class LinkTransform : MonoBehaviour
	{
		public Transform target;

		private bool isInit;

		private Vector3 targetPosition;

		private Vector3 targetScale;

		private Vector3 targetEuler;

		private Vector3 startPosition;

		private Vector3 startScale;

		private Vector3 startEuler;

		private Transform cachedTransform;

		private Transform CachedTransform
		{
			get
			{
				if (null == cachedTransform)
				{
					cachedTransform = base.transform;
				}
				return cachedTransform;
			}
		}

		private void Start()
		{
			StartCoroutine(CoUpdate());
		}

		private void Init()
		{
			targetPosition = target.position;
			targetScale = target.localScale;
			targetEuler = target.eulerAngles;
			startPosition = CachedTransform.position;
			startScale = CachedTransform.localScale;
			startEuler = CachedTransform.eulerAngles;
			isInit = true;
		}

		private IEnumerator CoUpdate()
		{
			while (true)
			{
				if (target.gameObject.activeSelf)
				{
					if (!isInit && target.gameObject.activeSelf)
					{
						if (target as RectTransform != null)
						{
							yield return null;
						}
						Init();
					}
					if (target.transform.hasChanged)
					{
						CachedTransform.position = startPosition + (target.position - targetPosition);
						CachedTransform.localScale = startScale + (target.localScale - targetScale);
						CachedTransform.eulerAngles = startEuler + (target.eulerAngles - targetEuler);
					}
				}
				yield return null;
			}
		}
	}
}
