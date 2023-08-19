using System.Collections.Generic;
using UnityEngine;
using Utage;
using UtageExtensions;

[AddComponentMenu("Utage/ADV/Examples/ChatLog")]
public class SampleChatLog : MonoBehaviour
{
	[SerializeField]
	protected AdvEngine engine;

	[SerializeField]
	protected GameObject itemPrefab;

	[SerializeField]
	protected Transform targetRoot;

	[SerializeField]
	protected int maxLog = 10;

	private List<GameObject> logs = new List<GameObject>();

	public AdvEngine Engine
	{
		get
		{
			return engine ?? (engine = Object.FindObjectOfType<AdvEngine>());
		}
	}

	private void Awake()
	{
		Engine.Page.OnEndPage.AddListener(OnEndPage);
	}

	private void OnEndPage(AdvPage page)
	{
		if (page.CurrentData.IsEmptyText)
		{
			return;
		}
		AdvBacklog lastLog = page.Engine.BacklogManager.LastLog;
		if (lastLog == null)
		{
			return;
		}
		if (itemPrefab == null || targetRoot == null)
		{
			Debug.LogError("itemPrefab or targetRoot is null");
			return;
		}
		if (logs.Count > 0 && logs.Count >= maxLog)
		{
			Object.Destroy(logs[0]);
			logs.RemoveAt(0);
		}
		GameObject gameObject = targetRoot.AddChildPrefab(itemPrefab);
		gameObject.SendMessage("OnInitData", lastLog);
		gameObject.transform.SetSiblingIndex(1);
		logs.Add(gameObject);
	}
}
