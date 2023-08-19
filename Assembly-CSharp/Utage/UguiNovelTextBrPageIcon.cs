using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/NovelTextBrPageIcon")]
	public class UguiNovelTextBrPageIcon : Text
	{
		public UguiNovelText novelText;

		private void Update()
		{
			Vector2 vector = novelText.EndPosition;
			base.gameObject.transform.localPosition = vector;
		}
	}
}
