using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/System UI/Dialog1Button")]
	public class SystemUiDialog1Button : MonoBehaviour
	{
		[SerializeField]
		protected Text titleText;

		[SerializeField]
		protected Text button1Text;

		[SerializeField]
		protected UnityEvent OnClickButton1;

		public virtual void Open(string text, string buttonText1, UnityAction callbackOnClickButton1)
		{
			titleText.text = text;
			button1Text.text = buttonText1;
			OnClickButton1.RemoveAllListeners();
			OnClickButton1.AddListener(callbackOnClickButton1);
			Open();
		}

		public virtual void OnClickButton1Sub()
		{
			OnClickButton1.Invoke();
			Close();
		}

		public virtual void Open()
		{
			base.gameObject.SetActive(true);
		}

		public virtual void Close()
		{
			base.gameObject.SetActive(false);
		}
	}
}
