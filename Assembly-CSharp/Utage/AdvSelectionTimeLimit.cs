using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/SelectionTimeLimit")]
	public class AdvSelectionTimeLimit : MonoBehaviour
	{
		[SerializeField]
		private bool disable;

		[SerializeField]
		protected AdvEngine engine;

		[SerializeField]
		protected AdvUguiSelection selection;

		public float limitTime = 10f;

		public int timeLimitIndex = -1;

		private float time;

		public bool Disable
		{
			get
			{
				return disable;
			}
			set
			{
				disable = value;
			}
		}

		public AdvEngine Engine
		{
			get
			{
				return engine ?? (engine = Object.FindObjectOfType<AdvEngine>());
			}
		}

		public AdvUguiSelection Selection
		{
			get
			{
				return selection ?? (selection = GetComponent<AdvUguiSelection>());
			}
		}

		public float TimeCount
		{
			get
			{
				return time;
			}
		}

		private void Awake()
		{
			Engine.SelectionManager.OnBeginWaitInput.AddListener(OnBeginWaitInput);
			Engine.SelectionManager.OnUpdateWaitInput.AddListener(OnUpdateWaitInput);
		}

		private void OnBeginWaitInput(AdvSelectionManager selection)
		{
			time = 0f - Time.deltaTime;
		}

		private void OnUpdateWaitInput(AdvSelectionManager selection)
		{
			time += Time.deltaTime;
			if (!(time >= limitTime) || !Engine.SelectionManager.IsWaitInput)
			{
				return;
			}
			if (timeLimitIndex < 0)
			{
				if (Selection != null)
				{
					selection.Select(Selection.Data);
				}
			}
			else
			{
				selection.Select(timeLimitIndex);
			}
		}
	}
}
