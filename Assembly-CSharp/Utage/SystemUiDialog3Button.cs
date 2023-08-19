using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/System UI/Dialog3Button")]
	public class SystemUiDialog3Button : SystemUiDialog2Button
	{
		[SerializeField]
		protected Text button3Text;

		[SerializeField]
		protected UnityEvent OnClickButton3;

		public virtual void Open(string text, string buttonText1, string buttonText2, string buttonText3, UnityAction callbackOnClickButton1, UnityAction callbackOnClickButton2, UnityAction callbackOnClickButton3)
		{
			button3Text.text = buttonText3;
			OnClickButton3.RemoveAllListeners();
			OnClickButton3.AddListener(callbackOnClickButton3);
			base.Open(text, buttonText1, buttonText2, callbackOnClickButton1, callbackOnClickButton2);
		}

		public virtual void OnClickButton3Sub()
		{
			OnClickButton3.Invoke();
			Close();
		}
	}
}
