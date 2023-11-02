using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/System UI/FramerateChanger")]
	public class SystemUiFramerateChanger : MonoBehaviour
	{
		[SerializeField]
		private Text text;

		private List<int> frameRateList = new List<int> { 30, 60, 120 };

		private List<int> vSyncCountList = new List<int> { 2, 1, 0 };

		private int currentIndex;

		private void Update()
		{
			if (text != null)
			{
				text.text = $"FPS:{Application.targetFrameRate}";
			}
		}

		public int TargetFrameRate()
		{
			return Application.targetFrameRate;
		}

		public void OnClickChangeFrameRate()
		{
			currentIndex = (currentIndex + 1) % frameRateList.Count;
			Application.targetFrameRate = frameRateList[currentIndex];
			QualitySettings.vSyncCount = vSyncCountList[currentIndex];
		}
	}
}
