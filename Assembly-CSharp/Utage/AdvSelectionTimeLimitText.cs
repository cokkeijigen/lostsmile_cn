using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/SelectionTimeLimitText")]
	public class AdvSelectionTimeLimitText : MonoBehaviour
	{
		[SerializeField]
		protected GameObject targetRoot;

		[SerializeField]
		protected Text text;

		protected AdvSelectionTimeLimit timeLimit;

		[SerializeField]
		protected AdvEngine engine;

		public GameObject TargetRoot => targetRoot ?? (targetRoot = base.gameObject);

		public Text Target => text ?? (text = TargetRoot.GetComponentInChildren<Text>());

		public AdvEngine Engine => engine ?? (engine = Object.FindObjectOfType<AdvEngine>());

		private void Awake()
		{
			Engine.SelectionManager.OnBeginWaitInput.AddListener(OnBeginWaitInput);
			Engine.SelectionManager.OnUpdateWaitInput.AddListener(OnUpdateWaitInput);
			Engine.SelectionManager.OnSelected.AddListener(OnSelected);
			Engine.SelectionManager.OnClear.AddListener(OnClear);
			TargetRoot.SetActive(false);
		}

		private void OnBeginWaitInput(AdvSelectionManager selection)
		{
			timeLimit = Object.FindObjectOfType<AdvSelectionTimeLimit>();
			if (timeLimit != null)
			{
				TargetRoot.SetActive(true);
			}
		}

		private void OnUpdateWaitInput(AdvSelectionManager selection)
		{
			if (TargetRoot.activeSelf && timeLimit != null)
			{
				Target.text = string.Concat(Mathf.CeilToInt(timeLimit.limitTime - timeLimit.TimeCount));
			}
		}

		private void OnSelected(AdvSelectionManager selection)
		{
			TargetRoot.SetActive(false);
		}

		private void OnClear(AdvSelectionManager selection)
		{
			TargetRoot.SetActive(false);
		}
	}
}
