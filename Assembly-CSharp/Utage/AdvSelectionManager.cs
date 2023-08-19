using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/SelectionManager")]
	public class AdvSelectionManager : MonoBehaviour, IAdvSaveData, IBinaryIO
	{
		private AdvEngine engine;

		private List<AdvSelection> selections = new List<AdvSelection>();

		private List<AdvSelection> spriteSelections = new List<AdvSelection>();

		[SerializeField]
		private SelectionEvent onClear = new SelectionEvent();

		[SerializeField]
		private SelectionEvent onBeginShow = new SelectionEvent();

		[SerializeField]
		private SelectionEvent onBeginWaitInput = new SelectionEvent();

		[SerializeField]
		private SelectionEvent onUpdateWaitInput = new SelectionEvent();

		[SerializeField]
		private SelectionEvent onSelected = new SelectionEvent();

		private AdvSelection selected;

		private const int VERSION = 1;

		private const int VERSION_0 = 0;

		public AdvEngine Engine
		{
			get
			{
				return engine ?? (engine = GetComponent<AdvEngine>());
			}
		}

		public List<AdvSelection> Selections
		{
			get
			{
				return selections;
			}
		}

		public List<AdvSelection> SpriteSelections
		{
			get
			{
				return spriteSelections;
			}
		}

		public bool IsSelected
		{
			get
			{
				return selected != null;
			}
		}

		public bool IsShowing { get; set; }

		public SelectionEvent OnClear
		{
			get
			{
				return onClear;
			}
		}

		public SelectionEvent OnBeginShow
		{
			get
			{
				return onBeginShow;
			}
		}

		public SelectionEvent OnBeginWaitInput
		{
			get
			{
				return onBeginWaitInput;
			}
		}

		public SelectionEvent OnUpdateWaitInput
		{
			get
			{
				return onUpdateWaitInput;
			}
		}

		public SelectionEvent OnSelected
		{
			get
			{
				return onSelected;
			}
		}

		public AdvSelection Selected
		{
			get
			{
				return selected;
			}
		}

		public bool IsWaitInput { get; set; }

		public int TotalCount
		{
			get
			{
				return Selections.Count + SpriteSelections.Count;
			}
		}

		public string SaveKey
		{
			get
			{
				return "AdvSelectionManager";
			}
		}

		public void AddSelection(string label, string text, ExpressionParser exp, string prefabName, float? x, float? y, StringGridRow row)
		{
			selections.Add(new AdvSelection(label, text, exp, prefabName, x, y, row));
		}

		public void AddSelectionClick(string label, string name, bool isPolygon, ExpressionParser exp, StringGridRow row)
		{
			AdvSelection advSelection = new AdvSelection(label, name, isPolygon, exp, row);
			spriteSelections.Add(advSelection);
			AddSpriteClickEvent(advSelection);
		}

		private void AddSpriteClickEvent(AdvSelection select)
		{
			Engine.GraphicManager.AddClickEvent(select.SpriteName, select.IsPolygon, select.RowData, delegate(BaseEventData eventData)
			{
				OnSpriteClick(eventData, select);
			});
		}

		private void OnSpriteClick(BaseEventData eventData, AdvSelection select)
		{
			Select(select);
		}

		public void Show()
		{
			selected = null;
			IsShowing = true;
			OnBeginShow.Invoke(this);
		}

		internal bool TryStartWaitInputIfShowing()
		{
			if (selections.Count > 0 || spriteSelections.Count > 0)
			{
				IsWaitInput = true;
				OnBeginWaitInput.Invoke(this);
				return true;
			}
			return false;
		}

		private void Update()
		{
			if (IsWaitInput)
			{
				OnUpdateWaitInput.Invoke(this);
			}
		}

		public void Select(int index)
		{
			Select(selections[index]);
		}

		public void Select(AdvSelection selected)
		{
			this.selected = selected;
			string jumpLabel = selected.JumpLabel;
			if (selected.Exp != null)
			{
				Engine.Param.CalcExpression(selected.Exp);
			}
			OnSelected.Invoke(this);
			Engine.SystemSaveData.SelectionData.AddData(selected);
			Clear();
			Engine.ScenarioPlayer.MainThread.JumpManager.RegistoreLabel(jumpLabel);
		}

		public void Clear()
		{
			ClearSub();
			OnClear.Invoke(this);
		}

		private void ClearSub()
		{
			selected = null;
			selections.Clear();
			foreach (AdvSelection spriteSelection in spriteSelections)
			{
				Engine.GraphicManager.RemoveClickEvent(spriteSelection.SpriteName);
			}
			spriteSelections.Clear();
			IsShowing = false;
			IsWaitInput = false;
		}

		internal void SelectWithTotalIndex(int index)
		{
			if (index < Selections.Count)
			{
				Select(index);
				return;
			}
			index -= Selections.Count;
			Select(SpriteSelections[index]);
		}

		void IAdvSaveData.OnClear()
		{
			Clear();
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(1);
			writer.Write(Selections.Count);
			foreach (AdvSelection selection in Selections)
			{
				selection.Write(writer);
			}
			writer.Write(SpriteSelections.Count);
			foreach (AdvSelection spriteSelection in SpriteSelections)
			{
				spriteSelection.Write(writer);
			}
		}

		public void OnRead(BinaryReader reader)
		{
			ClearSub();
			int num = reader.ReadInt32();
			switch (num)
			{
			case 1:
			{
				int num3 = reader.ReadInt32();
				for (int j = 0; j < num3; j++)
				{
					AdvSelection item2 = new AdvSelection(reader, engine);
					selections.Add(item2);
				}
				num3 = reader.ReadInt32();
				for (int k = 0; k < num3; k++)
				{
					AdvSelection advSelection = new AdvSelection(reader, engine);
					spriteSelections.Add(advSelection);
					AddSpriteClickEvent(advSelection);
				}
				break;
			}
			case 0:
			{
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					AdvSelection item = new AdvSelection(reader, engine);
					selections.Add(item);
				}
				break;
			}
			default:
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				break;
			}
		}
	}
}
