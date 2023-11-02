using UnityEngine;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/SampleJumpButton")]
public class SampleJumpButton : MonoBehaviour
{
	[SerializeField]
	protected AdvEngine engine;

	public string scenarioLabel;

	public AdvEngine Engine => engine ?? (engine = Object.FindObjectOfType<AdvEngine>());

	public void OnClickJump()
	{
		Engine.JumpScenario(scenarioLabel);
	}
}
