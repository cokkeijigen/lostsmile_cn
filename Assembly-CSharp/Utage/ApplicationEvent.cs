using UnityEngine;
using UnityEngine.Events;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Events/ApplicationEvent")]
	public class ApplicationEvent : MonoBehaviour
	{
		private static ApplicationEvent instance;

		public UnityEvent OnScreenSizeChanged = new UnityEvent();

		private int screenWidth;

		private int screenHeight;

		public static ApplicationEvent Get()
		{
			if (instance == null)
			{
				instance = new GameObject
				{
					hideFlags = HideFlags.HideAndDontSave
				}.AddComponent<ApplicationEvent>();
			}
			return instance;
		}

		private void Awake()
		{
			instance = this;
			screenWidth = Screen.width;
			screenHeight = Screen.height;
		}

		private void Update()
		{
			if (screenWidth != Screen.width || screenHeight != Screen.height)
			{
				screenWidth = Screen.width;
				screenHeight = Screen.height;
				OnScreenSizeChanged.Invoke();
			}
		}
	}
}
