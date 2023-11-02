using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/ToggledGroupIndexd")]
	public class UguiToggleGroupIndexed : MonoBehaviour
	{
		[Serializable]
		public class UguiTabButtonGroupEvent : UnityEvent<int>
		{
		}

		[SerializeField]
		protected List<Toggle> toggles = new List<Toggle>();

		public int firstIndexOnAwake;

		public bool ignoreValueChangeOnAwake = true;

		public bool autoToggleInteractiveOff = true;

		public bool isLoopShift = true;

		public Button shiftLeftButton;

		public Button shiftRightButton;

		public Button jumpLeftEdgeButton;

		public Button jumpRightEdgeButton;

		protected int currentIndex = -1;

		public UguiTabButtonGroupEvent OnValueChanged;

		protected bool isIgnoreValueChange;

		public Toggle[] TogglesToArray => toggles.ToArray();

		public virtual int CurrentIndex
		{
			get
			{
				return currentIndex;
			}
			set
			{
				if (value >= toggles.Count)
				{
					return;
				}
				for (int i = 0; i < toggles.Count; i++)
				{
					bool flag = i == value;
					toggles[i].isOn = flag;
					if (autoToggleInteractiveOff)
					{
						toggles[i].interactable = !flag;
					}
				}
				if (currentIndex != value)
				{
					currentIndex = value;
					OnValueChanged.Invoke(value);
				}
			}
		}

		public int Count => toggles.Count;

		protected virtual void Awake()
		{
			for (int i = 0; i < toggles.Count; i++)
			{
				Toggle toggle = toggles[i];
				toggle.onValueChanged.AddListener(delegate
				{
					OnToggleValueChanged(toggle);
				});
			}
			if (ignoreValueChangeOnAwake)
			{
				currentIndex = firstIndexOnAwake;
			}
			CurrentIndex = firstIndexOnAwake;
			if ((bool)shiftLeftButton)
			{
				shiftLeftButton.onClick.AddListener(ShiftLeft);
			}
			if ((bool)shiftRightButton)
			{
				shiftRightButton.onClick.AddListener(ShiftRight);
			}
			if ((bool)jumpLeftEdgeButton)
			{
				jumpLeftEdgeButton.onClick.AddListener(JumpLeftEdge);
			}
			if ((bool)jumpRightEdgeButton)
			{
				jumpRightEdgeButton.onClick.AddListener(JumpRightEdge);
			}
		}

		protected virtual void OnToggleValueChanged(Toggle toggle)
		{
			if (!isIgnoreValueChange)
			{
				isIgnoreValueChange = true;
				CurrentIndex = toggles.FindIndex((Toggle obj) => obj == toggle);
				isIgnoreValueChange = false;
			}
		}

		public virtual void Add(Toggle toggle)
		{
			toggles.Add(toggle);
			toggle.onValueChanged.AddListener(delegate
			{
				OnToggleValueChanged(toggle);
			});
		}

		public virtual void ClearToggles()
		{
			toggles.Clear();
		}

		public virtual void SetActiveLRButtons(bool isActive)
		{
			if ((bool)shiftLeftButton)
			{
				shiftLeftButton.gameObject.SetActive(isActive);
			}
			if ((bool)shiftRightButton)
			{
				shiftRightButton.gameObject.SetActive(isActive);
			}
			if ((bool)jumpLeftEdgeButton)
			{
				jumpLeftEdgeButton.gameObject.SetActive(isActive);
			}
			if ((bool)jumpRightEdgeButton)
			{
				jumpRightEdgeButton.gameObject.SetActive(isActive);
			}
		}

		public virtual void ShiftLeft()
		{
			if (Count > 0)
			{
				int num = CurrentIndex - 1;
				if (num < 0)
				{
					num = (isLoopShift ? (Count - 1) : 0);
				}
				CurrentIndex = num;
			}
		}

		public virtual void ShiftRight()
		{
			if (Count > 0)
			{
				int num = CurrentIndex + 1;
				if (num >= Count)
				{
					num = ((!isLoopShift) ? (Count - 1) : 0);
				}
				CurrentIndex = num;
			}
		}

		public virtual void JumpLeftEdge()
		{
			if (Count > 0)
			{
				CurrentIndex = 0;
			}
		}

		public virtual void JumpRightEdge()
		{
			if (Count > 0)
			{
				CurrentIndex = Count - 1;
			}
		}
	}
}
