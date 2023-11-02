using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/UiSelection")]
	public class AdvUguiSelection : MonoBehaviour
	{
		public Text text;

		protected AdvSelection data;

		public AdvSelection Data => data;

		public virtual void Init(AdvSelection data, Action<AdvUguiSelection> ButtonClickedEvent)
		{
			this.data = data;
			text.text = data.Text;
			GetComponent<Button>().onClick.AddListener(delegate
			{
				ButtonClickedEvent(this);
			});
		}

		public virtual void OnInitSelected(Color color)
		{
			text.color = color;
		}
	}
}
