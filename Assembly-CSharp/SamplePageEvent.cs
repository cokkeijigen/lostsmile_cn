using UnityEngine;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/SamplePageEvent")]
public class SamplePageEvent : MonoBehaviour
{
	private void Awake()
	{
	}

	public void OnBeginText(AdvPage page)
	{
		Debug.Log("OnBeginText");
	}

	public void OnEndText(AdvPage page)
	{
		Debug.Log("OnEndText");
	}

	public void OnChangeText(AdvPage page)
	{
		Debug.Log("OnChangeText");
	}
}
