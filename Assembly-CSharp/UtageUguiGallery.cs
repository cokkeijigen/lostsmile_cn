using UnityEngine;
using Utage;

[AddComponentMenu("Utage/TemplateUI/Gallery")]
public class UtageUguiGallery : UguiView
{
	public UguiView[] views;

	protected int tabIndex = -1;

	protected virtual void OnOpen()
	{
		if (tabIndex >= 0)
		{
			views[tabIndex].ToggleOpen(true);
		}
	}

	public virtual void Sleep()
	{
		base.gameObject.SetActive(false);
	}

	public virtual void WakeUp()
	{
		base.gameObject.SetActive(true);
	}

	public virtual void OnTabIndexChanged(int index)
	{
		if (index >= views.Length)
		{
			Debug.LogError("index < views.Length");
			return;
		}
		for (int i = 0; i < views.Length; i++)
		{
			if (i != index)
			{
				views[i].ToggleOpen(false);
			}
		}
		views[index].ToggleOpen(true);
		tabIndex = index;
	}
}
