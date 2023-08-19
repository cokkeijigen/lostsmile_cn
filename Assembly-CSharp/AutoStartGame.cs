using UnityEngine;

[AddComponentMenu("Utage/ADV/Examples/AutoStartGame")]
public class AutoStartGame : MonoBehaviour
{
	public UtageUguiTitle title;

	public float timeLimit;

	private float time;

	private void OnEnable()
	{
		time = 0f;
	}

	private void Update()
	{
		if (time > timeLimit)
		{
			title.OnTapStart();
		}
		time += Time.deltaTime;
	}
}
