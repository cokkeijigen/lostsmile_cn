using UnityEngine.Events;

namespace Utage
{
	public class ButtonEventInfo
	{
		public string text;

		public UnityAction callBackClicked;

		public ButtonEventInfo(string text, UnityAction callBackClicked)
		{
			this.text = text;
			this.callBackClicked = callBackClicked;
		}
	}
}
