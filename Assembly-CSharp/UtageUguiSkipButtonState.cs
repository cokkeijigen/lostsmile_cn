using UnityEngine;
using UnityEngine.UI;
using Utage;

[AddComponentMenu("Utage/TemplateUI/SkipButtonState")]
public class UtageUguiSkipButtonState : MonoBehaviour
{
	[SerializeField]
	protected AdvEngine engine;

	public Toggle target;

	public AdvEngine Engine
	{
		get
		{
			return engine ?? (engine = Object.FindObjectOfType<AdvEngine>());
		}
	}

	protected virtual void Update()
	{
		if (!(target == null))
		{
			target.interactable = Engine.Page.EnableSkip();
		}
	}
}
