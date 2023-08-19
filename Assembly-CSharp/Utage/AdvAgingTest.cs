using System;
using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/SelectionAutomatic")]
	public class AdvAgingTest : MonoBehaviour
	{
		public enum Type
		{
			Random,
			DepthFirst
		}

		[Flags]
		private enum SkipFlags
		{
			Voice = 1,
			Movie = 2
		}

		[SerializeField]
		private Type type;

		[SerializeField]
		private bool disable;

		[SerializeField]
		[EnumFlags]
		private SkipFlags skipFilter;

		[SerializeField]
		protected AdvEngine engine;

		public float waitTime = 1f;

		private float time;

		public bool clearOnEnd = true;

		private Dictionary<AdvScenarioPageData, int> selectedDictionary = new Dictionary<AdvScenarioPageData, int>();

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
				return engine ?? (engine = UnityEngine.Object.FindObjectOfType<AdvEngine>());
			}
		}

		private void Awake()
		{
			Engine.SelectionManager.OnBeginWaitInput.AddListener(OnBeginWaitInput);
			Engine.SelectionManager.OnUpdateWaitInput.AddListener(OnUpdateWaitInput);
			Engine.ScenarioPlayer.OnBeginCommand.AddListener(OnBeginCommand);
			Engine.ScenarioPlayer.OnUpdatePreWaitingCommand.AddListener(OnUpdatePreWaitingCommand);
			Engine.ScenarioPlayer.OnEndScenario.AddListener(OnEndScenario);
		}

		private void OnBeginWaitInput(AdvSelectionManager selection)
		{
			time = 0f - Time.deltaTime;
		}

		private void OnUpdateWaitInput(AdvSelectionManager selection)
		{
			if (!Disable)
			{
				time += Time.deltaTime;
				if (time >= waitTime)
				{
					selection.SelectWithTotalIndex(GetIndex(selection));
				}
			}
		}

		private void OnBeginCommand(AdvCommand command)
		{
			time = 0f - Time.deltaTime;
		}

		private void OnUpdatePreWaitingCommand(AdvCommand command)
		{
			if (Disable || !IsWaitInputCommand(command))
			{
				return;
			}
			time += Time.deltaTime;
			if (time >= waitTime)
			{
				if (command is AdvCommandWaitInput)
				{
					Engine.UiManager.IsInputTrig = true;
				}
				if (command is AdvCommandSendMessage)
				{
					engine.ScenarioPlayer.SendMessageTarget.SafeSendMessage("OnAgingInput", command);
				}
				if (command is AdvCommandMovie)
				{
					Engine.UiManager.IsInputTrig = true;
				}
				if (command is AdvCommandText && Engine.SoundManager.IsPlayingVoice())
				{
					Engine.Page.InputSendMessage();
				}
			}
		}

		private void OnEndScenario(AdvScenarioPlayer player)
		{
			if (clearOnEnd)
			{
				selectedDictionary.Clear();
			}
		}

		private bool IsWaitInputCommand(AdvCommand command)
		{
			if (command is AdvCommandWaitInput)
			{
				return true;
			}
			if (command is AdvCommandSendMessage)
			{
				return true;
			}
			if (command is AdvCommandMovie)
			{
				return (skipFilter & SkipFlags.Movie) == SkipFlags.Movie;
			}
			if (command is AdvCommandText)
			{
				return (skipFilter & SkipFlags.Voice) == SkipFlags.Voice;
			}
			return false;
		}

		private int GetIndex(AdvSelectionManager selection)
		{
			Type type = this.type;
			if (type == Type.DepthFirst)
			{
				return GetIndexDepthFirst(selection);
			}
			return UnityEngine.Random.Range(0, selection.TotalCount);
		}

		private int GetIndexDepthFirst(AdvSelectionManager selection)
		{
			int value;
			if (!selectedDictionary.TryGetValue(Engine.Page.CurrentData, out value))
			{
				value = 0;
				selectedDictionary.Add(Engine.Page.CurrentData, value);
			}
			else
			{
				if (value + 1 < selection.TotalCount)
				{
					value++;
				}
				selectedDictionary[Engine.Page.CurrentData] = value;
			}
			return value;
		}
	}
}
