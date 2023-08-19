using UnityEngine;
using Utage;

[AddComponentMenu("Utage/ADV/Examples/SampleCheckUnity56Newer")]
public class SampleCheckUnity56Newer : MonoBehaviour
{
	[SerializeField]
	protected AdvEngine engine;

	public AdvEngine Engine
	{
		get
		{
			return engine ?? (engine = Object.FindObjectOfType<AdvEngine>());
		}
	}

	private bool IsInit { get; set; }

	private bool Unity56OrNewer
	{
		get
		{
			return true;
		}
	}

	private void Start()
	{
		Engine.OnClear.AddListener(OnClear);
	}

	private void OnClear(AdvEngine engine)
	{
		bool unity56OrNewer = Unity56OrNewer;
		engine.Param.TrySetParameter("unity56OrNewer", unity56OrNewer);
	}
}
