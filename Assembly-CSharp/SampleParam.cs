using UnityEngine;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/SampleParam")]
public class SampleParam : MonoBehaviour
{
	[SerializeField]
	protected AdvEngine engine;

	public AdvEngine Engine => engine ?? (engine = Object.FindObjectOfType<AdvEngine>());

	public void ParamTest()
	{
		Engine.Param.GetParameter("flag1");
		Engine.Param.TrySetParameter("flag1", true);
	}
}
