using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/System UI/Dialog2Button")]
	public class SystemUiDialog2Button : SystemUiDialog1Button
	{
		[SerializeField]
		protected Text button2Text;

		[SerializeField]
		protected UnityEvent OnClickButton2;

		public virtual void Open(string text, string buttonText1, string buttonText2, UnityAction callbackOnClickButton1, UnityAction callbackOnClickButton2)
		{
			button2Text.text = buttonText2;
			OnClickButton2.RemoveAllListeners();
			OnClickButton2.AddListener(callbackOnClickButton2);
			base.Open(text, buttonText1, callbackOnClickButton1);
		}

		public virtual void OnClickButton2Sub()
		{
			OnClickButton2.Invoke();
			Close();
		}
	}
}
